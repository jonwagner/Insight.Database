using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(SqlCommand);
			}
		}

		/// <summary>
		/// Gets the type for ConnectionStringBuilders supported by this provider.
		/// </summary>
		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(SqlConnectionStringBuilder);
			}
		}

		/// <summary>
		/// Gets the type for Connections supported by this provider.
		/// </summary>
		public override Type ConnectionType
		{
			get
			{
				return typeof(SqlConnection);
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
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		protected override void DeriveParametersFromStoredProcedure(IDbCommand command)
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
