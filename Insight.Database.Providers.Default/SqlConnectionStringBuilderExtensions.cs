using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods for SqlConnectionStringBuilder.
    /// </summary>
    public static class SqlConnectionStringBuilderExtensions
    {
        /// <summary>
        /// Creates and returns a new SqlConnection.
        /// </summary>
        /// <param name="builder">The SqlConnectionStringBuilder containing the connection string.</param>
        /// <returns>A closed SqlConnection.</returns>
        public static SqlConnection Connection(this SqlConnectionStringBuilder builder)
        {
            if (builder == null) { throw new ArgumentNullException("builder"); }

            SqlConnection disposable = null;
            try
            {
                SqlConnection connection = new SqlConnection();
                disposable = connection;
                connection.ConnectionString = builder.ConnectionString;
                disposable = null;
                return connection;
            }
            finally
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates and returns a new DbConnection for the connection string and implments the given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implement on the connection.</typeparam>
        /// <param name="builder">The SqlConnectionStringBuilder containing the connection string.</param>
        /// <returns>A closed connection.</returns>
        public static T As<T>(this SqlConnectionStringBuilder builder) where T : class
        {
            return builder.Connection().As<T>();
        }

        /// <summary>
        /// Opens and returns a database connection.
        /// </summary>
        /// <param name="builder">The connection string to open and return.</param>
        /// <returns>The opened connection.</returns>
        public static SqlConnection Open(this SqlConnectionStringBuilder builder)
        {
            return builder.Connection().OpenConnection();
        }

        /// <summary>
        /// Opens and returns a database connection.
        /// </summary>
        /// <param name="builder">The connection string to open and return.</param>
        /// <returns>The opened connection.</returns>
        public static Task<SqlConnection> OpenAsync(this SqlConnectionStringBuilder builder)
        {
            return builder.Connection().OpenConnectionAsync();
        }

        /// <summary>
        /// Opens and returns a database connection implementing a given interface.
        /// </summary>
        /// <typeparam name="T">The interface to implmement.</typeparam>
        /// <param name="builder">The connection string to open and return.</param>
        /// <returns>The opened connection.</returns>
        public static T OpenAs<T>(this SqlConnectionStringBuilder builder) where T : class, IDbConnection
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
        public static Task<T> OpenAsAsync<T>(this SqlConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection
        {
            return builder.Connection().OpenAsAsync<T>(cancellationToken);
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this SqlConnectionStringBuilder builder)
        {
            return builder.Connection().OpenWithTransaction();
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction with the specified isolation level that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="isolationLevel">The isolationLevel for the transaction.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this SqlConnectionStringBuilder builder, IsolationLevel isolationLevel)
        {
            return builder.Connection().OpenWithTransaction(isolationLevel);
        }

        /// <summary>
        /// Asynchronously opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this SqlConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return builder.Connection().OpenWithTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static T OpenWithTransactionAs<T>(this SqlConnectionStringBuilder builder) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAs<T>();
        }

        /// <summary>
        /// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
        /// </summary>
        /// <typeparam name="T">The interface to implement.</typeparam>
        /// <param name="builder">The builder for the connection.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>A task returning a connection when the connection has been opened.</returns>
        public static Task<T> OpenWithTransactionAsAsync<T>(this SqlConnectionStringBuilder builder, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
        {
            return builder.Connection().OpenWithTransactionAsAsync<T>(cancellationToken);
        }
    }
}
