using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for DbConnection to make it easier to call the database.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	public static partial class DBConnectionExtensions
	{
		#region Private Fields
		/// <summary>
		/// A cache of the table schemas used for bulk copy.
		/// </summary>
		private static ConcurrentDictionary<Tuple<string, Type>, ObjectReader> _tableReaders = new ConcurrentDictionary<Tuple<string, Type>, ObjectReader>();
		#endregion

		#region Open Methods
		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="connection">The connection to open and return.</param>
		/// <typeparam name="T">The type of database connection.</typeparam>
		/// <returns>The opened connection.</returns>
		public static T OpenConnection<T>(this T connection) where T : IDbConnection
		{
			connection.Open();
			return connection;
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <typeparam name="T">The type of database connection.</typeparam>
		/// <param name="connection">The connection to open and return.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenConnectionAsync<T>(this T connection, CancellationToken? cancellationToken = null) where T : IDbConnection
		{
			CancellationToken ct = cancellationToken ?? CancellationToken.None;

#if NODBASYNC
			connection.Open();
			return Helpers.FromResult(connection);
#else
			DbConnection dbConnection = connection as DbConnection;

			// if the connection is not a DbConnection, then open it synchronously
			if (dbConnection == null)
			{
				connection.Open();
				return Helpers.FromResult(connection);
			}

			// DbConnection supports OpenAsync, but it doesn't return self
			return dbConnection.OpenAsync(ct)
					.ContinueWith(t => connection, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
#endif
		}

		/// <summary>
		/// Opens and returns a database connection that implmements the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <returns>The connection implementing the interface.</returns>
		public static T OpenAs<T>(this IDbConnection connection) where T : class, IDbConnection
		{
			if (connection == null) throw new ArgumentNullException("connection");

			connection.Open();
			return connection.As<T>();
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection that implmements the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning the connection and interface when the connection is opened.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Task<T> OpenAsAsync<T>(this IDbConnection connection, CancellationToken? cancellationToken = null) where T : class, IDbConnection
		{
			return OpenConnectionAsync(connection, cancellationToken)
					.ContinueWith(
						t =>
						{
							// need to catch and close the connection if the As<T> cast fails.
							var c = t.Result;
							var disposable = connection;
							try
							{
								var result = c.As<T>();
								disposable = null;
								return result;
							}
							finally
							{
								if (disposable != null)
									disposable.Dispose();
							}
						},
						TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <param name="connection">The connection to open.</param>
		/// <returns>A wrapper for the database connection.</returns>
		public static DbConnectionWrapper OpenWithTransaction(this IDbConnection connection)
		{
			// if the connection isn't wrapped, we need to wrap it
			DbConnectionWrapper wrapper = DbConnectionWrapper.Wrap(connection);
			wrapper.Open();
			return wrapper.BeginAutoTransaction();
		}

		/// <summary>
		/// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <returns>A wrapper for the database connection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static T OpenWithTransactionAs<T>(this T connection) where T : class, IDbConnection, IDbTransaction
		{
			// connection is already a T, so pass it in unwrapped
			// a connection won't leak from this code because the connection is already a T and should come back as a T
			// and there is no code between the construction and the return other than the cast
			return (T)(object)((IDbConnection)connection).OpenWithTransaction();
		}

		/// <summary>
		/// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <returns>A wrapper for the database connection.</returns>
		public static T OpenWithTransactionAs<T>(this IDbConnection connection) where T : class, IDbConnection, IDbTransaction
		{
			// convert to interface first, then open, so we only get one layer of wrapping
			return connection.As<T>().OpenWithTransactionAs();
		}

		/// <summary>
		/// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <param name="connection">The connection to open.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning a connection when the connection has been opened.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this IDbConnection connection, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = cancellationToken ?? CancellationToken.None;

			return DbConnectionWrapper.Wrap(connection)
				.OpenConnectionAsync(ct)
				.ContinueWith(
					t =>
					{
						// since beginautotransaction can fail, we may need to clean up the connection
						var disposable = t.Result;
						try
						{
							var result = disposable.BeginAutoTransaction();
							disposable = null;
							return result;
						}
						finally
						{
							if (disposable != null)
								disposable.Dispose();
						}
					},
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning a connection when the connection has been opened.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Task<T> OpenWithTransactionAsAsync<T>(this T connection, CancellationToken? cancellationToken = null) where T : class, IDbConnection, IDbTransaction
		{
			// connection is already a T, so just pass it in
			return OpenWithTransactionAsync((IDbConnection)connection, cancellationToken)
					.ContinueWith(
						t =>
						{
							// since c.As<T> can fail, we may need to clean up the connection
							var disposable = t.Result;
							try
							{
								var result = disposable.As<T>();
								disposable = null;
								return result;
							}
							finally
							{
								if (disposable != null)
									disposable.Dispose();
							}
						},
						TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="connection">The connection to open.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning a connection when the connection has been opened.</returns>
		public static Task<T> OpenWithTransactionAsAsync<T>(this IDbConnection connection, CancellationToken? cancellationToken = null) where T : class, IDbConnection, IDbTransaction
		{
			// convert to interface first, then open, so we only get one layer of wrapping
			return connection.As<T>().OpenWithTransactionAsAsync(cancellationToken);
		}
		#endregion

		#region Create Command Members
		/// <summary>
		/// Create a DbCommand for a given Sql and parameters. This method does not support auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">Object containing the parameters to send to the database.</param>
		/// <param name="commandType">The type of the command text.</param>
		/// <param name="commandTimeout">Optinal command timeout to use.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>An IDbCommand that can be executed on the connection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
		public static IDbCommand CreateCommand(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// create a db command
			IDbCommand cmd = connection.CreateCommand();
			cmd.CommandType = commandType;
			cmd.CommandText = sql;

			// unwrap the transaction because the transaction has to match the command and connection
			if (transaction != null)
				cmd.Transaction = UnwrapDbTransaction(transaction);
			if (commandTimeout != null)
				cmd.CommandTimeout = commandTimeout.Value;

			// add the parameters to the command
			cmd.AddParameters(parameters);

			return cmd;
		}

		/// <summary>
		/// Create a DbCommand for a given Sql and parameters. This method does not support auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">Object containing the parameters to send to the database.</param>
		/// <param name="commandTimeout">Optinal command timeout to use.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>An IDbCommand that can be executed on the connection.</returns>
		public static IDbCommand CreateCommandSql(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.CreateCommand(sql, parameters, CommandType.Text, commandTimeout, transaction);
		}
		#endregion

		#region Execute Methods
		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="closeConnection">True to auto-close the connection on completion.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static int Execute(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => null,
				(_, __) =>
				{
					using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
					{
						var result = cmd.ExecuteNonQuery();
						if (outputParameters != null)
							cmd.OutputParameters(outputParameters);
						return result;
					}
				},
				closeConnection);
		}

		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="closeConnection">True to auto-close the connection when complete.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static int ExecuteSql(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Execute(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region ExecuteScalar Methods
		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The return type of the object.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="closeConnection">True to auto-close the connection upon completion.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static T ExecuteScalar<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => null,
				(_, __) =>
				{
					using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
					{
						var result = (T)cmd.ExecuteScalar();
						if (outputParameters != null)
							cmd.OutputParameters(outputParameters);
						return result;
					}
				},
				closeConnection);
		}

		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="closeConnection">True to auto-close connection on completion.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static T ExecuteScalarSql<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteScalar<T>(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region GetReader Methods
		/// <summary>
		/// Create a command and execute it. This method does not support auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static IDataReader GetReader(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
				return cmd.ExecuteReader(commandBehavior);
		}

		/// <summary>
		/// Create a Sql Text command and execute it. This method does not support auto-open.
		/// </summary>
		/// <remarks>This is equivalent to calling Query with commandType = CommandType.Text.</remarks>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>		
		public static IDataReader GetReaderSql(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction);
		}
		#endregion

		#region Query Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set into a FastExpando. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<FastExpando> Query(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<FastExpando>(sql, parameters, Graph.Null, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					var result = r.ToList<TResult>(withGraph);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return result;
				},
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set into a FastExpando. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<FastExpando> QuerySql(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<FastExpando>(sql, parameters, Graph.Null, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region Single Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set into a single object or null. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static TResult Single<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, withGraph, commandType, commandBehavior, commandTimeout, transaction, outputParameters).FirstOrDefault();
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set into a single object or null. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static TResult SingleSql<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters).FirstOrDefault();
		}
		#endregion

		#region Query With Read Callback Methods
		/// <summary>
		/// Executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		public static void Query(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					read(r);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return false;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		public static void QuerySql(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <typeparam name="T">The type of object returned from the reader callback.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static T Query<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					var result = read(r);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return result;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <typeparam name="T">The type of object returned from the reader callback.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static T QuerySql<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region QueryResults Methods
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T">The type of the results. This must derive from Results&lt;T&gt;.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static T QueryResults<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null) where T : Results, new()
		{
			return connection.ExecuteAndAutoClose(
				c => connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					T results = new T();
					results.Read(cmd, r, withGraphs);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);

					return results;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results QueryResults(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T">The type of the results. This must derive from Results&lt;T&gt;.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static T QueryResultsSql<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null) where T : Results, new()
		{
			return connection.QueryResults<T>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results QueryResultsSql(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region ForEach Methods
		/// <summary>
		/// Executes a query and performs an action for each item in the result.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="action">The reader callback.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEach(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<dynamic> action,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			// we don't use this parameter, but it is necessary to have so the compiler can infer the proper method signature to use
			if (withGraph != null)
				throw new ArgumentException("withGraph should be null for returning dynamic objects.", "withGraph");

			connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					foreach (dynamic expando in r.AsEnumerable())
						action(expando);

					return false;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes a query and performs an action for each item in the result.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="action">The reader callback.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEachSql(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<dynamic> action,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ForEach(sql, parameters, action, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction);
		}

		/// <summary>
		/// Executes a query and performs an action for each item in the result.
		/// </summary>
		/// <typeparam name="T">The type of object to read.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="action">The reader callback.</param>
		/// <param name="withGraph">The type of graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEach<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<T> action,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					foreach (T t in r.AsEnumerable<T>(withGraph))
						action(t);

					return false;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes a query and performs an action for each item in the result.
		/// </summary>
		/// <typeparam name="T">The type of object to read.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="action">The reader callback.</param>
		/// <param name="withGraph">The type of graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEachSql<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<T> action,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ForEach<T>(sql, parameters, action, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction);
		}
		#endregion

		#region Dynamic Invocation Helper
#if !NODYNAMIC
		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <returns>A DynamicConnection using the given connection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is intended to return a connection")]
		public static dynamic Dynamic(this IDbConnection connection)
		{
			return new DynamicConnection(connection);
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <typeparam name="T">The type of object to return from queries.</typeparam>
		/// <returns>A DynamicConnection using the given connection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is intended to return a connection")]
		public static dynamic Dynamic<T>(this IDbConnection connection)
		{
			return new DynamicConnection<T>(connection);
		}
#endif
		#endregion

		#region Insert Members
		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is commonly used to retrieve identity values from the database upon an insert.
		/// The result set is expected to contain one record that is merged into the object upon return.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="inserted">
		/// The object that is being inserted and should be merged.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The object after merging the results.</returns>
		public static TResult Insert<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					var result = r.Merge(inserted);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return result;
				},
				commandBehavior);

			return inserted;
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is commonly used to retrieve identity values from the database upon an insert.
		/// The result set is expected to contain one record that is merged into the object upon return.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="inserted">
		/// The object that is being inserted and should be merged.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The object after merging the results.</returns>
		public static TResult InsertSql<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Insert<TResult>(sql, inserted, parameters, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is commonly used to retrieve identity values from the database upon an insert.
		/// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="inserted">
		/// The list of objects that is being inserted and should be merged.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The list of objects after merging the results.</returns>
		public static IEnumerable<TResult> InsertList<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
				(cmd, r) =>
				{
					var result = r.Merge(inserted);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return result;
				},
				commandBehavior);
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is commonly used to retrieve identity values from the database upon an insert.
		/// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="inserted">
		/// The list of objects that is being inserted and should be merged.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The list of objects after merging the results.</returns>
		public static IEnumerable<TResult> InsertListSql<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters ?? inserted, CommandType.Text, commandTimeout, transaction),
				(cmd, r) =>
				{
					var result = r.Merge(inserted);
					if (outputParameters != null)
						cmd.OutputParameters(outputParameters);
					return result;
				},
				commandBehavior);
		}
		#endregion

		#region QueryOnto Members
		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is the same behavior as Insert.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="onto">
		/// The list of objects to be merged onto.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The object after merging the results.</returns>
		public static TResult QueryOnto<TResult>(
			this IDbConnection connection,
			string sql,
			TResult onto,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Insert(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is the same behavior as Insert.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="onto">
		/// The list of objects to be merged onto.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The object after merging the results.</returns>
		public static TResult QueryOntoSql<TResult>(
			this IDbConnection connection,
			string sql,
			TResult onto,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.InsertSql<TResult>(sql, onto, parameters, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is the same behavior as Insert.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="onto">
		/// The list of objects to be merged onto.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The list of objects after merging the results.</returns>
		public static IEnumerable<TResult> QueryOntoList<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> onto,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.InsertList(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes the specified query and merges the results into the specified existing object.
		/// This is the same behavior as Insert.
		/// </summary>
		/// <typeparam name="TResult">The type of the object to merge into.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="onto">
		/// The list of objects to be merged onto.
		/// If null, then the results are merged into the parameters object.
		/// </param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The list of objects after merging the results.</returns>
		public static IEnumerable<TResult> QueryOntoListSql<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> onto,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.InsertListSql(sql, onto, parameters, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		#endregion

		#region Interface Members
		/// <summary>
		/// Uses a DbConnection to implement an interface. Calls to the interface are automatically mapped to stored procedure calls.
		/// </summary>
		/// <typeparam name="T">The interface type to implmement.</typeparam>
		/// <param name="connection">The connection to use for database calls.</param>
		/// <returns>An implementation of the interface that executes database calls.</returns>
		public static T As<T>(this IDbConnection connection) where T : class
		{
			// if the connection already supports T, then return it, otherwise we need a wrapper
			return (connection as T) ?? (T)InterfaceGenerator.GetImplementorOf(connection, typeof(T));
		}
		#endregion

		#region Bulk Copy Members
		/// <summary>
		/// Bulk copy a list of objects to the server. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of the objects.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="list">The list of objects.</param>
		/// <param name="configure">A callback for initialization of the BulkCopy object. The object is provider-dependent.</param>
		/// <param name="closeConnection">True to close the connection when complete.</param>
		/// <param name="options">The options to use for the bulk copy.</param>
		/// <param name="transaction">An optional external transaction.</param>
		/// <returns>Number of rows copied.</returns>
		public static int BulkCopy<T>(
			this IDbConnection connection,
			string tableName,
			IEnumerable<T> list,
			Action<InsightBulkCopy> configure = null,
			bool closeConnection = false,
			InsightBulkCopyOptions options = InsightBulkCopyOptions.Default,
			IDbTransaction transaction = null)
		{
			var provider = InsightDbProvider.For(connection);

			// see if there are any invalid bulk copy options set
			var invalidOptions = (options & ~(provider.SupportedBulkCopyOptions));
			if (invalidOptions != 0)
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "BulkCopyOption {0} is not supported for this provider", invalidOptions));

			try
			{
				connection.DetectAutoOpen(ref closeConnection);

				// see if we already have a mapping for the given table name and type
				// we can't use the schema mapping cache because we don't have the schema yet, just the name of the table
				var key = Tuple.Create<string, Type>(tableName, typeof(T));
				ObjectReader fieldReaderData = _tableReaders.GetOrAdd(
					key,
					t =>
					{
						// select a 0 row result set so we can determine the schema of the table
						using (var sqlReader = connection.GetReaderSql(
							provider.GetTableSchemaSql(connection, tableName),
							commandBehavior: CommandBehavior.SchemaOnly,
							transaction: transaction))
							return ObjectReader.GetObjectReader(connection.CreateCommand(), sqlReader, typeof(T));
					});

				// create a reader for the list
				using (var reader = new ObjectListDbDataReader(fieldReaderData, list))
				{
					return provider.BulkCopy(connection, tableName, reader, configure, options, transaction);
				}
			}
			finally
			{
				if (closeConnection)
					connection.Close();
			}
		}
		#endregion

		#region Unwrap Methods
		/// <summary>
		/// Unwraps an IDbTransaction to determine its inner DbTransaction to use with advanced features.
		/// </summary>
		/// <param name="transaction">The transaction to unwrap.</param>
		/// <returns>The inner DbTransaction.</returns>
		internal static DbTransaction UnwrapDbTransaction(this IDbTransaction transaction)
		{
			if (transaction == null)
				return null;

			// if we have a DbTransaction, use it
			DbTransaction dbTransaction = transaction as DbTransaction;
			if (dbTransaction != null)
				return dbTransaction;

			// if we have a wrapped transaction, unwrap it
			DbConnectionWrapper wrapper = transaction as DbConnectionWrapper;
			if (wrapper != null)
				return wrapper.InnerTransaction.UnwrapDbTransaction();

			// there is no inner transaction
			return null;
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Detect if a connection needs to be automatically opened and closed.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <param name="closeConnection">The closeConnection parameter to modify.</param>
		internal static void DetectAutoOpen(this IDbConnection connection, ref bool closeConnection)
		{
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
				closeConnection = true;
			}
		}

		/// <summary>
		/// Executes an action on a connection, then automatically closes the connection if necessary.
		/// </summary>
		/// <typeparam name="T">The return type of the action.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="getCommand">The action to perform to get the command to execute.</param>
		/// <param name="translate">The action to perform to translate a command and reader into results.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>The result of the action.</returns>
		internal static T ExecuteAndAutoClose<T>(
			this IDbConnection connection,
			Func<IDbConnection, IDbCommand> getCommand,
			Func<IDbCommand, IDataReader, T> translate,
			bool closeConnection)
		{
			return connection.ExecuteAndAutoClose<T>(getCommand, translate, closeConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
		}

		/// <summary>
		/// Executes an action on a connection, then automatically closes the connection if necessary.
		/// </summary>
		/// <typeparam name="T">The return type of the action.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="getCommand">The action to perform to get the command to execute.</param>
		/// <param name="translate">The action to perform to translate a command and reader into results.</param>
		/// <param name="commandBehavior">The CommandBehavior to use for the query.</param>
		/// <returns>The result of the action.</returns>
		internal static T ExecuteAndAutoClose<T>(
			this IDbConnection connection,
			Func<IDbConnection, IDbCommand> getCommand,
			Func<IDbCommand, IDataReader, T> translate,
			CommandBehavior commandBehavior)
		{
			IDataReader reader = null;
			bool closeConnection = commandBehavior.HasFlag(CommandBehavior.CloseConnection);

			try
			{
				DetectAutoOpen(connection, ref closeConnection);

				// generate the command
				var command = getCommand(connection);

				// if the command is not null, then automatically generate the reader
				if (command != null)
					reader = command.ExecuteReader(commandBehavior | CommandBehavior.SequentialAccess);

				return translate(command, reader);
			}
			finally
			{
				if (reader != null)
					reader.Dispose();

				if (closeConnection)
					connection.Close();
			}
		}
		#endregion
	}
}
