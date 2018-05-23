using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.SybaseAse;
using Sybase.Data.AseClient;
using Insight.Database.Reliable;
using System.Data;

namespace Insight.Tests.SybaseAse
{
	/// <summary>
	/// Sybase ASE-specific tests
	/// </summary>
	[TestFixture]
    public class SybaseAseTests
    {
		private AseConnectionStringBuilder _connectionStringBuilder;
		private IDbConnection _connection;

		public class ParentTestData
		{
			public int ID;
			public decimal Dec;
			public TestData TestData;
		}

		public class TestData
		{
			public int X;
			public int Z;
		}

		[OneTimeSetUp]
		public void SetUpFixture()
		{
			_connectionStringBuilder = new AseConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = String.Format("Data Source={0};Port=5000;User ID=test;Password=InsightTest1234;Database=test", BaseTest.TestHost ?? "localhost");
			_connection = new AseConnection(_connectionStringBuilder.ConnectionString);
			_connection.Open();
		}

		[Test]
		public void TestExecuteSql()
		{
			_connection.ExecuteSql("SELECT 1");
		}

		[Test]
		public void TestExecuteWithParameters()
		{
			var result = _connection.QuerySql<int>("SELECT @p as p", new { p = 5 });

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(5, result[0]);
		}

		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE AseTestExecute (@i int) AS SELECT NULL");
				var result = _connection.Execute("AseTestExecute", new { i = 5 });
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE AseTestExecute");
			}
		}


		[Test]
		public void TestExecuteProcedureWithOutputParameter()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE AseTestOutput (@x int, @z int output) AS SET @z = @x");
				var output = new TestData() { X = 11, Z = 0 };
				var result = _connection.Execute("AseTestOutput", output, outputParameters: output);

				Assert.AreEqual(output.X, output.Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE AseTestOutput");
			}
		}

		[Test]
		public void TestQueryProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE AseTestProc (@i int) AS SELECT @i as p");
				var result = _connection.Query<int>("AseTestProc", new { i = 5 });
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE AseTestProc");
			}
		}

		[Test]
		public void TestDynamicExecute()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE AseTestProc (@i int) AS select @i as p");
				var result = _connection.Dynamic<int>().AseTestProc(i: 5);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE AseTestProc");
			}
		}

		[Test]
		public void TestQueryRecordset()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE AseTestRecordset AS select 2 as x, 3 as z");
				var result = _connection.Query<TestData>("AseTestRecordset");

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].X);
				Assert.AreEqual(3, result[0].Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE AseTestRecordset");
			}
		}

		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT 0 AS p UNION SELECT 1 AS p UNION SELECT 2 AS p) AS foo WHERE p IN (@p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = _connection.QuerySql(sql, new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}
		}

		[Test]
		public void TestBulkLoad()
		{
			try
			{
				_connection.ExecuteSql("CREATE TABLE InsightTestData (ID int, Dec Decimal)");

				for (int i = 0; i < 3; i++)
				{
					// build test data
					ParentTestData[] array = new ParentTestData[i];
					for (int j = 0; j < i; j++)
						array[j] = new ParentTestData() { ID = j, Dec = j, TestData = new TestData() };

					// bulk load the data
					_connection.BulkCopy("InsightTestData", array, configure: bulkCopy => bulkCopy.BatchSize = 10);

					// run the query
					var items = _connection.QuerySql<ParentTestData>("SELECT * FROM InsightTestData");
					Assert.IsNotNull(items);
					Assert.AreEqual(i, items.Count);
					for (int j = 0; j < i; j++)
					{
						Assert.AreEqual(j, items[j].ID);
						Assert.AreEqual(j, items[j].Dec);
					}

					_connection.ExecuteSql("DELETE FROM InsightTestData");
				}
			}
			finally
			{
				try { _connection.ExecuteSql("DROP TABLE InsightTestData"); }
				catch { }
			}
		}
	}
}
