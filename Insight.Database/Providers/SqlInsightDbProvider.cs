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
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new SqlConnection();
		}

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public override IList<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			SqlCommand sqlCommand = command as SqlCommand;

			// call the server to get the parameters
			command.Connection.ExecuteAndAutoClose(
				_ =>
				{
					SqlCommandBuilder.DeriveParameters(sqlCommand);
					CheckForMissingParameters(sqlCommand);

					return null;
				},
				(_, __) => false,
				CommandBehavior.Default);

			// make the list of parameters
			List<IDbDataParameter> parameters = command.Parameters.Cast<IDbDataParameter>().ToList();
			parameters.ForEach(p => p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty).ToUpperInvariant());

			// clear the list so we can re-add them
			command.Parameters.Clear();

			return parameters;
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDbDataParameter CloneParameter(IDbCommand command, IDbDataParameter parameter)
		{
			SqlParameter p = base.CloneParameter(command, parameter) as SqlParameter;

			SqlParameter template = parameter as SqlParameter;
			p.SqlDbType = template.SqlDbType;
			p.TypeName = template.TypeName;
			p.UdtTypeName = template.UdtTypeName;

			return p;
		}

		/// <summary>
		/// Creates a parameter for a table-valued parameter.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="tableTypeName">The name of the table type.</param>
		/// <returns>An initialized parameter for the table.</returns>
		public override IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			SqlCommand sqlCommand = command as SqlCommand;

			// create the structured parameter
			SqlParameter p = new SqlParameter();
			p.SqlDbType = SqlDbType.Structured;
			p.ParameterName = parameterName;
			p.TypeName = tableTypeName;

			return p;
		}

		/// <summary>
		/// Gets the schema for a given user-defined table type.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="tableTypeName">The name of the table type.</param>
		/// <returns>An open reader with the schema.</returns>
		/// <remarks>The caller is responsible for closing the reader and the connection.</remarks>
		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			if (command == null) throw new ArgumentNullException("command");

			// select a 0 row result set so we can determine the schema of the table
			string sql = String.Format(CultureInfo.InvariantCulture, "DECLARE @schema {0} SELECT TOP 0 * FROM @schema", tableTypeName);
			return command.Connection.GetReaderSql(sql, commandBehavior: CommandBehavior.SchemaOnly, transaction: command.Transaction);
		}

		/// <summary>
		/// Determines if a parameter is a Table-valued parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is a table-valued parameter.</returns>
		public override bool IsTableValuedParameter(IDbCommand command, IDbDataParameter parameter)
		{
			SqlParameter p = parameter as SqlParameter;
			return p.SqlDbType == SqlDbType.Structured;
		}

		/// <summary>
		/// Calculates the table type name for a table parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>The name of the table parameter.</returns>
		public override string GetTableParameterTypeName(IDbCommand command, IDbDataParameter parameter)
		{
			SqlParameter p = parameter as SqlParameter;
			return p.TypeName;
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
