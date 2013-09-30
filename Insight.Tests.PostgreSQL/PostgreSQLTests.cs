using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.PostgreSQL;
using Insight.Database.Reliable;
using Moq;
using NUnit.Framework;
using Npgsql;

namespace Insight.Tests.PostgreSQL
{
	/// <summary>
	/// PostgreSQL-specific tests.
	/// </summary>
	[TestFixture]
	public class PostgreSQLTests : BaseDbTest
	{
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
		public override void SetUpFixture()
		{
			PostgreSQLInsightDbProvider.RegisterProvider();

			_connectionStringBuilder = new NpgsqlConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = "Host = testserver; User Id = postgres; Password = Password1";
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
			var result = _connection.QuerySql<int>("SELECT @p as p", new { p = 5 });

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(5, result[0]);
		}

		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (i int)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION PostgreSQLTestExecute (i int) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT i as i; 
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Execute("PostgreSQLTestExecute", new { i = 5 });
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestExecute (i int)"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestType"); } catch { }
			}
		}

		[Test]
		public void TestExecuteProcedureWithOutputParameter()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE FUNCTION PostgreSQLTestOutput (x int, out z int)
					AS $$
					BEGIN 
						z := x; 
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Query("PostgreSQLTestOutput", new TestData() { X = 11, Z = 0 });

				dynamic output = result[0];

				Assert.AreEqual(11, output.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestOutput (x int, out z int)"); } catch { }
			}
		}

		[Test]
		public void TestQueryProcedure()
		{
			try
			{
				// NOTE: when returning cursors, you need to open the query in a transaction (eek)
				using (var connection = _connectionStringBuilder.OpenWithTransaction())
				{
					connection.ExecuteSql("CREATE TABLE PostgreSQLTestTable (p int)");
					connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION PostgreSQLTestProc (i int) 
						RETURNS SETOF refcursor
						AS $$
						DECLARE
							rs refcursor;
						BEGIN 
							INSERT INTO PostgreSQLTestTable VALUES (@i);
							OPEN rs FOR SELECT * FROM PostgreSQLTestTable;
							RETURN NEXT rs;
						END;
						$$ LANGUAGE plpgsql;");
					var result = connection.Query<int>("PostgreSQLTestProc", new { i = 5 });
					Assert.AreEqual(1, result.Count);
					Assert.AreEqual(5, result[0]);
				}
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch {}
				try { _connection.ExecuteSql("DROP TABLE PostgreSQLTestTable"); } catch { }
			}
		}

		[Test]
		public void TestDynamicExecute()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (p int)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION PostgreSQLTestProc (i int) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT i as p; 
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Dynamic<int>().PostgreSQLTestProc(i: 5);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch {}
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestType"); } catch { }
			}
		}

		[Test]
		public void TestQueryRecordset()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (x int, z int)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION PostgreSQLTestRecordset()
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY select 2 as x, 3 as z;
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Query<TestData>("PostgreSQLTestRecordset");

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].X);
				Assert.AreEqual(3, result[0].Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestRecordset()"); } catch {}
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestType"); } catch { }
			}
		}

		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT 0 AS p UNION SELECT 1 AS p UNION SELECT 2 AS p) derived WHERE p IN (@p)";

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
			retryStrategy.Retrying += (sender, re) => { Console.WriteLine("Retrying. Attempt {0}", re.Attempt); retries++; };

			try
			{
				var builder = new NpgsqlConnectionStringBuilder(_connectionStringBuilder.ConnectionString);
				builder.Host = "testserver";
				builder.Port = 9999;
				using (var reliable = new ReliableConnection<NpgsqlConnection>(builder.ConnectionString, retryStrategy))
				{
					reliable.Open();
				}
			}
			catch
			{
			}

			Assert.AreEqual(1, retries);
		}

		[Test]
		public void TestXmlTypes()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestXmlType AS (id int, testdata xml)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION XmlTableProc (id int, testdata xml)
					RETURNS SETOF PostgreSQLTestXmlType
					AS $$
					BEGIN
						RETURN QUERY SELECT id as id, testdata as testdata;
					END;
					$$ LANGUAGE plpgsql;");

				var testData = new TestData() { X = 9, Z = 13 };
				var parentTestData = new ParentTestData() { ID = 1, TestData = testData };

				var results = _connection.Query<ParentTestData, TestData>("XmlTableProc", parentTestData);
				var resultParent = results[0];
				Assert.AreEqual(parentTestData.ID, resultParent.ID);
				Assert.IsNotNull(resultParent.TestData);
				Assert.AreEqual(parentTestData.TestData.X, resultParent.TestData.X);
				Assert.AreEqual(parentTestData.TestData.Z, resultParent.TestData.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION XmlTableProc (id int, testdata xml)"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestXmlType"); } catch { }
			}
		}

		[Test]
		public void TestBulkLoad()
		{
			try
			{
				_connection.ExecuteSql("CREATE TABLE InsightTestData (ID int, Dec Decimal, TestData xml)");

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
