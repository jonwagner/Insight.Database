using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Providers;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods for DbConnectionStringBuilder.
    /// </summary>
    public static class DbConnectionStringBuilderExtensions
    {
        /// <summary>
        /// Creates and returns a new DbConnection.
        /// </summary>
        /// <param name="builder">The DbConnectionStringBuilder containing the connection string.</param>
        /// <returns>A closed DbConnection.</returns>
        public static DbConnection Connection(this DbConnectionStringBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            DbConnection connection = null;
            DbConnection disposable = null;

            try
            {
                // get the connection from the provider
                connection = InsightDbProvider.For(builder).CreateDbConnection();
                disposable = connection;

                if (connection == null)
                    throw new ArgumentException("Cannot determine the type of connection from the ConnectionStringBuilder", "builder");

                connection.ConnectionString = builder.ConnectionString;

                disposable = null;
                return connection;
            }
            finally
            {
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates and returns a new connection implementing the given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implement on the connection.</typeparam>
        /// <param name="builder">The DbConnectionStringBuilder containing the connection string.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static T As<T>(this DbConnectionStringBuilder builder) where T : class
        {
            return builder.Connection().As<T>();
        }

        /// <summary>
        /// Creates and returns a new multi-threaded connection implementing the given interface.
        /// The object can support making multiple calls at the same time.
        /// </summary>
        /// <typeparam name="T">The interface to implement on the connection.</typeparam>
        /// <param name="builder">The DbConnectionStringBuilder containing the connection string.</param>
        /// <returns>A closed connection that implements the given interface.</returns>
        public static T AsParallel<T>(this DbConnectionStringBuilder builder) where T : class
        {
            Func<IDbConnection> constructor = (() => builder.Connection());
            return constructor.AsParallel<T>();
        }

        /// <summary>
        /// Opens and returns a database connection.
        /// </summary>
        /// <param name="builder">The connection string to open and return.</param>
        /// <returns>The opened connection.</returns>
        public static DbConnection Open(this DbConnectionStringBuilder builder)
        {
            return builder.Connection().OpenConnection();
        }

        /// <summary>
        /// Opens and returns a database connection.
        /// </summary>
        /// <param name="builder">The connection string to open and return.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>The opened connection.</returns>
        public static Task<DbConnection> OpenAsync(this DbConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return builder.Connection().OpenConnectionAsync(cancellationToken);
        }

        /// <summary>
        /// Opens and returns a database connection implementing a given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implmement.</typeparam>
        /// <param name="builder">The connection string to open and return.</param>
        /// <returns>The opened connection.</returns>
        public static T OpenAs<T>(this DbConnectionStringBuilder builder) where T : class, IDbConnection
        {
            return builder.Connection().OpenAs<T>();
        }

        /// <summary>
        /// Asynchronously opens and returns a database connection implementing a given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implmement.</typeparam>
        /// <param name="builder">The connection string to open and return.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>The opened connection.</returns>
        public static Task<T> OpenAsAsync<T>(this DbConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection
        {
            return builder.Connection().OpenAsAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this DbConnectionStringBuilder builder)
        {
            return builder.Connection().OpenWithTransaction();
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction with the specified isolation level that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="isolationLevel">The isolationLevel for the transaction.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this DbConnectionStringBuilder builder, IsolationLevel isolationLevel)
        {
            return builder.Connection().OpenWithTransaction(isolationLevel);
        }

        /// <summary>
        /// Asynchronously opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this DbConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return builder.Connection().OpenWithTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously opens a database connection and begins a new transaction with the specified isolation level that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="isolationLevel">The isolationLevel for the transaction.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this DbConnectionStringBuilder builder, IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return builder.Connection().OpenWithTransactionAsync(isolationLevel, cancellationToken);
        }

        /// <summary>
        /// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static T OpenWithTransactionAs<T>(this DbConnectionStringBuilder builder) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAs<T>();
        }

        /// <summary>
        /// Opens a database connection implementing a given interface and begins a new transaction with the specified isolation level that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="isolationLevel">The isolationLevel for the transaction.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static T OpenWithTransactionAs<T>(this DbConnectionStringBuilder builder, IsolationLevel isolationLevel) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAs<T>(isolationLevel);
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<T> OpenWithTransactionAsAsync<T>(this DbConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAsAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction with the specified isolation level that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="isolationLevel">The isolationLevel for the transaction.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<T> OpenWithTransactionAsAsync<T>(this DbConnectionStringBuilder builder, IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAsAsync<T>(isolationLevel, cancellationToken);
        }
    }
}
