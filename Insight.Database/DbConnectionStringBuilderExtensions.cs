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
		/// Creates and returns a new SqlConnection.
		/// </summary>
		/// <param name="builder">The SqlConnectionStringBuilder containing the connection string.</param>
		/// <returns>A closed SqlConnection.</returns>
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
		/// <returns>The opened connection.</returns>
		public static Task<DbConnection> OpenAsync(this DbConnectionStringBuilder builder)
		{
			DbConnection connection = builder.Connection();

#if NODBASYNC
			return Task<DbConnection>.Factory.StartNew(_ => { connection.Open(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#else
			return connection.OpenAsync().ContinueWith(t => { t.Wait(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#endif
		}

		/// <summary>
		/// Opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnectionWithTransaction OpenWithTransaction(this DbConnectionStringBuilder builder)
		{
			return new DbConnectionWithTransaction(builder.Open());
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnectionWithTransaction> OpenWithTransactionAsync(this DbConnectionStringBuilder builder)
		{
			return builder.OpenAsync().ContinueWith(t => new DbConnectionWithTransaction(t.Result), TaskContinuationOptions.ExecuteSynchronously);
		}
	}
}
