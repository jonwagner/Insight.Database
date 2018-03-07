using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sybase.Data.AseClient;

namespace Insight.Database.Providers.SybaseAse
{
	/// <summary>
	/// An InsightDbProvider for Sybase ASE.
	/// </summary>
    public class SybaseAseInsightDbProvider : InsightDbProvider
    {
		/// <summary>
		/// The prefix used on parameter names.
		/// </summary>
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(AseConnectionStringBuilder), typeof(AseConnection), typeof(AseCommand), typeof(AseDataReader), typeof(AseException), typeof(AseParameter)
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
			return InsightBulkCopyOptions.CheckConstraints |
				InsightBulkCopyOptions.FireTriggers |
				InsightBulkCopyOptions.KeepIdentity |
				InsightBulkCopyOptions.KeepNulls |
				InsightBulkCopyOptions.TableLock |
				InsightBulkCopyOptions.UseInternalTransaction;
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new AseConnection();
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			AseCommandBuilder.DeriveParameters(command as AseCommand);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<AseParameter>())
				p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty);
		}

		/// <inheritdoc/>
		public override void FixupCommand(IDbCommand command)
		{
			base.FixupCommand(command);

			foreach (var p in command.Parameters.OfType<AseParameter>())
				p.ParameterName = "@" + p.ParameterName;
		}

		/// <summary>
		/// Called before reading output parameters from the command.
		/// </summary>
		/// <param name="command">The command to fix up.</param>
		public override void FixupOutputParameters(IDbCommand command)
		{
			base.FixupOutputParameters(command);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<AseParameter>())
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
			// thank you, sybase
			AseParameter p = (AseParameter)parameter;
			return (IDataParameter)p.Clone();
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			return String.Format(CultureInfo.InvariantCulture, "SELECT TOP 0 * FROM {0}", tableName);
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

			AseBulkCopyOptions aseOptions = AseBulkCopyOptions.Default;
			if (options.HasFlag(InsightBulkCopyOptions.CheckConstraints))
				aseOptions |= AseBulkCopyOptions.CheckConstraints;
			if (options.HasFlag(InsightBulkCopyOptions.FireTriggers))
				aseOptions |= AseBulkCopyOptions.FireTriggers;
			if (options.HasFlag(InsightBulkCopyOptions.KeepIdentity))
				aseOptions |= AseBulkCopyOptions.KeepIdentity;
			if (options.HasFlag(InsightBulkCopyOptions.KeepNulls))
				aseOptions |= AseBulkCopyOptions.KeepNulls;
			if (options.HasFlag(InsightBulkCopyOptions.TableLock))
				aseOptions |= AseBulkCopyOptions.TableLock;
			if (options.HasFlag(InsightBulkCopyOptions.UseInternalTransaction))
				aseOptions |= AseBulkCopyOptions.UseInternalTransaction;

			using (var bulk = new AseBulkCopy((AseConnection)connection, aseOptions, (AseTransaction)transaction))
			using (var insightBulk = new SybaseAseInsightBulkCopy(bulk))
			{
				bulk.DestinationTableName = tableName;

				// map the columns by name, in case we skipped a readonly column
				foreach (DataRow row in reader.GetSchemaTable().Rows)
					bulk.ColumnMappings.Add(new AseBulkCopyColumnMapping((string)row["ColumnName"], (string)row["ColumnName"]));

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
			AseException aseException = (AseException)exception;

			return aseException.Errors.OfType<AseError>().Any(e => e.MessageNumber == 30012);
		}

		#region Bulk Copy Support
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
		class SybaseAseInsightBulkCopy : InsightBulkCopy, IDisposable
		{
			private AseBulkCopy _bulkCopy;

			public SybaseAseInsightBulkCopy(AseBulkCopy bulkCopy)
			{
				if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

				_bulkCopy = bulkCopy;
				_bulkCopy.AseRowsCopied += OnRowsCopied;
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
				_bulkCopy.AseRowsCopied -= OnRowsCopied;
			}

			private void OnRowsCopied(object sender, AseRowsCopiedEventArgs e)
			{
				var wrappedEvent = new SybaseAseInsightRowsCopiedEventArgs(e);
				if (RowsCopied != null)
					RowsCopied(sender, wrappedEvent);
			}

			class SybaseAseInsightRowsCopiedEventArgs : InsightRowsCopiedEventArgs
			{
				private AseRowsCopiedEventArgs _event;

				public SybaseAseInsightRowsCopiedEventArgs(AseRowsCopiedEventArgs e)
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
					get { return _event.RowCopied; }
				}
			}
		}
		#endregion
	}
}
