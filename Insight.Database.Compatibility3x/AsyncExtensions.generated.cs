using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	public static partial class DBConnectionExtensions
    {
        #region QueryResults Methods
		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
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
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6>> QueryResultsAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth set of data.</typeparam>
		/// <typeparam name="T16">The type of the data in the sixteenth set of data.</typeparam>
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
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(sql, parameters, withGraphs, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth set of data.</typeparam>
		/// <typeparam name="T16">The type of the data in the sixteenth set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters,
			Type[] withGraphs,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

        #endregion
    }
}
