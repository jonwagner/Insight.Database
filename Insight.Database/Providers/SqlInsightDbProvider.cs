﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for Sql connections.
	/// </summary>
	class SqlInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// The prefix used on parameter names.
		/// </summary>
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Cache for Table-Valued Parameter schemas.
		/// </summary>
		private static ConcurrentDictionary<Tuple<string, Type>, ObjectReader> _tvpReaders = new ConcurrentDictionary<Tuple<string, Type>, ObjectReader>();

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(SqlConnectionStringBuilder), typeof(SqlConnection), typeof(SqlCommand), typeof(SqlDataReader), typeof(SqlException)
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
		/// Gets the set of bulk copy options supported by this provider.
		/// </summary>
		public override InsightBulkCopyOptions SupportedBulkCopyOptions
		{
			get
			{
				return InsightBulkCopyOptions.KeepIdentity |
					InsightBulkCopyOptions.FireTriggers |
					InsightBulkCopyOptions.CheckConstraints |
					InsightBulkCopyOptions.TableLock |
					InsightBulkCopyOptions.KeepNulls |
					InsightBulkCopyOptions.UseInternalTransaction;
			}
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new SqlConnection();
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			SqlCommand sqlCommand = command as SqlCommand;
			SqlCommandBuilder.DeriveParameters(sqlCommand);
			AdjustSqlParameters(sqlCommand);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<SqlParameter>())
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
			SqlParameter p = (SqlParameter)base.CloneParameter(command, parameter);

			SqlParameter template = (SqlParameter)parameter;
			p.SqlDbType = template.SqlDbType;
			p.TypeName = template.TypeName;
			p.UdtTypeName = template.UdtTypeName;

			return p;
		}

		/// <inheritdoc/>
		public override void FixupParameter(IDbCommand command, IDataParameter parameter, DbType dbType, Type type)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (parameter == null) throw new ArgumentNullException("parameter");

			base.FixupParameter(command, parameter, dbType, type);

			// when calling sql text, we have to fill in the udttypename for some parameters
			if (command.CommandType != CommandType.StoredProcedure && TypeHelper.IsSqlUserDefinedType(type))
			{
				SqlParameter p = (SqlParameter)parameter;
				p.SqlDbType = SqlDbType.Udt;

				switch (type.Name)
				{
					case "SqlGeometry":
						p.UdtTypeName = "sys.geometry";
						break;

					case "SqlGeography":
						p.UdtTypeName = "sys.geography";
						break;

					case "SqlHierarchy":
						p.UdtTypeName = "sys.hierarchyid";
						break;
				}
			}
		}

		/// <summary>
		/// Set up a table-valued parameter to a procedure.
		/// </summary>
		/// <param name="command">The command to operate on.</param>
		/// <param name="parameter">The parameter to set up.</param>
		/// <param name="list">The list of records.</param>
		/// <param name="listType">The type of object in the list.</param>
		public override void SetupTableValuedParameter(IDbCommand command, IDataParameter parameter, System.Collections.IEnumerable list, Type listType)
		{
			// allow the provider to make sure the table parameter is set up properly
			string tableTypeName = GetTableParameterTypeName(parameter, listType);

			// see if we already have a reader for the given type and table type name
			// we can't use the schema cache because we don't have a schema yet
			var key = Tuple.Create<string, Type>(tableTypeName, listType);
			ObjectReader objectReader = _tvpReaders.GetOrAdd(
				key,
				k => command.Connection.ExecuteAndAutoClose(
					_ => null,
					(_, __) =>
					{
						using (var reader = GetTableTypeSchema(command, parameter))
							return ObjectReader.GetObjectReader(command, reader, listType);
					},
					CommandBehavior.Default));

			// create the structured parameter
			parameter.Value = new ObjectListDbDataReader(objectReader, list);
		}

		/// <summary>
		/// Determines if a parameter is a Table-valued parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is a table-valued parameter.</returns>
		public override bool IsTableValuedParameter(IDbCommand command, IDataParameter parameter)
		{
			SqlParameter p = parameter as SqlParameter;
			return p.SqlDbType == SqlDbType.Structured;
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
		/// Determines if the given column in the schema table is an XML column.
		/// </summary>
		/// <param name="command">The command associated with the reader.</param>
		/// <param name="schemaTable">The schema table to analyze.</param>
		/// <param name="index">The index of the column.</param>
		/// <returns>True if the column is an XML column.</returns>
		public override bool IsXmlColumn(IDbCommand command, DataTable schemaTable, int index)
		{
			if (schemaTable == null) throw new ArgumentNullException("schemaTable");

			return ((Type)schemaTable.Rows[index]["ProviderSpecificDataType"]) == typeof(SqlXml);
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
		/// <remarks>Number of rows copied if supported, -1 otherwise.</remarks>
		public override int BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			SqlBulkCopyOptions sqlOptions = SqlBulkCopyOptions.Default;
			if (options.HasFlag(InsightBulkCopyOptions.KeepIdentity))
				sqlOptions |= SqlBulkCopyOptions.KeepIdentity;
			if (options.HasFlag(InsightBulkCopyOptions.FireTriggers))
				sqlOptions |= SqlBulkCopyOptions.FireTriggers;
			if (options.HasFlag(InsightBulkCopyOptions.CheckConstraints))
				sqlOptions |= SqlBulkCopyOptions.CheckConstraints;
			if (options.HasFlag(InsightBulkCopyOptions.TableLock))
				sqlOptions |= SqlBulkCopyOptions.TableLock;
			if (options.HasFlag(InsightBulkCopyOptions.KeepNulls))
				sqlOptions |= SqlBulkCopyOptions.KeepNulls;
			if (options.HasFlag(InsightBulkCopyOptions.UseInternalTransaction))
				sqlOptions |= SqlBulkCopyOptions.UseInternalTransaction;

			using (SqlBulkCopy bulk = new SqlBulkCopy((SqlConnection)connection, sqlOptions, (SqlTransaction)transaction))
			using (var insightBulk = new SqlInsightBulkCopy(bulk))
			{
				bulk.DestinationTableName = tableName;
				if (configure != null)
					configure(insightBulk);
				bulk.WriteToServer(reader);
			    return reader.RecordsAffected;
			}
		}

		/// <summary>
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public override bool IsTransientException(Exception exception)
		{
			// we are only going to try to handle sql server exceptions
			SqlException sqlException = (SqlException)exception;

			switch (sqlException.Number)
			{
				case 40197:		// The service has encountered an error processing your request. Please try again.
				case 40501:		// The service is currently busy. Retry the request after 10 seconds.
				case 10053:		// A transport-level error has occurred when receiving results from the server. An established connection was aborted by the software in your host machine.
				case 10054:		// A transport-level error has occurred when sending the request to the server. (provider: TCP Provider, error: 0 – An existing connection was forcibly closed by the remote host.)
				case 10060:		// A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: TCP Provider, error: 0 – A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.)
				case 40613:		// Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer support, and provide them the session tracing ID of ZZZZZ.
				case 40143:		// The service has encountered an error processing your request. Please try again.
				case 233:		// The client was unable to establish a connection because of an error during connection initialization process before login. Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. (provider: TCP Provider, error: 0 – An existing connection was forcibly closed by the remote host.)
				case 64:		// A connection was successfully established with the server, but then an error occurred during the login process. (provider: TCP Provider, error: 0 – The specified network name is no longer available.)
				case 20:		// The instance of SQL Server you attempted to connect to does not support encryption.
					return true;
			}

			return false;
		}

		/// <summary>
		/// Fixes various issues with deriving parameters from SQL Server.
		/// </summary>
		/// <remarks>
		/// If the current user doesn't have execute permissions the type of a parameter,
		/// DeriveParameters won't return the parameter. This is very difficult to debug,
		/// so we are going to check to make sure that we got all of the parameters.
		/// </remarks>
		/// <param name="command">The command to analyze.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static void AdjustSqlParameters(SqlCommand command)
		{
			var parameters = command.Parameters.OfType<SqlParameter>();

			// if the current user doesn't have execute permissions on the database
			// DeriveParameters will just skip the parameter
			// so we are going to check the list ourselves for anything missing
			var parameterNames = command.Connection.QuerySql(
				@"SELECT ParameterName = p.name, SchemaName = s.name, TypeName = t.name FROM sys.parameters p
					LEFT JOIN sys.types t ON (p.user_type_id = t.user_type_id)
					LEFT JOIN sys.schemas s ON (t.schema_id = s.schema_id)
					WHERE p.object_id = OBJECT_ID(@Name)",
				new { Name = command.CommandText },
				transaction: command.Transaction);

			// make sure that we aren't missing any parameters
			// SQL will skip the parameter in DeriveParameters if the user does not have EXECUTE permissions on the type
			string missingParameter = parameterNames
				.Select(n => (string)n["ParameterName"])
				.FirstOrDefault((string parameterName) => !parameters.Any(p => String.Compare(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase) == 0));
			if (missingParameter != null)
				throw new InvalidOperationException(String.Format(
					CultureInfo.InvariantCulture,
					"{0} is missing parameter {1}. Check to see if the parameter is using a type that the current user does not have EXECUTE access to.",
					command.CommandText,
					missingParameter));

			// DeriveParameters will also mess up table type names that have dots in them, so we escape them ourselves
			// SQL will return them to us unescaped
			foreach (var p in parameters.Where(p => p.SqlDbType == SqlDbType.Structured))
			{
				var typeParameter = parameterNames.FirstOrDefault(n => String.Compare(p.ParameterName, (string)n["ParameterName"], StringComparison.OrdinalIgnoreCase) == 0);
				if (typeParameter != null)
					p.TypeName = String.Format(CultureInfo.InvariantCulture, "[{0}].[{1}]", typeParameter["SchemaName"], typeParameter["TypeName"]);
			}

			// in SQL2008, some UDTs will not have the proper type names, so we set them with good data
			foreach (var p in parameters.Where(p => p.SqlDbType == SqlDbType.Udt))
			{
				var typeParameter = parameterNames.FirstOrDefault(n => String.Compare(p.ParameterName, (string)n["ParameterName"], StringComparison.OrdinalIgnoreCase) == 0);
				if (typeParameter != null)
					p.UdtTypeName = String.Format(CultureInfo.InvariantCulture, "[{0}].[{1}]", typeParameter["SchemaName"], typeParameter["TypeName"]);
			}
		}

		/// <summary>
		/// Gets the schema for a given user-defined table type.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to use.</param>
		/// <returns>An open reader with the schema.</returns>
		/// <remarks>The caller is responsible for closing the reader and the connection.</remarks>
		private static IDataReader GetTableTypeSchema(IDbCommand command, IDataParameter parameter)
		{
			if (command == null) throw new ArgumentNullException("command");

			// select a 0 row result set so we can determine the schema of the table
			SqlParameter p = (SqlParameter)parameter;
			string sql = String.Format(CultureInfo.InvariantCulture, "DECLARE @schema {0} SELECT TOP 0 * FROM @schema", p.TypeName);
			return command.Connection.GetReaderSql(sql, commandBehavior: CommandBehavior.SchemaOnly, transaction: command.Transaction);
		}

		/// <summary>
		/// Calculates the table type name for a table parameter.
		/// </summary>
		/// <param name="parameter">The parameter to test.</param>
		/// <param name="listType">The type of object being stored in the table.</param>
		/// <returns>The name of the table parameter.</returns>
		private static string GetTableParameterTypeName(IDataParameter parameter, Type listType)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");
			if (listType == null) throw new ArgumentNullException("listType");

			SqlParameter p = parameter as SqlParameter;

			if (String.IsNullOrEmpty(p.TypeName))
			{
				p.SqlDbType = SqlDbType.Structured;
				p.TypeName = String.Format(CultureInfo.InstalledUICulture, "[{0}Table]", listType.Name);
			}

			return p.TypeName;
		}

		#region Bulk Copy Support
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
		class SqlInsightBulkCopy : InsightBulkCopy, IDisposable
		{
			private SqlBulkCopy _bulkCopy;

			public SqlInsightBulkCopy(SqlBulkCopy bulkCopy)
			{
				if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

				_bulkCopy = bulkCopy;
				_bulkCopy.SqlRowsCopied += OnRowsCopied;
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
				_bulkCopy.SqlRowsCopied -= OnRowsCopied;
			}

			private void OnRowsCopied(object sender, SqlRowsCopiedEventArgs e)
			{
				var wrappedEvent = new SqlInsightRowsCopiedEventArgs(e);
				if (RowsCopied != null)
					RowsCopied(sender, wrappedEvent);
			}

			class SqlInsightRowsCopiedEventArgs : InsightRowsCopiedEventArgs
			{
				private SqlRowsCopiedEventArgs _event;

				public SqlInsightRowsCopiedEventArgs(SqlRowsCopiedEventArgs e)
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
