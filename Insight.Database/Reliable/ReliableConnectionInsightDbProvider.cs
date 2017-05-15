using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Insight.Database.Providers;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Implements the Insight provider for Reliable connections.
	/// </summary>
	class ReliableConnectionInsightDbProvider : DbConnectionWrapperInsightDbProvider
	{
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(ReliableConnection), typeof(ReliableCommand)
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
		public override IDbConnection CloneDbConnection(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// clone the inner connection
			var reliable = (ReliableConnection)connection;
			var innerConnection = GetInnerConnection(connection);
			var innerProvider = InsightDbProvider.For(innerConnection);
			var clonedInnerConnection = innerProvider.CloneDbConnection(innerConnection);

			return new ReliableConnection((DbConnection)clonedInnerConnection, reliable.RetryStrategy);
		}
	}
}
