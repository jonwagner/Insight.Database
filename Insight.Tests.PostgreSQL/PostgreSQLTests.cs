using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.PostgreSQL;
using Insight.Database.Reliable;
using NUnit.Framework;
using Npgsql;
using System.Data;

namespace Insight.Tests.PostgreSQL
{
	/// <summary>
	/// PostgreSQL-specific tests.
	/// </summary>
	[TestFixture]
	public class PostgreSQLTests
	{
		private NpgsqlConnectionStringBuilder _connectionStringBuilder;
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
			_connectionStringBuilder = new NpgsqlConnectionStringBuilder();
			_connectionStringBuilder.ConnectionString = String.Format("Host={0}; User Id = postgres; Password={1}",
				BaseTest.TestHost ?? "localhost",
				BaseTest.Password ?? "Password1"
				);
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
		public void TestExecuteWithDateTimeParameters()
		{
			var date = DateTime.Parse("1/1/1978");
			var result = _connection.QuerySql<DateTime>("SELECT @p as p", new { p = date });

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(date, result[0]);
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
		public void TestExecuteProcedureWithDateParameter()
		{
			try
			{
				var date = DateTime.Now;
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestDateType AS (i date)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION PostgreSQLTestDateExecute (i date) 
					RETURNS SETOF PostgreSQLTestDateType
					AS $$
					BEGIN 
						RETURN QUERY SELECT i as i; 
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Execute("PostgreSQLTestDateExecute", new { i = date });
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestDateExecute (i date)"); }
				catch { }
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestDateType"); }
				catch { }
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
		public void TestMultipleRecordsets()
		{
			try
			{
				// NOTE: when returning cursors, you need to open the query in a transaction (eek)
				using (var connection = new NpgsqlConnectionWithRecordsets(_connectionStringBuilder).OpenWithTransaction())
				{
					connection.ExecuteSql("CREATE TABLE PostgreSQLTestTable (p int)");
					connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION PostgreSQLTestProc (i int) 
						RETURNS SETOF refcursor
						AS $$
						DECLARE
							rs refcursor;
							rs2 refcursor;
						BEGIN 
							INSERT INTO PostgreSQLTestTable VALUES (@i);
							OPEN rs FOR SELECT * FROM PostgreSQLTestTable;
							RETURN NEXT rs;
							OPEN rs2 FOR SELECT * FROM PostgreSQLTestTable;
							RETURN NEXT rs2;
						END;
						$$ LANGUAGE plpgsql;");
					var result = connection.QueryResults<int, int>(@"PostgreSQLTestProc", new { i = 5 });
					Assert.AreEqual(1, result.Set1.Count);
					Assert.AreEqual(5, result.Set1[0]);
					Assert.AreEqual(1, result.Set2.Count);
					Assert.AreEqual(5, result.Set2[0]);
				}
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch {}
				try { _connection.ExecuteSql("DROP TABLE PostgreSQLTestTable"); } catch { }
			}
		}

		[Test]
		public void TestMultipleRecordsetsAsync()
		{
			try
			{
				// NOTE: when returning cursors, you need to open the query in a transaction (eek)
				using (var connection = new NpgsqlConnectionWithRecordsets(_connectionStringBuilder).OpenWithTransaction())
				{
					connection.ExecuteSql("CREATE TABLE PostgreSQLTestTable (p int)");
					connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION PostgreSQLTestProc (i int) 
						RETURNS SETOF refcursor
						AS $$
						DECLARE
							rs refcursor;
							rs2 refcursor;
						BEGIN 
							INSERT INTO PostgreSQLTestTable VALUES (@i);
							OPEN rs FOR SELECT * FROM PostgreSQLTestTable;
							RETURN NEXT rs;
							OPEN rs2 FOR SELECT * FROM PostgreSQLTestTable;
							RETURN NEXT rs2;
						END;
						$$ LANGUAGE plpgsql;");
					var result = connection.QueryResultsAsync<int, int>(@"PostgreSQLTestProc", new { i = 5 }).Result;
					Assert.AreEqual(1, result.Set1.Count);
					Assert.AreEqual(5, result.Set1[0]);
					Assert.AreEqual(1, result.Set2.Count);
					Assert.AreEqual(5, result.Set2[0]);
				}
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch { }
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

		public class PostgresBulkData {
			public int ID;
			public Decimal Dec;
			public TestData TestData;
			public String Text;
		}

		[Test]
		public void TestBulkLoad()
		{
			try
			{
				_connection.ExecuteSql("CREATE TABLE InsightTestData (ID int, Dec Decimal, TestData xml, Text varchar(256))");

				for (int i = 0; i < 3; i++)
				{
					var text = "test with spaces and \" quotes in it";
					// build test data
					PostgresBulkData[] array = new PostgresBulkData[i];
					for (int j = 0; j < i; j++)
						array[j] = new PostgresBulkData() { ID = j, Dec = j, TestData = new TestData(), Text = text };

					// bulk load the data
					_connection.BulkCopy("InsightTestData", array, configure: bulkCopy => bulkCopy.BatchSize = 10);

					// run the query
					var items = _connection.QuerySql<PostgresBulkData>("SELECT * FROM InsightTestData");
					Assert.IsNotNull(items);
					Assert.AreEqual(i, items.Count);
					for (int j = 0; j < i; j++)
					{
						Assert.AreEqual(j, items[j].ID);
						Assert.AreEqual(j, items[j].Dec);
						Assert.AreEqual(text, items[j].Text);
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

        [Test]
        public void TestSchemaSwitching()
        {
            try
            {
                _connection.ExecuteSql("CREATE SCHEMA test1");
                _connection.ExecuteSql("CREATE TABLE test1.InsightTestData (ID int)");
                _connection.ExecuteSql("CREATE SCHEMA test2");
                _connection.ExecuteSql("CREATE TABLE test2.InsightTestData (ID int)");

                _connectionStringBuilder.ConnectionWithSchema("test1").ExecuteSql("INSERT INTO InsightTestData VALUES (1)");
                _connectionStringBuilder.ConnectionWithSchema("test2").ExecuteSql("INSERT INTO InsightTestData VALUES (2)");
                Assert.AreEqual(1, _connectionStringBuilder.ConnectionWithSchema("test1").ExecuteScalarSql<int>("SELECT * FROM InsightTestData"));
                Assert.AreEqual(2, _connectionStringBuilder.ConnectionWithSchema("test2").ExecuteScalarSql<int>("SELECT * FROM InsightTestData"));
            }
            finally
            {
                Cleanup("DROP TABLE test2.InsightTestData");
                Cleanup("DROP TABLE test1.InsightTestData");
                Cleanup("DROP SCHEMA test2");
                Cleanup("DROP SCHEMA test1");
            }
        }

		public class Users
		{
			public long Id { get; set; }
			public string JsonData { get; set; }
		}

		public class UsersWithTestData
		{
			public long Id { get; set; }

			[Column(SerializationMode=SerializationMode.Json)]
			public TestData JsonData { get; set; }
		}

		[Test]
		public void TestStringToJsonProcParameters()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new Users() { Id = 1, JsonData = (string)JsonObjectSerializer.Serializer.SerializeObject(typeof(TestData), input) };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData json)");
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (Id integer, JsonData json)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION TestStringToJsonProcParameters (Id integer, JsonData json) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT id as id, JsonData as JsonData;
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Query<Users>("TestStringToJsonProcParameters", users).First();

				Assert.AreEqual(users.JsonData, result.JsonData);
			}
			finally
			{
				Cleanup("DROP FUNCTION TestStringToJsonProcParameters (Id integer, JsonData json) ");
				Cleanup("DROP TYPE PostgreSQLTestType");
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestStringToJsonbProcParameters()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new Users() { Id = 1, JsonData = (string)JsonObjectSerializer.Serializer.SerializeObject(typeof(TestData), input) };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData jsonb)");
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (Id integer, JsonData jsonb)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION TestStringToJsonbProcParameters (Id integer, JsonData jsonb) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT id as id, JsonData as JsonData;
					END;
					$$ LANGUAGE plpgsql;");
				var result = _connection.Query<Users>("TestStringToJsonbProcParameters", users).First();

				var deserialized = (TestData)JsonObjectSerializer.Serializer.DeserializeObject(typeof(TestData), result.JsonData);
				Assert.AreEqual(input.X, deserialized.X);
				Assert.AreEqual(input.Z, deserialized.Z);
			}
			finally
			{
				Cleanup("DROP FUNCTION TestStringToJsonbProcParameters (Id integer, JsonData jsonb) ");
				Cleanup("DROP TYPE PostgreSQLTestType");
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestObjectToJsonProcParameters()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new UsersWithTestData() { Id = 1, JsonData = input };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData json)");
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (Id integer, JsonData json)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION TestObjectToJsonProcParameters (Id integer, JsonData json) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT id as id, JsonData as JsonData;
					END;
					$$ LANGUAGE plpgsql;");

				var result = _connection.Query<UsersWithTestData>("TestObjectToJsonProcParameters", users).First();
				Assert.AreEqual(input.X, result.JsonData.X);
				Assert.AreEqual(input.Z, result.JsonData.Z);
			}
			finally
			{
				Cleanup("DROP FUNCTION TestObjectToJsonProcParameters (Id integer, JsonData json) ");
				Cleanup("DROP TYPE PostgreSQLTestType");
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestObjectToJsonbProcParameters()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new UsersWithTestData() { Id = 1, JsonData = input };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData jsonb)");
				_connection.ExecuteSql("CREATE TYPE PostgreSQLTestType AS (Id integer, JsonData jsonb)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION TestObjectToJsonbProcParameters (Id integer, JsonData jsonb) 
					RETURNS SETOF PostgreSQLTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT id as id, JsonData as JsonData;
					END;
					$$ LANGUAGE plpgsql;");

				var result = _connection.Query<UsersWithTestData>("TestObjectToJsonbProcParameters", users).First();
				Assert.AreEqual(input.X, result.JsonData.X);
				Assert.AreEqual(input.Z, result.JsonData.Z);
			}
			finally
			{
				Cleanup("DROP FUNCTION TestObjectToJsonbProcParameters (Id integer, JsonData jsonb) ");
				Cleanup("DROP TYPE PostgreSQLTestType");
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestStringToJsonSqlParameter()
		{
			try
			{
				var users = new Users()
				{
					Id = 1,
					JsonData = (string)JsonObjectSerializer.Serializer.SerializeObject(typeof (TestData), new TestData() { X = 1, Z = 2 })
				};

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData json)");
				_connection.ExecuteSql("INSERT INTO Users (Id, JsonData) VALUES (@Id, @JsonData::JSON)", users);

				var result = _connection.QuerySql<Users>(@"SELECT Users.* FROM Users").First();

				Assert.AreEqual(users.JsonData, result.JsonData);
			}
			finally
			{
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestObjectToJsonSqlParameter()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new UsersWithTestData() { Id = 1, JsonData = input };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData json)");
				_connection.ExecuteSql("INSERT INTO Users (Id, JsonData) VALUES (@Id, @JsonData)", users);

				var result = _connection.QuerySql<UsersWithTestData>(@"SELECT Users.* FROM Users").First();
				Assert.AreEqual(input.X, result.JsonData.X);
				Assert.AreEqual(input.Z, result.JsonData.Z);
			}
			finally
			{
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestObjectToJsonbSqlParameter()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2 };
				var users = new UsersWithTestData() { Id = 1, JsonData = input };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData jsonb)");
				_connection.ExecuteSql("INSERT INTO Users (Id, JsonData) VALUES (@Id, @JsonData)", users);

				var result = _connection.QuerySql<UsersWithTestData>(@"SELECT Users.* FROM Users").First();
				Assert.AreEqual(input.X, result.JsonData.X);
				Assert.AreEqual(input.Z, result.JsonData.Z);
			}
			finally
			{
				Cleanup("DROP TABLE Users");
			}
		}
       
        [Test]
        public void InvalidSchemaShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => _connectionStringBuilder.ConnectionWithSchema("; DROP TABLE InsightTestData"));
        }

