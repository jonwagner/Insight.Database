using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Separating the IDBConnection interface improves readability.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Separating the IDBConnection interface improves readability.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	public class DynamicConnection : DynamicObject, IDbConnection
	{
		#region Fields
        /// <summary>
        /// The types of parameters to pass to Query methods.
        /// </summary>
        private static Type[] _queryParameters = new Type[] { typeof(IDbConnection), typeof(IDbCommand), typeof(Type), typeof(CommandBehavior) };

        /// <summary>
        /// The types of parameters to pass to QueryResults methods.
        /// </summary>
        private static Type[] _queryResultsParameters = new Type[] { typeof(IDbConnection), typeof(IDbCommand), typeof(Type[]), typeof(CommandBehavior) };

        /// <summary>
        /// Caches for MethodInfo for query methods.
        /// </summary>
        private static ConcurrentDictionary<Type, MethodInfo> _queryResultsAsyncMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> _queryResultsMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> _queryAsyncMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> _queryMethods = new ConcurrentDictionary<Type, MethodInfo>();

		/// <summary>
		/// The internal cache for parameters to stored procedures.
		/// </summary>
		private ConcurrentDictionary<string, List<IDbDataParameter>> _parameters = new ConcurrentDictionary<string, List<IDbDataParameter>>();

		/// <summary>
		/// The connection to use to connect to the database.
		/// </summary>
		private IDbConnection _connection;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the DynamicConnection class.
		/// </summary>
		/// <param name="connection">The connection to use as the inner database connection.</param>
		public DynamicConnection(IDbConnection connection)
		{
			_connection = connection;
		}
		#endregion

		#region DynamicObject Members
		/// <summary>
		/// Provides the implementation for operations that invoke a member. Classes derived from the DynamicObject class can override this method to specify dynamic behavior for operations such as calling a method.
		/// </summary>
		/// <param name="binder">Provides information about the dynamic operation.</param>
		/// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
		/// <param name="result">The result of the operation.</param>
		/// <returns>True if the operation is successful; otherwise, false.</returns>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			result = DoInvokeMember(binder, args, typeof(FastExpando));

			return true;
		}

        /// <summary>
        /// Implements the procedure invocation process.
        /// </summary>
        /// <param name="binder">The binder used on the method.</param>
        /// <param name="args">The arguments to the method.</param>
        /// <param name="type">The default type of object to return if no type parameter is specified.</param>
        /// <returns>The results of the stored procedure call.</returns>
        protected object DoInvokeMember(InvokeMemberBinder binder, object[] args, Type type)
        {
            bool doAsync = false;
            IDbCommand cmd = null;
            int specialParameters = 0;
            int? timeout = null;
            IDbTransaction transaction = null;
            object withGraph = null; // could be withGraph:Type or withGraphs:Type[]

            CallInfo callInfo = binder.CallInfo;
            int unnamedParameterCount = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;

            // check the proc name - if it ends with Async, then call it asynchronously and return the results
            string procName = binder.Name;
            if (procName.EndsWith("async", StringComparison.OrdinalIgnoreCase))
            {
                procName = procName.Substring(0, procName.Length - 5);
                doAsync = true;
            }

            // go through the arguments and look for our special arguments
            // NOTE: this is intentionally case-sensitive so that you can use other cases if you need to pass a parameter by the same name.
            var argumentNames = callInfo.ArgumentNames;
            for (int i = 0; i < argumentNames.Count; i++)
            {
                switch (argumentNames[i])
                {
                    case "transaction":
                        transaction = (IDbTransaction)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "timeout":
                        timeout = (int)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "returnType":
                        type = (Type)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "withGraph":
                        withGraph = (Type)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "withGraphs":
                        withGraph = (Type[])args[i + unnamedParameterCount];
                        specialParameters++;
                        break;
                }
            }

            // if there is exactly one unnamed parameter, and the named parameters are all special parameters, and it's a reference type (and not a string)
            // then we will attempt to use the object's fields as the parameter values
            // this is so you can send an entire object to an insert method
            if (unnamedParameterCount == 1 &&
                (callInfo.ArgumentNames.Count == specialParameters) &&
                !args[0].GetType().IsValueType && args[0].GetType() != typeof(String))
            {
                cmd = _connection.CreateCommand(procName, args[0], CommandType.StoredProcedure, timeout, transaction);
            }
            else
            {
                // this isn't a single-object parameter, so we are going to map the parameters by position and by name

                // create a command
                cmd = _connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;
                cmd.Connection = _connection;
                cmd.Transaction = transaction;

                // fill in the parameters for the command object
                // we will do the values next
                DeriveParameters(cmd);

                // look at the unnamed parameters first. we will add them by position.
                for (int i = 0; i < unnamedParameterCount; i++)
                {
                    // add the unnamed parameters by index
                    IDbDataParameter p = (IDbDataParameter)cmd.Parameters[i];
                    p.Value = args[i];
                }

                // go through all of the named arguments next. Note that they may overwrite indexed parameters.
                for (int i = unnamedParameterCount; i < callInfo.ArgumentNames.Count; i++)
                {
                    string argumentName = callInfo.ArgumentNames[i];

                    // ignore out special parameters
                    if (argumentName == "transaction" || argumentName == "timeout" || argumentName == "returnType" || argumentName == "withGraph" || argumentName == "withGraphs")
                        continue;

                    string parameterName = "@" + argumentName;
                    IDbDataParameter p = cmd.Parameters.OfType<IDbDataParameter>().FirstOrDefault(parameter => String.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
                    p.Value = args[i];
                }
            }

            // get the proper query method to call based on whether we are doing this async and whether there is a single or multiple result set
            // the nice thing is that the generic expansion will automatically create the proper return type like IList<T> or Results<T>.
            MethodInfo method = GetQueryMethod(type, doAsync);

            return method.Invoke(null, new object[] { _connection, cmd, withGraph, CommandBehavior.Default });
        }

        /// <summary>
        /// Returns a MethodInfo for a query method based on the type and whether the call should be async.
        /// </summary>
        /// <param name="type">The type of object to return.</param>
        /// <param name="doAsync">True to return the asynchronous method.</param>
        /// <returns>The MethodInfo for the query method.</returns>
        private static MethodInfo GetQueryMethod(Type type, bool doAsync)
        {
            MethodInfo method;
            if (type.IsSubclassOf(typeof(Results)))
            {
                if (doAsync)
                    method = _queryResultsAsyncMethods.GetOrAdd(type, t => typeof(AsyncExtensions).GetMethod("QueryResultsAsync", _queryResultsParameters).MakeGenericMethod(t));
                else
                    method = _queryResultsMethods.GetOrAdd(type, t => typeof(DBConnectionExtensions).GetMethod("QueryResults", _queryResultsParameters).MakeGenericMethod(t));
            }
            else
            {
                if (doAsync)
                    method = _queryAsyncMethods.GetOrAdd(type, t => typeof(AsyncExtensions).GetMethod("QueryAsync", _queryParameters).MakeGenericMethod(t));
                else
                    method = _queryMethods.GetOrAdd(type, t => typeof(DBConnectionExtensions).GetMethod("Query", _queryParameters).MakeGenericMethod(t));
            }

            return method;
        }

        /// <summary>
		/// Derive the parameters that are needed to execute a given command.
		/// </summary>
		/// <param name="cmd">The command to execute.</param>
		private void DeriveParameters(IDbCommand cmd)
		{
			// look in the concurrent dictionary to find the parameters.
			// if not found, call SQL Server to get them.
			var parameterList = _parameters.GetOrAdd(
				cmd.CommandText,
				name =>
				{
					SqlConnection connection = (SqlConnection)cmd.Connection;
					bool autoClose = connection.State != ConnectionState.Open;
					if (autoClose)
						connection.Open();

					try
					{
						// call the server to get the parameters
						SqlCommandBuilder.DeriveParameters((SqlCommand)cmd);
					}
					finally
					{
						if (autoClose)
							connection.Close();
					}

					// copy the parameters so we can reuse them
					List<IDbDataParameter> parameters = cmd.Parameters.Cast<IDbDataParameter>().Where(p => p.Direction == ParameterDirection.Input).ToList();
					cmd.Parameters.Clear();

					return parameters;
				});

			// copy the parameter list
			foreach (IDbDataParameter parameter in parameterList)
			{
				IDbDataParameter p = cmd.CreateParameter();
				p.ParameterName = parameter.ParameterName;
				p.DbType = parameter.DbType;
				p.Direction = parameter.Direction;
				cmd.Parameters.Add(p);
			}
		}
		#endregion

		#region IDbConnection Members
		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return _connection.BeginTransaction(il);
		}

		public IDbTransaction BeginTransaction()
		{
			return _connection.BeginTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			_connection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			_connection.Close();
		}

		public string ConnectionString
		{
			get
			{
				return _connection.ConnectionString;
			}

			set
			{
				_connection.ConnectionString = value;
			}
		}

		public int ConnectionTimeout
		{
			get { return _connection.ConnectionTimeout; }
		}

		public IDbCommand CreateCommand()
		{
			return _connection.CreateCommand();
		}

		public string Database
		{
			get { return _connection.Database; }
		}

		public void Open()
		{
			_connection.Open();
		}

		public ConnectionState State
		{
			get { return _connection.State; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			_connection.Dispose();
		}
		#endregion
	}

    #region DynamicConnection Generic Class
    /// <summary>
	/// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
    /// You should usually use the non-generic version of DynamicConnection so you can return different types of objects.
    /// e.g.
    /// var dynamicConnection = _connection.Dynamic()
    /// IList&lt;MyData&gt; results = dynamicConnection.MyProc(10, returnType: typeof(MyData));
    /// This class generates methods that always return objects of type T.
    /// </summary>
	/// <typeparam name="T">The type of object to return from all queries from this connection.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public sealed class DynamicConnection<T> : DynamicConnection
	{
		/// <summary>
		/// Initializes a new instance of the DynamicConnection class.
		/// </summary>
		/// <param name="connection">The connection to use as the inner database connection.</param>
		public DynamicConnection(IDbConnection connection)
			: base(connection)
		{
		}

		/// <summary>
		/// Provides the implementation for operations that invoke a member. Classes derived from the DynamicObject class can override this method to specify dynamic behavior for operations such as calling a method.
		/// </summary>
		/// <param name="binder">Provides information about the dynamic operation.</param>
		/// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
		/// <param name="result">The result of the operation.</param>
		/// <returns>True if the operation is successful; otherwise, false.</returns>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			result = DoInvokeMember(binder, args, typeof(T));

			return true;
		}
	}
    #endregion
}
