using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if USE_CORE
using IBM.Data.DB2.Core;
#else
using IBM.Data.DB2;
#endif

namespace Insight.Database.Providers.DB2
{
	/// <summary>
	/// Implements a provider to use DB2 with Insight.Database.
	/// </summary>
    public class DB2InsightDbProvider : InsightDbProvider
    {
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(DB2ConnectionStringBuilder), typeof(DB2Connection), typeof(DB2Command), typeof(DB2DataReader), typeof(DB2Exception), typeof(DB2Parameter)
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
		/// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
		/// </summary>
		public static void RegisterProvider()
		{
			InsightDbProvider.RegisterProvider(new DB2InsightDbProvider());
		}

		/// <inheritdoc/>
		public override InsightBulkCopyOptions GetSupportedBulkCopyOptions(IDbConnection connection)
		{
			return InsightBulkCopyOptions.KeepIdentity |
				InsightBulkCopyOptions.TableLock |
				InsightBulkCopyOptions.Truncate;
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new DB2Connection();
		}

#if !NO_DERIVE_PARAMETERS
		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			DB2CommandBuilder.DeriveParameters(command as DB2Command);
		}
#endif

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			DB2Parameter p = (DB2Parameter)base.CloneParameter(command, parameter);
			DB2Parameter db2p = (DB2Parameter)parameter;

			p.ArrayLength = db2p.ArrayLength;
			p.DB2Type = db2p.DB2Type;
			p.DB2TypeOutput = db2p.DB2TypeOutput;
			p.InternalProperty1 = db2p.InternalProperty1;
			p.IsDefault = db2p.IsDefault;
			p.IsUnassigned = db2p.IsUnassigned;
			p.Precision = db2p.Precision;
			p.Scale = db2p.Scale;

			return p;
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

			var op = (DB2Parameter)parameter;
			return op.DB2Type == DB2Type.Xml;
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			return String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} FETCH FIRST 1 ROWS ONLY", tableName);
		}

		/// <inheritdoc/>
		public override bool IsXmlColumn(IDataReader reader, int index)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			return string.Compare(((DbDataReader)reader).GetDataTypeName(index), "XML", StringComparison.OrdinalIgnoreCase) == 0;
		}

#if !NO_BULK_COPY
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
				throw new ArgumentException("DB2Provider does not support external transactions for bulk copy", "transaction");

			DB2BulkCopyOptions db2Options = DB2BulkCopyOptions.Default;
			if (options.HasFlag(InsightBulkCopyOptions.KeepIdentity))
				db2Options |= DB2BulkCopyOptions.KeepIdentity;
			if (options.HasFlag(InsightBulkCopyOptions.TableLock))
				db2Options |= DB2BulkCopyOptions.TableLock;
			if (options.HasFlag(InsightBulkCopyOptions.Truncate))
				db2Options |= DB2BulkCopyOptions.Truncate;

			using (var bulk = new DB2BulkCopy((DB2Connection)connection, db2Options))
			using (var insightBulk = new DB2InsightBulkCopy(bulk))
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
#endif

		/// <summary>
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public override bool IsTransientException(Exception exception)
		{
			DB2Exception db2Exception = (DB2Exception)exception;

			return db2Exception.Errors.OfType<DB2Error>().Any(
				e =>
				{
					switch (e.NativeError)
					{
						case -30080:				// communication error
						case -30081:				// communication error
							return true;
					}

					return false;
				});
		}

#if !NO_BULK_COPY
		#region Bulk Copy Support
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
		class DB2InsightBulkCopy : InsightBulkCopy, IDisposable
		{
			private DB2BulkCopy _bulkCopy;

			public DB2InsightBulkCopy(DB2BulkCopy bulkCopy)
			{
				if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

				_bulkCopy = bulkCopy;
				_bulkCopy.DB2RowsCopied += OnRowsCopied;
			}

			public override event InsightRowsCopiedEventHandler RowsCopied;

			public override int BatchSize { get; set; }

			public override int BulkCopyTimeout
			{
				get { return _bulkCopy.BulkCopyTimeout; }
				set { _bulkCopy.BulkCopyTimeout = value; }
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
				_bulkCopy.DB2RowsCopied -= OnRowsCopied;
			}

			private void OnRowsCopied(object sender, DB2RowsCopiedEventArgs e)
			{
				var wrappedEvent = new DB2InsightRowsCopiedEventArgs(e);
				if (RowsCopied != null)
					RowsCopied(sender, wrappedEvent);
			}

			class DB2InsightRowsCopiedEventArgs : InsightRowsCopiedEventArgs
			{
				private DB2RowsCopiedEventArgs _event;

				public DB2InsightRowsCopiedEventArgs(DB2RowsCopiedEventArgs e)
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
#endif
	}
}
