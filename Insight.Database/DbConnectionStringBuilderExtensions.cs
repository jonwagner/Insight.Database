using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			DbConnection connection = null;

			// get the connection from the provider
			// if the provider is not specified, then attempt to get the type
			if (builder is SqlConnectionStringBuilder)
				connection = new SqlConnection();
			else if (builder is OdbcConnectionStringBuilder)
				connection = new OdbcConnection();
			else if (builder is OleDbConnectionStringBuilder)
				connection = new OleDbConnection();

			if (connection == null)
				throw new ArgumentException("Cannot determine the type of connection from the ConnectionStringBuilder", "builder");

			connection.ConnectionString = builder.ConnectionString;
			return connection;
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
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnection Open(this DbConnectionStringBuilder builder)
		{
			return builder.Connection().OpenConnection();
		}

		/// <summary>
		/// Opens and returns a database connection implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static T OpenAs<T>(this DbConnectionStringBuilder builder) where T : class
		{
			return builder.Open().As<T>();
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnection> OpenAsync(this DbConnectionStringBuilder builder)
		{
			DbConnection connection = builder.Connection();

#if NODBASYNC
			return Task<DbConnection>.Factory.StartNew(_ => { connection.Open(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#else
			return connection.OpenAsync().ContinueWith(t => connection, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
#endif
		}

		/// <summary>
		/// Opens and returns a database connection implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenAsAsync<T>(this DbConnectionStringBuilder builder) where T : class
		{
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)builder.Connection().As<T>();

			return connection.OpenAsync().ContinueWith(t => (T)(object)connection, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnectionWrapper OpenWithTransaction(this DbConnectionStringBuilder builder)
		{
			var connection = new DbConnectionWrapper(builder.Open());
			connection.BeginAutoTransaction();
			return connection;
		}

		/// <summary>
		/// Opens and returns a database connection with an open transaction implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static T OpenWithTransactionAs<T>(this DbConnectionStringBuilder builder) where T : class
		{
			var t = builder.OpenAs<T>();
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)t;
			connection.BeginAutoTransaction();
			return t;
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this DbConnectionStringBuilder builder)
		{
			var connection = new DbConnectionWrapper(builder.Connection());

			return connection.OpenAsync().ContinueWith(
				task =>
				{
					connection.BeginAutoTransaction();
					return connection;
				},
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection with an open transaction implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenWithTransactionAsAsync<T>(this DbConnectionStringBuilder builder) where T : class
		{
			var t = builder.Connection().As<T>();
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)t;

			return connection.OpenAsync().ContinueWith(
				task =>
				{
					connection.BeginAutoTransaction();
					return t;
				},
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}
	}
}
