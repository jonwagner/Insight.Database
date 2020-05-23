using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if USE_CORE
using IBM.Data.DB2.Core;
#else
using IBM.Data.DB2;
#endif
using Insight.Database;
using Insight.Database.Providers.DB2;
using Insight.Database.Reliable;
using NUnit.Framework;
using System.Data;

namespace Insight.Tests.DB2
{
    [TestFixture]
    public class DB2Tests
    {
        [Test]
        public void EmptyTest()
        {
            // FTW
        }

        // having some trouble getting connections running with the new core package
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
            var connectionString = String.Format("Server={0}:50000;database=testdb;userid=db2inst1;password={1};Pooling=false",
                BaseTest.TestHost ?? "localhost",
                BaseTest.Password ?? "sql");
            _connection = new DB2Connection(connectionString);
            _connection.Open();
        }

        [Test]
        public void TestExecuteSql()
        {
            _connection.ExecuteSql("SELECT * FROM SYSIBM.DUAL");
        }

        [Test]
        public void TestExecuteWithParameters()
        {
            var result = _connection.QuerySql<int>("SELECT CAST(@p AS INT) as p FROM SYSIBM.DUAL", new { p = 5 });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(5, result[0]);
        }

        [Test]
        public void TestQueryObject()
        {
            var result = _connection.QuerySql<TestData>("SELECT 5 as x, 7 as z FROM SYSIBM.DUAL;").First();

            Assert.AreEqual(5, result.X);
            Assert.AreEqual(7, result.Z);
        }

#if !NO_DERIVE_PARAMETERS
		[Test]
		public void TestExecuteProcedure()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE DB2TestExecute (i int) BEGIN END");
				var result = _connection.Execute("DB2TestExecute", new { i = 5 });
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE DB2TestExecute");
			}
		}

		[Test]
		public void TestExecuteProcedureWithOutputParameter()
		{
			try
			{
				_connection.ExecuteSql("CREATE OR REPLACE PROCEDURE DB2TestOutput (x int, out z int) BEGIN SET z = x; END");
				var output = new TestData() { X = 11, Z = 0 };
				var result = _connection.Execute("DB2TestOutput", output, outputParameters: output);

				Assert.AreEqual(output.X, output.Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE DB2TestOutput");
			}
		}

		[Test]
		public void TestQueryProcedure()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE OR REPLACE PROCEDURE DB2TestProc (i int) 
					RESULT SET 1 
					BEGIN 
					DECLARE c CURSOR WITH RETURN FOR SELECT i as p FROM SYSIBM.DUAL; 
					OPEN c;
					END");
				var result = _connection.Query<int>("DB2TestProc", new { i = 5 });
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE DB2TestProc");
			}
		}

		[Test]
		public void TestDynamicExecute()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE OR REPLACE PROCEDURE DB2TestProc (i int) 
					RESULT SET 1 
					BEGIN 
					DECLARE c CURSOR WITH RETURN FOR SELECT i as p FROM SYSIBM.DUAL; 
					OPEN c;
					END");
				var result = _connection.Dynamic<int>().DB2TestProc(i: 5);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(5, result[0]);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE DB2TestProc");
			}
		}

		[Test]
		public void TestQueryRecordset()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE OR REPLACE PROCEDURE DB2TestRecordset
					RESULT SET 1 
					BEGIN 
					DECLARE c CURSOR WITH RETURN FOR SELECT 2 AS x, 3 as z FROM SYSIBM.DUAL; 
					OPEN c;
					END");
				var result = _connection.Query<TestData>("DB2TestRecordset");

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].X);
				Assert.AreEqual(3, result[0].Z);
			}
			finally
			{
				_connection.ExecuteSql("DROP PROCEDURE DB2TestRecordset");
			}
		}

		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT 0 AS p FROM SYSIBM.DUAL UNION SELECT 1 AS p FROM SYSIBM.DUAL UNION SELECT 2 AS p FROM SYSIBM.DUAL) WHERE p IN (@x)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = _connection.QuerySql(sql, new { x = array });
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
					CREATE OR REPLACE PROCEDURE DB2XmlTableProc (id int, testdata xml)
					RESULT SET 1
					BEGIN
						DECLARE c CURSOR WITH RETURN FOR
							SELECT id as id, testdata as testdata FROM SYSIBM.dual;
						OPEN c;
					END");

				var testData = new TestData() { X = 9, Z = 13 };
				var parentTestData = new ParentTestData() { ID = 1, TestData = testData };

				var results = _connection.Query<ParentTestData, TestData>("DB2XmlTableProc", parentTestData);
				var resultParent = results[0];
				Assert.AreEqual(parentTestData.ID, resultParent.ID);
				Assert.IsNotNull(resultParent.TestData);
				Assert.AreEqual(parentTestData.TestData.X, resultParent.TestData.X);
				Assert.AreEqual(parentTestData.TestData.Z, resultParent.TestData.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROCEDURE DB2XmlTableProc"); }
				catch { }
			}
		}
#endif

#if !NO_BULK_COPY
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
#endif
    }
}
