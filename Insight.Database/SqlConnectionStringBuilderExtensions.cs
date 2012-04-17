using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for DbConnectionStringBuilder.
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
			return new SqlConnection(builder.ConnectionString);
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
	}
}
