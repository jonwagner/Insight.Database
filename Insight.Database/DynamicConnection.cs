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
			result = DoInvokeMember(
				binder,
				args,
				(cmd, doAsync) =>
				{
					// execute the command
					if (doAsync)
						return ((SqlCommand)cmd).QueryAsync();
					else
						return _connection.Query(cmd);
				});

			return true;
		}

		/// <summary>
		/// Implements the procedure invocation process.
		/// </summary>
		/// <param name="binder">The binder used on the method.</param>
		/// <param name="args">The arguments to the method.</param>
		/// <param name="queryMethod">A method used to execute the query and return the results.</param>
		/// <returns>The results of the stored procedure call.</returns>
		protected object DoInvokeMember(InvokeMemberBinder binder, object[] args, Func<IDbCommand, bool, object> queryMethod)
		{
			CallInfo callInfo = binder.CallInfo;
			int unnamedParameterCount = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;

			// check the proc name - if it ends with Async, then call it asynchronously and return the results
			bool doAsync = false;
			string procName = binder.Name;
			if (procName.EndsWith("async", StringComparison.OrdinalIgnoreCase))
			{
				procName = procName.Substring(0, procName.Length - 5);
				doAsync = true;
			}

			// create a command
			IDbCommand cmd = _connection.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = procName;
			cmd.Connection = _connection;

			// check for a transaction
			int txIndex = callInfo.ArgumentNames.IndexOf("transaction");
			if (txIndex >= 0)
			{
				cmd.Transaction = (IDbTransaction)args[txIndex + unnamedParameterCount];
			}

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
				string parameterName = "@" + callInfo.ArgumentNames[i];

				// ignore the transaction parameter
				if (String.Equals(parameterName, "@transaction", StringComparison.OrdinalIgnoreCase))
					continue;

				IDbDataParameter p = cmd.Parameters.OfType<IDbDataParameter>().FirstOrDefault(parameter => String.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
				p.Value = args[i];
			}

			// run the query and translate the results
			return queryMethod(cmd, doAsync);
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
					// call the server to get the parameters
					SqlCommandBuilder.DeriveParameters((SqlCommand)cmd);

					// copy the parameters so we can reuse them
					List<IDbDataParameter> parameters = cmd.Parameters.Cast<IDbDataParameter>().ToList();
					cmd.Parameters.Clear();

					return parameters;
				});

			// copy the parameter
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

	/// <summary>
	/// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
	/// </summary>
	/// <typeparam name="T">The type of object to return from queries.</typeparam>
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
			result = DoInvokeMember(
				binder,
				args,
				(cmd, doAsync) =>
				{
					// execute the command
					if (doAsync)
						return ((SqlCommand)cmd).QueryAsync<T>();
					else
						return cmd.Connection.Query<T>(cmd);
				});

			return true;
		}
	}

	/// <summary>
	/// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
	/// </summary>
	/// <typeparam name="TResult">The type of object to return from queries.</typeparam>
	/// <typeparam name="TSub1">The type of the first sub-object to return from queries.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintenanceRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public sealed class DynamicConnection<TResult, TSub1> : DynamicConnection
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
			result = DoInvokeMember(
				binder,
				args,
				(cmd, doAsync) =>
				{
					// execute the command
					if (doAsync)
						return ((SqlCommand)cmd).QueryAsync<TResult, TSub1>();
					else
						return cmd.Connection.Query<TResult, TSub1>(cmd);
				});

			return true;
		}
	}
}
