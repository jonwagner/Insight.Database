using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Tests.Cases;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class MultipleResultsTests : BaseTest
	{
		#region Multiple Recordset Tests
		[Test]
		public void TwoRecordsetsAreReturned()
		{
			var results = Connection().QueryResultsSql<ParentTestData, TestData2>(ParentTestData.Sql + TestData2.Sql);

			ClassicAssert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);
			TestData2.Verify(results.Set2);
		}

		[Test]
		public void NullListsAreReturnedWhenFewerRecordsetsAreReturned1()
		{
			// expect one recordsets, but retrieve none. remaining recordsets should be null, not empty.

			var results = Connection().QueryResultsSql<Results<ParentTestData>>("--");
			ClassicAssert.IsNotNull(results);
			ClassicAssert.IsNotNull(results.Set1);
			ClassicAssert.AreEqual(0, results.Set1.Count);
		}

		[Test]
		public void NullListsAreReturnedWhenFewerRecordsetsAreReturned2()
		{
			// expect two recordsets, but retrieve one. remaining recordsets should be null, not empty.

			var results = Connection().QueryResultsSql<ParentTestData, TestData2>(ParentTestData.Sql);
			ClassicAssert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);
			ClassicAssert.IsNotNull(results.Set2);
			ClassicAssert.AreEqual(0, results.Set2.Count);
		}

		[Test]
		public void NullListsAreReturnedWhenFewerRecordsetsAreReturnedAsync1()
		{
			// expect one recordsets, but retrieve none. remaining recordsets should be null, not empty.

			var results = Connection().QueryResultsSqlAsync<Results<ParentTestData>>("--").Result;
			ClassicAssert.IsNotNull(results);
			ClassicAssert.IsNotNull(results.Set1);
			ClassicAssert.AreEqual(0, results.Set1.Count);
		}

		[Test]
		public void NullListsAreReturnedWhenFewerRecordsetsAreReturnedAsync2()
		{
			// expect two recordsets, but retrieve one. remaining recordsets should be null, not empty.

			var results = Connection().QueryResultsSqlAsync<ParentTestData, TestData2>(ParentTestData.Sql).Result;
			ClassicAssert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);
			ClassicAssert.IsNotNull(results.Set2);
			ClassicAssert.AreEqual(0, results.Set2.Count);
		}

		[Test]
		public void EmptyListDoesNotTerminateResults()
		{
			// expect two recordsets, first set is empty. first set should be empty, second should be full
			var results = Connection().QueryResultsSql<ParentTestData, TestData2>("SELECT 0 WHERE 0=1;" + TestData2.Sql);

			ClassicAssert.IsNotNull(results);
			ClassicAssert.IsNotNull(results.Set1);
			ClassicAssert.AreEqual(0, results.Set1.Count);
			TestData2.Verify(results.Set2);
		}
		#endregion

		#region Results with Dynamic Tests
		[Test]
		public void ResultWithDynamicCreatesFastExpando()
		{
			// Results<dynamic> gets compiled as Results<object>
			// Insight will assume that if you want an object, you are really saying dynamic
			// Because, seriously, why would you just want an object back from the database?
			var results = Connection().QueryResultsSql<Results<dynamic>>("SELECT x=1, y=2");
			
			ClassicAssert.AreEqual(1, results.Set1.Count);
			var item = results.Set1[0];
			ClassicAssert.AreEqual(1, item["X"]);
			ClassicAssert.AreEqual(2, item["Y"]);
		}
		#endregion

		#region Derived Recordsets Tests
		[Test]
		public void DerivedRecordsetsCanBeReturned()
		{
			var results = Connection().QueryResultsSql<PageData<ParentTestData>>(ParentTestData.Sql + "SELECT TotalCount=70");

			ClassicAssert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);

			ClassicAssert.IsNotNull(results.Set2);
			ClassicAssert.AreEqual(1, results.Set2.Count);
			ClassicAssert.AreEqual(70, results.TotalCount);
		}
		#endregion
	}
}
