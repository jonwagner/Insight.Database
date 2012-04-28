using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public static Task<int> AsyncExecute(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c =>
				{
					SqlCommand command = (SqlCommand)c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);

					return Task<int>.Factory.FromAsync(command.BeginExecuteNonQuery(), ar => command.EndExecuteNonQuery(ar));
				}, 
				closeConnection);
		}
	
		/// <summary>
		/// Executes a command and returns the result. This method supports auto-open.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="closeConnection">True to close the connection after the operation is complete.</param>
		/// <returns>A task that returns a SqlDataReader upon completion.</returns>
		public static Task<int> AsyncExecute(this SqlCommand command, bool closeConnection = false)
		{
			DBConnectionExtensions.DetectAutoOpen(command.Connection, ref closeConnection);

			return Task<int>.Factory.FromAsync(
				command.BeginExecuteNonQuery(),
				ar =>
				{
					try
					{
						// return the number of records affected
						return command.EndExecuteNonQuery(ar);
					}
					finally
					{
						// close the connection if required
						if (closeConnection)
							command.Connection.Close();
					}
				});
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
		public static Task<int> AsyncExecuteSql(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecute(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction);
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
		public static Task<IList<FastExpando>> AsyncQuery(
			this SqlConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult, TSub1>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult, TSub1, TSub2>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult, TSub1, TSub2, TSub3>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult, TSub1, TSub2, TSub3, TSub4>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
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
		public static Task<IList<TResult>> AsyncQuery<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
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
		public static Task<IList<FastExpando>> AsyncQuerySql(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult, TSub1>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult, TSub1, TSub2>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult, TSub1, TSub2, TSub3>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult, TSub1, TSub2, TSub3, TSub4>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
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
		public static Task<IList<TResult>> AsyncQuerySql<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this SqlConnection connection, 
			string sql, 
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
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
		public static Task<IList<FastExpando>> AsyncQuery(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList(),
				commandBehavior);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> AsyncQuery<T>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T>(),
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
		public static Task<IList<T>> AsyncQuery<T, TSub1>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T, TSub1>(),
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
		public static Task<IList<T>> AsyncQuery<T, TSub1, TSub2>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T, TSub1, TSub2>(),
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
		public static Task<IList<T>> AsyncQuery<T, TSub1, TSub2, TSub3>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T, TSub1, TSub2, TSub3>(),
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
		public static Task<IList<T>> AsyncQuery<T, TSub1, TSub2, TSub3, TSub4>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T, TSub1, TSub2, TSub3, TSub4>(),
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
		public static Task<IList<T>> AsyncQuery<T, TSub1, TSub2, TSub3, TSub4, TSub5>(this SqlCommand cmd, System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default)
		{
			return cmd.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(commandBehavior).ToList<T, TSub1, TSub2, TSub3, TSub4, TSub5>(),
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
		public static Task AsyncQuery(
			this SqlConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction)
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
		public static Task AsyncQuerySql(
			this SqlConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction)
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
		public static Task<T> AsyncQuery<T>(
			this SqlConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction)
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
		public static Task<T> AsyncQuerySql<T>(
			this SqlConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.AsyncExecuteAndAutoClose(
				c => c.AsyncGetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction)
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
		private static Task<IDataReader> AsyncGetReader(
			this SqlConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			SqlCommand cmd = (SqlCommand)connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);

			return cmd.AsyncGetReader(commandBehavior);
		}

		/// <summary>
		/// Executes a command and returns a task that generates a SqlDataReader. This method does not support auto-open.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The behavior for the command.</param>
		/// <returns>A task that returns a SqlDataReader upon completion.</returns>
		private static Task<IDataReader> AsyncGetReader(this SqlCommand command, CommandBehavior commandBehavior = CommandBehavior.Default)
		{
			return Task<IDataReader>.Factory.FromAsync(command.BeginExecuteReader(commandBehavior), ar => command.EndExecuteReader(ar));
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="commandBehavior">The commandbehavior of the command.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> AsyncExecuteAndAutoClose<T>(this SqlConnection connection, Func<SqlConnection, Task<T>> action, CommandBehavior commandBehavior)
		{
			return connection.AsyncExecuteAndAutoClose(action, commandBehavior.HasFlag(CommandBehavior.CloseConnection));
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> AsyncExecuteAndAutoClose<T>(this SqlConnection connection, Func<SqlConnection, Task<T>> action, bool closeConnection)
		{
			DBConnectionExtensions.DetectAutoOpen(connection, ref closeConnection);

			Task<T> task = action(connection);

			if (!closeConnection)
				return task;

			return task.ContinueWith(
					t =>
					{
						connection.Close();
						return t.Result;
					});
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="command">The command to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="commandBehavior">The commandbehavior of the command.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> AsyncExecuteAndAutoClose<T>(this SqlCommand command, Func<SqlCommand, Task<T>> action, CommandBehavior commandBehavior)
		{
			return command.AsyncExecuteAndAutoClose(action, commandBehavior.HasFlag(CommandBehavior.CloseConnection));
		}

		/// <summary>
		/// Execute an asynchronous action, ensuring that the connection is auto-closed.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="command">The command to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>A task that returns the result of the command after closing the connection.</returns>
		private static Task<T> AsyncExecuteAndAutoClose<T>(this SqlCommand command, Func<SqlCommand, Task<T>> action, bool closeConnection)
		{
			DBConnectionExtensions.DetectAutoOpen(command.Connection, ref closeConnection);

			Task<T> task = action(command);

			if (!closeConnection)
				return task;

			return task.ContinueWith(
					t =>
					{
						command.Connection.Close();
						return t.Result;
					});
		}
		#endregion
	}
}
