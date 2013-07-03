using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using MySql.Data.MySqlClient;
using Npgsql;
using ODP = Oracle.DataAccess.Client;
using ODPManaged = Oracle.DataAccess.Client;

namespace Insight.Database.Providers.Sample
{
	class Program
	{
		private static DbConnectionStringBuilder[] _connections = new DbConnectionStringBuilder[]
		{
			new MySqlConnectionStringBuilder("Server = localhost; Database = test; User Id = root; Password = Password1"),
			new NpgsqlConnectionStringBuilder("Host = localhost; User Id = postgres; Password = Password1"),
			new ODP.OracleConnectionStringBuilder("Data Source = (DESCRIPTION=(CONNECT_DATA=(SERVICE_NAME=))(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))); User Id = system; Password = Password1"),
			new ODPManaged.OracleConnectionStringBuilder("Data Source = (DESCRIPTION=(CONNECT_DATA=(SERVICE_NAME=))(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))); User Id = system; Password = Password1"),
		};

		static void Main(string[] args)
		{
			// register the providers
			Insight.Database.Providers.MySql.MySqlInsightDbProvider.RegisterProvider();
			Insight.Database.Providers.Oracle.OracleInsightDbProvider.RegisterProvider();
			Insight.Database.Providers.OracleManaged.OracleInsightDbProvider.RegisterProvider();
			Insight.Database.Providers.PostgreSQL.PostgreSQLInsightDbProvider.RegisterProvider();

			foreach (var connection in _connections.Select(c => c.Connection()))
			{
				if (connection is ODP.OracleConnection || connection is ODPManaged.OracleConnection)
					connection.QuerySql("SELECT 1 as p FROM dual");
				else
					connection.QuerySql("SELECT 1 as p");
			}
		}
	}
}
