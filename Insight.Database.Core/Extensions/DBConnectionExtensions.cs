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
using Insight.Database.Structure;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods for DbConnection to make it easier to call the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public static partial class DBConnectionExtensions
    {
        #region Private Fields
        /// <summary>
        /// A cache of the table schemas used for bulk copy.
        /// </summary>
        private static ConcurrentDictionary<Tuple<string, Type>, ObjectReader> _tableReaders = new ConcurrentDictionary<Tuple<string, Type>, ObjectReader>();
        #endregion

        #region Open Methods
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

        /// <summary>
        /// Opens and returns a database connection.
        /// </summary>
        /// <typeparam name="T">The type of database connection.</typeparam>
        /// <param name="connection">The connection to open and return.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>The opened connection.</returns>
        public static async Task<T> OpenConnectionAsync<T>(this T connection, CancellationToken cancellationToken = default(CancellationToken)) where T : IDbConnection
        {
            DbConnection dbConnection = connection as DbConnection;

            // if the connection is not a DbConnection, then open it synchronously
            if (dbConnection == null)
            {
                connection.Open();
                return connection;
            }

            // DbConnection supports OpenAsync, but it doesn't return self
            await dbConnection.OpenAsync(cancellationToken);
            return connection;
        }

        /// <summary>
        /// Opens and returns a database connection that implmements the given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <returns>The connection implementing the interface.</returns>
        public static T OpenAs<T>(this IDbConnection connection) where T : class, IDbConnection
        {
            if (connection == null) throw new ArgumentNullException("connection");

            connection.Open();
            return connection.As<T>();
        }

        /// <summary>
        /// Asynchronously opens and returns a database connection that implmements the given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning the connection and interface when the connection is opened.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static async Task<T> OpenAsAsync<T>(this IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection
        {
            try
            {
                await OpenConnectionAsync(connection, cancellationToken);

                return connection.As<T>();
            }
            catch
            {
                connection.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this IDbConnection connection)
        {
            // if the connection isn't wrapped, we need to wrap it
            DbConnectionWrapper wrapper = DbConnectionWrapper.Wrap(connection);
            wrapper.Open();
            return wrapper.BeginAutoTransaction();
        }

        /// <summary>
        /// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <returns>A wrapper for the database connection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The connection is returned")]
        public static T OpenWithTransactionAs<T>(this T connection) where T : class, IDbConnection, IDbTransaction
        {
            // connection is already a T, so pass it in unwrapped
            // a connection won't leak from this code because the connection is already a T and should come back as a T
            // and there is no code between the construction and the return other than the cast
            return (T)(object)((IDbConnection)connection).OpenWithTransaction();
        }

        /// <summary>
        /// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static T OpenWithTransactionAs<T>(this IDbConnection connection) where T : class, IDbConnection, IDbTransaction
        {
            // convert to interface first, then open, so we only get one layer of wrapping
            return connection.As<T>().OpenWithTransactionAs();
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The connection is returned")]
        public static async Task<DbConnectionWrapper> OpenWithTransactionAsync(this IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            var wrapper = await DbConnectionWrapper.Wrap(connection).OpenConnectionAsync(cancellationToken);
            try
            {
                wrapper.BeginAutoTransaction();
                return wrapper;
            }
            catch
            {
                wrapper.Dispose();

                throw;
            }
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The connection is returned")]
        public static async Task<T> OpenWithTransactionAsAsync<T>(this T connection, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
        {
            var open = await OpenWithTransactionAsync((IDbConnection)connection, cancellationToken);

            try
            {
                return open.As<T>();
            }
            catch
            {
                open.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="connection">The connection to open.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<T> OpenWithTransactionAsAsync<T>(this IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
        {
            // convert to interface first, then open, so we only get one layer of wrapping
            return connection.As<T>().OpenWithTransactionAsAsync(cancellationToken);
        }

        /// <summary>
        /// Wraps the connection with an existing open database transaction.
		/// Note that the caller is responsible for the lifetime of the transaction.
        /// </summary>
        /// <param name="connection">The connection to wrap.</param>
        /// <param name="transaction">The transaction to bind to the wrapper.</param>
        /// <returns>A wrapped connection that is enlisted in the transaction.</returns>
		public static DbConnectionWrapper UsingTransaction(this IDbConnection connection, IDbTransaction transaction)
		{
            // if the connection isn't wrapped, we need to wrap it
            DbConnectionWrapper wrapper = DbConnectionWrapper.Wrap(connection);
            return wrapper.UsingTransaction(transaction);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
        public static IDbCommand CreateCommand(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // create a db command
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            // unwrap the transaction because the transaction has to match the command and connection
            if (transaction != null)
                cmd.Transaction = UnwrapDbTransaction(transaction);
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static int Execute(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c => null,
                (_, __) =>
                {
                    using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
                    {
                        var result = cmd.ExecuteNonQuery();
                        cmd.OutputParameters(parameters, outputParameters);
                        return result;
                    }
                },
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static int ExecuteSql(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Execute(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, outputParameters);
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static T ExecuteScalar<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            CommandType commandType = CommandType.StoredProcedure,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c => null,
                (_, __) =>
                {
                    using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
                    {
                        return ConvertScalar<T>(cmd, parameters, outputParameters, cmd.ExecuteScalar());
                    }
                },
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static T ExecuteScalarSql<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            bool closeConnection = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteScalar<T>(sql, parameters, CommandType.Text, closeConnection, commandTimeout, transaction, outputParameters);
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
            using (var cmd = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
                return cmd.ExecuteReader(commandBehavior);
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
        /// Executes a query and returns the results.
        /// </summary>
        /// <typeparam name="T">The type of the result of the query.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="returns">The reader to use to read objects from the query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the query.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="outputParameters">An object to write output parameters into.</param>
        /// <returns>The result of the query.</returns>
        public static T Query<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            IQueryReader<T> returns,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    var results = returns.Read(cmd, r);
                    cmd.OutputParameters(parameters, outputParameters);

                    return results;
                },
                commandBehavior);
        }

        /// <summary>
        /// Executes a query and returns the results.
        /// </summary>
        /// <typeparam name="T">The type of the result of the query.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="returns">The reader to use to read objects from the query.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the query.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="outputParameters">An object to write output parameters into.</param>
        /// <returns>The result of the query.</returns>
        public static T QuerySql<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            IQueryReader<T> returns,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Query(sql, parameters, returns, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
        }

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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static IList<FastExpando> Query(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Query(sql, parameters, ListReader<FastExpando>.Default, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A data reader with the results.</returns>
        public static IList<FastExpando> QuerySql(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Query(sql, parameters, ListReader<FastExpando>.Default, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
        }
        #endregion

        #region Query Results Methods
        /// <summary>
        /// Executes a query and returns the results.
        /// </summary>
        /// <typeparam name="T">The type of the result of the query.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the query.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="outputParameters">An object to write output parameters into.</param>
        /// <returns>The result of the query.</returns>
        public static T QueryResults<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null) where T : Results, new()
        {
            return connection.Query(
                sql,
                parameters,
                DerivedResultsReader<T>.Default,
                commandType,
                commandBehavior,
                commandTimeout,
                transaction,
                outputParameters);
        }

        /// <summary>
        /// Executes a query and returns the results.
        /// </summary>
        /// <typeparam name="T">The type of the result of the query.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandBehavior">The CommandBehavior to use.</param>
        /// <param name="commandTimeout">The timeout for the query.</param>
        /// <param name="transaction">The transaction to execute in.</param>
        /// <param name="outputParameters">An object to write output parameters into.</param>
        /// <returns>The result of the query.</returns>
        public static T QueryResultsSql<T>(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null) where T : Results, new()
        {
            return connection.QueryResults<T>(
                sql,
                parameters,
                CommandType.Text,
                commandBehavior,
                commandTimeout,
                transaction,
                outputParameters);
        }
        #endregion

        #region Query With Read Callback Methods
        /// <summary>
        /// Executes a query and performs a callback to read the data in the IDataReader.
        /// </summary>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="read">The reader callback.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        public static void Query(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<IDataReader> read,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    read(r);
                    cmd.OutputParameters(parameters, outputParameters);
                    return false;
                },
                commandBehavior);
        }

        /// <summary>
        /// Executes a query and performs a callback to read the data in the IDataReader.
        /// </summary>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="read">The reader callback.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        public static void QuerySql(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<IDataReader> read,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static T Query<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Func<IDataReader, T> read,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    var result = read(r);
                    if (outputParameters != null)
                        cmd.OutputParameters(outputParameters);
                    return result;
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>A task representing the completion of the query and read operation.</returns>
        public static T QuerySql<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Func<IDataReader, T> read,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Query(sql, parameters, read, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
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
        public static void ForEachDynamic(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<dynamic> action,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            connection.ForEach(sql, parameters, action, OneToOne<FastExpando>.Records, commandType, commandBehavior, commandTimeout, transaction);
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
        public static void ForEachDynamicSql(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<dynamic> action,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            connection.ForEach(sql, parameters, action, OneToOne<FastExpando>.Records, CommandType.Text, commandBehavior, commandTimeout, transaction);
        }

        /// <summary>
        /// Executes a query and performs an action for each item in the result.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="action">The reader callback.</param>
        /// <param name="returns">The reader to use to read the objects from the stream.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        public static void ForEach<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<T> action,
            IRecordReader<T> returns = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            if (returns == null)
                returns = OneToOne<T>.Records;

            connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    foreach (T t in r.AsEnumerable(returns))
                        action(t);

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
        /// <param name="returns">The reader to use to read the objects from the stream.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        public static void ForEachSql<T>(
            this IDbConnection connection,
            string sql,
            object parameters,
            Action<T> action,
            IRecordReader<T> returns = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            connection.ForEach(sql, parameters, action, returns, CommandType.Text, commandBehavior, commandTimeout, transaction);
        }
        #endregion

        #region Dynamic Invocation Helper
        /// <summary>
        /// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <returns>A DynamicConnection using the given connection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is intended to return a connection")]
        public static dynamic Dynamic(this IDbConnection connection)
        {
            return new DynamicConnection(connection);
        }

        /// <summary>
        /// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <typeparam name="T">The type of object to return from queries.</typeparam>
        /// <returns>A DynamicConnection using the given connection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is intended to return a connection")]
        public static dynamic Dynamic<T>(this IDbConnection connection)
        {
            return new DynamicConnection<T>(connection);
        }
        #endregion

        #region Insert Members
        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record that is merged into the object upon return.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The object after merging the results.</returns>
        public static TResult Insert<TResult>(
            this IDbConnection connection,
            string sql,
            TResult inserted,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            parameters = parameters ?? inserted;

            connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    var result = r.Merge(inserted);
                    cmd.OutputParameters(parameters, outputParameters);
                    return result;
                },
                commandBehavior);

            return inserted;
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record that is merged into the object upon return.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The object after merging the results.</returns>
        public static TResult InsertSql<TResult>(
            this IDbConnection connection,
            string sql,
            TResult inserted,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Insert<TResult>(sql, inserted, parameters, CommandType.Text, commandBehavior | CommandBehavior.SequentialAccess, commandTimeout, transaction, outputParameters);
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The list of objects after merging the results.</returns>
        public static IEnumerable<TResult> InsertList<TResult>(
            this IDbConnection connection,
            string sql,
            IEnumerable<TResult> inserted,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            parameters = parameters ?? inserted;

            return connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction),
                (cmd, r) =>
                {
                    var result = r.Merge(inserted);
                    cmd.OutputParameters(parameters, outputParameters);
                    return result;
                },
                commandBehavior);
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is commonly used to retrieve identity values from the database upon an insert.
        /// The result set is expected to contain one record per insertedObject, in the same order as the insertedObjects.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The list of objects after merging the results.</returns>
        public static IEnumerable<TResult> InsertListSql<TResult>(
            this IDbConnection connection,
            string sql,
            IEnumerable<TResult> inserted,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c => c.CreateCommand(sql, parameters ?? inserted, CommandType.Text, commandTimeout, transaction),
                (cmd, r) =>
                {
                    var result = r.Merge(inserted);
                    cmd.OutputParameters(parameters, outputParameters);
                    return result;
                },
                commandBehavior);
        }
        #endregion

        #region QueryOnto Members
        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as Insert.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The object after merging the results.</returns>
        public static TResult QueryOnto<TResult>(
            this IDbConnection connection,
            string sql,
            TResult onto,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.Insert(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as Insert.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The object after merging the results.</returns>
        public static TResult QueryOntoSql<TResult>(
            this IDbConnection connection,
            string sql,
            TResult onto,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.InsertSql<TResult>(sql, onto, parameters, commandBehavior, commandTimeout, transaction, outputParameters);
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as Insert.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The list of objects after merging the results.</returns>
        public static IEnumerable<TResult> QueryOntoList<TResult>(
            this IDbConnection connection,
            string sql,
            IEnumerable<TResult> onto,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.InsertList(sql, onto, parameters, commandType, commandBehavior, commandTimeout, transaction, outputParameters);
        }

        /// <summary>
        /// Executes the specified query and merges the results into the specified existing object.
        /// This is the same behavior as Insert.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to merge into.</typeparam>
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
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>The list of objects after merging the results.</returns>
        public static IEnumerable<TResult> QueryOntoListSql<TResult>(
            this IDbConnection connection,
            string sql,
            IEnumerable<TResult> onto,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.InsertListSql(sql, onto, parameters, commandBehavior, commandTimeout, transaction, outputParameters);
        }
        #endregion

        #region Interface Members
        /// <summary>
        /// Uses a DbConnection to implement an interface. Calls to the interface are automatically mapped to stored procedure calls.
        /// </summary>
        /// <typeparam name="T">The interface type to implmement.</typeparam>
        /// <param name="connection">The connection to use for database calls.</param>
        /// <returns>An implementation of the interface that executes database calls.</returns>
        public static T As<T>(this IDbConnection connection) where T : class
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // if the connection already supports T, then return it, otherwise we need a wrapper
            return (connection as T) ?? (T)InterfaceGenerator.GetImplementorOf(typeof(T), () => connection, singleThreaded: true);
        }

        /// <summary>
        /// Uses a DbConnection to implement an interface. Calls to the interface are automatically mapped to stored procedure calls.
        /// </summary>
        /// <param name="connection">The connection to use for database calls.</param>
        /// <param name="type">The type of interface to implement.</param>
        /// <returns>An implementation of the interface that executes database calls.</returns>
        public static object As(this IDbConnection connection, Type type)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // if the connection already supports T, then return it, otherwise we need a wrapper
            return InterfaceGenerator.GetImplementorOf(type, () => connection, singleThreaded: true);
        }

        /// <summary>
        /// Creates and returns a new multi-threaded connection implementing the given interface.
        /// The object can support making multiple calls at the same time.
        /// </summary>
        /// <typeparam name="T">The interface to implement on the connection.</typeparam>
        /// <param name="connection">The connection to use as a template for opening connections. The connection will not be used.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static T AsParallel<T>(this IDbConnection connection) where T : class
        {
            if (connection == null) throw new ArgumentNullException("connection");

            return (T)connection.AsParallel(typeof(T));
        }

        /// <summary>
        /// Creates and returns a new multi-threaded connection implementing the given interface.
        /// The object can support making multiple calls at the same time.
        /// </summary>
        /// <param name="connection">The connection to use as a template for opening connections. The connection will not be used.</param>
        /// <param name="type">The type of interface to implement.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static object AsParallel(this IDbConnection connection, Type type)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            var provider = InsightDbProvider.For(connection);
            Func<IDbConnection> constructor = () => provider.CloneDbConnection(connection);
            return constructor.AsParallel(type);
        }

        /// <summary>
        /// Creates and returns a new multi-threaded connection implementing the given interface.
        /// The object can support making multiple calls at the same time.
        /// </summary>
        /// <typeparam name="T">The interface to implement on the connection.</typeparam>
        /// <param name="provider">A method that provides a new instance of the DbConnection.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static T AsParallel<T>(this Func<IDbConnection> provider) where T : class
        {
            if (provider == null) throw new ArgumentNullException("provider");

            return (T)provider.AsParallel(typeof(T));
        }

        /// <summary>
        /// Creates and returns a new multi-threaded connection implementing the given interface.
        /// The object can support making multiple calls at the same time.
        /// </summary>
        /// <param name="provider">A method that provides a new instance of the DbConnection.</param>
        /// <param name="type">The type of interface to implement.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static object AsParallel(this Func<IDbConnection> provider, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (provider == null) throw new ArgumentNullException("provider");

            // can't invoke DbConnectionWrapper AsParallel. AsParallel creates a new instance for each call.
            // This would be confusing to developers.
            if (type.IsSubclassOf(typeof(DbConnectionWrapper)))
                throw new InvalidOperationException("Types derived from DbConnectionWrapper cannot be invoked AsParallel.");

            return InterfaceGenerator.GetImplementorOf(type, provider, singleThreaded: false);
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
        /// <param name="configure">A callback for initialization of the BulkCopy object. The object is provider-dependent.</param>
        /// <param name="closeConnection">True to close the connection when complete.</param>
        /// <param name="options">The options to use for the bulk copy.</param>
        /// <param name="transaction">An optional external transaction.</param>
        /// <returns>Number of rows copied.</returns>
        public static int BulkCopy<T>(
            this IDbConnection connection,
            string tableName,
            IEnumerable<T> list,
            Action<InsightBulkCopy> configure = null,
            bool closeConnection = false,
            InsightBulkCopyOptions options = InsightBulkCopyOptions.Default,
            IDbTransaction transaction = null)
        {
            // create a reader for the list
            using (var reader = GetObjectReader(connection, tableName, transaction, list))
            {
                return connection.BulkCopy(
                    tableName,
                    reader,
                    configure,
                    closeConnection,
                    options,
                    transaction);
            }
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
        /// <returns>Number of rows copied.</returns>
        public static int BulkCopy(
            this IDbConnection connection,
            string tableName,
            IDataReader source,
            Action<InsightBulkCopy> configure = null,
            bool closeConnection = false,
            InsightBulkCopyOptions options = InsightBulkCopyOptions.Default,
            IDbTransaction transaction = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            // see if there are any invalid bulk copy options set
            var provider = InsightDbProvider.For(connection);
            var invalidOptions = (options & ~(provider.GetSupportedBulkCopyOptions(connection)));
            if (invalidOptions != 0)
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "BulkCopyOption {0} is not supported for this provider", invalidOptions));

            return connection.ExecuteAndAutoClose(
                c =>
                {
                    using (source)
                    {
                        provider.BulkCopy(connection, tableName, source, configure, options, transaction);
                    }

                    // need to dispose the source in order to get the records affected
                    return source.RecordsAffected;
                },
                closeConnection);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Executes an action on a connection, then automatically closes the connection if necessary.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="getCommand">The action to perform to get the command to execute.</param>
        /// <param name="translate">The action to perform to translate a command and reader into results.</param>
        /// <param name="closeConnection">True to force a close of the connection upon completion.</param>
        /// <returns>The result of the action.</returns>
        public static T ExecuteAndAutoClose<T>(
            this IDbConnection connection,
            Func<IDbConnection, IDbCommand> getCommand,
            Func<IDbCommand, IDataReader, T> translate,
            bool closeConnection)
        {
            return connection.ExecuteAndAutoClose<T>(getCommand, translate, closeConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
        }

        /// <summary>
        /// Executes an action on a connection, then automatically closes the connection if necessary.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="getCommand">The action to perform to get the command to execute.</param>
        /// <param name="translate">The action to perform to translate a command and reader into results.</param>
        /// <param name="commandBehavior">The CommandBehavior to use for the query.</param>
        /// <returns>The result of the action.</returns>
        public static T ExecuteAndAutoClose<T>(
            this IDbConnection connection,
            Func<IDbConnection, IDbCommand> getCommand,
            Func<IDbCommand, IDataReader, T> translate,
            CommandBehavior commandBehavior)
        {
            IDataReader reader = null;
            bool closeConnection = commandBehavior.HasFlag(CommandBehavior.CloseConnection);

            try
            {
                DetectAutoOpen(connection, ref closeConnection);

                // generate the command
                var command = getCommand(connection);

                // if the command is not null, then automatically generate the reader
                if (command != null)
				{
					commandBehavior = InsightDbProvider.For(connection).FixupCommandBehavior(command, commandBehavior | CommandBehavior.SequentialAccess);
                    reader = command.ExecuteReader(commandBehavior);
				}

                return translate(command, reader);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Dispose();

                if (closeConnection)
                    connection.EnsureIsClosed();
            }
        }

        /// <summary>
        /// Executes the operation, opening the connection if necessary.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="closeConnection">True to force closing the connection.</param>
        /// <returns>The result of the operation.</returns>
        public static T ExecuteAndAutoClose<T>(this IDbConnection connection, Func<IDbConnection, T> action, bool closeConnection = false)
        {
            try
            {
                connection.DetectAutoOpen(ref closeConnection);

                return action(connection);
            }
            finally
            {
                if (closeConnection)
                    connection.EnsureIsClosed();
            }
        }

        /// <summary>
        /// Detect if a connection needs to be automatically opened and closed.
        /// </summary>
        /// <param name="connection">The connection to test.</param>
        /// <param name="closeConnection">The closeConnection parameter to modify.</param>
        internal static void DetectAutoOpen(this IDbConnection connection, ref bool closeConnection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                closeConnection = true;
            }
        }

        /// <summary>
        /// Will automatically close the connection of the <see cref="IDbConnection"/> instance in context, if it is not currently closed.
        /// </summary>
        /// <param name="connection">The connection in context.</param>
        internal static void EnsureIsClosed(this IDbConnection connection)
        {
            if (connection == null || connection.State == ConnectionState.Closed)
            {
                return;
            }

            connection.Close();
        }

        /// <summary>
        /// Will automatically open the connection of the <see cref="IDbConnection"/> instance in context, if it is not currently open.
        /// </summary>
        /// <param name="connection">The connection in context.</param>
        internal static void EnsureIsOpen(this IDbConnection connection)
        {
            if (connection == null || connection.State == ConnectionState.Open)
            {
                return;
            }

            connection.Open();
        }

        /// <summary>
        /// Will automatically open the connection of the <see cref="IDbConnection"/> instance in context, if it is not currently open.
        /// </summary>
        /// <param name="connection">The connection in context.</param>
        /// <param name="cancellationToken">A CancellationToken for the operation.</param>
        /// <returns>A task representing the completion of the operation.</returns>
        internal static Task EnsureIsOpenAsync(this DbConnectionWrapper connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null || connection.State == ConnectionState.Open)
            {
                return Task.FromResult(false);
            }

            return connection.OpenAsync(cancellationToken);
        }

        #region Unwrap Methods
        /// <summary>
        /// Unwraps an IDbTransaction to determine its inner DbTransaction to use with advanced features.
        /// </summary>
        /// <param name="transaction">The transaction to unwrap.</param>
        /// <returns>The inner DbTransaction.</returns>
        internal static DbTransaction UnwrapDbTransaction(this IDbTransaction transaction)
        {
            if (transaction == null)
                return null;

            // if we have a DbTransaction, use it
            DbTransaction dbTransaction = transaction as DbTransaction;
            if (dbTransaction != null)
                return dbTransaction;

            // if we have a wrapped transaction, unwrap it
            DbConnectionWrapper wrapper = transaction as DbConnectionWrapper;
            if (wrapper != null)
                return wrapper.InnerTransaction.UnwrapDbTransaction();

            // there is no inner transaction
            return null;
        }
        #endregion

        /// <summary>
        /// Gets a reader that can read the list of objects into the given table. Used for bulk copy.
        /// </summary>
        /// <typeparam name="T">The type of object in the list.</typeparam>
        /// <param name="connection">The connection to use.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The currently open transaction.</param>
        /// <param name="list">The list of objects.</param>
        /// <returns>A reader that can be streamed into the table.</returns>
        private static IDataReader GetObjectReader<T>(IDbConnection connection, string tableName, IDbTransaction transaction, IEnumerable<T> list)
        {
            return connection.ExecuteAndAutoClose(
                c =>
                {
                    // see if we already have a mapping for the given table name and type
                    // we can't use the schema mapping cache because we don't have the schema yet, just the name of the table
                    var key = Tuple.Create<string, Type>(tableName, typeof(T));
                    ObjectReader fieldReaderData = _tableReaders.GetOrAdd(
                        key,
                        t =>
                        {
                            // select a 0 row result set so we can determine the schema of the table
                            using (var sqlReader = connection.GetReaderSql(
                                InsightDbProvider.For(connection).GetTableSchemaSql(connection, tableName),
                                commandBehavior: CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo,
                                transaction: transaction))
                                return ObjectReader.GetObjectReader(connection.CreateCommand(), sqlReader, typeof(T));
                        });

                    // create a reader for the list
                    return new ObjectListDbDataReader(fieldReaderData, list);
                });
        }

        /// <summary>
        /// Gets the results of an ExecuteScalar, including output parameters.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="cmd">The command that was executed.</param>
        /// <param name="parameters">The parameters to the command.</param>
        /// <param name="outputParameters">The output parameter object.</param>
        /// <param name="result">The result of the command.</param>
        /// <returns>The result of the command, converted to the given type.</returns>
        private static T ConvertScalar<T>(IDbCommand cmd, object parameters, object outputParameters, object result)
        {
            if ((result == null || result == DBNull.Value) &&
                typeof(T).GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                throw new InvalidOperationException("Recordset returned no rows, but ExecuteScalar is trying to return a non-nullable type.");

            cmd.OutputParameters(parameters, outputParameters);

            if (result == DBNull.Value)
                return default(T);

            return (T)result;
        }
        #endregion
    }
}
