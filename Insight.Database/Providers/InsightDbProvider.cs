using Insight.Database.Reliable;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	public abstract class InsightDbProvider
	{
		static InsightDbProvider()
		{
			Providers.Add(new SqlInsightDbProvider());
			Providers.Add(new ReliableInsightDbProvider());
			Providers.Add(new OdbcInsightDbProvider());
			Providers.Add(new OleDbInsightDbProvider());
			Providers.Add(new ProfiledInsightDbProvider());
			Providers.Add(new OracleInsightDbProvider());
		}

		public abstract bool SupportsCommand(IDbCommand command);

		public abstract bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder);

		public virtual DbConnection GetDbConnection()
		{
			throw new NotImplementedException();
		}

		public virtual List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			throw new NotImplementedException();
		}

		public virtual IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			throw new NotImplementedException();
		}

		public virtual IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			throw new NotImplementedException();
		}

		private static List<InsightDbProvider> Providers = new List<InsightDbProvider>();

		internal static InsightDbProvider For(IDbCommand command)
		{
			var provider = Providers.Where(p => p.SupportsCommand(command)).FirstOrDefault();
			if (provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of command.");

			return provider;
		}

		internal static InsightDbProvider For(DbConnectionStringBuilder builder)
		{
			var provider = Providers.Where(p => p.SupportsConnectionStringBuilder(builder)).FirstOrDefault();
			if (provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of connection string builder.");

			return provider;
		}
	}

	public class ReliableInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is ReliableCommand;
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return false;
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			command = reliableCommand.InnerCommand;
			return InsightDbProvider.For(command).DeriveParameters(command);
		}

		public override IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			command = reliableCommand.InnerCommand;

			return InsightDbProvider.For(command).CreateTableValuedParameter(reliableCommand.InnerCommand, parameterName, tableTypeName);
		}

		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			command = reliableCommand.InnerCommand;

			return InsightDbProvider.For(command).GetTableTypeSchema(command, tableTypeName);
		}
	}

	public class ProfiledInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command.GetType().Name == "ProfiledDbCommand";
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return false;
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			dynamic dynamicCommand = command;
			command = dynamicCommand.InternalCommand;
			return InsightDbProvider.For(command).DeriveParameters(command);
		}

		public override IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			dynamic dynamicCommand = command;
			command = dynamicCommand.InternalCommand;
			return InsightDbProvider.For(command).CreateTableValuedParameter(dynamicCommand.InnerCommand, parameterName, tableTypeName);
		}

		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			dynamic dynamicCommand = command;
			command = dynamicCommand.InternalCommand;
			return InsightDbProvider.For(command).GetTableTypeSchema(command, tableTypeName);
		}
	}

	public class SqlInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is SqlCommand;
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return builder is SqlConnectionStringBuilder;
		}

		public override DbConnection GetDbConnection()
		{
			return new SqlConnection();
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
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

		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			// select a 0 row result set so we can determine the schema of the table
			string sql = String.Format(CultureInfo.InvariantCulture, "DECLARE @schema {0} SELECT TOP 0 * FROM @schema", tableTypeName);
			return command.Connection.GetReaderSql(sql, commandBehavior: CommandBehavior.SchemaOnly, transaction: command.Transaction);
		}
	
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}

	public class OdbcInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is OdbcCommand;
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return builder is OdbcConnectionStringBuilder;
		}

		public override DbConnection GetDbConnection()
		{
			return new OdbcConnection();
		}
	}

	public class OleDbInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is OleDbCommand;
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return builder is OleDbConnectionStringBuilder;
		}

		public override DbConnection GetDbConnection()
		{
			return new OleDbConnection();
		}
	}

	public class OracleInsightDbProvider : InsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is OracleCommand;
		}

		public override bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return builder is OracleConnectionStringBuilder;
		}

		public override DbConnection GetDbConnection()
		{
			return new OracleConnection();
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			OracleCommand oracleCommand = command as OracleCommand;

			// call the server to get the parameters
			command.Connection.ExecuteAndAutoClose(
				_ =>
				{
					OracleCommandBuilder.DeriveParameters(oracleCommand);

					return null;
				},
				(_, __) => false,
				CommandBehavior.Default);

			// make the list of parameters
			List<IDbDataParameter> parameters = command.Parameters.Cast<IDbDataParameter>().ToList();

			// clear the list so we can re-add them
			command.Parameters.Clear();

			return parameters;
		}
	}
}
