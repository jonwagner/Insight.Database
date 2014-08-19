using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		/// Regex used to auto-detect cursors in queries.
		/// </summary>
		private static Regex _cursorSql = new Regex(@"OPEN\s+[@:](?<cursor>\w+)\s+FOR", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(OracleConnectionStringBuilder), typeof(OracleConnection), typeof(OracleCommand), typeof(OracleDataReader), typeof(OracleException)
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
			return InsightBulkCopyOptions.UseInternalTransaction;
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new OracleConnection();
		}

		/// <inheritdoc/>
		public override IDbCommand CreateCommand(IDbConnection connection)
		{
			var command = (OracleCommand)connection.CreateCommand();
			command.BindByName = true;

			return command;
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OracleCommandBuilder.DeriveParameters(command as OracleCommand);
		}

		/// <inheritdoc/>
		public override void DeriveParametersFromSqlText(IDbCommand command)
		{
			base.DeriveParametersFromSqlText(command);

			// detect cursors in the command so we can automatically add the parameters as refcursors
			var cursors = _cursorSql.Matches(command.CommandText).OfType<Match>().Select(m => m.Groups[1].Value);

			if (cursors.Any())
			{
				foreach (var c in command.Parameters.OfType<OracleParameter>().Where(p => cursors.Contains(p.ParameterName)))
				{
					c.Direction = ParameterDirection.Output;
					c.OracleDbType = OracleDbType.RefCursor;
				}
			}
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
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			if (transaction != null)
				throw new ArgumentException("OracleProvider does not support external transactions for bulk copy", "transaction");

			OracleBulkCopyOptions oracleOptions = OracleBulkCopyOptions.Default;
			if (options.HasFlag(InsightBulkCopyOptions.UseInternalTransaction))
				oracleOptions |= OracleBulkCopyOptions.UseInternalTransaction;

			using (var bulk = new OracleBulkCopy((OracleConnection)connection, oracleOptions))
			using (var insightBulk = new OracleInsightBulkCopy(bulk))
			{
				bulk.DestinationTableName = tableName;

				// map the columns by name, in case we skipped a readonly column
				foreach (DataRow row in reader.GetSchemaTable().Rows)
					bulk.ColumnMappings.Add((string)row["ColumnName"], (string)row["ColumnName"]);

				if (configure != null)
					configure(insightBulk);
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
				case -6403:					// network address connect timeout
				case 51:					// timeout waiting for a resource
				case 1033:					// ORACLE initialization or shutdown in progress
				case 1034:					// ORACLE not available
				case 1089:					// immediate shutdown in progress - no operations are permitted
				case 3113:					// Closed connection
				case 3135:					// connection lost contact
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
				case 12537:					// TNS: connection closed
				case 12541:					// TNS:no listener
				case 12543:					// TNS:destination host unreachable
				case 12545:					// Connection Failed (Generally a network failure - Cannot Reach Host)
				case 12552:					// TNS: Unable to send break
				case 12571:					// TNS: packet writer failure
					return true;
			}

			return false;
		}

		#region Bulk Copy Support
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
		class OracleInsightBulkCopy : InsightBulkCopy, IDisposable
		{
			private OracleBulkCopy _bulkCopy;

			public OracleInsightBulkCopy(OracleBulkCopy bulkCopy)
			{
				if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

				_bulkCopy = bulkCopy;
				_bulkCopy.OracleRowsCopied += OnRowsCopied;
			}

			public override event InsightRowsCopiedEventHandler RowsCopied;

			public override int BatchSize
			{
				get { return _bulkCopy.BatchSize; }
				set { _bulkCopy.BatchSize = value; }
			}

			public override int BulkCopyTimeout
			{
				get { return _bulkCopy.BulkCopyTimeout; }
				set { _bulkCopy.BulkCopyTimeout = value; }
			}

			public override InsightBulkCopyMappingCollection ColumnMappings
			{
				get { throw new NotImplementedException(); }
			}

			public override int NotifyAfter
			{
				get { return _bulkCopy.NotifyAfter; }
				set { _bulkCopy.NotifyAfter = value; }
			}

			public override string DestinationTableName
			{
				get { return _bulkCopy.DestinationTableName; }
				set { _bulkCopy.DestinationTableName = value; }
			}

			public override object InnerBulkCopy
			{
				get { return _bulkCopy; }
			}

			public void Dispose()
			{
				_bulkCopy.OracleRowsCopied -= OnRowsCopied;
			}

			private void OnRowsCopied(object sender, OracleRowsCopiedEventArgs e)
			{
				var wrappedEvent = new OracleInsightRowsCopiedEventArgs(e);
				if (RowsCopied != null)
					RowsCopied(sender, wrappedEvent);
			}

			class OracleInsightRowsCopiedEventArgs : InsightRowsCopiedEventArgs
			{
				private OracleRowsCopiedEventArgs _event;

				public OracleInsightRowsCopiedEventArgs(OracleRowsCopiedEventArgs e)
				{
					_event = e;
				}

				public override bool Abort
				{
					get { return _event.Abort; }
					set { _event.Abort = value; }
				}

				public override long RowsCopied
				{
					get { return _event.RowsCopied; }
				}
			}
		}
		#endregion
	}
}
