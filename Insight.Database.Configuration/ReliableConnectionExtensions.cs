#if !NO_CONNECTION_SETTINGS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
#region ConnectionStringSettings Extensions
		/// <summary>
		/// Creates and returns a new SqlConnection.
		/// </summary>
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed SqlConnection.</returns>
		public static ReliableConnection ReliableConnection(this ConnectionStringSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings", "ConnectionStringSettings cannot be null");
			}

			return new ReliableConnection(settings.Connection());
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static ReliableConnection ReliableOpen(this ConnectionStringSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings", "ConnectionStringSettings cannot be null");
			}

			ReliableConnection connection = settings.ReliableConnection();
			connection.Open();
			return connection;
		}
#endregion

#region Dynamic Methods
		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
		/// </summary>
		/// <param name="settings">The connection to use.</param>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic ReliableDynamic(this ConnectionStringSettings settings)
		{
			return settings.ReliableConnection().Dynamic();
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
		/// </summary>
		/// <param name="settings">The connection to use.</param>
		/// <typeparam name="T">The type of object to return from queries.</typeparam>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic ReliableDynamic<T>(this ConnectionStringSettings settings)
		{
			return settings.ReliableConnection().Dynamic<T>();
		}
#endregion
	}
}
#endif
