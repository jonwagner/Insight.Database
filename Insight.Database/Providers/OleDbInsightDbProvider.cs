using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	class OleDbInsightDbProvider : InsightDbProvider
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
}
