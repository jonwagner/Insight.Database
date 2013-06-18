using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Insight.Database.Providers
{
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

		public override DbConnection CreateDbConnection()
		{
			return new OracleConnection();
		}

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			var connection = command.Connection;
			bool autoClose = false;

			try
			{
				if (connection.State != ConnectionState.Open)
				{
					autoClose = true;
					connection.Open();
				}

				OracleCommandBuilder.DeriveParameters(command as OracleCommand);
			}
			finally
			{
				if (autoClose)
					connection.Close();
			}

			// make the list of parameters
			List<IDbDataParameter> parameters = command.Parameters.Cast<IDbDataParameter>().ToList();

			// clear the list so we can re-add them
			command.Parameters.Clear();

			return parameters;
		}
	}

}
