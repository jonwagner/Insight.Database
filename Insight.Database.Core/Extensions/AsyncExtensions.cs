using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Reliable;
using Insight.Database.Structure;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods to support asynchronous database operations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
    public static partial class DBConnectionExtensions
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        public static Task<int> ExecuteAsync(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters,
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                false,
                async (cmd, r) =>
                {
                    // DbCommand now supports async execute
                    DbCommand dbCommand = cmd as DbCommand;
                    if (dbCommand != null)
                        return await dbCommand.ExecuteNonQueryAsync(cancellationToken);
                    else
                        return cmd.ExecuteNonQuery();
                },
                closeConnection,
                cancellationToken,
                outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        public static Task<int> ExecuteSqlAsync(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsync(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, cancellationToken, outputParameters);
        }
        #endregion

        #region ExecuteScalar Members
        /// <summary>
        /// Create a command and execute it, returning the first column of the first row. This method supports auto-open.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="closeConnection">True to close the connection after the query.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        /// <typeparam name="T">The type of the data to be returned.</typeparam>
        public static Task<T> ExecuteScalarAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters,
                c => connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                false,
                async (cmd, r) =>
                {
                    // DbCommand now supports async execute
                    DbCommand dbCommand = cmd as DbCommand;
                    if (dbCommand != null)
                    {
                        var scalar = await dbCommand.ExecuteScalarAsync(cancellationToken);
                        return ConvertScalar<T>(cmd, parameters, outputParameters, scalar);
                    }
                    else
                    {
                        return ConvertScalar<T>(cmd, parameters, outputParameters, cmd.ExecuteScalar());
                    }
                },
                closeConnection,
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Create a command and execute it, returning the first column of the first row. This method supports auto-open.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="closeConnection">True to close the connection after the query.</param>
        /// <param name="commandTimeout">The timeout for the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        /// <typeparam name="T">The type of the data to be returned.</typeparam>
        public static Task<T> ExecuteScalarSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteScalarAsync<T>(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, cancellationToken, outputParameters);
        }
        #endregion

        #region Query Connection Methods
        /// <summary>
        /// Create a command, execute it, and translate the result set. This method supports auto-open.
        /// </summary>
        /// <typeparam name="T">The type of object to return from the query.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="returns">The reader to use to return the data.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        public static Task<T> QueryAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            IQueryReader<T> returns,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters,
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                true,
                (cmd, r) => returns.ReadAsync(cmd, r, cancellationToken),
                commandBehavior,
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Create a command, execute it, and translate the result set. This method supports auto-open.
        /// </summary>
        /// <typeparam name="T">The type of object to return from the query.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="returns">The reader to use to return the data.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        public static Task<T> QuerySqlAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            IQueryReader<T> returns,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.QueryAsync(sql, parameters, returns, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
        }

        /// <summary>
        /// Create a command, execute it, and translate the result set. This method supports auto-open.
        /// </summary>
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
        public static Task<IList<FastExpando>> QueryAsync(
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
            return connection.QueryAsync(sql, parameters, ListReader<FastExpando>.Default, commandType, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A data reader with the results.</returns>
        public static Task<IList<FastExpando>> QuerySqlAsync(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.QueryAsync(sql, parameters, ListReader<FastExpando>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
        }
        #endregion

        #region Query Command Methods
        /// <summary>
        /// Run a command asynchronously and return a list of objects as FastExpandos. This method supports auto-open.
        /// The Connection property of the command should be initialized before calling this method.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task that returns a list of objects as the result of the query.</returns>
        public static Task<IList<FastExpando>> QueryAsync(
            this IDbCommand command,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return command.QueryAsync(ListReader<FastExpando>.Default, commandBehavior, cancellationToken, outputParameters);
        }

        /// <summary>
        /// Run a command asynchronously and return a list of objects. This method supports auto-open.
        /// The Connection property of the command should be initialized before calling this method.
        /// </summary>
        /// <typeparam name="T">The type of objects to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="returns">The reader to use to return the data.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task that returns a list of objects as the result of the query.</returns>
        public static Task<T> QueryAsync<T>(
            this IDbCommand command,
            IQueryReader<T> returns,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (returns == null) throw new ArgumentNullException("returns");

            return command.Connection.ExecuteAsyncAndAutoClose(
                null,
                c => command,
                true,
                (cmd, r) => returns.ReadAsync(command, r, cancellationToken),
                commandBehavior.HasFlag(CommandBehavior.CloseConnection),
                cancellationToken,
                outputParameters);
        }
        #endregion

        #region Query Results Methods
        /// <summary>
        /// Asynchronously executes a query and returns a result object.
        /// </summary>
        /// <typeparam name="T">The type of the results object to return.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the command.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <param name="outputParameters">An object to write the output parameters to.</param>
        /// <returns>The result of the query.</returns>
        public static Task<T> QueryResultsAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null) where T : Results, new()
        {
            return connection.QueryAsync(
                sql,
                parameters,
                DerivedResultsReader<T>.Default,
                commandType,
                commandBehavior,
                commandTimeout,
                transaction,
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Asynchronously executes a query and returns a result object.
        /// </summary>
        /// <typeparam name="T">The type of the results object to return.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the command.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <param name="outputParameters">An object to write the output parameters to.</param>
        /// <returns>The result of the query.</returns>
        public static Task<T> QueryResultsSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null) where T : Results, new()
        {
            return connection.QueryResultsAsync<T>(
                sql,
                parameters,
                CommandType.Text,
                commandBehavior,
                commandTimeout,
                transaction,
                cancellationToken,
                outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static Task QueryAsync(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<IDataReader> read,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters,
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                true,
                (cmd, r) => { read(r); return Helpers.FalseTask; },
                commandBehavior.HasFlag(CommandBehavior.CloseConnection),
                cancellationToken,
                outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static Task QuerySqlAsync(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<IDataReader> read,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.QueryAsync(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static Task<T> QueryAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Func<IDataReader, T> read,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters,
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                true,
                (cmd, r) => Helpers.FromResult(read(r)),
                commandBehavior.HasFlag(CommandBehavior.CloseConnection),
                cancellationToken,
                outputParameters);
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
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static Task<T> QuerySqlAsync<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Func<IDataReader, T> read,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.QueryAsync(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken, outputParameters);
        }
        #endregion

        #region Translation Methods
        /// <summary>
        /// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
        /// </summary>
        /// <param name="reader">The data reader to read from.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>A task that returns the list of objects.</returns>
        public static Task<IList<FastExpando>> ToListAsync(this IDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
        {
            return reader.ToListAsync(OneToOne<FastExpando>.Records, cancellationToken);
        }

        /// <summary>
        /// Chain an asynchronous data reader task with a translation to a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="reader">The data reader to read from.</param>
        /// <param name="recordReader">The reader to use to read the record.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>A task that returns the list of objects.</returns>
        public static Task<IList<T>> ToListAsync<T>(this IDataReader reader, IRecordReader<T> recordReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            return reader.ToListAsync(recordReader, cancellationToken, firstRecordOnly: false);
        }

        /// <summary>
        /// Chain an asynchronous data reader task with a translation to a list of objects as FastExpandos.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize from the reader.</typeparam>
        /// <param name="task">The task returning the reader to read from.</param>
        /// <param name="recordReader">The reader to use to read the record.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>A task that returns the list of objects.</returns>
        public static async Task<IList<T>> ToListAsync<T>(this Task<IDataReader> task, IRecordReader<T> recordReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (task == null) throw new ArgumentNullException("task");

            cancellationToken.ThrowIfCancellationRequested();

            var reader = await task;

            return await reader.ToListAsync(recordReader);
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record that is merged into the object upon return.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="inserted">
        /// The object that is being inserted and should be merged.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task whose completion is the object after merging the results.</returns>
        public static Task<T> InsertAsync<T>(
            this IDbConnection connection,
            string sql,
            T inserted,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters ?? inserted,
                c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
                true,
                (cmd, r) => r.MergeAsync(inserted, cancellationToken),
                commandBehavior.HasFlag(CommandBehavior.CloseConnection),
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record that is merged into the object upon return.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="inserted">
        /// The object that is being inserted and should be merged.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the object after merging the results.</returns>
        public static Task<T> InsertSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            T inserted,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertAsync(sql, inserted, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="inserted">
        /// The list of objects that is being inserted and should be merged.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task whose completion is the list of objects after merging the results.</returns>
        public static Task<IEnumerable<T>> InsertListAsync<T>(
            this IDbConnection connection,
            string sql,
            IEnumerable<T> inserted,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            return connection.ExecuteAsyncAndAutoClose(
                parameters ?? inserted,
                c => c.CreateCommand(sql, parameters ?? inserted, commandType, commandTimeout, transaction),
                true,
                (cmd, r) => r.MergeAsync(inserted, cancellationToken),
                commandBehavior.HasFlag(CommandBehavior.CloseConnection),
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="inserted">
        /// The list of objects that is being inserted and should be merged.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the list of objects after merging the results.</returns>
        public static Task<IEnumerable<T>> InsertListSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            IEnumerable<T> inserted,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertListAsync(sql, inserted, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction, cancellationToken);
        }
        #endregion

        #region QueryOnto Methods
        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as InsertAsync.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="onto">
        /// The list of objects to be merged onto.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the object after merging the results.</returns>
        public static Task<T> QueryOntoAsync<T>(
            this IDbConnection connection,
            string sql,
            T onto,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertAsync(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as InsertAsync.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="onto">
        /// The list of objects to be merged onto.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the object after merging the results.</returns>
        public static Task<T> QueryOntoSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            T onto,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertSqlAsync(sql, onto, parameters, commandBehavior, commandTimeout, transaction, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as InsertAsync.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="onto">
        /// The list of objects to be merged onto.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the list of objects after merging the results.</returns>
        public static Task<IEnumerable<T>> QueryOntoListAsync<T>(
            this IDbConnection connection,
            string sql,
            IEnumerable<T> onto,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertListAsync(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as InsertAsync.
        /// </summary>
        /// <typeparam name="T">The type of the object to merge into.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="onto">
        /// The list of objects to be merged onto.
        /// If null, then the results are merged into the parameters object.
        /// </param>
        /// <param name="parameters">The parameter to pass.</param>
        /// <param name="commandBehavior">The behavior of the command when executed.</param>
        /// <param name="commandTimeout">The timeout of the command.</param>
        /// <param name="transaction">The transaction to participate in it.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task whose completion is the list of objects after merging the results.</returns>
        public static Task<IEnumerable<T>> QueryOntoListSqlAsync<T>(
            this IDbConnection connection,
            string sql,
            IEnumerable<T> onto,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return connection.InsertListSqlAsync(sql, onto, parameters, commandBehavior, commandTimeout, transaction, cancellationToken);
        }
        #endregion

        #region Merge Methods
        /// <summary>
        /// Merges the results of a recordset into an existing object.
        /// </summary>
        /// <typeparam name="T">The type of object to merge into.</typeparam>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="item">The item to merge into.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>The item that has been merged.</returns>
        /// <remarks>
        /// This method reads a single record from the reader and overwrites the values of the object.
        /// The reader is then advanced to the next result or disposed.
        /// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
        /// </remarks>
        public static async Task<T> MergeAsync<T>(this IDataReader reader, T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            await reader.MergeAsync<T>(new T[] { item }, cancellationToken).ConfigureAwait(false);

            return item;
        }

        /// <summary>
        /// Merges the results of a recordset into an existing object.
        /// </summary>
        /// <typeparam name="T">The type of object to merge into.</typeparam>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="items">The list of items to merge into.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>The item that has been merged.</returns>
        /// <remarks>
        /// This method reads a single record from the reader and overwrites the values of the object.
        /// The reader is then advanced to the next result or disposed.
        /// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
        /// </remarks>
        public static async Task<IEnumerable<T>> MergeAsync<T>(this IDataReader reader, IEnumerable<T> items, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool moreResults = false;

            try
            {
                // see if the reader support async reads
                DbDataReader dbReader = reader as DbDataReader;
                if (dbReader == null)
                    return reader.Merge(items);

                var merger = DbReaderDeserializer.GetMerger<T>(reader);

                // read the identities of each item from the recordset and merge them into the objects
                foreach (T item in items)
                {
                    await dbReader.ReadAsync(cancellationToken).ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    merger(reader, item);
                }

                // we are done with this result set, so move onto the next or clean up the reader
                moreResults = await dbReader.NextResultAsync(cancellationToken).ConfigureAwait(false);

                return items;
            }
            finally
            {
                if (!moreResults)
                    reader.Dispose();
            }
        }
        #endregion

        #region GetReader Methods
        /// <summary>
        /// Executes a command and returns a task that generates a SqlDataReader. This method does not support auto-open.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The behavior for the command.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>A task that returns a SqlDataReader upon completion.</returns>
        public static async Task<IDataReader> GetReaderAsync(this IDbCommand command, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            // DbCommand now supports async
            DbCommand dbCommand = command as DbCommand;
            if (dbCommand != null)
                return (IDataReader)await dbCommand.ExecuteReaderAsync(commandBehavior, cancellationToken);

            return command.ExecuteReader(commandBehavior);
        }
        #endregion

        #region BulkCopy Methods
        /// <summary>
        /// Bulk copy a list of objects to the server. This method supports auto-open.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="list">The list of objects.</param>
        /// <param name="configure">A callback for initialization of the BulkCopy object. The object is provider-dependent.</param>
        /// <param name="closeConnection">True to close the connection when complete.</param>
        /// <param name="options">The options to use for the bulk copy.</param>
        /// <param name="transaction">An optional external transaction.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>Number of rows copied.</returns>
        public static Task<int> BulkCopyAsync<T>(
            this IDbConnection connection,
            string tableName,
            IEnumerable<T> list,
            Action<InsightBulkCopy> configure = null,
            bool closeConnection = false,
            InsightBulkCopyOptions options = InsightBulkCopyOptions.Default,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var reader = GetObjectReader(connection, tableName, transaction, list);

            return connection.BulkCopyAsync(
                tableName,
                reader,
                configure,
                closeConnection,
                options,
                transaction,
                cancellationToken);
        }

        /// <summary>
        /// Bulk copy a list of objects to the server. This method supports auto-open.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="source">The list of data to read.</param>
        /// <param name="configure">A callback for initialization of the BulkCopy object. The object is provider-dependent.</param>
        /// <param name="closeConnection">True to close the connection when complete.</param>
        /// <param name="options">The options to use for the bulk copy.</param>
        /// <param name="transaction">An optional external transaction.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>Number of rows copied.</returns>
        public static Task<int> BulkCopyAsync(
            this IDbConnection connection,
            string tableName,
            IDataReader source,
            Action<InsightBulkCopy> configure = null,
            bool closeConnection = false,
            InsightBulkCopyOptions options = InsightBulkCopyOptions.Default,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null) throw new ArgumentNullException("source");

            // see if there are any invalid bulk copy options set
            var provider = InsightDbProvider.For(connection);
            var invalidOptions = (options & ~(provider.GetSupportedBulkCopyOptions(connection)));
            if (invalidOptions != 0)
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "BulkCopyOption {0} is not supported for this provider", invalidOptions));

            try
            {
                return connection.ExecuteAsyncAndAutoClose(
                    closeConnection,
                    async (c, ct) =>
                    {
                        await provider.BulkCopyAsync(connection, tableName, source, configure, options, transaction, ct);
                        return source.RecordsAffected;
                    },
                    cancellationToken);
            }
            finally
            {
                source.Dispose();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Chain an asynchronous data reader task with a translation to a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="reader">The data reader to read from.</param>
        /// <param name="recordReader">The reader to use to read the record.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <param name="firstRecordOnly">True to only read in the first record (for Single cases).</param>
        /// <returns>A task that returns the list of objects.</returns>
        internal static async Task<IList<T>> ToListAsync<T>(this IDataReader reader, IRecordReader<T> recordReader, CancellationToken cancellationToken, bool firstRecordOnly)
        {
			var asyncReader = reader.AsEnumerableAsync(recordReader, cancellationToken);

            IList<T> list = new List<T>();

			while (await asyncReader.MoveNextAsync().ConfigureAwait(false))
			{
				list.Add(asyncReader.Current);

				// if we only want the first record in the set, then skip the rest of this recordset
				if (firstRecordOnly)
				{
					await asyncReader.NextResultAsync().ConfigureAwait(false);
					break;
				}
			}

			return list;
        }

        /// <summary>
        /// Execute an asynchronous action, ensuring that the connection is auto-closed.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="parameters">The parameters for the call.</param>
        /// <param name="getCommand">An action to perform to get the command to execute.</param>
        /// <param name="callGetReader">True to automatically get the reader from the command.</param>
        /// <param name="translate">An action to perform to translate the reader into results.</param>
        /// <param name="closeConnection">True to force the connection to close after the operation completes.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task that returns the result of the command after closing the connection.</returns>
        private static Task<T> ExecuteAsyncAndAutoClose<T>(
            this IDbConnection connection,
            object parameters,
            Func<IDbConnection, IDbCommand> getCommand,
            bool callGetReader,
            Func<IDbCommand, IDataReader, Task<T>> translate,
            bool closeConnection,
            CancellationToken cancellationToken,
            object outputParameters)
        {
            return connection.ExecuteAsyncAndAutoClose<T>(
                parameters,
                getCommand,
                callGetReader,
                translate,
                closeConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default,
                cancellationToken,
                outputParameters);
        }

        /// <summary>
        /// Execute an asynchronous action, ensuring that the connection is auto-closed.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="parameters">The parameters for the call.</param>
        /// <param name="getCommand">An action to perform to get the command to execute.</param>
        /// <param name="callGetReader">True to automatically call GetReader on the command.</param>
        /// <param name="translate">An action to perform to translate the reader into results.</param>
        /// <param name="commandBehavior">The CommandBehavior to use to execute the command.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <param name="outputParameters">An optional additional object to output parameters onto.</param>
        /// <returns>A task that returns the result of the command after closing the connection.</returns>
        private static async Task<T> ExecuteAsyncAndAutoClose<T>(
            this IDbConnection connection,
            object parameters,
            Func<IDbConnection, IDbCommand> getCommand,
            bool callGetReader,
            Func<IDbCommand, IDataReader, Task<T>> translate,
            CommandBehavior commandBehavior,
            CancellationToken cancellationToken,
            object outputParameters)
        {
            IDataReader reader = null;

            bool closeConnection = commandBehavior.HasFlag(CommandBehavior.CloseConnection);
            closeConnection |= await AutoOpenAsync(connection, cancellationToken);

            try
            {
                // get the command
                IDbCommand command = getCommand(connection);

                // if we have a command, execute it
                if (command != null && callGetReader)
				{
					commandBehavior = InsightDbProvider.For(connection).FixupCommandBehavior(command, commandBehavior | CommandBehavior.SequentialAccess);
                    reader = await command.GetReaderAsync(commandBehavior, cancellationToken);
				}

                T result = await translate(command, reader);

                if (command != null)
                {
                    // make sure we go to the end so we can get the outputs
                    if (reader != null && !reader.IsClosed)
                        while (reader.NextResult()) { }

                    command.OutputParameters(parameters, outputParameters);
                }

                return result;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
                if (closeConnection)
                    connection.Close();
            }
        }

        private static async Task<T> ExecuteAsyncAndAutoClose<T>(
            this IDbConnection connection,
            bool closeConnection,
            Func<IDbConnection, CancellationToken, Task<T>> action,
            CancellationToken cancellationToken)
        {
            try
            {
                closeConnection |= await AutoOpenAsync(connection, cancellationToken);

                return await action(connection, cancellationToken);
            }
            finally
            {
                // close before accessing the result so we can guarantee that the connection doesn't leak
                if (closeConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Detect if a connection needs to be automatically opened and closed.
        /// </summary>
        /// <param name="connection">The connection to test.</param>
        /// <param name="cancellationToken">The CancellationToken to use for the operation or null to not use cancellation.</param>
        /// <returns>
        /// A task representing the completion of the open operation
        /// and a flag indicating whether the connection should be closed at the end of the operation.
        /// </returns>
        private static async Task<bool> AutoOpenAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            // if the connection is already open, then it doesn't need to be opened or closed.
            if (connection.State == ConnectionState.Open)
                return false;

            // open the connection and plan to close it
            DbConnection dbConnection = connection as DbConnection;
            if (dbConnection != null)
            {
                await dbConnection.OpenAsync(cancellationToken);

                return dbConnection.State == ConnectionState.Open;
            }

            // we don't have an asynchronous open method, so do it synchronously in a task
            await Task.Run(() => connection.Open());
            return true;
        }

        /// <summary>
        /// Lets us call QueryCoreAsync into a simple delegate for dynamic calls.
        /// </summary>
        /// <typeparam name="T">The type of object returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="returns">The definition of the return structure.</param>
        /// <param name="commandBehavior">The commandBehavior to use.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <param name="outputParameters">Optional output parameters.</param>
        /// <returns>The result of the query.</returns>
        private static Task<T> QueryCoreAsyncUntyped<T>(
            this IDbCommand command,
            IQueryReader returns,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken),
            object outputParameters = null)
        {
            // this method lets us convert QueryCoreAsync to a delegate for dynamic calls
            return command.QueryAsync<T>((IQueryReader<T>)returns, commandBehavior, cancellationToken, outputParameters);
        }
        #endregion

		#region AsEnumerable Methods
		/// <summary>
        /// Returns an enumerator that can read records from the stream asynchronously.
        /// </summary>
		/// <typeparam name="T">The type of object to return from the reader.</typeparam>
        /// <param name="reader">The data reader to read from.</param>
        /// <param name="recordReader">The record reader to use to translate the records.</param>
        /// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
        /// <returns>An asynchrnous enumerator.</returns>
		public static IAsyncEnumerable<T> AsEnumerableAsync<T>(this IDataReader reader, IRecordReader<T> recordReader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new AsyncReader<T>(reader, recordReader, cancellationToken);
		}

		class AsyncReader<T> : IAsyncEnumerable<T>
		{
			private bool _completed = false;
			private IDataReader _reader;
			private DbDataReader _dbReader;
			private IRecordReader<T> _recordReader;
			private Func<IDataReader, T> _mapper;
			private CancellationToken _cancellationToken;

			public AsyncReader(IDataReader reader, IRecordReader<T> recordReader, CancellationToken ct)
			{
				_reader = reader;
				_dbReader = reader as DbDataReader;
				_recordReader = recordReader;
				_cancellationToken = ct;
			}

			public T Current { get; private set; }

			public async Task<bool> MoveNextAsync()
			{
				if (_completed || _reader.IsClosed)
					return false;

				if (!await ReadNextAsync().ConfigureAwait(false))
				{
					if (!await NextResultAsync().ConfigureAwait(false))
						_reader.Dispose();

					_completed = true;
					Current = default(T);

					return false;
				}

				if (_mapper == null)
				{
					_cancellationToken.ThrowIfCancellationRequested();
					_mapper = _recordReader.GetRecordReader(_reader);
				}

				Current = _mapper(_reader);

				return true;
			}

			public Task<bool> NextResultAsync()
			{
				if (_dbReader != null)
					return _dbReader.NextResultAsync(_cancellationToken);
				else
					return Task.FromResult(_reader.NextResult());
			}

			public void Dispose()
			{
				if (_dbReader != null)
					_dbReader.Dispose();
				else if (_reader != null)
					_reader.Dispose();
			}

			private Task<bool> ReadNextAsync()
			{
				if (_dbReader != null)
					return _dbReader.ReadAsync(_cancellationToken);
				else
					return Task.FromResult(_reader.Read());
			}
		}
		#endregion
    }
}
