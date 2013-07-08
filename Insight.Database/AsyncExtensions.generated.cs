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
	public static partial class AsyncExtensions
    {
        #region Query Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
				/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
				/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QueryAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return from the query.</typeparam>
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<TResult>> QuerySqlAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QuerySqlAsync<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), commandBehavior, commandTimeout, transaction, cancellationToken);
		}

		/// <summary>
		/// Run a command asynchronously and return a list of objects. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
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
				/// <param name="cmd">The command to execute.</param>
		/// <param name="commandBehavior">The command behavior.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns a list of objects as the result of the query.</returns>
		public static Task<IList<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbCommand cmd, 
			System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default,
			CancellationToken? cancellationToken = null)
		{
			return cmd.QueryAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), commandBehavior, cancellationToken);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
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
				/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T>> ToListAsync<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Task<IDataReader> task, CancellationToken? cancellationToken = null)
		{
			return task.ToListAsync<T>(typeof(Graph<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), cancellationToken);
		}

		#endregion

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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
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
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken? cancellationToken = null)
		{
			return connection.QueryResultsAsync<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
		}

        #endregion
    }
}
