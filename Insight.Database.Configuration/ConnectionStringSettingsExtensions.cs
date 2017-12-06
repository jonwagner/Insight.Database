#if !NO_CONNECTION_SETTINGS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
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
			{
				throw new ArgumentNullException("settings");
			}

			DbConnection disposable = null;
			try
			{
				DbConnection connection = null;

				// if there is a provider on the connection string, use that to create the connection
				// otherwise use a sql connection
				if (String.IsNullOrEmpty(settings.ProviderName))
				{
					connection = new System.Data.SqlClient.SqlConnection();
				}
				else
				{
					connection = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection();
				}

				disposable = connection;
				connection.ConnectionString = settings.ConnectionString;
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
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed connection.</returns>
		public static T As<T>(this ConnectionStringSettings settings) where T : class
		{
			return settings.Connection().As<T>();
		}

		/// <summary>
		/// Creates and returns a new multi-threaded connection implementing the given interface.
		/// The object can support making multiple calls at the same time.
		/// </summary>
		/// <typeparam name="T">The interface to implement on the connection.</typeparam>
		/// <param name="settings">The ConnectionStringSettings containing the connection string.</param>
		/// <returns>A closed connection that implements the given interface.</returns>
		public static T AsParallel<T>(this ConnectionStringSettings settings) where T : class
		{
			Func<IDbConnection> constructor = (() => settings.Connection());
			return constructor.AsParallel<T>();
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static DbConnection Open(this ConnectionStringSettings settings)
		{
			return settings.Connection().OpenConnection();
		}

		/// <summary>
		/// Opens and returns a database connection.
		/// </summary>
		/// <param name="settings">The connection string to open and return.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>The opened connection.</returns>
		public static Task<DbConnection> OpenAsync(this ConnectionStringSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			return settings.Connection().OpenConnectionAsync(cancellationToken);
		}

		/// <summary>
		/// Opens and returns a database connection implementing a given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implmement.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <returns>The opened connection.</returns>
		public static T OpenAs<T>(this ConnectionStringSettings settings) where T : class, IDbConnection
		{
			return settings.Connection().OpenAs<T>();
		}

		/// <summary>
		/// Asynchronously opens and returns a database connection implementing a given interface.
		/// </summary>
		/// <typeparam name="T">The interface to implmement.</typeparam>
		/// <param name="settings">The connection string to open and return.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>The opened connection.</returns>
		public static Task<T> OpenAsAsync<T>(this ConnectionStringSettings settings, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection
		{
			return settings.Connection().OpenAsAsync<T>(cancellationToken);
		}

		/// <summary>
		/// Opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <param name="settings">The settings for the connection.</param>
		/// <returns>A wrapper for the database connection.</returns>
		public static DbConnectionWrapper OpenWithTransaction(this ConnectionStringSettings settings)
		{
			return settings.Connection().OpenWithTransaction();
		}

		/// <summary>
		/// Asynchronously opens a database connection and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <param name="settings">The settings for the connection.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning a connection when the connection has been opened.</returns>
		public static Task<DbConnectionWrapper> OpenWithTransactionAsync(this ConnectionStringSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			return settings.Connection().OpenWithTransactionAsync(cancellationToken);
		}

		/// <summary>
		/// Opens a database connection implementing a given interface and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="settings">The settings for the connection.</param>
		/// <returns>A wrapper for the database connection.</returns>
		public static T OpenWithTransactionAs<T>(this ConnectionStringSettings settings) where T : class, IDbConnection, IDbTransaction
		{
			return settings.Connection().OpenWithTransactionAs<T>();
		}

		/// <summary>
		/// Asynchronously opens a database connection implementing a given interface, and begins a new transaction that is disposed when the returned object is disposed.
		/// </summary>
		/// <typeparam name="T">The interface to implement.</typeparam>
		/// <param name="settings">The settings for the connection.</param>
		/// <param name="cancellationToken">The cancellation token to use for the operation.</param>
		/// <returns>A task returning a connection when the connection has been opened.</returns>
		public static Task<T> OpenWithTransactionAsAsync<T>(this ConnectionStringSettings settings, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IDbConnection, IDbTransaction
		{
			return settings.Connection().OpenWithTransactionAsAsync<T>(cancellationToken);
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
#endif
