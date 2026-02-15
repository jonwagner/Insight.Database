using System;
using System.Linq;
using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using MySqlConnector;
using System.Data;

namespace Insight.Tests.MySqlConnector
{
	/// <summary>
	/// MySql-specific tests.
	/// </summary>
	[TestFixture]
	public class MySqlConnectorTests
	{
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
			_connection = new MySqlConnection(String.Format("Server = {0}; Database = test; User Id = root", BaseTest.TestHost ?? "localhost"));
			_connection.Open();
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

			ClassicAssert.AreEqual(1, result.Count);
			ClassicAssert.AreEqual(5, result[0]);
		}

#if NET6_0_OR_GREATER
		[Test]
		public void TestDateOnlyConversions()
		{
			var expected = new DateOnly(2024, 3, 15);

			var raw = _connection.ExecuteScalarSql<object>("SELECT CAST('2024-03-15' AS DATE)");
			ClassicAssert.IsTrue(raw is DateOnly || raw is DateTime, "Expected provider date to materialize as DateOnly or DateTime.");

			var scalar = _connection.QuerySql<DateOnly>("SELECT CAST('2024-03-15' AS DATE)").First();
			ClassicAssert.AreEqual(expected, scalar);

			var fromParameter = _connection.QuerySql<DateOnly>("SELECT CAST(@p AS DATE)", new { p = expected }).First();
			ClassicAssert.AreEqual(expected, fromParameter);

			var mapped = _connection.QuerySql<DateOnlyData>("SELECT CAST('2024-03-15' AS DATE) AS Value, CAST(NULL AS DATE) AS NullableValue").First();
			ClassicAssert.AreEqual(expected, mapped.Value);
			ClassicAssert.IsNull(mapped.NullableValue);

			var nullable = _connection.QuerySql<DateOnly?>("SELECT CAST(NULL AS DATE)").First();
			ClassicAssert.IsNull(nullable);
		}
#endif

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

				ClassicAssert.AreEqual(output.X, output.Z);
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
				ClassicAssert.AreEqual(1, result.Count);
				ClassicAssert.AreEqual(5, result[0]);
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

				ClassicAssert.AreEqual(1, result.Count);
				ClassicAssert.AreEqual(5, result[0]);
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

				ClassicAssert.AreEqual(1, result.Count);
				ClassicAssert.AreEqual(2, result[0].X);
				ClassicAssert.AreEqual(3, result[0].Z);
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
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
			}
		}
	}
}
