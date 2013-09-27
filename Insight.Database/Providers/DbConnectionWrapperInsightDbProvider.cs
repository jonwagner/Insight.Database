using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Reliable;

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
			typeof(DbConnectionWrapper)
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
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			DbConnectionWrapper wrapped = (DbConnectionWrapper)connection;

			base.BulkCopy(connection, tableName, reader, configure, options, transaction ?? wrapped.InnerTransaction);
		}
	}
}
