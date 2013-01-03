using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class SubObjectTests : BaseDbTest
	{
		class TestData
		{
			public int ID;
			public int SubDataID;
			public TestSubData SubData;
			public int SubDataID2;
			public TestSubData2 SubData2;
			public TestOtherData OtherData;
			public int Foo;
		}

		class TestSubData
		{
			public int ID;
			public int SubInt;
			public TestSubSubData SubSubData;
		}

		class TestSubData2
		{
			public int ID;
			public int SubInt;
		}

		class TestSubSubData
		{
			public int ID;
			public int SubInt;
		}

		class TestOtherData
		{
			public int OtherID;
			public TestSubData SubData;
			public int Bar;
		}

		class TestProperties
		{
			public TestSubData SubData { get; set; }
		}

		[Test]
		public void SelectSubObject()
		{
			var results = _connection.QuerySql<TestData, TestSubData>(@"
				CREATE TABLE #TestData ([ID] [int], [SubDataID] [int])
				CREATE TABLE #TestSubData ([ID] [int], [SubInt] [int])
				INSERT INTO #TestData VALUES (1, 2)
				INSERT INTO #TestSubData VALUES (2, 3)
				SELECT * 
					FROM #TestData t 
					LEFT JOIN #TestSubData s ON (t.SubDataID = s.ID)
					ORDER BY t.ID
			");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.AreEqual(3, testData.SubData.SubInt);
		}

		[Test]
		public void SelectWithEmptyLeftJoinShouldContainNullObject()
		{
			var results = _connection.QuerySql<TestData, TestSubData>(@"
				CREATE TABLE #TestData ([ID] [int], [SubDataID] [int])
				CREATE TABLE #TestSubData ([ID] [int], [SubInt] [int])
				INSERT INTO #TestData VALUES (1, 2)
				SELECT * 
					FROM #TestData t 
					LEFT JOIN #TestSubData s ON (t.SubDataID = s.ID)
					ORDER BY t.ID
			");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubDataID);

			Assert.IsNull(testData.SubData, "Sub object should be null in left join");
		}

		[Test]
		public void SelectWithNoColumnsForSubObjectShouldContainNullObject()
		{
			var results = _connection.QuerySql<TestData, TestSubData>(@"SELECT ID=1");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should be set");
			Assert.IsNull(testData.SubData, "Sub object should be null in left join");
		}

		[Test]
		public void SelectTwoSubObjects()
		{
			var results = _connection.QuerySql<TestData, TestSubData, TestSubData2>(@"
				CREATE TABLE #TestData ([ID] [int], [SubDataID] [int], [SubDataID2] [int])
				CREATE TABLE #TestSubData ([ID] [int], [SubInt] [int])
				INSERT INTO #TestData VALUES (1, 2, 3)
				INSERT INTO #TestSubData VALUES (2, 3)
				INSERT INTO #TestSubData VALUES (3, 4)
				SELECT * 
					FROM #TestData t 
					LEFT JOIN #TestSubData s ON (t.SubDataID = s.ID)
					LEFT JOIN #TestSubData s2 ON (t.SubDataID2 = s2.ID)
					ORDER BY t.ID
			");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.AreEqual(3, testData.SubData.SubInt);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.SubData2);
			Assert.AreEqual(3, testData.SubData2.ID);
			Assert.AreEqual(4, testData.SubData2.SubInt);
		}

		[Test]
		public void SelectTwoSubObjectsWithMissingMiddleObjectShouldHaveNullMiddleObject()
		{
			var results = _connection.QuerySql<TestData, TestSubData, TestOtherData>(@"
				SELECT ID=1, Foo=3, OtherID=2, Bar=4
			");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should be set");
			Assert.AreEqual(3, testData.Foo, "Foo should be set");

			// test that we got a sub object back in object 1
			Assert.IsNull(testData.SubData);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.OtherData);
			Assert.AreEqual(2, testData.OtherData.OtherID);
			Assert.AreEqual(4, testData.OtherData.Bar);
		}

		[Test]
		public void SelectSubSubObjectWithoutCustomMapper()
		{
			var reader = _connection.GetReaderSql(@"
				SELECT ID=1, ID=2, ID=3, SubInt=4
			");

			var results = reader.AsEnumerable<TestData, TestSubData, TestSubSubData>().ToList();

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.IsNotNull(testData.SubData.SubSubData);
			Assert.AreEqual(3, testData.SubData.SubSubData.ID);
			Assert.AreEqual(4, testData.SubData.SubSubData.SubInt);
		}

		[Test]
		public void SelectSubSubObject()
		{
			var reader = _connection.GetReaderSql(@"
				SELECT ID=1, OtherID=2, ID=3, SubInt=4
			");

			var results = reader.AsEnumerable<TestData, TestOtherData, TestSubData>
			(
				// test custom callback function
				callback: (t, t1, t2) =>
				{
					t.OtherData = t1;
					t.OtherData.SubData = t2;
				},
				// test custom ID mapper
				idColumns: new Dictionary<Type, string>() { { typeof(TestOtherData), "OtherID" } }
			).ToList();

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.IsNotNull(testData.OtherData);
			Assert.AreEqual(2, testData.OtherData.OtherID);
			Assert.IsNotNull(testData.OtherData.SubData);
			Assert.AreEqual(3, testData.OtherData.SubData.ID);
			Assert.AreEqual(4, testData.OtherData.SubData.SubInt);
		}

		[Test]
		public void SelectSubSubObjectWithObjectArray()
		{
			var reader = _connection.GetReaderSql(@"
				SELECT ID=1, OtherID=2, ID=3, SubInt=4
			");

			var results = reader.AsEnumerable<TestData>
			(
				// test custom callback function
				callback: (object[] objects) =>
				{
					TestData t = (TestData)objects[0];
					t.OtherData = (TestOtherData)objects[1];
					t.OtherData.SubData = (TestSubData)objects[2];
				},
				// test custom ID mapper
				idColumns: new Dictionary<Type, string>() { { typeof(TestOtherData), "OtherID" } },
				withGraph: typeof(Graph<TestData, TestOtherData, TestSubData>)
			).ToList();

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.IsNotNull(testData.OtherData);
			Assert.AreEqual(2, testData.OtherData.OtherID);
			Assert.IsNotNull(testData.OtherData.SubData);
			Assert.AreEqual(3, testData.OtherData.SubData.ID);
			Assert.AreEqual(4, testData.OtherData.SubData.SubInt);
		}

		[Test]
		public void ShouldAutomaticallyDetectSplitBoundariesWhenNoKeysSpecified()
		{
			var results = _connection.QuerySql<TestData, TestSubData, TestSubData2>(@"
				SELECT ID=1, ID=2, ID=3
			");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.AreEqual(3, testData.SubData2.ID);
		}

		[Test]
		public void ShouldAutomaticallyMapSubObjectsWhenNoMapProvided()
		{
			var results = _connection.QuerySql<TestOtherData, TestSubData>(@"SELECT OtherID=1, ID=2");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.OtherID);
			Assert.AreEqual(2, testData.SubData.ID);
		}

		[Test]
		public void TestObjectWithPropertySubObject()
		{
			var results = _connection.QuerySql<TestProperties, TestSubData>(@"SELECT ID=2");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
		}

		#region DefaultGraph Test Cases
		[DefaultGraph(typeof(Graph<DefaultGraphTestData, TestSubData>))]
		class DefaultGraphTestData
		{
			public TestSubData SubData;
		}

		[Test]
		public void TestObjectWithDefaultGraphDefinition()
		{
			var results = _connection.QuerySql<DefaultGraphTestData>(@"SELECT ID=2");

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
		}
		#endregion

		#region ToList WithGraph Cases
		[Test]
		public void SelectSubObjectToListWithGraph()
		{
			var reader = _connection.GetReaderSql(@"SELECT ID=1, SubDataID=2, ID=2, SubInt=3");
			var results = reader.ToList<TestData>(typeof(Graph<TestData, TestSubData>));

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.AreEqual(3, testData.SubData.SubInt);
		}

		[Test]
		public void SelectSubObjectSingleWithGraph()
		{
			var reader = _connection.GetReaderSql(@"SELECT ID=1, SubDataID=2, ID=2, SubInt=3");
			var testData = reader.Single<TestData>(typeof(Graph<TestData, TestSubData>));

			// test that we got data back
			Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			Assert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			Assert.IsNotNull(testData.SubData);
			Assert.AreEqual(2, testData.SubData.ID);
			Assert.AreEqual(3, testData.SubData.SubInt);
		}

		[Test]
		public void ForEachSubObjectSingleWithGraph()
		{
			_connection.ForEachSql<TestData>(
				@"SELECT ID=1, SubDataID=2, ID=2, SubInt=3",
				Parameters.Empty,
				testData =>
				{
					// test that we got data back
					Assert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
					Assert.AreEqual(2, testData.SubDataID);

					// test that we got a sub object back in object 1
					Assert.IsNotNull(testData.SubData);
					Assert.AreEqual(2, testData.SubData.ID);
					Assert.AreEqual(3, testData.SubData.SubInt);
				},
				withGraph: typeof(Graph<TestData, TestSubData>));
		}
		#endregion
	}
}
