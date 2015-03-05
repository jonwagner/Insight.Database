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
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods to support asynchronous database operations.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	public static partial class DBConnectionExtensions
    {
        #region Query Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
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
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
		/// </summary>
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
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Records, cancellationToken);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<IList<T1>> QuerySqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ListReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Chain an asynchronous data reader task with a translation to a list of objects.
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
		/// <param name="task">The data reader task to continue.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>A task that returns the list of objects.</returns>
		public static Task<IList<T1>> ToListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Task<IDataReader> task, CancellationToken cancellationToken = default(CancellationToken))
		{
			return task.ToListAsync(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Records, cancellationToken);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2>> QueryResultsAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Asynchronously executes a query that returns multiple result sets and reads the results.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2>> QueryResultsSqlAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3>> QueryResultsAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3>> QueryResultsSqlAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4>> QueryResultsAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4>> QueryResultsSqlAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5>> QueryResultsAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5>> QueryResultsSqlAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6>> QueryResultsAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <returns>The results object filled with the data.</returns>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> QueryResultsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Task<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> QueryResultsSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, cancellationToken, outputParameters);
		}
        #endregion

        #region Single Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
		/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
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
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
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
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in it.</param>
		/// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
		/// <param name="outputParameters">An optional additional object to output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static Task<T1> SingleSqlAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			CancellationToken cancellationToken = default(CancellationToken),
			object outputParameters = null)
		{
			return connection.QueryAsync(sql, parameters, SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
		}
		#endregion
    }
}
