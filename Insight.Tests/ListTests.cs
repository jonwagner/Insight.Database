using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Database;
using System.Data;
using System.Dynamic;
using System.Data.SqlClient;

namespace Insight.Tests
{
	[TestFixture]
	public class ListTests : BaseTest
	{
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
				var items = Connection().QuerySql(sql, new { p = array });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
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
				var items = Connection().QuerySql<InsightTestData>("SELECT * FROM @p", new { p = array });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					ClassicAssert.AreEqual(j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<SqlException>(() => Connection().QuerySql<InsightTestData>("SELECT * FROM @p", new { p = new InsightTestData[] { new InsightTestData(), null, new InsightTestData() } }));
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableClassListParameters()
		{
			for (int i = 1; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// run the query
				var items = Connection().QuerySql<InsightTestData>("SELECT * FROM @p", new { p = array.ToList() });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					ClassicAssert.AreEqual(j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<SqlException>(() => Connection().QuerySql<InsightTestData>("SELECT * FROM @p", new { p = new List<InsightTestData>() { new InsightTestData(), null, new InsightTestData() } }));
		}

		class RenamedInsightTestData : InsightTestData
		{
		}

		/// <summary>
		/// Allow the table type of a TVP to be detected in sql text.
		/// </summary>
		[Test]
		public void SqlTextTVPShouldAutoDetectTableType()
		{
			for (int i = 1; i < 3; i++)
			{
				// build test data
				RenamedInsightTestData[] array = new RenamedInsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new RenamedInsightTestData() { Int = j };

				// run the query
				var items = Connection().QuerySql<RenamedInsightTestData>("SELECT * FROM @InsightTestDataTable", new { InsightTestDataTable = array.ToList() });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					ClassicAssert.AreEqual(j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<SqlException>(() => Connection().QuerySql<InsightTestData>("SELECT * FROM @p", new { p = new List<InsightTestData>() { new InsightTestData(), null, new InsightTestData() } }));
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
			var items = Connection().Query<InsightTestData>("InsightTestDataTestProc", array);
			ClassicAssert.IsNotNull(items);
			ClassicAssert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				ClassicAssert.AreEqual(j, items[j].Int);
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
			var items = Connection().Query<int>("Int32TestProc", array);
			ClassicAssert.IsNotNull(items);
			ClassicAssert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				ClassicAssert.AreEqual(j, items[j]);
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
			var items = Connection().Query<int>("Int32TestProc", ids);
			ClassicAssert.IsNotNull(items);
			ClassicAssert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				ClassicAssert.AreEqual(j, items[j]);
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableValueParametersToStoredProc()
		{
			ClassicAssert.AreEqual(3, Connection().Query<InsightTestData>("Int32TestProc", new { p = new int?[] { 0, null, 1 } }).Count);

			for (int i = 0; i < 3; i++)
			{
				// build test data
				int[] array = Enumerable.Range(0, i).ToArray();

				// run the query
				var items = Connection().Query<InsightTestData>("Int32TestProc", new { p = array });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
			}

			// make sure that we CAN send up a null item in the list
			ClassicAssert.AreEqual(3, Connection().Query<InsightTestData>("Int32TestProc", new { p = new int?[] { 0, null, 1 } }).Count);
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

			Connection().Query("EvilProc", new { p = list });
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
				var items = Connection().Query<InsightTestDataString>("InsightTestDataStringTestProc", new { p = array.ToList() });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					ClassicAssert.AreEqual(j.ToString(), items[j].String);
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
				var items = Connection().Query<InsightTestDataString>("InsightTestDataStringTestProc", new { p = array.ToList() });
				ClassicAssert.IsNotNull(items);
				ClassicAssert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					ClassicAssert.AreEqual(j.ToString(), items[j].String);
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
			var items = Connection().QueryAsync<InsightTestData>("InsightTestDataTestProc", array).Result;
			ClassicAssert.IsNotNull(items);
			ClassicAssert.AreEqual(3, items.Count);
			for (int j = 0; j < 3; j++)
				ClassicAssert.AreEqual(j, items[j].Int);
		}

		[Test]
		public void TestSerializingBasedOnWhereSelectIterator()
		{
			var connection = Connection();

			Dictionary<string, string> map = new Dictionary<string,string>();
			map["from"] = "to";

			// because we are going to pass in this select iterator, we are testing that we pull out the proper underlying IEnumerable interface
			// and underlying types (ick)
			var list = map.Select(m => new { m.Key, m.Value });

			var results = connection.Query("TestMap", list);
			dynamic first = results[0];
			ClassicAssert.AreEqual("from", first["Key"]);
			ClassicAssert.AreEqual("to", first["Value"]);
		}

		/// <summary>
		/// There is some special IL for handling structs, so test that here.
		/// </summary>
		[Test]
		public void TestSerializingListOfStructures()
		{
			var connection = Connection();
			var list = new KeyValuePair<string, string>[] { new KeyValuePair<string, string> ("from", "to") };

			var results = connection.Query("TestMap", list);
			dynamic first = results[0];
			ClassicAssert.AreEqual("from", first["Key"]);
			ClassicAssert.AreEqual("to", first["Value"]);
		}
		#endregion

		#region Mismatch Tests
		[Test]
		public void TestTypeMismatchInObjectReader()
		{
			InsightTestData[] array = new InsightTestData[] { new InsightTestData() };

			// run the query
			Connection().QuerySql<InsightTestData>("InsightTestDataTestProc2", new { p = array.ToList() });
		}
		#endregion

		#region Regression Tests
		[Test]
		public void TestIssue36()
		{
			List<Guid> list = new List<Guid>() { Guid.NewGuid() };
			Connection().Query<string>("VarCharProc", list);
		}
		#endregion
	}
}
