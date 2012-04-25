using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for DbConnection to make it easier to call the database.
	/// </summary>
	public static class DBConnectionExtensions
	{
		#region Private Fields
		/// <summary>
		/// A cache of the table schemas.
		/// </summary>
		private static ConcurrentDictionary<string, DataTable> _tableSchemas = new ConcurrentDictionary<string, DataTable>();
		#endregion

		#region Open Method
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
		public static IDbCommand CreateCommand(
			this IDbConnection connection, 
			string sql, 
			object parameters = null, 
			CommandType commandType = CommandType.StoredProcedure, 
			int? commandTimeout = null, 
			IDbTransaction transaction = null)
		{
			// create a db command
			IDbCommand cmd = connection.CreateCommand();
			cmd.CommandType = commandType;
			cmd.CommandText = sql;
			cmd.Transaction = transaction;
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
		/// <returns>A data reader with the results.</returns>
		public static int Execute(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction).ExecuteNonQuery(), 
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
		/// <returns>A data reader with the results.</returns>
		public static int ExecuteSql(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Execute(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction);
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
		/// <returns>A data reader with the results.</returns>
		public static T ExecuteScalar<T>(
			this IDbConnection connection, 
			string sql, 
			object parameters,
			CommandType commandType = CommandType.StoredProcedure,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => (T)c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction).ExecuteScalar(),
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
		/// <returns>A data reader with the results.</returns>
		public static T ExecuteScalarSql<T>(
			this IDbConnection connection, 
			string sql, 
			object parameters = null,
			bool closeConnection = false,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteScalar<T>(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction);
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
			return connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction).ExecuteReader(commandBehavior);
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
		/// <returns>A data reader with the results.</returns>
		public static List<FastExpando> Query(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult, TSub1>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
				commandBehavior); 
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult, TSub1, TSub2>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type to return as subobject 4.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type to return as subobject 5.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> Query<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
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
		/// <returns>A data reader with the results.</returns>
		public static List<FastExpando> QuerySql(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult, TSub1>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult, TSub1, TSub2>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type to return as subobject 4.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4>(),
				commandBehavior);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type to return as subobject 1.</typeparam>
		/// <typeparam name="TSub2">The type to return as subobject 2.</typeparam>
		/// <typeparam name="TSub3">The type to return as subobject 3.</typeparam>
		/// <typeparam name="TSub4">The type to return as subobject 4.</typeparam>
		/// <typeparam name="TSub5">The type to return as subobject 5.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <returns>A data reader with the results.</returns>
		public static List<TResult> QuerySql<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection, 
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c => c.GetReader(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction).ToList<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(),
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
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void Query(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ExecuteAndAutoClose(
				c => 
				{
					using (IDataReader reader = c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction))
					{
						read(reader);
					}

					return false;
				},
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
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void QuerySql(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<IDataReader> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static T Query<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.ExecuteAndAutoClose(
				c =>
				{
					using (IDataReader reader = c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction))
					{
						return read(reader);
					}
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
		/// <returns>A task representing the completion of the query and read operation.</returns>
		public static T QuerySql<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Func<IDataReader, T> read,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEach(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<dynamic> action,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ExecuteAndAutoClose(
				c =>
				{
					using (IDataReader reader = c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction))
					{
						foreach (FastExpando expando in reader.AsEnumerable())
							action(expando);
					}

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
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEachSql(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<dynamic> action,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ForEach(sql, parameters, action, CommandType.Text, commandBehavior, commandTimeout, transaction);
		}

		/// <summary>
		/// Executes a query and performs an action for each item in the result.
		/// </summary>
		/// <typeparam name="T">The type of object to read.</typeparam>
		/// <param name="connection">The connection to execute on.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameters for the query.</param>
		/// <param name="action">The reader callback.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEach<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<T> action,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ExecuteAndAutoClose(
				c =>
				{
					using (IDataReader reader = c.GetReader(sql, parameters, commandType, commandBehavior, commandTimeout, transaction))
					{
						foreach (T t in reader.AsEnumerable<T>())
							action(t);
					}

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
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <param name="commandTimeout">An optional timeout for the command.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public static void ForEachSql<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Action<T> action,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			connection.ForEach(sql, parameters, action, CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		/// <param name="batchSize">An optional batch size.</param>
		/// <param name="closeConnection">True to close the connection when complete.</param>
		/// <param name="options">The options to use for the bulk copy.</param>
		/// <param name="transaction">An optional external transaction.</param>
		public static void BulkCopy<T>(
			this SqlConnection connection, 
			string tableName, 
			IEnumerable<T> list,
			int? batchSize = null,
			bool closeConnection = false,
			SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
			SqlTransaction transaction = null)
		{
			try
			{
				DetectAutoOpen(connection, ref closeConnection);

				// create a bulk copier
				SqlBulkCopy bulk = new SqlBulkCopy(connection, options, transaction);
				bulk.DestinationTableName = tableName;
				if (batchSize != null)
					bulk.BatchSize = batchSize.Value;

				// ask Sql Server for the schema table
				DataTable schemaTable = _tableSchemas.GetOrAdd(
					tableName, 
					t =>
					{
						using (IDataReader sqlReader = connection.GetReaderSql(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}", tableName), null, CommandBehavior.SchemaOnly))
							return sqlReader.GetSchemaTable();
					});

				// create a reader for the list
				ObjectListDbDataReader reader = new ObjectListDbDataReader(schemaTable, typeof(T), list);

				// write the data to the server
				bulk.WriteToServer(reader);
			}
			finally
			{
				if (closeConnection)
					connection.Close();
			}
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Detect if a connection needs to be automatically opened and closed.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <param name="closeConnection">The closeConnection parameter to modify.</param>
		internal static void DetectAutoOpen(IDbConnection connection, ref bool closeConnection)
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
		/// <param name="action">The action to perform.</param>
		/// <param name="commandBehavior">The behavior of the command.</param>
		/// <returns>The result of the action.</returns>
		private static T ExecuteAndAutoClose<T>(this IDbConnection connection, Func<IDbConnection, T> action, CommandBehavior commandBehavior)
		{
			return connection.ExecuteAndAutoClose(action, commandBehavior.HasFlag(CommandBehavior.CloseConnection));
		}

		/// <summary>
		/// Executes an action on a connection, then automatically closes the connection if necessary.
		/// </summary>
		/// <typeparam name="T">The return type of the action.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="closeConnection">True to force a close of the connection upon completion.</param>
		/// <returns>The result of the action.</returns>
		private static T ExecuteAndAutoClose<T>(this IDbConnection connection, Func<IDbConnection, T> action, bool closeConnection)
		{
			try
			{
				DetectAutoOpen(connection, ref closeConnection);

				return action(connection);
			}
			finally
			{
				if (closeConnection)
					connection.Close();
			}
		}
		#endregion
	}
}
