using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.MySql;
using Insight.Database.Reliable;
using Moq;
using NUnit.Framework;
using MySql.Data.MySqlClient;
using System.Data;

namespace Insight.Tests.MySql
{
	/// <summary>
	/// MySql-specific tests.
	/// </summary>
	[TestFixture]
	public class MySqlTests
	{
		private MySqlConnectionStringBuilder _connectionStringBuilder;
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

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			_connectionStringBuilder = new MySqlConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = "Server = localhost; Database = test; User Id = root";
			_connection = _connectionStringBuilder.Open();
		}

		[Test]
		public void TestExecuteSql()
		{
			_connection.ExecuteSql("SELECT 1 as p");
		}

		[Test]
		public void TestExecuteWithParameters()
		{
			var result = _connection.QuerySql<long>("SELECT @p as p", new { p = 5 });

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(5, result[0]);
		}

		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE PROCEDURE MySqlTestExecute (i int) 
					BEGIN 
						SELECT i as i; 
					END");
				var result = _connection.Execute("MySqlTestExecute", new { i = 5 });
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE MySqlTestExecute"); } catch {}
			}
		}

		[Test]
		public void TestExecuteProcedureWithOutputParameter()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE PROCEDURE MySqlTestOutput (x int, out z int)
					BEGIN 
						SET z = x; 
					END");
				var output = new TestData() { X = 11, Z = 0 };
				var result = _connection.Execute("MySqlTestOutput", output, outputParameters: output);

				Assert.AreEqual(output.X, output.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE MySqlTestOutput"); } catch {}
			}
		}

		[Test]
		public void TestQueryProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE MySqlTestProc (i int) BEGIN select i as p; END");
				var result = _connection.Query<int>("MySqlTestProc", new { i = 5 });
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE MySqlTestProc"); } catch {}
			}
		}

		[Test]
		public void TestDynamicExecute()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE MySqlTestProc (i int) BEGIN select i as p; END");
				var result = _connection.Dynamic<int>().MySqlTestProc(i: 5);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE MySqlTestProc");
			}
		}

		[Test]
		public void TestQueryRecordset()
		{
			try
			{
				_connection.ExecuteSql("CREATE PROCEDURE MySqlTestRecordset() BEGIN select 2 as x, 3 as z; END");
				var result = _connection.Query<TestData>("MySqlTestRecordset");

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].X);
				Assert.AreEqual(3, result[0].Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE MySqlTestRecordset");
			}
		}

		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT 0 AS p FROM dual UNION SELECT 1 AS p UNION SELECT 2 AS p) derived WHERE p IN (@p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = _connection.QuerySql(sql, new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}
		}

		[Test, Ignore]
		public void TestReliableConnection()
		{
			int retries = 0;
			var retryStrategy = new RetryStrategy();
			retryStrategy.MaxRetryCount = 1;
			retryStrategy.Retrying += (sender, re) => { retries++; };

			try
			{
				var builder = new MySqlConnectionStringBuilder(_connectionStringBuilder.ConnectionString);
				builder.Server = "testserver";
				builder.Port = 9999;
				using (var reliable = new ReliableConnection<MySqlConnection>(builder.ConnectionString, retryStrategy))
				{
					reliable.Open();
				}
			}
			catch
			{
			}

			Assert.AreEqual(1, retries);
		}
	}
}
