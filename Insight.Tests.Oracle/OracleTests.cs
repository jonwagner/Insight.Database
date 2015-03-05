using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.Oracle;
using Insight.Database.Reliable;
using Moq;
using NUnit.Framework;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
using System.Data.Common;

namespace Insight.Tests.Oracle
{
	/// <summary>
	/// Oracle-specific tests.
	/// </summary>
	[TestFixture]
	public class OracleTests
	{
		private OracleConnectionStringBuilder _connectionStringBuilder;
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
			_connectionStringBuilder = new OracleConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = "Data Source = (DESCRIPTION=(CONNECT_DATA=(SERVICE_NAME=))(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))); User Id = system; Password = password";
			_connection = _connectionStringBuilder.Open();
		}

		[Test]
		public void TestExecuteSql()
		{
			_connection.ExecuteSql("SELECT 1 as p FROM dual");
		}

		[Test]
		public void TestExecuteWithParameters()
		{
			var result = _connection.QuerySql<decimal>("SELECT :p as p FROM dual", new { p = 5 });

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(5, result[0]);
		}

		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleTestExecute (i int) IS BEGIN null; END;");
				var result = _connection.Execute("OracleTestExecute", new { i = 5 });
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE OracleTestExecute");
			}
		}

		[Test]
		public void TestExecuteProcedureWithOutputParameter()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleTestOutput (x int, z out int) IS BEGIN z := x; END;");
				var output = new TestData() { X = 11, Z = 0 };
				var result = _connection.Execute("OracleTestOutput", output, outputParameters: output);

				Assert.AreEqual(output.X, output.Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE OracleTestOutput");
			}
		}

		[Test]
		public void TestQueryProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleTestProc (i int, r out sys_refcursor) IS BEGIN open r for select i as p from dual; END;");
				var result = _connection.Query<decimal>("OracleTestProc", new { i = 5 });
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
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
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleTestProc (i int, r out sys_refcursor) IS BEGIN open r for select i as p from dual; END;");
				var result = _connection.Dynamic<decimal>().OracleTestProc(i: 5);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE OracleTestProc");
			}
		}

		[Test]
		public void TestQueryRecordset()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleTestRecordset (r out sys_refcursor) IS BEGIN open r for select 2 as x, 3 as z from dual; END;");
				var result = _connection.Query<TestData>("OracleTestRecordset");

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].X);
				Assert.AreEqual(3, result[0].Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE OracleTestRecordset");
			}
		}

		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT 0 AS p FROM dual UNION SELECT 1 AS p FROM dual UNION SELECT 2 AS p FROM dual) WHERE p IN (:p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = _connection.QuerySql(sql, new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}
		}

		[Test]
		public void TestXmlTypes()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE OR REPLACE PROCEDURE OracleXmlTableProc (id int, testdata xmltype, r out sys_refcursor)
					IS
					BEGIN
						OPEN r FOR SELECT id as id, testdata as testdata FROM dual;
					END;");

				var testData = new TestData() { X = 9, Z = 13 };
				var parentTestData = new ParentTestData() { ID = 1, TestData = testData };

				var results = _connection.Execute("OracleXmlTableProc", parentTestData);
/*
				var resultParent = results[0];
				Assert.AreEqual(parentTestData.ID, resultParent.ID);
				Assert.IsNotNull(resultParent.TestData);
				Assert.AreEqual(parentTestData.TestData.X, resultParent.TestData.X);
				Assert.AreEqual(parentTestData.TestData.Z, resultParent.TestData.Z);*/
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE OracleXmlTableProc"); } catch {}
			}
		}

		private void TestAutoDetectCursorsInSql()
		{
			var results = _connection.QueryResultsSql<decimal, decimal>(@"BEGIN
					OPEN :r1 FOR Select 1 as p from dual;
					OPEN :r2 FOR Select 2 as p from dual;
				END;");
			Assert.AreEqual(1, results.Set1.First());
			Assert.AreEqual(2, results.Set2.First());
		}

		[Test]
		public void TestBulkLoad()
		{
			try
			{
				// NOTE: I have not been able to get the xmltype to bulk copy. It throws "unsupported column datatype".
				// Insight will store the value of the object as a string in the bulk copy
				//_connection.ExecuteSql("CREATE TABLE InsightTestData (ID int, Dec Decimal, TestData xmltype)");
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
				try { _connection.ExecuteSql("DROP TABLE InsightTestData"); } catch { }
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
				var builder = new OracleConnectionStringBuilder(_connectionStringBuilder.ConnectionString);
				builder.DataSource = "testserver:9999";
				using (var reliable = new ReliableConnection<OracleConnection>(builder.ConnectionString, retryStrategy))
				{
					reliable.Open();
				}
			}
			catch
			{
			}

			Assert.AreEqual(1, retries);
		}

		#region Table-Valued Parameter Tests
		public class TvpData : OracleCustomType
		{
			[OracleObjectMapping("X")]
			public int? X { get; set; }
			[OracleObjectMapping("Z")]
			public int Z { get; set; }

			public override void FromCustomObject(OracleConnection con, IntPtr pUdt)
			{
				if (X.HasValue)
					OracleUdt.SetValue(con, pUdt, 0, X);
				OracleUdt.SetValue(con, pUdt, 1, Z);
			}
		}

		[OracleCustomTypeMapping("SYSTEM.ORACLEINSIGHTTESTTYPE")]
		public class TvpDataFactory : OracleObjectFactory<TvpData>
		{
		}

		[OracleCustomTypeMapping("SYSTEM.ORACLEINSIGHTTESTTABLE")]
		public class TvpDataArrayFactory : OracleArrayFactory<TvpData>
		{
		}

		[Test]
		public void TestTableValuedParameters()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE TYPE OracleInsightTestType AS OBJECT (x int, z int)");
				_connection.ExecuteSql("CREATE OR REPLACE TYPE OracleInsightTestTable AS TABLE OF OracleInsightTestType");
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE OracleInsightTestTableProc (t OracleInsightTestTable, rs out sys_refcursor) IS BEGIN OPEN rs FOR SELECT * FROM TABLE(t); END;");

				var list = new List<TvpData>();
				list.Add(new TvpData() { X = 1, Z = 2 });
				var results = _connection.Query("OracleInsightTestTableProc", new { t = list });
				dynamic result = results[0];
				Assert.AreEqual(1, result.X);
				Assert.AreEqual(2, result.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE OracleInsightTestTableProc"); } catch {}
				try { _connection.ExecuteSql("DROP TYPE OracleInsightTestTable"); } catch {}
				try { _connection.ExecuteSql("DROP TYPE OracleInsightTestType"); } catch { }
			}
		}
		#endregion

        [Test]
        public void TestBindByName()
        {
            // if the sql uses the same name twice, we need to set bindbyname to true
            var sql = @"select 
                       :p1 p1,
                        :p1 p1,
                        :p2 p2
                        from dual";

            var args = new
            {
                p1 = "one",
                p2 = "two"
            };

            var result = _connection.QuerySql<dynamic>(sql, args);
            result = _connection.QuerySql<dynamic>(sql, args);
        }

		[Test]
		public void TestBindByNameWithWrappedConnection()
		{
			var wrapped = new ReliableConnection((DbConnection)_connection);

			// if the sql uses the same name twice, we need to set bindbyname to true
			var sql = @"select 
                       :p1 p1,
                        :p1 p1,
                        :p2 p2
                        from dual";

			var args = new
			{
				p1 = "one",
				p2 = "two"
			};

			var result = wrapped.QuerySql<dynamic>(sql, args);
			result = wrapped.QuerySql<dynamic>(sql, args);
		}
	}
}