        private void Cleanup(string sql)
        {
            try
            {
                _connection.ExecuteSql(sql);
            }
            catch { }
		}

		#region Issue 210
		public class JsonColumn
		{
			[Column("id")]
			public int Id { get; set; }

			[Column("list", SerializationMode = SerializationMode.Json)]
			public IEnumerable<int> List { get; set; }
		}

		[Test]
        public void TestIssue210() 
        {
			using (var connection = _connectionStringBuilder.Connection().OpenWithTransaction())
			{
				connection.ExecuteSql("DROP TABLE IF EXISTS foo");
				connection.ExecuteSql("DROP FUNCTION IF EXISTS insert_foo(integer, jsonb)");
				connection.ExecuteSql("CREATE TABLE foo (id integer, list jsonb)");
				connection.ExecuteSql(
					@"CREATE FUNCTION insert_foo(id integer, list jsonb) RETURNS void
						AS $body$ INSERT INTO foo VALUES (id, list) $body$ LANGUAGE SQL");
				connection.Execute("insert_foo", new JsonColumn { Id = 1, List = new[] { 1, 2, 3, 4, 5 } });
			}
		}
		#endregion

		#region Issue 207
		[Test]
		public void TestIssue207()
		{
			using (var connection = _connectionStringBuilder.Connection().OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE TABLE foo (id int, isnew bool)");
				connection.ExecuteSql("INSERT INTO foo VALUES (1, true)");
				connection.ExecuteSql("SELECT * FROM foo WHERE isnew = @isnew", new { isnew = true });
			}
		}
		#endregion

		#region Issue 342
		public class User
		{
			public string first_name { get; set; }    
			public string last_name { get; set; }
			public string email { get; set; }
		}

		[Test]
		public void TestIssue342()
		{
			var data = new List<User>()
			{
				new User() { email = "john@dough.com", first_name = "John", last_name = "Dough"},
				new User() { email = "jane@dough.com", first_name = "Jane", last_name = "Dough"}
			};

			try
			{
				_connection.ExecuteSql("CREATE TABLE InsightTestData (email varchar(256), first_name varchar(256), last_name varchar(256))");
				_connection.BulkCopy("InsightTestData", data);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP TABLE InsightTestData"); }
				catch { }
			}				
		}
		#endregion
	}
}
