using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glimpse.Ado.AlternateType;
using Insight.Database.Providers;

namespace Insight.Database.Providers.Glimpse
{
	/// <summary>
	/// Implements the Insight provider for Glimpse connections.
	/// </summary>
    public class GlimpseInsightDbProvider : WrappedInsightDbProvider
    {
		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(GlimpseDbConnection), typeof(GlimpseDbCommand) };
			}
		}

		/// <summary>
		/// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
		/// </summary>
		public static void RegisterProvider()
		{
			InsightDbProvider.RegisterProvider(new GlimpseInsightDbProvider());
		}

		/// <inheritdoc/>
		public override IDbConnection CloneDbConnection(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// clone the inner connection
			var innerConnection = GetInnerConnection(connection);
			var innerProvider = InsightDbProvider.For(innerConnection);
			var clonedInnerConnection = (DbConnection)innerProvider.CloneDbConnection(innerConnection);

			return new GlimpseDbConnection(clonedInnerConnection);
		}

		/// <summary>
		/// Unwraps the given connection and returns the inner connection.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public override IDbConnection GetInnerConnection(IDbConnection connection)
		{
			GlimpseDbConnection profiledConnection = (GlimpseDbConnection)connection;
			return profiledConnection.InnerConnection;
		}

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			GlimpseDbCommand profiledCommand = (GlimpseDbCommand)command;
			return profiledCommand.InnerCommand;
		}

        /// <inheritdoc/>
        public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
        {
            connection = GetInnerConnection(connection);
            transaction = ((GlimpseDbTransaction)transaction).InnerTransaction;
            InsightDbProvider.For(connection).BulkCopy(connection, tableName, reader, configure, options, transaction);
        }
	}
}
