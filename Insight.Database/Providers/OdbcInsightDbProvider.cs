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
	/// <summary>
	/// Implements the Insight provider for Odbc connections.
	/// </summary>
	class OdbcInsightDbProvider : InsightDbProvider
	{
		public override Type CommandType
		{
			get
			{
				return typeof(OdbcCommand);
			}
		}

		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(OdbcConnectionStringBuilder);
			}
		}

		public override DbConnection CreateDbConnection()
		{
			return new OdbcConnection();
		}
	}
}
