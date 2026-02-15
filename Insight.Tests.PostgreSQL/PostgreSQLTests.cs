using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Providers.PostgreSQL;
using Insight.Database.Reliable;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
			public string Text;
		}

#if NET6_0_OR_GREATER
		public class DateOnlyData
		{
			public DateOnly Value;
			public DateOnly? NullableValue;
		}
#endif

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

			ClassicAssert.AreEqual(1, result.Count);
			ClassicAssert.AreEqual(5, result[0]);
		}

		[Test]
		public void TestExecuteWithDateTimeParameters()
		{
			var date = DateTime.Parse("1/1/1978").ToUniversalTime();
			var result = _connection.QuerySql<DateTime>("SELECT @p as p", new { p = date });

			ClassicAssert.AreEqual(1, result.Count);
			ClassicAssert.AreEqual(date, result[0]);
		}

#if NET6_0_OR_GREATER
		[Test]
		public void TestDateOnlyConversions()
		{
			var expected = new DateOnly(2024, 3, 15);

			var raw = _connection.ExecuteScalarSql<object>("SELECT DATE '2024-03-15'");
			ClassicAssert.IsTrue(raw is DateOnly || raw is DateTime, "Expected provider date to materialize as DateOnly or DateTime.");

			var scalar = _connection.QuerySql<DateOnly>("SELECT DATE '2024-03-15'").First();
			ClassicAssert.AreEqual(expected, scalar);

			var fromParameter = _connection.QuerySql<DateOnly>("SELECT @p::date", new { p = expected }).First();
			ClassicAssert.AreEqual(expected, fromParameter);

			var mapped = _connection.QuerySql<DateOnlyData>("SELECT DATE '2024-03-15' AS Value, NULL::date AS NullableValue").First();
			ClassicAssert.AreEqual(expected, mapped.Value);
			ClassicAssert.IsNull(mapped.NullableValue);

			var nullable = _connection.QuerySql<DateOnly?>("SELECT NULL::date").First();
			ClassicAssert.IsNull(nullable);
		}
