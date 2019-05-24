using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for connections wrapped in transactions.
	/// </summary>
	class DbConnectionWrapperInsightDbProvider : WrappedInsightDbProvider
	{
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(DbConnectionWrapper), typeof(DbCommandWrapper)
		};

		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return _supportedTypes;
			}
		}

		/// <inheritdoc/>
		public override InsightBulkCopyOptions GetSupportedBulkCopyOptions(IDbConnection connection)
		{
			connection = GetInnerConnection(connection);
			return InsightDbProvider.For(connection).GetSupportedBulkCopyOptions(connection);
		}

		/// <summary>
		/// Clones a new DbConnection supported by this provider.
		/// </summary>
		/// <param name="connection">The connection to clone.</param>
		/// <returns>A new DbConnection.</returns>
		public override IDbConnection CloneDbConnection(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// clone the inner connection
			var innerConnection = GetInnerConnection(connection);
			var innerProvider = InsightDbProvider.For(innerConnection);
			var clonedInnerConnection = innerProvider.CloneDbConnection(innerConnection);

			var wrapper = new DbConnectionWrapper();
			try
			{
				var clone = (DbConnectionWrapper)wrapper;
				clone.InnerConnection = (DbConnection)clonedInnerConnection;
				return clone;
			}
			catch
			{
				wrapper.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Unwraps the given connection and returns the inner command.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public override IDbConnection GetInnerConnection(IDbConnection connection)
		{
			DbConnectionWrapper wrapped = (DbConnectionWrapper)connection;
			return wrapped.InnerConnection;
		}

		/// <inheritdoc/>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			DbCommandWrapper wrapped = command as DbCommandWrapper;
			return wrapped?.InnerCommand ?? command;
		}

		/// <inheritdoc/>
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			DbConnectionWrapper wrapped = (DbConnectionWrapper)connection;

			base.BulkCopy(connection, tableName, reader, configure, options, transaction ?? wrapped.InnerTransaction);
		}

		/// <inheritdoc/>
		public override Task BulkCopyAsync(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction, CancellationToken cancellationToken)
		{
			DbConnectionWrapper wrapped = (DbConnectionWrapper)connection;

			return base.BulkCopyAsync(connection, tableName, reader, configure, options, transaction ?? wrapped.InnerTransaction, cancellationToken);
		}
	}
}
