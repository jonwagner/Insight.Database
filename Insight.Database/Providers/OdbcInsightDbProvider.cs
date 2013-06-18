using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	class OdbcInsightDbProvider : InsightDbProvider
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
}
