using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data;
using System.Dynamic;

namespace Insight.Tests
{
	[TestFixture]
	public class ListTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("CREATE TYPE [Int32Table] AS TABLE ([Value] [int])");
			_connection.ExecuteSql("CREATE TYPE [InsightTestDataTable] AS TABLE ([Int] [int] NOT NULL, [IntNull][int])");
			_connection.ExecuteSql("CREATE TYPE [InsightTestDataTable2] AS TABLE ([Int] [decimal] NOT NULL, [IntNull][decimal])");
			_connection.ExecuteSql("CREATE TYPE [InsightTestDataStringTable] AS TABLE ([String] [varchar](128) NOT NULL)");
			_connection.ExecuteSql("CREATE PROCEDURE [Int32TestProc] @p [Int32Table] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql("CREATE PROCEDURE [InsightTestDataTestProc] @p [InsightTestDataTable] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql("CREATE PROCEDURE [InsightTestDataTestProc2] @p [InsightTestDataTable2] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql("CREATE PROCEDURE [InsightTestDataStringTestProc] @p [InsightTestDataStringTable] READONLY AS SELECT * FROM @p");

			_connection.ExecuteSql("CREATE TYPE [EvilTypes] AS TABLE (a [money], b [smallmoney], c [date])");
			_connection.ExecuteSql("CREATE PROCEDURE [EvilProc] @p [EvilTypes] READONLY AS SELECT * FROM @p");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			CleanupObjects();

			base.TearDownFixture();
		}

		private void CleanupObjects()
		{
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'Int32TestProc') DROP PROCEDURE [Int32TestProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataTestProc') DROP PROCEDURE [InsightTestDataTestProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataTestProc2') DROP PROCEDURE [InsightTestDataTestProc2]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataStringTestProc') DROP PROCEDURE [InsightTestDataStringTestProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'EvilProc') DROP PROCEDURE [EvilProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'Int32Table') DROP TYPE [Int32Table]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataTable') DROP TYPE [InsightTestDataTable]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataTable2') DROP TYPE [InsightTestDataTable2]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataStringTable') DROP TYPE [InsightTestDataStringTable]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'EvilTypes') DROP TYPE [EvilTypes]");
		}
		#endregion

		#region Helper Class
		class InsightTestData
		{
			public int Int;
			public int? IntNull { get; set; }
		}

		class InsightTestDataString
		{
			public string String;
		}

		class InsightTestDataIntString
		{
			public int String;
		}
		#endregion

		#region List Tests
		/// <summary>
		/// Test support for enumerable parameters (a parameter is IEnumerable ValueType)
		/// </summary>
		[Test]
		public void TestEnumerableValueParameters()
		{
			string sql = "SELECT p FROM (SELECT p=0 UNION SELECT 1 UNION SELECT 2) as v WHERE p IN (@p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = _connection.QuerySql(sql, new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableClassArrayParameters()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// run the query
				var items = _connection.QuerySql<InsightTestData>("SELECT * FROM @p", new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<InvalidOperationException>(() => _connection.QuerySql<InsightTestData>("SELECT * FROM @p", new { p = new InsightTestData[] { new InsightTestData(), null, new InsightTestData() } }));
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableClassListParameters()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// run the query
				var items = _connection.QuerySql<InsightTestData>("SELECT * FROM @p", new { p = array.ToList() });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<InvalidOperationException>(() => _connection.QuerySql<InsightTestData>("SELECT * FROM @p", new { p = new List<InsightTestData>() { new InsightTestData(), null, new InsightTestData() } }));
		}

		/// <summary>
		/// This method name COULD be longer.
		/// We want: connection.Query("PROC", list, Parameters.Empty) to send list to a single TVP
		/// </summary>
		[Test]
		public void EnumerablePassedDirectlyToProcWithOneTVPParameterShouldMapListToParameter()
		{
			// build test data
			InsightTestData[] array = new InsightTestData[3];
			for (int j = 0; j < 3; j++)
				array[j] = new InsightTestData() { Int = j };

			// run the query
			var items = _connection.Query<InsightTestData>("InsightTestDataTestProc", array);
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				Assert.AreEqual(j, items[j].Int);
		}

		/// <summary>
		/// This method name COULD be longer.
		/// We want: connection.Query("PROC", list, Parameters.Empty) to send list to a single TVP
		/// </summary>
		[Test]
		public void ArrayOfValuesPassedDirectlyToProcWithOneTVPParameterShouldMapListToParameter()
		{
			// build test data
			int[] array = new int[3];
			for (int j = 0; j < 3; j++)
				array[j] = j;

			// run the query
			var items = _connection.Query<int>("Int32TestProc", array);
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				Assert.AreEqual(j, items[j]);
		}

		/// <summary>
		/// This method name COULD be longer.
		/// We want: connection.Query("PROC", list, Parameters.Empty) to send list to a single TVP
		/// </summary>
		[Test]
		public void EnumerableOfValuesPassedDirectlyToProcWithOneTVPParameterShouldMapListToParameter()
		{
			// build test data
			IList<int> ids = new List<int>();
			for (int j = 0; j < 3; j++)
				ids.Add(j);

			// run the query
			var items = _connection.Query<int>("Int32TestProc", ids);
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				Assert.AreEqual(j, items[j]);
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableValueParametersToStoredProc()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				int[] array = Enumerable.Range(0, i).ToArray();

				// run the query
				var items = _connection.Query<InsightTestData>("Int32TestProc", new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}

			// make sure that we CAN send up a null item in the list
			Assert.AreEqual(3, _connection.Query<InsightTestData>("Int32TestProc", new { p = new int?[] { 0, null, 1 } }).Count);
		}

		class Data
		{
			public decimal a;
			public decimal b;
			public DateTime c;
		}

		[Test]
		public void TestThatEvilTypesCanBeSentToServer()
		{
			var o = new Data()
			{
				a = 0.1m,
				b = 0.2m,
				c = DateTime.Now
			};

			var list = new List<Data>() { o };

			_connection.Query("EvilProc", new { p = list });
		}

		[Test]
		public void TestThatStringTypesAreReadProperly()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestDataString[] array = new InsightTestDataString[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestDataString() { String = j.ToString() };

				// run the query
				var items = _connection.Query<InsightTestDataString>("InsightTestDataStringTestProc", new { p = array.ToList() });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j.ToString(), items[j].String);
			}
		}

		[Test]
		public void TestThatIntsAreConvertedToStringsProperly()
		{
			// send up an int and see if it is coerced to a string properly

			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestDataIntString[] array = new InsightTestDataIntString[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestDataIntString() { String = j };

				// run the query
				var items = _connection.Query<InsightTestDataString>("InsightTestDataStringTestProc", new { p = array.ToList() });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j.ToString(), items[j].String);
			}
		}

		/// <summary>
		/// This method name COULD be longer.
		/// We want: connection.Query("PROC", list, Parameters.Empty) to send list to a single TVP
		/// </summary>
		[Test]
		public void AsyncEnumerablePassedDirectlyToProcWithOneTVPParameterShouldMapListToParameter()
		{
			// build test data
			InsightTestData[] array = new InsightTestData[3];
			for (int j = 0; j < 3; j++)
				array[j] = new InsightTestData() { Int = j };

			// run the query
			var items = _connection.QueryAsync<InsightTestData>("InsightTestDataTestProc", array).Result;
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				Assert.AreEqual(j, items[j].Int);
		}

		[Test]
		public void TestSerializingBasedOnWhereSelectIterator()
		{
			Dictionary<string, string> map = new Dictionary<string,string>();
			map["from"] = "to";

			// because we are going to pass in this select iterator, we are testing that we pull out the proper underlying IEnumerable interface
			// and underlying types (ick)
			var list = map.Select(m => new { m.Key, m.Value });

			try
			{
				_connection.ExecuteSql(@"
					CREATE TYPE [StringMapTable] AS TABLE
					(
						[Key] [varchar](1024),
						[Value] [varchar](1024)
					)");
				_connection.ExecuteSql(@"GRANT EXEC ON TYPE::dbo.StringMapTable TO public");
				_connection.ExecuteSql(@"
					CREATE PROC [TestMap]
						@Map [StringMapTable] READONLY
					AS
						SELECT * FROM @Map
					");

				var results = _connection.Query("TestMap", list);
				dynamic first = results[0];
				Assert.AreEqual("from", first.Key);
				Assert.AreEqual("to", first.Value);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROC TestMap"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE StringMapTable"); } catch {}
			}
		}

		/// <summary>
		/// There is some special IL for handling structs, so test that here.
		/// </summary>
		[Test]
		public void TestSerializingListOfStructures()
		{
			var list = new KeyValuePair<string, string>[] { new KeyValuePair<string, string> ("from", "to") };

			try
			{
				_connection.ExecuteSql(@"
					CREATE TYPE [StringMapTable] AS TABLE
					(
						[Key] [varchar](1024),
						[Value] [varchar](1024)
					)");
				_connection.ExecuteSql(@"GRANT EXEC ON TYPE::dbo.StringMapTable TO public");
				_connection.ExecuteSql(@"
					CREATE PROC [TestMap]
						@Map [StringMapTable] READONLY
					AS
						SELECT * FROM @Map
					");

				var results = _connection.Query("TestMap", list);
				dynamic first = results[0];
				Assert.AreEqual("from", first.Key);
				Assert.AreEqual("to", first.Value);
			}
			finally
			{
				try { _connection.ExecuteSql("DROP PROC TestMap"); } catch { }
				try { _connection.ExecuteSql("DROP TYPE StringMapTable"); } catch {}
			}
		}
		#endregion

		#region Mismatch Tests
		[Test]
		public void TestTypeMismatchInObjectReader()
		{
			InsightTestData[] array = new InsightTestData[] { new InsightTestData() };

			// run the query
			_connection.QuerySql<InsightTestData>("InsightTestDataTestProc2", new { p = array.ToList() });
		}
		#endregion

		#region Regression Tests
		[Test]
		public void TestIssue36()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE [VarcharIDTableType] AS TABLE ([ID] [varchar](300))");
				_connection.ExecuteSql("CREATE PROCEDURE [VarCharProc] (@p [VarcharIDTableType] READONLY) AS SELECT * FROM @p");

				List<Guid> list = new List<Guid>() { Guid.NewGuid() };
				_connection.Query<string>("VarCharProc", list);
			}
			finally
			{
				Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'VarCharProc') DROP PROCEDURE [VarCharProc]");
				Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'VarcharIDTableType') DROP TYPE [VarcharIDTableType]");
			}
		}
		#endregion
	}
}
