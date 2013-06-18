using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers;
using NUnit.Framework;
using Oracle.DataAccess.Client;

namespace Insight.Tests.Oracle
{
	/// <summary>
	/// Oracle-specific tests.
	/// </summary>
	[TestFixture]
	public class OracleTests : BaseDbTest
	{
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			new OracleInsightDbProvider().Register();

			_connectionStringBuilder = new OracleConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = "Data Source = localhost; User Id = system; Password = Password1";
			_connection = _connectionStringBuilder.Open();
		}

		[Test]
		public void TestExecuteSql()
		{
			_connection.QuerySql<decimal>("SELECT 1 as p FROM dual");
		}

		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE OracleTestProc (i int) IS BEGIN null; END;");
				_connection.Execute("OracleTestProc", new { i = 5 });
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE OracleTestProc");
			}
		}

		[Test]
		public void TestDynamicExecute()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE InsightTestProc (Value int) IS BEGIN null; END;");

				_connection.Dynamic().InsightTestProc(value: 5);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE InsightTestProc");
			}
		}
	}
}
