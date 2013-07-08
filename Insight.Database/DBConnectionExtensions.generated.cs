using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for DbConnection to make it easier to call the database.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	public static partial class DBConnectionExtensions
	{
		#region Query Methods
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
				/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), commandType, commandBehavior, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Create a command, execute it, and translate the result set. This method supports auto-open.
		/// </summary>
		/// <typeparam name="TResult">The type of object to return.</typeparam>
				/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
				/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
				/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
				/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
				/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
				/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
				/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
				/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
				/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
				/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
				/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
				/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
				/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
				/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
				/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
				/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
				/// <param name="connection">The connection to use.</param>
		/// <param name="sql">The sql to execute.</param>
		/// <param name="parameters">The parameter to pass.</param>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="commandTimeout">The timeout of the command.</param>
		/// <param name="transaction">The transaction to participate in.</param>
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> QuerySql<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.Query<TResult>(sql, parameters, typeof(Graph<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
		}
	#endregion

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2> QueryResults<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2> QueryResultsSql<T1, T2>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3> QueryResults<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3> QueryResultsSql<T1, T2, T3>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4> QueryResults<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4> QueryResultsSql<T1, T2, T3, T4>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5> QueryResults<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5> QueryResultsSql<T1, T2, T3, T4, T5>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6> QueryResults<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6> QueryResultsSql<T1, T2, T3, T4, T5, T6>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7> QueryResults<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> QueryResults<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandType commandType = CommandType.StoredProcedure,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(sql, parameters, withGraphs, commandType, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}

		/// <summary>
		/// Executes a query that returns multiple result sets and reads the results.
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
		/// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
		/// <returns>The results object filled with the data.</returns>
		public static Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> QueryResultsSql<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IDbConnection connection,
			string sql,
			object parameters = null,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			int? commandTimeout = null,
			IDbTransaction transaction = null,
			object outputParameters = null)
		{
			return connection.QueryResults<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(sql, parameters, withGraphs, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
		}
    }
}
