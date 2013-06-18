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
	/// <summary>
	/// Implements the Insight provider for OleDb connections.
	/// </summary>
	class OleDbInsightDbProvider : InsightDbProvider
	{
		public override Type CommandType
		{
			get
			{
				return typeof(OleDbCommand);
			}
		}

		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(OleDbConnectionStringBuilder);
			}
		}

		public override DbConnection CreateDbConnection()
		{
			return new OleDbConnection();
		}
	}
}
