﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Database;
using Insight.Tests.Cases;
using Insight.Database.Structure;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class SubObjectTests : BaseTest
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
			var results = Connection().QuerySql<TestData, TestSubData>(@"
				CREATE TABLE #TestData ([ID] [int], [SubDataID] [int])
				CREATE TABLE #TestSubData ([ID] [int], [SubInt] [int])
				INSERT INTO #TestData VALUES (1, 2)
				INSERT INTO #TestSubData VALUES (2, 3)
				SELECT * 
					FROM #TestData t 
					LEFT JOIN #TestSubData s ON (t.SubDataID = s.ID)
					ORDER BY t.ID
			");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			ClassicAssert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			ClassicAssert.IsNotNull(testData.SubData);
			ClassicAssert.AreEqual(2, testData.SubData.ID);
			ClassicAssert.AreEqual(3, testData.SubData.SubInt);
		}

		[Test]
		public void SelectWithEmptyLeftJoinShouldContainNullObject()
		{
			var results = Connection().QuerySql<TestData, TestSubData>(@"
				CREATE TABLE #TestData ([ID] [int], [SubDataID] [int])
				CREATE TABLE #TestSubData ([ID] [int], [SubInt] [int])
				INSERT INTO #TestData VALUES (1, 2)
				SELECT * 
					FROM #TestData t 
					LEFT JOIN #TestSubData s ON (t.SubDataID = s.ID)
					ORDER BY t.ID
			");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			ClassicAssert.AreEqual(2, testData.SubDataID);

			ClassicAssert.IsNull(testData.SubData, "Sub object should be null in left join");
		}

		[Test]
		public void SelectWithNoColumnsForSubObjectShouldContainNullObject()
		{
			var results = Connection().QuerySql<TestData, TestSubData>(@"SELECT ID=1");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should be set");
			ClassicAssert.IsNull(testData.SubData, "Sub object should be null in left join");
		}

		[Test]
		public void SelectTwoSubObjects()
		{
			var results = Connection().QuerySql<TestData, TestSubData, TestSubData2>(@"
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

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			ClassicAssert.AreEqual(2, testData.SubDataID);

			// test that we got a sub object back in object 1
			ClassicAssert.IsNotNull(testData.SubData);
			ClassicAssert.AreEqual(2, testData.SubData.ID);
			ClassicAssert.AreEqual(3, testData.SubData.SubInt);

			// test that we got a sub object back in object 1
			ClassicAssert.IsNotNull(testData.SubData2);
			ClassicAssert.AreEqual(3, testData.SubData2.ID);
			ClassicAssert.AreEqual(4, testData.SubData2.SubInt);
		}

		[Test]
		public void SelectTwoSubObjectsWithMissingMiddleObjectShouldHaveNullMiddleObject()
		{
			var results = Connection().QuerySql<TestData, TestSubData, TestOtherData>(@"
				SELECT ID=1, Foo=3, OtherID=2, Bar=4
			");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should be set");
			ClassicAssert.AreEqual(3, testData.Foo, "Foo should be set");

			// test that we got a sub object back in object 1
			ClassicAssert.IsNull(testData.SubData);

			// test that we got a sub object back in object 1
			ClassicAssert.IsNotNull(testData.OtherData);
			ClassicAssert.AreEqual(2, testData.OtherData.OtherID);
			ClassicAssert.AreEqual(4, testData.OtherData.Bar);
		}

		[Test]
		public void SelectSubSubObjectWithoutCustomMapper()
		{
			using (var connection = Connection().OpenConnection())
			{
				var reader = connection.GetReaderSql(@"
					SELECT ID=1, ID=2, ID=3, SubInt=4
				");

				var results = reader.AsEnumerable<TestData, TestSubData, TestSubSubData>().ToList();

				ClassicAssert.IsNotNull(results);
				ClassicAssert.AreEqual(1, results.Count);

				// test that we got data back
				var testData = results[0];
				ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
				ClassicAssert.IsNotNull(testData.SubData);
				ClassicAssert.AreEqual(2, testData.SubData.ID);
				ClassicAssert.IsNotNull(testData.SubData.SubSubData);
				ClassicAssert.AreEqual(3, testData.SubData.SubSubData.ID);
				ClassicAssert.AreEqual(4, testData.SubData.SubSubData.SubInt);
			}
		}

		[Test]
		public void SelectSubSubObjectWithObjectArray()
		{
			using (var connection = Connection().OpenConnection())
			{
				var reader = connection.GetReaderSql(@"
					SELECT ID=1, OtherID=2, ID=3, SubInt=4
				");

				var results = reader.AsEnumerable<TestData>
				(new OneToOne<TestData, TestOtherData, TestSubData>(
					// test custom callback function
					callback: (t, t2, t3) =>
					{
						t.OtherData = t2;
						t.OtherData.SubData = t3;
					},
					// test custom ID mapper
					splitColumns: new Dictionary<Type, string>() { { typeof(TestOtherData), "OtherID" } }
				)).ToList();

				ClassicAssert.IsNotNull(results);
				ClassicAssert.AreEqual(1, results.Count);

				// test that we got data back
				var testData = results[0];
				ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
				ClassicAssert.IsNotNull(testData.OtherData);
				ClassicAssert.AreEqual(2, testData.OtherData.OtherID);
				ClassicAssert.IsNotNull(testData.OtherData.SubData);
				ClassicAssert.AreEqual(3, testData.OtherData.SubData.ID);
				ClassicAssert.AreEqual(4, testData.OtherData.SubData.SubInt);
			}
		}

		[Test]
		public void ShouldAutomaticallyDetectSplitBoundariesWhenNoKeysSpecified()
		{
			var results = Connection().QuerySql<TestData, TestSubData, TestSubData2>(@"
				SELECT ID=1, ID=2, ID=3
			");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.ID, "ID should not be overwritten by sub-object id");
			ClassicAssert.AreEqual(2, testData.SubData.ID);
			ClassicAssert.AreEqual(3, testData.SubData2.ID);
		}

		[Test]
		public void ShouldAutomaticallyMapSubObjectsWhenNoMapProvided()
		{
			var results = Connection().QuerySql<TestOtherData, TestSubData>(@"SELECT OtherID=1, ID=2");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.AreEqual(1, testData.OtherID);
			ClassicAssert.AreEqual(2, testData.SubData.ID);
		}

		[Test]
		public void TestObjectWithPropertySubObject()
		{
			var results = Connection().QuerySql<TestProperties, TestSubData>(@"SELECT ID=2");

			ClassicAssert.IsNotNull(results);
			ClassicAssert.AreEqual(1, results.Count);

			// test that we got data back
			var testData = results[0];
			ClassicAssert.IsNotNull(testData.SubData);
			ClassicAssert.AreEqual(2, testData.SubData.ID);
		}

		#region Null SubObject Tests
		public class Address
		{
			public string city;
			public string country;
		}

		public class Supplier
		{
			public int id;
			public string name;
			public Address site;
		}

		/// <summary>
		/// This tests issue #61.
		/// </summary>
		[Test]
		public void ReturningSubObjectShouldNotThrowWhenAllColumnsAreNull()
		{
			var supplier = Connection().QuerySql<Supplier, Address>("select id=1, name='supplier', city=null, country=null").First();
			ClassicAssert.IsNull(supplier.site);
		}

		/// <summary>
		/// This tests issue #61.
		/// </summary>
		[Test]
		public void ReturningSubObjectShouldNotThrowWhenFirstColumnIsNull()
		{
			var supplier = Connection().QuerySql<Supplier, Address>("select id=1, name='supplier', city=null, country='usa'").First();
			ClassicAssert.IsNotNull(supplier.site);
			ClassicAssert.IsNull(supplier.site.city);
			ClassicAssert.IsNotNull(supplier.site.country);
		}
		#endregion

		#region Child Mapping Tests
		[Test]
		public void ParentIDFieldCanBeString()
		{
			// when ParentID is a string column, the mapping was failing on a string to object conversion
			var result = Connection().QuerySql("EXEC " + Beer.SelectAllProc + "; SELECT ParentID='1', ID=1", null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records));

			// there should be two parents, and both of them should have the same child
			ClassicAssert.AreEqual(3, result.Count);
			ClassicAssert.AreEqual(1, result[0].List.Count);
		}

		[Test]
		public void ParentIDFieldCanBeNamedAnything()
		{
			// the ParentID column is always the first column
			Connection().QuerySql("EXEC " + Beer.SelectAllProc + "; SELECT xyzzy='1', ID=1", null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records));
		}

		[Test]
		public void CanHaveDuplicateParentIDs()
		{
			// the ParentID column is always the first column
			var result = Connection().QuerySql("SELECT ID=1 UNION ALL SELECT ID=1; SELECT ParentID=1, ID=1", null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records));

			// there should be two parents, and both of them should have the same child
			ClassicAssert.AreEqual(2, result.Count);
			ClassicAssert.AreEqual(1, result[0].List.Count);
			ClassicAssert.AreEqual(1, result[1].List.Count);
			ClassicAssert.AreEqual(result[0].List[0], result[1].List[0]);
		}

		[Test]
		public void SingleParentHandlesAllChildren()
		{
			// the ParentID column is ignored for singles
			var result = Connection().QuerySql("EXEC " + Beer.SelectAllProc + "; SELECT ID=1 UNION SELECT ID=2", null,
				Query.ReturnsSingle<InfiniteBeerList>()
					.ThenChildren(Some<InfiniteBeerList>.Records));

			ClassicAssert.AreEqual(2, result.List.Count);
		}

		[Test]
		public void SingleParentCanHaveMultipleLevels()
		{
			// the ParentID column is always the first column
			var result = Connection().QuerySql("EXEC " + Beer.SelectAllProc + "; SELECT ID=1 UNION SELECT ID=2; SELECT ParentID=1, ID=1 UNION SELECT ParentID=2, ID=2", null,
				Query.ReturnsSingle<InfiniteBeerList>()
					.ThenChildren(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, parents: b => b.List));

			ClassicAssert.AreEqual(2, result.List.Count);
			ClassicAssert.AreEqual(1, result.List[0].List[0].ID);
			ClassicAssert.AreEqual(2, result.List[1].List[0].ID);
		}
		#endregion

		#region Duplicate Subobject Tests
		[BindChildren(BindChildrenFor.All)]
		public class TestDataWithDuplicateChild
		{
			public TestDuplicateChild SubData;
			public TestDuplicateChild SubData2;
		}

		public class TestDuplicateChild
		{
			public int SubInt;
		}

		public interface IReturnDuplicateChildren
		{
			[Sql("SELECT SubInt=1, SubInt=2")]
			[Recordset(0, typeof(TestDataWithDuplicateChild), typeof(TestDuplicateChild), typeof(TestDuplicateChild))]
			TestDataWithDuplicateChild GetDuplicateChildren();
		}

		[Test]
		public void CanReturnDuplicateChildren()
		{
			var results = Connection().SingleSql<TestDataWithDuplicateChild, TestDuplicateChild, TestDuplicateChild>(@"
				SELECT SubInt=1, SubInt=2
			");

			ClassicAssert.AreEqual(1, results.SubData.SubInt);
			ClassicAssert.AreEqual(2, results.SubData2.SubInt);
		}
		
		[Test]
		public void InterfaceCanReturnDuplicateChildrenInNameOrder()
		{
			var results = Connection().As<IReturnDuplicateChildren>().GetDuplicateChildren();

			// the children are bound in alphabetical order	
			ClassicAssert.AreEqual(1, results.SubData.SubInt);
			ClassicAssert.AreEqual(2, results.SubData2.SubInt);
		}
		#endregion
	}
}
