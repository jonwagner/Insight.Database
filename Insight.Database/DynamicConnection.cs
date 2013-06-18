using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;

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
		private static Type[] _queryParameters = new Type[] { typeof(IDbCommand), typeof(Type), typeof(CommandBehavior) };

		/// <summary>
		/// The types of parameters to pass to QueryAsync methods.
		/// </summary>
		private static Type[] _queryAsyncParameters = new Type[] { typeof(IDbCommand), typeof(Type), typeof(CommandBehavior), typeof(CancellationToken?) };

		/// <summary>
		/// The types of parameters to pass to QueryResults methods.
		/// </summary>
		private static Type[] _queryResultsParameters = new Type[] { typeof(IDbCommand), typeof(Type[]), typeof(CommandBehavior) };

		/// <summary>
		/// The types of parameters to pass to QueryResultsAsync methods.
		/// </summary>
		private static Type[] _queryResultsAsyncParameters = new Type[] { typeof(IDbCommand), typeof(Type[]), typeof(CommandBehavior), typeof(CancellationToken?) };

		/// <summary>
		/// Caches for MethodInfo for query methods.
		/// </summary>
		private static ConcurrentDictionary<Type, Func<IDbCommand, Type[], CommandBehavior, CancellationToken?, object>> _queryResultsAsyncMethods = new ConcurrentDictionary<Type, Func<IDbCommand, Type[], CommandBehavior, CancellationToken?, object>>();
		private static ConcurrentDictionary<Type, Func<IDbCommand, Type[], CommandBehavior, object>> _queryResultsMethods = new ConcurrentDictionary<Type, Func<IDbCommand, Type[], CommandBehavior, object>>();
		private static ConcurrentDictionary<Type, Func<IDbCommand, Type, CommandBehavior, CancellationToken?, object>> _queryAsyncMethods = new ConcurrentDictionary<Type, Func<IDbCommand, Type, CommandBehavior, CancellationToken?, object>>();
		private static ConcurrentDictionary<Type, Func<IDbCommand, Type, CommandBehavior, object>> _queryMethods = new ConcurrentDictionary<Type, Func<IDbCommand, Type, CommandBehavior, object>>();

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
			result = DoInvokeMember(binder, args, null);

			return true;
		}

		/// <summary>
		/// Implements the procedure invocation process.
		/// </summary>
		/// <param name="binder">The binder used on the method.</param>
		/// <param name="args">The arguments to the method.</param>
		/// <param name="returnType">The default type of object to return if no type parameter is specified.</param>
		/// <returns>The results of the stored procedure call.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
		protected internal object DoInvokeMember(InvokeMemberBinder binder, object[] args, Type returnType)
		{
			if (binder == null) throw new ArgumentNullException("binder");
			if (args == null) throw new ArgumentNullException("args");

			bool doAsync = false;
			IDbCommand cmd = null;
			int specialParameters = 0;
			int? timeout = null;
			IDbTransaction transaction = null;
			object withGraph = null; // could be withGraph:Type or withGraphs:Type[]
			CancellationToken cancellationToken = CancellationToken.None;

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
					case "cancellationToken":
						cancellationToken = (CancellationToken)args[i + unnamedParameterCount];
						specialParameters++;
						break;

					case "transaction":
						transaction = (IDbTransaction)args[i + unnamedParameterCount];
						specialParameters++;
						break;

					case "commandTimeout":
						timeout = (int)args[i + unnamedParameterCount];
						specialParameters++;
						break;

					case "returnType":
						returnType = (Type)args[i + unnamedParameterCount];
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

			try
			{
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

					// unwrap the transaction because the transaction has to match the command and connection
					if (transaction != null)
						cmd.Transaction = DBConnectionExtensions.UnwrapDbTransaction(transaction);

					// fill in the parameters for the command object
					// we will do the values next
					DeriveParameters(cmd);

					// look at the unnamed parameters first. we will add them by position.
					var inputParameters = cmd.Parameters.OfType<IDbDataParameter>().Where(p => p.Direction.HasFlag(ParameterDirection.Input)).ToList();
					for (int i = 0; i < unnamedParameterCount; i++)
					{
						// add the unnamed parameters by index
						IDbDataParameter p = (IDbDataParameter)inputParameters[i];
						p.Value = args[i];
					}

					// go through all of the named arguments next. Note that they may overwrite indexed parameters.
					for (int i = unnamedParameterCount; i < callInfo.ArgumentNames.Count; i++)
					{
						string argumentName = callInfo.ArgumentNames[i];

						// ignore our special parameters
						if (argumentName == "cancellationToken" ||
							argumentName == "transaction" ||
							argumentName == "commandTimeout" ||
							argumentName == "returnType" ||
							argumentName == "withGraph" ||
							argumentName == "withGraphs")
							continue;

						IDbDataParameter p = cmd.Parameters.OfType<IDbDataParameter>().FirstOrDefault(parameter => String.Equals(parameter.ParameterName, argumentName, StringComparison.OrdinalIgnoreCase));
						p.Value = args[i];
					}

					// special handling for table parameters
					var provider = InsightDbProvider.For(cmd);
					foreach (var p in cmd.Parameters.OfType<IDbDataParameter>().Where(p => provider.IsTableValuedParameter(cmd, p)).ToList())
					{
						// if any parameters are missing table parameters, then alert the developer
						if (p.Value == null)
							throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Table parameter {0} must be specified", p.ParameterName));

						// swap out the parameter with something enumerable
						cmd.Parameters.Remove(p);
						Type listType = p.Value.GetType().GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
						if (listType == null)
							throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Table parameter {0} expects an enumerable", p.ParameterName));
						Type elementType = listType.GetGenericArguments()[0];

						DbParameterGenerator.ListParameterHelper.AddEnumerableParameters(cmd, p.ParameterName, provider.GetTableParameterTypeName(cmd, p), elementType, p.Value);
					}
				}

				// if type was not specified, but withGraph was specified, then use the first item in the graph as the type
				if (returnType == null && withGraph != null)
				{
					Type withGraphType = withGraph as Type;
					if (withGraphType != null && withGraphType.IsSubclassOf(typeof(Graph)))
					{
						returnType = withGraphType.GetGenericArguments()[0];
					}
				}

				// if we don't have a type, use FastExpando
				if (returnType == null)
					returnType = typeof(FastExpando);

				// get the proper query method to call based on whether we are doing this async and whether there is a single or multiple result set
				// the nice thing is that the generic expansion will automatically create the proper return type like IList<T> or Results<T>.
				if (returnType.IsSubclassOf(typeof(Results)))
				{
					if (doAsync)
						return CallQueryResultsAsync(returnType, cmd, withGraph, cancellationToken);
					else
						return CallQueryResults(returnType, cmd, withGraph);
				}
				else
				{
					if (doAsync)
						return CallQueryAsync(returnType, cmd, withGraph, cancellationToken);
					else
						return CallQuery(returnType, cmd, withGraph);
				}
			}
			finally
			{
				if (cmd != null)
					cmd.Dispose();
			}
		}

		#region Delegate Invocation Methods
		/// <summary>
		/// Invokes a cached delegate of Query, taking a command and withGraph, and returning the specified type.
		/// </summary>
		/// <param name="returnType">The type to return.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The type of graph to generate.</param>
		/// <returns>The result of the invocation.</returns>
		private static object CallQuery(Type returnType, IDbCommand command, object withGraph)
		{
			var method = _queryMethods.GetOrAdd(
				returnType,
				t => (Func<IDbCommand, Type, CommandBehavior, object>)Delegate.CreateDelegate(
					typeof(Func<IDbCommand, Type, CommandBehavior, object>),
					typeof(DBCommandExtensions).GetMethod("Query", _queryParameters).MakeGenericMethod(t)));
			return method(command, (Type)withGraph, CommandBehavior.Default);
		}

		/// <summary>
		/// Invokes a cached delegate of QueryAsync, taking a command, withGraph and cancellationToken, and returning the specified type.
		/// </summary>
		/// <param name="returnType">The type to return.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The type of graph to generate.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>The result of the invocation.</returns>
		private static object CallQueryAsync(Type returnType, IDbCommand command, object withGraph, CancellationToken? cancellationToken)
		{
			var method = _queryAsyncMethods.GetOrAdd(
				returnType,
				t => (Func<IDbCommand, Type, CommandBehavior, CancellationToken?, object>)Delegate.CreateDelegate(
					typeof(Func<IDbCommand, Type, CommandBehavior, CancellationToken?, object>),
					typeof(AsyncExtensions).GetMethod("QueryAsync", _queryAsyncParameters).MakeGenericMethod(t)));
			return method(command, (Type)withGraph, CommandBehavior.Default, cancellationToken);
		}

		/// <summary>
		/// Invokes a cached delegate of QueryResults, taking a command and withGraph, and returning the specified type.
		/// </summary>
		/// <param name="returnType">The type to return.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The type of graph to generate.</param>
		/// <returns>The result of the invocation.</returns>
		private static object CallQueryResults(Type returnType, IDbCommand command, object withGraph)
		{
			var method = _queryResultsMethods.GetOrAdd(
				returnType,
				t => (Func<IDbCommand, Type[], CommandBehavior, object>)Delegate.CreateDelegate(
					typeof(Func<IDbCommand, Type[], CommandBehavior, object>),
					typeof(DBCommandExtensions).GetMethod("QueryResults", _queryResultsParameters).MakeGenericMethod(t)));
			return method(command, (Type[])withGraph, CommandBehavior.Default);
		}

		/// <summary>
		/// Invokes a cached delegate of QueryResultsAsync, taking a command, withGraph and cancellationToken, and returning the specified type.
		/// </summary>
		/// <param name="returnType">The type to return.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The type of graph to generate.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>The result of the invocation.</returns>
		private static object CallQueryResultsAsync(Type returnType, IDbCommand command, object withGraph, CancellationToken? cancellationToken)
		{
			var method = _queryResultsAsyncMethods.GetOrAdd(
				returnType,
				t => (Func<IDbCommand, Type[], CommandBehavior, CancellationToken?, object>)Delegate.CreateDelegate(
					typeof(Func<IDbCommand, Type[], CommandBehavior, CancellationToken?, object>),
					typeof(AsyncExtensions).GetMethod("QueryResultsAsync", _queryResultsAsyncParameters).MakeGenericMethod(t)));
			return method(command, (Type[])withGraph, CommandBehavior.Default, cancellationToken);
		}
		#endregion

		/// <summary>
		/// Derive the parameters that are needed to execute a given command.
		/// </summary>
		/// <param name="cmd">The command to execute.</param>
		private void DeriveParameters(IDbCommand cmd)
		{
			var provider = InsightDbProvider.For(cmd);

			// look in the concurrent dictionary to find the parameters.
			// if not found, call the Server to get them.
			var parameterList = _parameters.GetOrAdd(
				cmd.CommandText,
				name => provider.DeriveParameters(cmd).ToList());

			// copy the parameter list
			foreach (IDbDataParameter parameter in parameterList)
				cmd.Parameters.Add(provider.CloneParameter(cmd, parameter));
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
