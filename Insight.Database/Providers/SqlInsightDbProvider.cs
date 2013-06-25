using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(SqlConnectionStringBuilder), typeof(SqlConnection), typeof(SqlCommand), typeof(SqlDataReader), typeof(SqlException) };
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
			CheckForMissingParameters(sqlCommand);

			foreach (var p in command.Parameters.OfType<SqlParameter>())
			{
				// remove the @ from any parameters
				p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty).ToUpperInvariant();

				// trim any prefixes from type names
				string tableTypeName = p.TypeName;
				if (tableTypeName.Count(c => c == '.') > 1)
					tableTypeName = tableTypeName.Split(new char[] { '.' }, 2)[1];
				p.TypeName = tableTypeName;
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
			SqlParameter p = (SqlParameter)base.CloneParameter(command, parameter);

			SqlParameter template = (SqlParameter)parameter;
			p.SqlDbType = template.SqlDbType;
			p.TypeName = template.TypeName;
			p.UdtTypeName = template.UdtTypeName;

			return p;
		}

		/// <summary>
		/// Gets the schema for a given user-defined table type.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to use.</param>
		/// <returns>An open reader with the schema.</returns>
		/// <remarks>The caller is responsible for closing the reader and the connection.</remarks>
		public override IDataReader GetTableTypeSchema(IDbCommand command, IDataParameter parameter)
		{
			if (command == null) throw new ArgumentNullException("command");

			// select a 0 row result set so we can determine the schema of the table
			SqlParameter p = (SqlParameter)parameter;
			string sql = String.Format(CultureInfo.InvariantCulture, "DECLARE @schema {0} SELECT TOP 0 * FROM @schema", p.TypeName);
			return command.Connection.GetReaderSql(sql, commandBehavior: CommandBehavior.SchemaOnly, transaction: command.Transaction);
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
		/// Calculates the table type name for a table parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <param name="listType">The type of object being stored in the table.</param>
		/// <returns>The name of the table parameter.</returns>
		public override string GetTableParameterTypeName(IDbCommand command, IDataParameter parameter, Type listType)
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
		public override void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<object> configure, int? options, IDbTransaction transaction)
		{
			if (options == null)
				options = (int)SqlBulkCopyOptions.Default;

			using (SqlBulkCopy bulk = new SqlBulkCopy((SqlConnection)connection, (SqlBulkCopyOptions)options, (SqlTransaction)transaction))
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
		/// Verify that DeriveParameters returned the correct parameters.
		/// </summary>
		/// <remarks>
		/// If the current user doesn't have execute permissions the type of a parameter,
		/// DeriveParameters won't return the parameter. This is very difficult to debug,
		/// so we are going to check to make sure that we got all of the parameters.
		/// </remarks>
		/// <param name="command">The command to analyze.</param>
		private static void CheckForMissingParameters(SqlCommand command)
		{
			// if the current user doesn't have execute permissions on the database
			// DeriveParameters will just skip the parameter
			// so we are going to check the list ourselves for anything missing
			var parameterNames = command.Connection.QuerySql<string>(
				"SELECT p.Name FROM sys.parameters p WHERE p.object_id = OBJECT_ID(@Name)",
				new { Name = command.CommandText },
				transaction: command.Transaction);

			var parameters = command.Parameters.OfType<SqlParameter>();
			string missingParameter = parameterNames.FirstOrDefault(n => !parameters.Any(p => String.Compare(p.ParameterName, n, StringComparison.OrdinalIgnoreCase) == 0));
			if (missingParameter != null)
				throw new InvalidOperationException(String.Format(
					CultureInfo.InvariantCulture,
					"{0} is missing parameter {1}. Check to see if the parameter is using a type that the current user does not have EXECUTE access to.",
					command.CommandText,
					missingParameter));
		}
	}
}
