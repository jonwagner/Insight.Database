using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for ConnectionStringSettings.
	/// </summary>
	public static class ConnectionStringSettingsExtensions
	{
		/// <summary>
		/// Creates and returns a new SqlConnection.
		/// </summary>
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed SqlConnection.</returns>
		public static SqlConnection Connection(this ConnectionStringSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings", "ConnectionStringSettings cannot be null");

			return new SqlConnection(settings.ConnectionString);
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static SqlConnection Open(this ConnectionStringSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings", "ConnectionStringSettings cannot be null");

			SqlConnection connection = settings.Connection();
			connection.Open();
			return connection;
		}

		#region Dynamic Invocation Helper
		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of FastExpando.
		/// </summary>
		/// <param name="settings">The connection to use.</param>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic Dynamic(this ConnectionStringSettings settings)
		{
			return settings.Connection().Dynamic();
		}

		/// <summary>
		/// Converts the connection to a connection that can be invoked dynamically to return lists of type T.
		/// </summary>
		/// <param name="settings">The connection to use.</param>
		/// <typeparam name="T">The type of object to return from queries.</typeparam>
		/// <returns>A DynamicConnection using the given connection.</returns>
		public static dynamic Dynamic<T>(this ConnectionStringSettings settings)
		{
			return settings.Connection().Dynamic<T>();
		}
		#endregion
	}
}
