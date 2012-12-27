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

		#region Dynamic Invocation Helper
		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
		/// </summary>
		/// <param name="builder">The connection to use.</param>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic Dynamic(this SqlConnectionStringBuilder builder)
		{
			return builder.Connection().Dynamic();
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
		/// </summary>
		/// <param name="builder">The connection to use.</param>
		/// <typeparam name="T">The type of object to return from queries.</typeparam>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic Dynamic<T>(this SqlConnectionStringBuilder builder)
		{
			return builder.Connection().Dynamic<T>();
		}
		#endregion
	}
}
