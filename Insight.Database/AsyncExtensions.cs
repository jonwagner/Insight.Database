using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods to support asynchronous database operations.
	/// </summary>
	public static partial class AsyncExtensions
	{
		#region Execute Members
		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of command to execute.</param>
		/// <param name="closeConnection">True to close the connection after the query.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<int> ExecuteAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => null,
				(cmd, r) =>
				{
					// NOTE: we open the connection before creating the command because we may need to use the retry logic
					// when deriving the stored procedure parameters
					IDbCommand command = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);

#if NODBASYNC
					// Only SqlCommand supports execute async
					SqlCommand sqlCommand = command as SqlCommand;
					if (sqlCommand != null)
						return Task<int>.Factory.FromAsync(sqlCommand.BeginExecuteNonQuery(), ar => sqlCommand.EndExecuteNonQuery(ar));
					else
						return Task<int>.Factory.StartNew(() => command.ExecuteNonQuery(), ct);
#else
					// DbCommand now supports async execute
					DbCommand dbCommand = command as DbCommand;
					if (dbCommand != null)
						return dbCommand.ExecuteNonQueryAsync(ct);
					else
						return Task<int>.Factory.StartNew(() => command.ExecuteNonQuery(), ct);
#endif
				},
				closeConnection,
				ct);
		}

		/// <summary>
		/// Create a command and execute it. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="closeConnection">True to close the connection after the query.</param>
		/// <param name="commandTimeout">The timeout for the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<int> ExecuteSqlAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.ExecuteAsync(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region ExecuteScalar Members
		/// <summary>
		/// Create a command and execute it, returning the first column of the first row. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of command to execute.</param>
		/// <param name="closeConnection">True to close the connection after the query.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		/// <typeparam name="T">The type of the data to be returned.</typeparam>
		public static Task<T> ExecuteScalarAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => null,
				(cmd, r) =>
				{
					// NOTE: we open the connection before creating the command because we may need to use the retry logic
					// when deriving the stored procedure parameters
					IDbCommand command = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);

#if NODBASYNC
					// not supported in .NET 4.0
					return Task<T>.Factory.StartNew(() => (T)command.ExecuteScalar(), ct);
#else
					// DbCommand now supports async execute
					DbCommand dbCommand = command as DbCommand;
					if (dbCommand != null)
						return dbCommand.ExecuteScalarAsync(ct).ContinueWith(t => (T)t.Result, TaskContinuationOptions.ExecuteSynchronously);
					else
						return Task<T>.Factory.StartNew(() => (T)command.ExecuteScalar(), ct);