#endif

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

				ClassicAssert.AreEqual(11, output.Z);
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
					ClassicAssert.AreEqual(1, result.Set1.Count);
					ClassicAssert.AreEqual(5, result.Set1[0]);
					ClassicAssert.AreEqual(1, result.Set2.Count);
					ClassicAssert.AreEqual(5, result.Set2[0]);
				}
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch { }
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
					ClassicAssert.AreEqual(1, result.Set1.Count);
					ClassicAssert.AreEqual(5, result.Set1[0]);
					ClassicAssert.AreEqual(1, result.Set2.Count);
					ClassicAssert.AreEqual(5, result.Set2[0]);
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

				ClassicAssert.AreEqual(1, result.Count);
				ClassicAssert.AreEqual(5, result[0]);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestProc (i int)"); } catch { }
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

				ClassicAssert.AreEqual(1, result.Count);
				ClassicAssert.AreEqual(2, result[0].X);
				ClassicAssert.AreEqual(3, result[0].Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgreSQLTestRecordset()"); } catch { }
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
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
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
				ClassicAssert.AreEqual(parentTestData.ID, resultParent.ID);
				ClassicAssert.IsNotNull(resultParent.TestData);
				ClassicAssert.AreEqual(parentTestData.TestData.X, resultParent.TestData.X);
				ClassicAssert.AreEqual(parentTestData.TestData.Z, resultParent.TestData.Z);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION XmlTableProc (id int, testdata xml)"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE PostgreSQLTestXmlType"); } catch { }
			}
		}

		public class PostgresBulkData
		{
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
					ClassicAssert.IsNotNull(items);
					ClassicAssert.AreEqual(i, items.Count);
					for (int j = 0; j < i; j++)
					{
						ClassicAssert.AreEqual(j, items[j].ID);
						ClassicAssert.AreEqual(j, items[j].Dec);
						ClassicAssert.AreEqual(text, items[j].Text);
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

		public interface ISwitchSchemas
		{
			[Sql("SELECT * FROM InsightTestData")]
			int SelectValue();
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
				ClassicAssert.AreEqual(1, _connectionStringBuilder.ConnectionWithSchema("test1").ExecuteScalarSql<int>("SELECT * FROM InsightTestData"));
				ClassicAssert.AreEqual(2, _connectionStringBuilder.ConnectionWithSchema("test2").ExecuteScalarSql<int>("SELECT * FROM InsightTestData"));

				// npgsql 3.x fails this test
#if !NETCOREAPP1_0
				var parallel = _connectionStringBuilder.ConnectionWithSchema("test2").AsParallel<ISwitchSchemas>();
				ClassicAssert.AreEqual(2, parallel.SelectValue());
				ClassicAssert.AreEqual(2, parallel.SelectValue());
#endif
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

			[Column(SerializationMode = SerializationMode.Json)]
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

				ClassicAssert.AreEqual(users.JsonData, result.JsonData);
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
				ClassicAssert.AreEqual(input.X, deserialized.X);
				ClassicAssert.AreEqual(input.Z, deserialized.Z);
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
				ClassicAssert.AreEqual(input.X, result.JsonData.X);
				ClassicAssert.AreEqual(input.Z, result.JsonData.Z);
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
				ClassicAssert.AreEqual(input.X, result.JsonData.X);
				ClassicAssert.AreEqual(input.Z, result.JsonData.Z);
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
					JsonData = (string)JsonObjectSerializer.Serializer.SerializeObject(typeof(TestData), new TestData() { X = 1, Z = 2 })
				};

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData json)");
				_connection.ExecuteSql("INSERT INTO Users (Id, JsonData) VALUES (@Id, @JsonData::JSON)", users);

				var result = _connection.QuerySql<Users>(@"SELECT Users.* FROM Users").First();

				ClassicAssert.AreEqual(users.JsonData, result.JsonData);
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
				ClassicAssert.AreEqual(input.X, result.JsonData.X);
				ClassicAssert.AreEqual(input.Z, result.JsonData.Z);
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
				ClassicAssert.AreEqual(input.X, result.JsonData.X);
				ClassicAssert.AreEqual(input.Z, result.JsonData.Z);
			}
			finally
			{
				Cleanup("DROP TABLE Users");
			}
		}

		[Test]
		public void TestJsonQueryParameter()
		{
			try
			{
				var input = new TestData() { X = 1, Z = 2, Text = "MyText" };
				var users = new UsersWithTestData() { Id = 1, JsonData = input };

				_connection.ExecuteSql(@"CREATE TABLE Users (Id integer NOT NULL, JsonData jsonb)");
				_connection.ExecuteSql("INSERT INTO Users (Id, JsonData) VALUES (@Id, @JsonData)", users);

				var result = _connection.QuerySql<UsersWithTestData>(
												//												"SELECT Users.* FROM Users WHERE JsonData @> '{ \"Text\": \"MyText\" }'",
												"SELECT Users.* FROM Users WHERE JsonData @> ('{ \"Text\": \"' || @Text || '\" }')::jsonb",
												new { Text = "MyText" }).FirstOrDefault();
				ClassicAssert.AreEqual(input.X, result.JsonData.X);
				ClassicAssert.AreEqual(input.Z, result.JsonData.Z);
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

		#region Enum Test
		public enum MyEnum
		{
			One = 10,
			Two = 20
		}

		public class ClassWithEnum
		{
			public MyEnum Value;
			public MyEnum? NullableValue;
			public MyEnum? NullValue;
		}

		[Test]
		public void TestEnumToSmallInt()
		{
			var e = new ClassWithEnum() { Value = MyEnum.One, NullableValue = MyEnum.Two };

			try
			{
				_connection.ExecuteSql("CREATE TYPE EnumTestType AS (Value smallint, NullableValue smallint, NullValue smallint)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION PostgresSQLSmallInt (Value smallint, NullableValue smallint, NullValue smallint) 
					RETURNS SETOF EnumTestType
					AS $$
					BEGIN 
						RETURN QUERY SELECT Value as Value, NullableValue as NullableValue, NullValue as NullValue; 
					END;
					$$ LANGUAGE plpgsql;");

				var result = _connection.Query<ClassWithEnum>("PostgresSQLSmallInt", e);

				ClassicAssert.AreEqual(MyEnum.One, result.First().Value);
				ClassicAssert.AreEqual(MyEnum.Two, result.First().NullableValue);
				ClassicAssert.AreEqual(null, result.First().NullValue);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP FUNCTION PostgresSQLSmallInt (i smallint, NullableValue smallint, NullValue smallint)"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE EnumTestType"); } catch { }
			}
		}
		#endregion

		#region Issue 380
		public class Widget
		{
			public int id;
			public string name;
		}

		public interface IWidgetRepository
		{
			[Sql("get_widget")]
			Widget GetWidget(int _id);

			[Sql("insert_widget")]
			int InsertWidget(Widget widget);
		}

		[Test]
		public void TestReturnSetofTable()
		{
			using (var connection = _connectionStringBuilder.Connection().OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE TABLE widget (id INTEGER PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY NOT NULL, name TEXT)");
				connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION insert_widget(name TEXT) RETURNS INT AS
						$$
						DECLARE
							_id INT;
						BEGIN
							INSERT INTO Widget (Name) VALUES (name) RETURNING id INTO _id;
							RETURN _id;
						END;
						$$ LANGUAGE plpgsql;
					");
				connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION get_widget(_id INT) RETURNS SETOF widget AS
						$$
						BEGIN
							RETURN QUERY SELECT * FROM widget WHERE id = _id;
						END;
						$$ LANGUAGE plpgsql;
					");

				var repo = connection.As<IWidgetRepository>();
				var id = repo.InsertWidget(new Widget { name = "Test" });
				var widget = repo.GetWidget(id);

				ClassicAssert.AreEqual("Test", widget.name);
			}
		}
		#endregion

		#region Issue 381
		public class GuidWidget
		{
			public Guid id;
			public string name;
		}

		public interface IGuidWidgetRepository
		{
			[Sql("get_widget")]
			GuidWidget GetWidget(Guid _id);
		}

		[Test]
		public void TestGuidParameter()
		{
			using (var connection = _connectionStringBuilder.Connection().OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE TABLE widget (id UUID PRIMARY KEY NOT NULL, name TEXT)");
				connection.ExecuteSql(@"
						CREATE OR REPLACE FUNCTION get_widget(_id UUID) RETURNS SETOF widget AS
						$$
						BEGIN
							RETURN QUERY SELECT _id AS id, 'Test' as name;
						END;
						$$ LANGUAGE plpgsql;
					");

				var guid = new Guid();
				var repo = connection.As<IGuidWidgetRepository>();
				var widget = repo.GetWidget(guid);

				ClassicAssert.AreEqual(guid, widget.id);
				ClassicAssert.AreEqual("Test", widget.name);
			}
		}
		#endregion

		#region Issue 514

		public class TestPOCO
		{
			public long Id { get; set; }
			public string Text { get; set; }
		}


		[Test]
		public async Task TestDirectBulkCopy()
		{
			TestPOCO[] testItems = [
					new TestPOCO() { Id = 1, Text = "foo" },
					new TestPOCO() { Id = 2 },
				];

			try
			{
				_connection.ExecuteSql(@"CREATE TABLE TestData (Id integer NOT NULL, Text text)");

				var count = await _connection.BulkCopyAsync<TestPOCO>("TestData", testItems, conf =>
				{
				}, false, InsightBulkCopyOptions.Default);

				ClassicAssert.AreEqual(count, 2);
			}
			catch (Exception)
			{
				// Expected
				ClassicAssert.Fail("Nullref thrown by BulkCopy");
			}
			finally
			{
				Cleanup("DROP TABLE TestData");
			}
		}

		#endregion

		#region Issue 528
		class TestData528
		{
			public int P { get; set; }
			public string Q { get; set; }
		}
		[Test]
		public void TestIssue528()
		{
			using (var connection = new NpgsqlConnectionWithRecordsets(_connectionStringBuilder).OpenWithTransaction())
			{
				try
				{
					connection.ExecuteSql("CREATE TABLE PostgreSQLTestTable (p int, q varchar(256))");
					connection.ExecuteSql("INSERT INTO PostgreSQLTestTable (p, q) VALUES (1, 'a')");
					connection.ExecuteSql("INSERT INTO PostgreSQLTestTable (p, q) VALUES (2, 'b')");

					connection.ExecuteSql(@"
						CREATE FUNCTION PostgreSQLTestOutput (x int, out z int, out cur refcursor)
						AS $$
						BEGIN 
							z := x; 
							cur ='cur';
							open cur for select * from PostgreSQLTestTable;
						END; 
						$$ LANGUAGE plpgsql;");

					var testData = new TestData() { X = 11, Z = 0 };

					var result = connection.Query<TestData528>("PostgreSQLTestOutput", testData);

					ClassicAssert.AreEqual(11, testData.Z);
					ClassicAssert.AreEqual(2, result.Count);
					ClassicAssert.AreEqual(1, result[0].P);
					ClassicAssert.AreEqual("a", result[0].Q);
					ClassicAssert.AreEqual(2, result[1].P);
					ClassicAssert.AreEqual("b", result[1].Q);
				}
				finally
				{
					try { connection.ExecuteSql("DROP FUNCTION PostgreSQLTestOutput (x int, out z int)"); } catch { }
					try { connection.ExecuteSql("DROP TABLE PostgreSQLTestTable"); } catch { }
				}
			}

		}
		#endregion

		#region Issue 531
		[Test]
		public void TestCountRecordset()
		{
			try
			{
				_connection.ExecuteSql("CREATE TABLE CountTestTable (id int)");
				_connection.ExecuteSql("INSERT INTO CountTestTable VALUES (1), (2), (3)");
				_connection.ExecuteSql(@"
					CREATE OR REPLACE FUNCTION GetCount()
					RETURNS TABLE (count bigint)
					AS $$
					BEGIN
						RETURN QUERY SELECT COUNT(*) FROM CountTestTable;
					END;
					$$ LANGUAGE plpgsql;");

				// PostgreSQL COUNT(*) returns bigint, which should fail when trying to cast to int
				// This is intentional to force developers to handle the conversion explicitly
				Assert.Throws<InvalidCastException>(() => _connection.Query<int>("GetCount"));
			}
			finally
			{
				Cleanup("DROP FUNCTION GetCount()");
				Cleanup("DROP TABLE CountTestTable");
			}
		}
		#endregion

		#region Issue 529
		[Test]
		public void TestIssue529()
		{
			try
			{
				_connection.ExecuteSql("CREATE TABLE InventoryItemLocation (InventoryItemLocationId int, IsAvailable boolean, MaximumLevel int)");

				var sql = @" UPDATE InventoryItemLocation SET IsAvailable=@p_IsAvailable, MaximumLevel=@p_MaximumLevel WHERE InventoryItemLocationId=@p_LocationId";
				_connection.ExecuteSql(sql, new { p_IsAvailable = true, p_MaximumLevel = (int?)null, p_LocationId = 1 });
			}
			finally
			{
				Cleanup("DROP TABLE InventoryItemLocation");
			}
		}
		#endregion

		#region Composite Types
		public record InventoryItem
		{
			public string Name { get; set; } = "";
			public int Supplierid { get; set; }
			public decimal Price { get; set; }
		}

		[Test]
		public void TestCompositeTypeAsParameter()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE TYPE inventory_item AS (
						name            text,
						supplierid      integer,
						price           numeric
					);");

				_connection.ExecuteSql(@"
					CREATE FUNCTION test_composite_type(item inventory_item)
					RETURNS SETOF inventory_item
					AS $$
					BEGIN
						RETURN QUERY SELECT item.*;
					END;
					$$ LANGUAGE plpgsql;");

				var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionStringBuilder.ConnectionString);
				dataSourceBuilder.MapComposite<InventoryItem>();
				using (var dataSource = dataSourceBuilder.Build())
				{
					using (var connection = dataSource.OpenConnection())
					{
						var item = new InventoryItem { Name = "Test", Supplierid = 1, Price = 100 };

						var result = connection.Single<InventoryItem>("test_composite_type", new
						{
							item = item
						});

						ClassicAssert.AreEqual(item.Name, result.Name);
						ClassicAssert.AreEqual(item.Supplierid, result.Supplierid);
						ClassicAssert.AreEqual(item.Price, result.Price);
					}
				}
			}
			finally
			{
				Cleanup("DROP FUNCTION test_composite_type(inventory_item)");
				Cleanup("DROP TYPE inventory_item");
			}
		}

		[Test]
		public void TestCompositeTypeAsTVP()
		{
			try
			{
				_connection.ExecuteSql(@"
					CREATE TYPE inventory_item AS (
						name            text,
						supplierid     integer,
						price           numeric
					);");

				_connection.ExecuteSql(@"
					CREATE FUNCTION test_composite_type(items inventory_item[])
					RETURNS SETOF inventory_item
					AS $$
					BEGIN
						RETURN QUERY SELECT * FROM unnest(items);
					END;
					$$ LANGUAGE plpgsql;");

				var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionStringBuilder.ConnectionString);
				dataSourceBuilder.MapComposite<InventoryItem>();
				using (var dataSource = dataSourceBuilder.Build())
				{
					using (var connection = dataSource.OpenConnection())
					{
						var items = new[] {
							new InventoryItem { Name = "Test", Supplierid = 1, Price = 100 },
							new InventoryItem { Name = "Test 2", Supplierid = 2, Price = 200 },
						};

						// call directly with npgsql
						using var cmd = new NpgsqlCommand("SELECT test_composite_type(@input)", connection);
						cmd.Parameters.AddWithValue("input", items);
						var r = cmd.ExecuteScalar();

						// same query with insight
						var result = connection.Query<InventoryItem>("test_composite_type", new
						{
							items = items
						});

						ClassicAssert.AreEqual(2, result.Count);
						ClassicAssert.AreEqual(items[1].Name, result[1].Name);
						ClassicAssert.AreEqual(items[1].Supplierid, result[1].Supplierid);
						ClassicAssert.AreEqual(items[1].Price, result[1].Price);
					}
				}
			}
			finally
			{
				Cleanup("DROP FUNCTION test_composite_type(inventory_item[])");
				Cleanup("DROP TYPE inventory_item");
			}
		}
		#endregion
	}
}
