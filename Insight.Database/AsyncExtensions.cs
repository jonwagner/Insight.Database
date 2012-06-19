using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods to support asynchronous database operations.
	/// </summary>
	public static class AsyncExtensions
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
		/// <returns>A data reader with the results.</returns>
		public static Task<int> ExecuteAsync(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			IDbCommand command = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);

			return command.ExecuteAsync(closeConnection);
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
		/// <returns>A data reader with the results.</returns>
		public static Task<int> ExecuteSqlAsync(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsync(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction);
		}

		/// <summary>
		/// Executes a command and returns the result. This method supports auto-open.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="closeConnection">True to close the connection after the operation is complete.</param>
		/// <returns>A task that returns a SqlDataReader upon completion.</returns>
		public static Task<int> ExecuteAsync(this IDbCommand command, bool closeConnection = false)
		{
			return command.Connection.ExecuteAsyncAndAutoClose(
				c =>
				{
#if NODBASYNC
					// Only SqlCommand supports execute async
					SqlCommand sqlCommand = command as SqlCommand;
					if (sqlCommand != null)
						return Task<int>.Factory.FromAsync(sqlCommand.BeginExecuteNonQuery(), ar => sqlCommand.EndExecuteNonQuery(ar));
					else
						return Task<int>.Factory.StartNew(() => command.ExecuteNonQuery());
#else
					// DbCommand now supports async execute
					DbCommand dbCommand = command as DbCommand;
					if (dbCommand != null)
						return dbCommand.ExecuteNonQueryAsync();
					else
						return Task<int>.Factory.StartNew(() => command.ExecuteNonQuery());
#endif
				},
				closeConnection);
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
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<FastExpando>> QueryAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList(),
				commandBehavior);
		}

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
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, TSub1>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, TSub1, TSub2>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type of object to return as subobject 5.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
				commandBehavior);
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
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<FastExpando>> QuerySqlAsync(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, TSub1>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, TSub1, TSub2>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type of object to return as subobject 5.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
				commandBehavior);
		}
		#endregion

		#region Query Command Methods
		/// <summary>
		/// Run a command asynchronously and return a list of objects as FastExpandos. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<FastExpando>> QueryAsync(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T>(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, TSub1>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T, TSub1>(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, TSub1, TSub2>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T, TSub1, TSub2>(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, TSub1, TSub2, TSub3>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T, TSub1, TSub2, TSub3>(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, TSub1, TSub2, TSub3, TSub4>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T, TSub1, TSub2, TSub3, TSub4>(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type of object to return as subobject 5.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, TSub1, TSub2, TSub3, TSub4, TSub5>(this IDbCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(commandBehavior).ToList<T, TSub1, TSub2, TSub3, TSub4, TSub5>(),
				commandBehavior);
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task QueryAsync(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction)
					.ContinueWith(t => { read(t.Result); return false; }),
				commandBehavior);
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task QuerySqlAsync(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction)
					.ContinueWith(t => { read(t.Result); return false; }),
				commandBehavior);
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task<T> QueryAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, commandType, commandBehavior, commandTimeout, transaction)
					.ContinueWith(t => read(t.Result)),
				commandBehavior);
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static Task<T> QuerySqlAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction)
					.ContinueWith(t => read(t.Result)),
				commandBehavior);
		}
		#endregion

		#region Translation Methods
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<FastExpando>> ToList(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T>(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T, TSub1>(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1, TSub2>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T, TSub1, TSub2>(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1, TSub2, TSub3>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T, TSub1, TSub2, TSub3>(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1, TSub2, TSub3, TSub4>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T, TSub1, TSub2, TSub3, TSub4>(), TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type of object to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type of object to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type of object to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type of object to return as subobject 5.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1, TSub2, TSub3, TSub4, TSub5>(this Task<IDataReader> task)
		{
			// Continue the task with a translation. Run it synchronously so we consume the data ASAP and release the reader
			return task.ContinueWith(t => t.Result.ToList<T, TSub1, TSub2, TSub3, TSub4, TSub5>(), TaskContinuationOptions.ExecuteSynchronously);
		}
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
		/// <returns>A task whose completion is the object after merging the results.</returns>
		public static Task<TResult> InsertAsync<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters ?? inserted, commandType, commandBehavior, commandTimeout, transaction)
						.ContinueWith(t => t.Result.Merge(inserted)),
				commandBehavior);
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
		/// <returns>A task whose completion is the object after merging the results.</returns>
		public static Task<TResult> InsertSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			TResult inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters ?? inserted, CommandType.Text, commandBehavior, commandTimeout, transaction)
						.ContinueWith(t => t.Result.Merge(inserted)),
				commandBehavior);
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
		/// <returns>A task whose completion is the list of objects after merging the results.</returns>
		public static Task<IEnumerable<TResult>> InsertListAsync<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters ?? inserted, commandType, commandBehavior, commandTimeout, transaction)
						.ContinueWith(t => t.Result.Merge(inserted)),
				commandBehavior);
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
		/// <returns>A task whose completion is the list of objects after merging the results.</returns>
		public static Task<IEnumerable<TResult>> InsertListSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			IEnumerable<TResult> inserted,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAsyncAndAutoClose(
				c => c.GetReaderAsync(sql, parameters ?? inserted, CommandType.Text, commandBehavior, commandTimeout, transaction)
						.ContinueWith(t => t.Result.Merge(inserted)),
				commandBehavior);
		}
		#endregion

		#region GetReader Methods
		/// <summary>
		/// Executes a command and returns a task that generates a SqlDataReader. This method does not support auto-open.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The behavior for the command.</param>
		/// <returns>A task that returns a SqlDataReader upon completion.</returns>
		internal static Task<IDataReader> GetReaderAsync(this IDbCommand command, CommandBehavior commandBehavior = CommandBehavior.Default)
		{
#if NODBASYNC
			// Only SqlCommand supports async
			SqlCommand sqlCommand = command as SqlCommand;
			if (sqlCommand != null)
				return Task<IDataReader>.Factory.FromAsync(sqlCommand.BeginExecuteReader(commandBehavior), ar => sqlCommand.EndExecuteReader(ar));

			// allow reliable commands to handle the icky task logic
			ReliableCommand reliableCommand = command as ReliableCommand;
			if (reliableCommand != null)
				return reliableCommand.GetReaderAsync(commandBehavior);
#else
			// DbCommand now supports async
			DbCommand dbCommand = command as DbCommand;
			if (dbCommand != null)
				return dbCommand.ExecuteReaderAsync(commandBehavior).ContinueWith(t => (IDataReader)t.Result);
#endif

			// the command doesn't support async so stick it in a dumb task
			return Task<IDataReader>.Factory.StartNew(() => command.ExecuteReader(commandBehavior));
		}

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
		private static Task<IDataReader> GetReaderAsync(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction).GetReaderAsync(commandBehavior);
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(this IDbConnection connection, Func<IDbConnection, Task<T>> action, bool closeConnection)
		{
			return AutoOpenAsync(connection)
				.ContinueWith(t =>
				{
					closeConnection |= t.Result;
					return action(connection);
				})
				.Unwrap()
				.ContinueWith(t =>
				{
					if (closeConnection)
						connection.Close();

					return t.Result;
				});
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="commandBehavior">The commandbehavior of the command.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(this IDbConnection connection, Func<IDbConnection, Task<T>> action, CommandBehavior commandBehavior)
		{
			return connection.ExecuteAsyncAndAutoClose(action, commandBehavior.HasFlag(CommandBehavior.CloseConnection));
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="command">The command to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="commandBehavior">The commandbehavior of the command.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(this IDbCommand command, Func<IDbCommand, Task<T>> action, CommandBehavior commandBehavior)
		{
			return command.ExecuteAsyncAndAutoClose(action, commandBehavior.HasFlag(CommandBehavior.CloseConnection));
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="command">The command to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> ExecuteAsyncAndAutoClose<T>(this IDbCommand command, Func<IDbCommand, Task<T>> action, bool closeConnection)
		{
			return command.Connection.ExecuteAsyncAndAutoClose(_ => action(command), closeConnection);
		}

		/// <summary>
		/// Detect if a connection needs to be automatically opened and closed.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <returns>
		/// A task representing the completion of the open operation
		/// and a flag indicating whether the connection should be closed at the end of the operation.
		/// </returns>
		private static Task<bool> AutoOpenAsync(IDbConnection connection)
		{
			// if the connection is already open, then it doesn't need to be opened or closed.
			if (connection.State == ConnectionState.Open)
			{
				return Task<bool>.Factory.StartNew(() => false);
			}
			else
			{
#if !NODBASYNC
				// open the connection and plan to close it
				DbConnection dbConnection = connection.UnwrapDbConnection();
				if (dbConnection != null)
					return dbConnection.OpenAsync().ContinueWith(t => { t.Wait(); return true; });
#endif

				return Task<bool>.Factory.StartNew(() => { connection.Open(); return true; });
			}
		}
		#endregion
	}
}
