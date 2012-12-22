using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods to support asynchronous database operations.
	/// </summary>
	/// <remarks>
	/// The methods in this file are considered obsolete because they can all be replaced with methods that accept a graph.
	/// They can stick around for backwards compatibility, but they shouldn't be extended with things like CancellationToken.
	/// </remarks>
	public static partial class AsyncExtensions
	{
		#region Query Connection Methods
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
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1>), commandType, commandBehavior, commandTimeout, transaction);
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
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2>), commandType, commandBehavior, commandTimeout, transaction);
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
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3>), commandType, commandBehavior, commandTimeout, transaction);
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
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4>), commandType, commandBehavior, commandTimeout, transaction);
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
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>), commandType, commandBehavior, commandTimeout, transaction);
		}
		#endregion

		#region QuerySql Connection Methods
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
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1>), commandBehavior, commandTimeout, transaction);
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
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2>), commandBehavior, commandTimeout, transaction);
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
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3>), commandBehavior, commandTimeout, transaction);
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
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4>), commandBehavior, commandTimeout, transaction);
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
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, TSub1, TSub2, TSub3, TSub4, TSub5>), commandBehavior, commandTimeout, transaction);
		}
		#endregion

		#region Query Command Methods
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
			return cmd.QueryAsync<T>(typeof(Graph<T, TSub1>), commandBehavior);
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
			return cmd.QueryAsync<T>(typeof(Graph<T, TSub1, TSub2>), commandBehavior);
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
			return cmd.QueryAsync<T>(typeof(Graph<T, TSub1, TSub2, TSub3>), commandBehavior);
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
			return cmd.QueryAsync<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4>), commandBehavior);
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
			return cmd.QueryAsync<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5>), commandBehavior);
		}
		#endregion

		#region Transation Methods
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <typeparam name="TSub1">The type of object to return as subobject 1.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToList<T, TSub1>(this Task<IDataReader> task)
		{
			return task.ToList<T>(typeof(Graph<T, TSub1>));
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
			return task.ToList<T>(typeof(Graph<T, TSub1, TSub2>));
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
			return task.ToList<T>(typeof(Graph<T, TSub1, TSub2, TSub3>));
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
			return task.ToList<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4>));
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
			return task.ToList<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5>));
		}
		#endregion
	}
}
