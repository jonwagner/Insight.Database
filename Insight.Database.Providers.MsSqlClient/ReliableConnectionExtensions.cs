using System;
using System.Collections.Generic;
#if !NO_CONNECTION_SETTINGS
using System.Configuration;
#endif
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Extension methods for Reliable Connections.
	/// </summary>
	public static class ReliableConnectionExtensions
	{
		#region SqlConnectionStringBuilder Extensions
		/// <summary>
		/// Creates and returns a new SqlConnection.
		/// </summary>
		/// <param name="builder">The SqlConnectionStringBuilder containing the connection string.</param>
		/// <returns>A closed SqlConnection.</returns>
		public static ReliableConnection<SqlConnection> ReliableConnection(this SqlConnectionStringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder", "SqlConnectionStringBuilder cannot be null");
			}

			return new ReliableConnection<SqlConnection>(builder.ConnectionString);
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="builder">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static ReliableConnection<SqlConnection> ReliableOpen(this SqlConnectionStringBuilder builder)
		{
			return builder.ReliableConnection().OpenConnection();
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
		/// </summary>
		/// <param name="builder">The connection to use.</param>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic ReliableDynamic(this SqlConnectionStringBuilder builder)
		{
			return builder.ReliableConnection().Dynamic();
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
		/// </summary>
		/// <param name="builder">The connection to use.</param>
		/// <typeparam name="T">The type of object to return from queries.</typeparam>
		/// <returns>A DynamicConnection using the given connection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public static dynamic ReliableDynamic<T>(this SqlConnectionStringBuilder builder)
		{
			return builder.ReliableConnection().Dynamic<T>();
		}
		#endregion
	}
}
