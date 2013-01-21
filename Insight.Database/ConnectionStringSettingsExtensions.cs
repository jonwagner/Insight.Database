using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for ConnectionStringSettings.
	/// </summary>
	public static class ConnectionStringSettingsExtensions
	{
		#region Connection and Open Members
		/// <summary>
		/// Creates and returns a new DbConnection for the connection string.
		/// </summary>
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed DbConnection.</returns>
		public static DbConnection Connection(this ConnectionStringSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			DbConnection connection;

			// if there is a provider on the connection string, use that to create the connection
			// otherwise use a sql connection
			if (String.IsNullOrEmpty(settings.ProviderName))
				connection = new SqlConnection();
			else
				connection = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection();

			connection.ConnectionString = settings.ConnectionString;
			return connection;
		}

		/// <summary>
		/// Creates and returns a new DbConnection for the connection string and implments the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed connection.</returns>
		public static T As<T>(this ConnectionStringSettings settings) where T : class
		{
			return settings.Connection().As<T>();
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnection Open(this ConnectionStringSettings settings)
		{
			DbConnection connection = settings.Connection();
			connection.Open();
			return connection;
		}

		/// <summary>
		/// Opens and returns a database connection implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static T OpenAs<T>(this ConnectionStringSettings settings) where T : class
		{
			return settings.Open().As<T>();
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnection> OpenAsync(this ConnectionStringSettings settings)
		{
			DbConnection connection = settings.Connection();

#if NODBASYNC
			return Task<DbConnection>.Factory.StartNew(_ => { connection.Open(); return connection; }, TaskContinuationOptions.ExecuteSynchronously);
#else
			return connection.OpenAsync().ContinueWith(t => connection, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
#endif
		}

		/// <summary>
		/// Opens and returns a database connection implmenting the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenAsAsync<T>(this ConnectionStringSettings settings) where T : class
		{
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)settings.Connection().As<T>();

			return connection.OpenAsync().ContinueWith(t => (T)(object)connection, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnectionWrapper OpenWithTransaction(this ConnectionStringSettings settings)
		{
			var connection = new DbConnectionWrapper(settings.Connection());
			connection.Open();
			connection.BeginTransaction();
			return connection;
		}

		/// <summary>
		/// Opens and returns a database connection with an open transaction and implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static T OpenWithTransactionAs<T>(this ConnectionStringSettings settings) where T : class
		{
			var t = settings.OpenAs<T>();
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)t;
			connection.BeginAutoTransaction();
			return t;
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection with an open transaction.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this ConnectionStringSettings settings)
		{
			var connection = new DbConnectionWrapper(settings.Connection());

			return connection.OpenAsync().ContinueWith(
				t =>
				{
					connection.BeginAutoTransaction();
					return connection;
				},
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection with an open transaction and implementing the given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenWithTransactionAsAsync<T>(this ConnectionStringSettings settings) where T : class
		{
			var t = settings.Connection().As<T>();
			DbConnectionWrapper connection = (DbConnectionWrapper)(object)t;

			return connection.OpenAsync().ContinueWith(
				task =>
				{
					connection.BeginAutoTransaction();
					return t;
				},
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
		}
		#endregion

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
