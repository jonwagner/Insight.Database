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
	// TODO: remove unwrapsqlcommand
	// TODO: dynamicconnection sqlconnection use (convert to provider)

	public class InsightDbProvider
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

		public virtual DbConnection GetDbConnection(DbConnectionStringBuilder builder)
		{
			return null;
		}

		public virtual List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			return null;
		}

		private static List<InsightDbProvider> Providers = new List<InsightDbProvider>();

		public static T First<T>(Func<InsightDbProvider, T> function) where T : class
		{
			return Providers.Select(p => function(p)).Where(result => result != null).FirstOrDefault();
		}
	}

	public class ReliableInsightDbProvider : InsightDbProvider
	{
		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			if (reliableCommand == null)
				return base.DeriveParameters(command);

			return InsightDbProvider.First(p => p.DeriveParameters(reliableCommand.InnerCommand));
		}
	}

	public class ProfiledInsightDbProvider : InsightDbProvider
	{
		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			// if the command is not a SqlCommand, then maybe it is wrapped by something like MiniProfiler
			if (command.GetType().Name != "ProfiledDbCommand")
				return base.DeriveParameters(command);

			dynamic dynamicCommand = command;
			return InsightDbProvider.First(p => p.DeriveParameters(dynamicCommand.InternalCommand));
		}
	}

	public class SqlInsightDbProvider : InsightDbProvider
	{
		public override DbConnection GetDbConnection(DbConnectionStringBuilder builder)
		{
			if (builder is SqlConnectionStringBuilder)
				return new SqlConnection();

			return base.GetDbConnection(builder);
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			SqlCommand sqlCommand = command as SqlCommand;
			if (sqlCommand == null)
				return base.DeriveParameters(command);

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

		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}

	public class OdbcInsightDbProvider : InsightDbProvider
	{
		public override DbConnection GetDbConnection(DbConnectionStringBuilder builder)
		{
			if (builder is OdbcConnectionStringBuilder)
				return new OdbcConnection();

			return base.GetDbConnection(builder);
		}
	}

	public class OleDbInsightDbProvider : InsightDbProvider
	{
		public override DbConnection GetDbConnection(DbConnectionStringBuilder builder)
		{
			if (builder is OleDbConnectionStringBuilder)
				return new OleDbConnection();

			return base.GetDbConnection(builder);
		}
	}

	public class OracleInsightDbProvider : InsightDbProvider
	{
		public override DbConnection GetDbConnection(DbConnectionStringBuilder builder)
		{
			if (builder is OracleConnectionStringBuilder)
				return new OracleConnection();

			return base.GetDbConnection(builder);
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			OracleCommand oracleCommand = command as OracleCommand;
			if (oracleCommand == null)
				return base.DeriveParameters(command);

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
