using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Mapping;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for wrapped connections.
	/// </summary>
	/// <remarks>
	/// For connections that wrap commands in other commands,
	/// this class unwraps the command when Insight needs to access advanced features.
	/// </remarks>
	public abstract class WrappedInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Unwraps the given connection and returns the inner connection.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public abstract IDbConnection GetInnerConnection(IDbConnection connection);

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public abstract IDbCommand GetInnerCommand(IDbCommand command);

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public override IList<IDataParameter> DeriveParameters(IDbCommand command)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).DeriveParameters(command);
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			command = GetInnerCommand(command);
			InsightDbProvider.For(command).DeriveParametersFromStoredProcedure(command);
		}

		/// <summary>
		/// Derives the parameter list from a sql text command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromSqlText(IDbCommand command)
		{
			command = GetInnerCommand(command);
			InsightDbProvider.For(command).DeriveParametersFromSqlText(command);
		}

		/// <inheritdoc/>
		public override void FixupCommand(IDbCommand command)
		{
			command = GetInnerCommand(command);
			InsightDbProvider.For(command).FixupCommand(command);
		}

		/// <inheritdoc/>
		public override CommandBehavior FixupCommandBehavior(IDbCommand command, CommandBehavior commandBehavior)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).FixupCommandBehavior(command, commandBehavior);
		}

		/// <inheritdoc/>
		public override void FixupParameter(IDbCommand command, IDataParameter parameter, DbType dbType, Type type, SerializationMode serializationMode)
		{
			command = GetInnerCommand(command);
			InsightDbProvider.For(command).FixupParameter(command, parameter, dbType, type, serializationMode);
		}

		/// <inheritdoc/>
		public override InsightBulkCopyOptions GetSupportedBulkCopyOptions(IDbConnection connection)
		{
			connection = GetInnerConnection(connection);
			return InsightDbProvider.For(connection).GetSupportedBulkCopyOptions(connection);
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).CloneParameter(command, parameter);
		}

		/// <summary>
		/// Determines if a parameter is an XML type parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is an XML parameter.</returns>
		public override bool IsXmlParameter(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).IsXmlParameter(command, parameter);
		}

		/// <summary>
		/// Determines if a parameter is a Table-valued parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is a table-valued parameter.</returns>
		public override bool IsTableValuedParameter(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).IsTableValuedParameter(command, parameter);
		}

		/// <summary>
		/// Set up a table-valued parameter to a procedure.
		/// </summary>
		/// <param name="command">The command to operate on.</param>
		/// <param name="parameter">The parameter to set up.</param>
		/// <param name="list">The list of records.</param>
		/// <param name="listType">The type of object in the list.</param>
		public override void SetupTableValuedParameter(IDbCommand command, IDataParameter parameter, IEnumerable list, Type listType)
		{
			command = GetInnerCommand(command);
			InsightDbProvider.For(command).SetupTableValuedParameter(command, parameter, list, listType);
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			connection = GetInnerConnection(connection);
			return InsightDbProvider.For(connection).GetTableSchemaSql(connection, tableName);
		}

		/// <summary>
		/// Bulk copies a set of objects to the server.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="reader">The reader to read objects from.</param>
		/// <param name="configure">A callback method to configure the bulk copy object.</param>
		/// <param name="options">Options for initializing the bulk copy object.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <remarks>Number of rows copied.</remarks>
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			connection = GetInnerConnection(connection);
			InsightDbProvider.For(connection).BulkCopy(connection, tableName, reader, configure, options, transaction);
		}

        /// <inheritdoc/>
        public override Task BulkCopyAsync(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            connection = GetInnerConnection(connection);
            return InsightDbProvider.For(connection).BulkCopyAsync(connection, tableName, reader, configure, options, transaction, cancellationToken);
        }
	}
}