#endif
				},
				closeConnection,
				ct);
		}

		/// <summary>
		/// Create a command and execute it, returning the first column of the first row. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="closeConnection">True to close the connection after the query.</param>
		/// <param name="commandTimeout">The timeout for the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		/// <typeparam name="T">The type of the data to be returned.</typeparam>
		public static Task<T> ExecuteScalarSqlAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.ExecuteScalarAsync<T>(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region Query Connection Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<FastExpando>> QueryAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<FastExpando>(sql, parameters, Graph.Null, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) => r.ToListAsync<TResult>(withGraph, cancellationToken),
				commandBehavior,
				ct);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<FastExpando>> QuerySqlAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<FastExpando>(sql, parameters, Graph.Null, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region Single Methods
		/// <summary>
		/// Asynchronously create a command, execute it, and translate the result set into a single object or null. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<TResult> SingleAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, withGraph, commandType, commandBehavior, commandTimeout, transaction, cancellationToken)
				.ContinueWith(t => t.Result.FirstOrDefault(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Asynchronously create a command, execute it, and translate the result set into a single object or null. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<TResult> SingleSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.SingleAsync<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region Query Command Methods
		/// <summary>
		/// Run a command asynchronously and return a list of objects as FastExpandos. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<FastExpando>> QueryAsync(
			this IDbCommand command,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return command.QueryAsync<FastExpando>(Graph.Null, commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T>(
			this IDbCommand command,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return command.Connection.ExecuteAsyncAndAutoClose(
				c => command,
				(cmd, r) => r.ToListAsync<T>(withGraph, cancellationToken),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}
		#endregion

		#region Query Reader Methods
		/// <summary>
		/// Asynchronously executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optiona transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task QueryAsync(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) => { read(r); return Helpers.FalseTask; },
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optiona transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task QuerySqlAsync(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <typeparam name="T">The type returned from the reader callback.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optiona transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task<T> QueryAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) => Helpers.FromResult(read(r)),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes a query and performs a callback to read the data in the IDataReader.
		/// </summary>
		/// <typeparam name="T">The type returned from the reader callback.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="read">The reader callback.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optiona transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task<T> QuerySqlAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region QueryResults Methods
		/// <summary>
		/// Asynchronously executes an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <typeparam name="T">The type of object to return in the result set.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static Task<T> QueryResultsAsync<T>(
			this IDbCommand command,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			CancellationToken? cancellationToken = null) where T : Results, new()
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return command.Connection.ExecuteAsyncAndAutoClose(
				c => command,
				(cmd, r) => new T().ReadAsync<T>(cmd, r, withGraphs, cancellationToken),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
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
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<T> QueryResultsAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null) where T : Results, new()
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
				(cmd, r) => new T().ReadAsync<T>(cmd, r, withGraphs, cancellationToken),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T">The type of the results. This must derive from Results&lt;T&gt;.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<T> QueryResultsSqlAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null) where T : Results, new()
		{
			return connection.QueryResultsAsync<T>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2>> QueryResultsAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2>> QueryResultsSqlAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3>> QueryResultsAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3>> QueryResultsSqlAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4>> QueryResultsAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4>> QueryResultsSqlAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth data set.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5>> QueryResultsAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first data set.</typeparam>
		/// <typeparam name="T2">The type of the data in the second data set.</typeparam>
		/// <typeparam name="T3">The type of the data in the third data set.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth data set.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth data set.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5>> QueryResultsSqlAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region Translation Methods
#if NODBASYNC
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<FastExpando>> ToListAsync(this IDataReader reader, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return Task<IList<FastExpando>>.Factory.StartNew(() => reader.ToList(), ct);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <typeparam name="TResult">The type of object to deserialize from the reader.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<TResult>> ToListAsync<TResult>(this IDataReader reader, Type withGraph = null, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return Task<IList<TResult>>.Factory.StartNew(() => reader.ToList<TResult>(withGraph), ct);
		}
#else
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <param name="reader">The data reader to read from.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<FastExpando>> ToListAsync(this IDataReader reader, CancellationToken? cancellationToken = null)
		{
			return reader.ToListAsync<FastExpando>(Graph.Null, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="reader">The data reader to read from.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static async Task<IList<TResult>> ToListAsync<TResult>(this IDataReader reader, Type withGraph = null, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;
			ct.ThrowIfCancellationRequested();

			// only DbReader supports async read
			DbDataReader dbReader = reader as DbDataReader;
			if (dbReader == null)
				return reader.ToList<TResult>();

			// get the mapper for the records
			var mapper = DbReaderDeserializer.GetDeserializer<TResult>(dbReader, withGraph);

			bool moreResults = false;

			try
			{
				IList<TResult> list = new List<TResult>();

				// read in all of the records
				while (await dbReader.ReadAsync(ct).ConfigureAwait(false))
				{
					list.Add(mapper(dbReader));

					// allow cancellation to occur within the list processing
					ct.ThrowIfCancellationRequested();
				}

				// move to the next result set - the token should already be here
				moreResults = await dbReader.NextResultAsync(ct).ConfigureAwait(false);

				return list;
			}
			finally
			{
				// clean up the reader unless there are more results
				if (!moreResults)
					reader.Dispose();
			}
		}
#endif
		#endregion

		#region Insert Methods
		/// <summary>
		/// Asynchronously executes the specified query and merges the results into the specified existing object.
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
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task whose completion is the object after merging the results.</returns>
		public static Task<TResult> InsertAsync<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
				(cmd, r) => r.MergeAsync(inserted, cancellationToken),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes the specified query and merges the results into the specified existing object.
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
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task whose completion is the object after merging the results.</returns>
		public static Task<TResult> InsertSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.InsertAsync(sql, inserted, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes the specified query and merges the results into the specified existing object.
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
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task whose completion is the list of objects after merging the results.</returns>
		public static Task<IEnumerable<TResult>> InsertListAsync<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return connection.ExecuteAsyncAndAutoClose(
				c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
				(cmd, r) => r.MergeAsync(inserted, cancellationToken),
				commandBehavior.HasFlag(CommandBehavior.CloseConnection),
				ct);
		}

		/// <summary>
		/// Asynchronously executes the specified query and merges the results into the specified existing object.
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
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task whose completion is the list of objects after merging the results.</returns>
		public static Task<IEnumerable<TResult>> InsertListSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.InsertListAsync(sql, inserted, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}
		#endregion

		#region Merge Methods
#if NODBASYNC
		/// <summary>
		/// Merges the results of a recordset into an existing object.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="item">The item to merge into.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>The item that has been merged.</returns>
		/// <remarks>
		/// This method reads a single record from the reader and overwrites the values of the object.
		/// The reader is then advanced to the next result or disposed.
		/// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
		/// </remarks>
		public static Task<T> MergeAsync<T>(this IDataReader reader, T item, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return Task<T>.Factory.StartNew(() => reader.Merge(item), ct);
		}

		/// <summary>
		/// Merges the results of a recordset into an existing object.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="items">The list of items to merge into.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>The item that has been merged.</returns>
		/// <remarks>
		/// This method reads a single record from the reader and overwrites the values of the object.
		/// The reader is then advanced to the next result or disposed.
		/// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
		/// </remarks>
		public static Task<IEnumerable<T>> MergeAsync<T>(this IDataReader reader, IEnumerable<T> items, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return Task<IEnumerable<T>>.Factory.StartNew(() => reader.Merge(items), ct);
		}
#else
		/// <summary>
		/// Merges the results of a recordset into an existing object.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="item">The item to merge into.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>The item that has been merged.</returns>
		/// <remarks>
		/// This method reads a single record from the reader and overwrites the values of the object.
		/// The reader is then advanced to the next result or disposed.
		/// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
		/// </remarks>
		public static async Task<T> MergeAsync<T>(this IDataReader reader, T item, CancellationToken? cancellationToken = null)
		{
			await reader.MergeAsync<T>(new T[] { item }, cancellationToken).ConfigureAwait(false);

			return item;
		}

		/// <summary>
		/// Merges the results of a recordset into an existing object.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="items">The list of items to merge into.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>The item that has been merged.</returns>
		/// <remarks>
		/// This method reads a single record from the reader and overwrites the values of the object.
		/// The reader is then advanced to the next result or disposed.
		/// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
		/// </remarks>
		public static async Task<IEnumerable<T>> MergeAsync<T>(this IDataReader reader, IEnumerable<T> items, CancellationToken? cancellationToken = null)
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;
			ct.ThrowIfCancellationRequested();

			bool moreResults = false;

			try
			{
				// see if the reader support async reads
				DbDataReader dbReader = reader as DbDataReader;
				if (dbReader == null)
					return reader.Merge(items);

				var merger = DbReaderDeserializer.GetMerger<T>(reader);

				// read the identities of each item from the recordset and merge them into the objects
				foreach (T item in items)
				{
					await dbReader.ReadAsync(ct).ConfigureAwait(false);

					ct.ThrowIfCancellationRequested();

					merger(reader, item);
				}

				// we are done with this result set, so move onto the next or clean up the reader
				moreResults = await dbReader.NextResultAsync(ct).ConfigureAwait(false);

				return items;
			}
			finally
			{
				if (!moreResults)
					reader.Dispose();
			}
		}
#endif
		#endregion

		#region GetReader Methods
		/// <summary>
		/// Executes a command and returns a task that generates a SqlDataReader. This method does not support auto-open.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The behavior for the command.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a SqlDataReader upon completion.</returns>
		internal static Task<IDataReader> GetReaderAsync(this IDbCommand command, CommandBehavior commandBehavior, CancellationToken cancellationToken)
		{
#if NODBASYNC
			// Only SqlCommand supports async
			SqlCommand sqlCommand = command as SqlCommand;
			if (sqlCommand != null)
				return Task<IDataReader>.Factory.FromAsync(sqlCommand.BeginExecuteReader(commandBehavior), ar => sqlCommand.EndExecuteReader(ar));

			// allow reliable commands to handle the icky task logic
			ReliableCommand reliableCommand = command as ReliableCommand;
			if (reliableCommand != null)
				return reliableCommand.GetReaderAsync(commandBehavior, cancellationToken);
#else
			// DbCommand now supports async
			DbCommand dbCommand = command as DbCommand;
			if (dbCommand != null)
				return dbCommand.ExecuteReaderAsync(commandBehavior, cancellationToken).ContinueWith(t => (IDataReader)t.Result, TaskContinuationOptions.ExecuteSynchronously);
#endif

			// the command doesn't support async so stick it in a dumb task
			return Task<IDataReader>.Factory.StartNew(() => command.ExecuteReader(commandBehavior), cancellationToken);
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="getCommand">An action to perform to get the command to execute.</param>
		/// <param name="translate">An action to perform to translate the reader into results.</param>
		/// <param name="closeConnection">True to force the connection to close after the operation completes.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(
			this IDbConnection connection,
			Func<IDbConnection, IDbCommand> getCommand,
			Func<IDbCommand, IDataReader, Task<T>> translate,
			bool closeConnection,
			CancellationToken cancellationToken)
		{
			return connection.ExecuteAsyncAndAutoClose<T>(getCommand, translate, closeConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken);
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="getCommand">An action to perform to get the command to execute.</param>
		/// <param name="translate">An action to perform to translate the reader into results.</param>
		/// <param name="commandBehavior">The CommandBehavior to use to execute the command.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(
			this IDbConnection connection,
			Func<IDbConnection, IDbCommand> getCommand,
			Func<IDbCommand, IDataReader, Task<T>> translate,
			CommandBehavior commandBehavior,
			CancellationToken cancellationToken)
		{
			bool closeConnection = commandBehavior.HasFlag(CommandBehavior.CloseConnection);
			IDbCommand command = null;
			IDataReader reader = null;

			return AutoOpenAsync(connection, cancellationToken)
				.ContinueWith(
					t =>
					{
						// this needs to run even if the open has been cancelled so we don't leak a connection
						closeConnection |= t.Result;

						// if the operation has been cancelled, throw after we know that the connection has been opened
						// but before taking the action
						cancellationToken.ThrowIfCancellationRequested();

						// get the command
						command = getCommand(connection);

						// if we have a command, execute it
						if (command != null)
							return command.GetReaderAsync(commandBehavior | CommandBehavior.SequentialAccess, cancellationToken);

						return Helpers.FromResult<IDataReader>(null);
					},
					TaskContinuationOptions.ExecuteSynchronously)
				.Unwrap()
				.ContinueWith(
					t =>
					{
						reader = t.Result;
						return translate(command, reader);
					},
					TaskContinuationOptions.ExecuteSynchronously)
				.Unwrap()
				.ContinueWith(
					t =>
					{
						if (reader != null)
							reader.Dispose();

						// close before accessing the result so we can guarantee that the connection doesn't leak
						if (closeConnection)
							connection.Close();

						return t.Result;
					},
					TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Detect if a connection needs to be automatically opened and closed.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>
		/// A task representing the completion of the open operation
		/// and a flag indicating whether the connection should be closed at the end of the operation.
		/// </returns>
		private static Task<bool> AutoOpenAsync(IDbConnection connection, CancellationToken cancellationToken)
		{
			// if the connection is already open, then it doesn't need to be opened or closed.
			if (connection.State == ConnectionState.Open)
				return Helpers.FalseTask;

#if !NODBASYNC
			// open the connection and plan to close it
			DbConnection dbConnection = connection.UnwrapDbConnection();
			if (dbConnection != null)
			{
				return dbConnection.OpenAsync(cancellationToken).ContinueWith(
					t =>
					{
						// call wait on the task to re-throw any connection errors
						// otherwise we just get a task cancelled error
						t.Wait();

						return dbConnection.State == ConnectionState.Open;
					},
					TaskContinuationOptions.ExecuteSynchronously);
			}
#endif

			// we don't have an asynchronous open method, so do it synchronously in a task
			return Task<bool>.Factory.StartNew(
				() =>
				{
					// synchronous open is not cancellable on its own
					connection.Open();

					// since we opened the connection, we should close it
					return true;
				},
				cancellationToken);
		}
		#endregion
	}
}
