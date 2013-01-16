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
			SqlConnection connection = new SqlConnection();
			connection.ConnectionString = builder.ConnectionString;
			return connection;
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
			SqlConnection connection = builder.Connection();

#if NODBASYNC
			return Task<SqlConnection>.Factory.StartNew(_ => { connection.Open(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#else
			return connection.OpenAsync().ContinueWith(t => { t.Wait(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#endif
		}
	}
}
