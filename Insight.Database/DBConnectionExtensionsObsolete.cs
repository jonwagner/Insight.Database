using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods to support database operations.
	/// </summary>
	/// <remarks>
	/// The methods in this file are considered obsolete because they can all be replaced with methods that accept a graph.
	/// They can stick around for backwards compatibility, but they shouldn't be extended with new things.
	/// </remarks>
	public static partial class DBConnectionExtensions
	{
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
		public static IList<TResult> Query<TResult, TSub1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1>), commandType, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> Query<TResult, TSub1, TSub2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2>), commandType, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> Query<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3>), commandType, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> Query<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4>), commandType, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> Query<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>), commandType, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> QuerySql<TResult, TSub1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1>), CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> QuerySql<TResult, TSub1, TSub2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2>), CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> QuerySql<TResult, TSub1, TSub2, TSub3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3>), CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> QuerySql<TResult, TSub1, TSub2, TSub3, TSub4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4>), CommandType.Text, commandBehavior, commandTimeout, transaction);
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
		public static IList<TResult> QuerySql<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>), CommandType.Text, commandBehavior, commandTimeout, transaction);
		}
	}
}
