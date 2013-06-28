using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Insight.Database.Providers.Oracle
{
	/// <summary>
	/// Implements the Insight provider for Oracle ODP.NET connections.
	/// </summary>
	public class OracleInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Prevents a default instance of the <see cref="OracleInsightDbProvider"/> class from being created.
		/// </summary>
		private OracleInsightDbProvider()
		{
		}

		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(OracleConnectionStringBuilder), typeof(OracleConnection), typeof(OracleCommand), typeof(OracleDataReader), typeof(OracleException) };
			}
		}

		/// <summary>
		/// Registers the Oracle Provider
		/// </summary>
		public static void RegisterProvider()
		{
			new OracleInsightDbProvider().Register();
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
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OracleCommandBuilder.DeriveParameters(command as OracleCommand);
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
		/// <param name="command">The command associated with the reader.</param>
		/// <returns>A string that represents selecting an empty recordset with a single column</returns>
		public override string GenerateEmptySql(IDbCommand command)
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
		/// Set up a table-valued parameter to a procedure.
		/// </summary>
		/// <param name="command">The command to operate on.</param>
		/// <param name="parameter">The parameter to set up.</param>
		/// <param name="list">The list of records.</param>
		/// <param name="listType">The type of object in the list.</param>
		public override void SetupTableValuedParameter(IDbCommand command, IDataParameter parameter, IEnumerable list, Type listType)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");

			// of the many sad things in Oracle, we have to have an array to send the list to the server.

			// return arrays directly
			if (list is Array)
			{
				parameter.Value = list;
				return;
			}

			// this can handle any type that is already a list
			ICollection ilist = list as ICollection;
			if (ilist != null)
			{
				var array = new object[ilist.Count];
				ilist.CopyTo(array, 0);
				parameter.Value = array;
				return;
			}

			// enumerate the rest :(
			parameter.Value = list.Cast<object>().ToArray();
		}

		/// <summary>
		/// Determines if the given column in the schema table is an XML column.
		/// </summary>
		/// <param name="command">The command associated with the reader.</param>
		/// <param name="schemaTable">The schema table to analyze.</param>
		/// <param name="index">The index of the column.</param>
		/// <returns>True if the column is an XML column.</returns>
		public override bool IsXmlColumn(IDbCommand command, DataTable schemaTable, int index)
		{
			if (schemaTable == null) throw new ArgumentNullException("schemaTable");

			return ((OracleDbType)schemaTable.Rows[index]["ProviderType"]) == OracleDbType.XmlType;
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
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public override bool IsTransientException(Exception exception)
		{
			OracleException oracleException = (OracleException)exception;

			// there may be more error codes that we need but there are so many to go through....
			// http://docs.oracle.com/cd/B19306_01/server.102/b14219.pdf
			switch (oracleException.Number)
			{
				case 51:					// timeout waiting for a resource
				case 12150:					// TNS:unable to send data
				case 12153:					// TNS:not connected
				case 12154:					// TNS:could not resolve the connect identifier specified
				case 12157:					// TNS:internal network communication error
				case 12161:					// TNS:internal error: partial data received
				case 12170:					// TNS:connect timeout occurred
				case 12171:					// TNS:could not resolve connect identifier
				case 12203:					// TNS:could not connect to destination
				case 12224:					// TNS:no listener
				case 12225:					// TNS:destination host unreachable
				case 12541:					// TNS:no listener
				case 12543:					// TNS:destination host unreachable
					return true;
			}

			return false;
		}
	}
}
