using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	public static partial class DBConnectionExtensions
	{
		#region Translation Methods
#if NODBASYNC
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <typeparam name="TResult">The type of object to deserialize from the reader.</typeparam>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<TResult>> ToListAsync<TResult>(this IDataReader reader, Type withGraph = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;

			return Task<IList<TResult>>.Factory.StartNew(() => reader.ToList<TResult>(withGraph), ct);
		}
#else
		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
		/// <param name="reader">The data reader to read from.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<TResult>> ToListAsync<TResult>(this IDataReader reader, Type withGraph = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var oneToOne = Graph.GetOneToOne<TResult>(withGraph);
			return reader.ToListAsync(oneToOne, cancellationToken);
		}
#endif

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
		/// </summary>
		/// <typeparam name="TResult">The type of object to deserialize from the reader.</typeparam>
		/// <param name="task">The task returning the reader to read from.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<TResult>> ToListAsync<TResult>(this Task<IDataReader> task, Type withGraph = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (task == null) throw new ArgumentNullException("task");

			cancellationToken.ThrowIfCancellationRequested();

			return task.ContinueWith(reader => reader.ToListAsync<TResult>(withGraph), cancellationToken).Unwrap();
		}
		#endregion

		#region Query Command Methods
		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T>(
			this IDbCommand command,
			Type withGraph,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			var returns = new ListReader<T>(Graph.GetOneToOne<T>(withGraph));
			return command.QueryAsync(returns, commandBehavior, cancellationToken, outputParameters);
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
		/// <param name="withGraph">The object graph to use to deserialize the objects or null to use the default graph.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type withGraph,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryResultsAsync<Results<TResult>>(sql, parameters, Graph.ToArray(withGraph), commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters)
				.ContinueWith(t => t.Result.Set1, TaskContinuationOptions.ExecuteSynchronously);
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type withGraph,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <typeparam name="T">The type of object to return in the result set.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static Task<T> QueryResultsAsync<T>(
			this IDbCommand command,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null) where T : Results, new()
		{
			var definition = (IQueryReader<T>)Graph.GetDefinitionFromGraphArray(typeof(T), withGraphs);
			return command.QueryAsync(definition, commandBehavior, cancellationToken, outputParameters);
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<T> QueryResultsAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null) where T : Results, new()
		{
			var returns = (IQueryReader<T>)Graph.GetDefinitionFromGraphArray(typeof(T), withGraphs);

			return connection.QueryAsync(sql, parameters, returns, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<T> QueryResultsSqlAsync<T>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null) where T : Results, new()
		{
			var returns = (IQueryReader<T>)Graph.GetDefinitionFromGraphArray(typeof(T), withGraphs);

			return connection.QueryAsync(sql, parameters, returns, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<TResult> SingleAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type withGraph,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, withGraph, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters)
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<TResult> SingleSqlAsync<TResult>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type withGraph,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.SingleAsync<TResult>(sql, parameters, withGraph, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		#endregion
	}
}
