using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Insight.Database.Providers.PostgreSQL
{
	/// <summary>
	/// Implements the Insight provider for PostgreSQL connections.
	/// </summary>
	public class PostgreSQLInsightDbProvider : InsightDbProvider
	{
		#region Fields
		/// <summary>
		/// The delimiter for the CSV stream.
		/// </summary>
		private const char CsvDelimiter = ',';

		/// <summary>
		/// The Quote character for the CSV stream.
		/// </summary>
		private const char CsvQuote = '"';

		/// <summary>
		/// The replacement pattern for the CSV stream.
		/// </summary>
		private const string CsvReplacement = "$+$+";

		/// <summary>
		/// The replacement regex for the CSV stream.
		/// </summary>
		private static Regex _csvRegex = new Regex(@"("")", RegexOptions.Compiled);

		/// <summary>
		/// The prefix used on parameter names.
		/// </summary>
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(NpgsqlConnectionStringBuilder), typeof(NpgsqlConnection), typeof(NpgsqlCommand), typeof(NpgsqlParameter), typeof(NpgsqlDataReader), typeof(NpgsqlException)
		};
		#endregion

		/// <summary>
		/// Prevents a default instance of the <see cref="PostgreSQLInsightDbProvider"/> class from being created.
		/// </summary>
		private PostgreSQLInsightDbProvider()
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
				return _supportedTypes;
			}
		}

		/// <summary>
		/// Registers the Oracle Provider
		/// </summary>
		public static void RegisterProvider()
		{
			new PostgreSQLInsightDbProvider().Register();
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new NpgsqlConnection();
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			NpgsqlCommandBuilder.DeriveParameters(command as NpgsqlCommand);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<NpgsqlParameter>())
				p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty);
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			NpgsqlParameter p = (NpgsqlParameter)parameter;
			return (IDataParameter)p.Clone();
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

			return String.Compare(schemaTable.Rows[index]["ProviderType"].ToString(), "xml", StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			return String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} LIMIT 0", tableName);
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

			NpgsqlCopyIn bulk = new NpgsqlCopyIn(String.Format(CultureInfo.InvariantCulture, "COPY {0} FROM STDIN WITH CSV", tableName), (NpgsqlConnection)connection);
			PostgreSQLInsightBulkCopy insightBulkCopy = new PostgreSQLInsightBulkCopy(bulk);

			try
			{
				bulk.Start();

				var stream = bulk.CopyStream;
				StreamWriter writer = new StreamWriter(stream);

				int row = 0;
				while (reader.Read())
				{
					for (int i = 0; i < reader.FieldCount; i++)
					{
						if (i > 0)
							writer.Write(CsvDelimiter);

						object value = reader.GetValue(i);

						if (value != DBNull.Value)
						{
							writer.Write(CsvQuote);
							writer.Write(_csvRegex.Replace(value.ToString(), CsvReplacement));
							writer.Write(CsvQuote);
						}
					}

					writer.WriteLine();

					row++;
					if (insightBulkCopy.NotifyAfter != 0 && row % insightBulkCopy.NotifyAfter == 0)
					{
						InsightRowsCopiedEventArgs e = new InsightRowsCopiedEventArgs();
						e.RowsCopied = row;
						insightBulkCopy.OnRowsCopied(insightBulkCopy, e);
						if (e.Abort)
						{
							bulk.Cancel("Cancelled");
							return;
						}
					}
				}

				// must call flush before end
				// cannot call close on the stream before end
				writer.Flush();
				bulk.End();
			}
			catch (Exception e)
			{
				bulk.Cancel(e.Message);

				throw;
			}
		}

		/// <summary>
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public override bool IsTransientException(Exception exception)
		{
			NpgsqlException mex = (NpgsqlException)exception;
			switch (mex.ErrorCode)
			{
				case -2147467259:			// socket exception - could not connect to host
					return true;
			}

			return false;
		}

		#region Bulk Copy Support
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
		sealed class PostgreSQLInsightBulkCopy : InsightBulkCopy
		{
			private NpgsqlCopyIn _bulkCopy;

			public PostgreSQLInsightBulkCopy(NpgsqlCopyIn bulkCopy)
			{
				if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

				_bulkCopy = bulkCopy;
				NotifyAfter = 1000;
			}

			public override event InsightRowsCopiedEventHandler RowsCopied;

			public override int BatchSize
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public override int BulkCopyTimeout
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public override InsightBulkCopyMappingCollection ColumnMappings
			{
				get { throw new NotImplementedException(); }
			}

			public override int NotifyAfter { get; set; }

			public override string DestinationTableName
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public override object InnerBulkCopy
			{
				get { return _bulkCopy; }
			}

			internal void OnRowsCopied(object sender, InsightRowsCopiedEventArgs e)
			{
				if (RowsCopied != null)
					RowsCopied(sender, e);
			}
		}
		#endregion
	}
}
