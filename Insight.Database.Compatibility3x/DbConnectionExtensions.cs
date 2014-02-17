using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	public static partial class DBConnectionExtensions3x
	{
		#region Query Methods
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
			object parameters,
			Type withGraph,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<TResult>>(sql, parameters, Graph.ToArray(withGraph), commandType, commandBehavior, commandTimeout, transaction, outputParameters).Set1;
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
			object parameters,
			Type withGraph,
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
			object parameters,
			Type withGraph,
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
			object parameters,
			Type withGraph,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters).FirstOrDefault();
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
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null) where T : Results, new()
		{
			var command = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction);
			var results = command.QueryResults<T>(withGraphs, commandBehavior, outputParameters);
			command.OutputParameters(parameters);

			return results;
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
			object parameters,
			Type[] withGraphs,
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
			object parameters,
			Type[] withGraphs,
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
			object parameters,
			Type[] withGraphs,
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
			connection.ForEach(sql, parameters, action, Graph.GetOneToOne<T>(withGraph), commandType, commandBehavior, commandTimeout, transaction);
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
			connection.ForEach(sql, parameters, action, Graph.GetOneToOne<T>(withGraph), CommandType.Text, commandBehavior, commandTimeout, transaction);
		}
		#endregion
	}
}
