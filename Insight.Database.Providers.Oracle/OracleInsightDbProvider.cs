using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for Oracle ODP.NET connections.
	/// </summary>
	public class OracleInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(OracleCommand);
			}
		}

		/// <summary>
		/// Gets the type for ConnectionStringBuilders supported by this provider.
		/// </summary>
		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(OracleConnectionStringBuilder);
			}
		}

		/// <summary>
		/// Gets the type for Connections supported by this provider.
		/// </summary>
		public override Type ConnectionType
		{
			get
			{
				return typeof(OracleConnection);
			}
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new OracleConnection();
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			// thank you, oracle
			OracleParameter p = (OracleParameter)parameter;
			return (IDataParameter)p.Clone();
		}

		/// <summary>
		/// Returns a string that represents selecting an empty recordset with a single column.
		/// </summary>
		/// <returns>A string that represents selecting an empty recordset with a single column</returns>
		public override string GenerateEmptySql()
		{
			return "SELECT * FROM dual WHERE 1 = 0";
		}

		/// <summary>
		/// Determines if a parameter is an XML type parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is an XML parameter.</returns>
		public override bool IsXmlParameter(IDbCommand command, IDataParameter parameter)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");

			var op = (OracleParameter)parameter;
			return op.OracleDbType == OracleDbType.XmlType;
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			return String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE rownum = 0", tableName);
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
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<object> configure, int? options, IDbTransaction transaction)
		{
			if (transaction != null)
				throw new ArgumentException("OracleProvider does not support external transactions for bulk copy", "transaction");

			if (options == null)
				options = (int)OracleBulkCopyOptions.Default;

			using (var bulk = new OracleBulkCopy((OracleConnection)connection, (OracleBulkCopyOptions)options))
			{
				bulk.DestinationTableName = tableName;
				if (configure != null)
					configure(bulk);
				bulk.WriteToServer(reader);
			}
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		protected override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OracleCommandBuilder.DeriveParameters(command as OracleCommand);
		}
	}
}
