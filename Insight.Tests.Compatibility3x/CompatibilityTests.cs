using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using System.Data;
using System.Collections;
using System.Runtime.CompilerServices;

#pragma warning disable 0649

[assembly: InternalsVisibleTo("Insight.Database")]

namespace Insight.Tests
{
	class CompatibilityTests : BaseTest
	{
		#region Async with Graph
		/// <summary>
		/// Asynchronously run a stored procedure with parameter detection
		/// </summary>
		[Test]
		public void TestAsyncQueryWithDefaultGraph()
		{
			var results = Connection().QuerySqlAsync<ParentTestDataWithDefaultGraph>(ParentTestData.Sql).Result;

			ParentTestData.Verify(results);
		}

		/// <summary>
		/// Asynchronously run a stored procedure with parameter detection
		/// </summary>
		[Test]
		public void TestAsyncQueryWithGraph()
		{
			var results = Connection().QuerySqlAsync<ParentTestData>(ParentTestData.Sql, null, withGraph: typeof(Graph<ParentTestData, TestData>)).Result;

			ParentTestData.Verify(results);
		}
		#endregion

#if !NODYNAMIC
		#region Dynamic with Graph
		[Test]
		public void TestReturnTypeOverrideWithJustWithGraph()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc AS " + ParentTestData.Sql);

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				IList<ParentTestData> results = dc.InsightTestProc(withGraph: typeof(Graph<ParentTestData>));
				ParentTestData.Verify(results, false);
			}
		}

		[Test]
		public void TestMultipleRecordsets()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS " + ParentTestData.Sql + TestData2.Sql);

				string value = "foo";

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				Results<ParentTestDataWithDefaultGraph, TestData2> results = dc.InsightTestProc(value, returnType: typeof(Results<ParentTestDataWithDefaultGraph, TestData2>));

				Assert.IsNotNull(results);
				ParentTestData.Verify(results.Set1, withGraph: true);
				TestData2.Verify(results.Set2);
			}
		}

		[Test]
		public void TestMultipleRecordsetsAsync()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS " + ParentTestData.Sql + TestData2.Sql);

				string value = "foo";

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				Task<Results<ParentTestDataWithDefaultGraph, TestData2>> task = dc.InsightTestProcAsync(value, returnType: typeof(Results<ParentTestDataWithDefaultGraph, TestData2>));
				var results = task.Result;

				Assert.IsNotNull(results);
				ParentTestData.Verify(results.Set1, withGraph: true);
				TestData2.Verify(results.Set2);
			}
		}

		[Test]
		public void TestMultipleRecordsetsWithGraph()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS " + ParentTestData.Sql + TestData2.Sql);

				string value = "foo";

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				Results<ParentTestData, TestData2> results = dc.InsightTestProc(
					value,
					returnType: typeof(Results<ParentTestData, TestData2>),
					withGraphs: new Type[] { typeof(Graph<ParentTestData, TestData>) });

				Assert.IsNotNull(results);
				ParentTestData.Verify(results.Set1, withGraph: true);
				TestData2.Verify(results.Set2);
			}
		}
		#endregion
#endif

		#region Interface With Graph
		internal interface IHaveGraph
		{
			[DefaultGraph(typeof(Graph<ParentTestData, TestData>))]
			[Sql("SELECT ParentID=2, ChildID=5")]
			ParentTestData SingleWithGraph();

			[DefaultGraph(typeof(Graph<ParentTestData, TestData>))]
			[Sql("SELECT ParentID=2, ChildID=5")]
			IList<ParentTestData> QueryListWithGraph();
		}

		[Test]
		public void InterfaceIsGenerated()
		{
			var connection = Connection();

			// make sure that we can create an interface
			IHaveGraph i = connection.As<IHaveGraph>();
			Assert.IsNotNull(i);

			// graphs
			i.SingleWithGraph().Verify(true);
			i.QueryListWithGraph().First().Verify(true);
		}

		[Test]
		public void RecordsetWithDefaultGraphIsReturned()
		{
			var results = Connection().QueryResultsSql<ParentTestDataWithDefaultGraph, TestData2>(ParentTestData.Sql + TestData2.Sql);

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: true);
			TestData2.Verify(results.Set2);
		}
		#endregion

		#region DefaultGraph Test Classes
		[DefaultGraph(typeof(Graph<ParentTestDataWithDefaultGraph, TestData>))]
		public class ParentTestDataWithDefaultGraph : ParentTestData
		{
		}

		public class ParentTestData
		{
			public int ParentID;
			public TestData TestData;

			public static readonly string Sql = "SELECT ParentID=2, ChildID=5 ";

			public void Verify(bool withGraph = true)
			{
				Assert.AreEqual(2, ParentID);

				if (withGraph)
				{
					Assert.IsNotNull(TestData);
					Assert.AreEqual(5, TestData.ChildID);
				}
				else
					Assert.IsNull(TestData);
			}

			public static void Verify(IEnumerable results, bool withGraph = true)
			{
				var list = results.OfType<ParentTestData>().ToList();

				Assert.IsNotNull(results);
				Assert.AreEqual(1, list.Count);

				list[0].Verify(withGraph);
			}
		}

		public class TestData
		{
			public int ChildID;
		}
		#endregion

		#region Synchronous Test Cases
		[Test]
		public void TestObjectWithDefaultGraphDefinition()
		{
			var results = Connection().QuerySql<ParentTestDataWithDefaultGraph>(ParentTestData.Sql);

			ParentTestData.Verify(results, withGraph: true);
		}
		#endregion 

		#region ToList WithGraph Cases
		[Test]
		public void SelectSubObjectToListWithGraph()
		{
			using (var connection = Connection().OpenConnection())
			{
				var reader = connection.GetReaderSql(ParentTestData.Sql);
				var results = reader.ToList<ParentTestData>(typeof(Graph<ParentTestData, TestData>));

				ParentTestData.Verify(results, withGraph: true);
			}
		}

		[Test]
		public void SelectSubObjectSingleWithGraph()
		{
			using (var connection = Connection().OpenConnection())
			{
				var reader = connection.GetReaderSql(ParentTestData.Sql);
				var results = reader.Single<ParentTestData>(typeof(Graph<ParentTestData, TestData>));

				ParentTestData.Verify(new List<ParentTestData>() { results }, withGraph: true);
			}
		}

		[Test]
		public void ForEachSubObjectSingleWithGraph()
		{
			Connection().ForEachSql<ParentTestData>(
				ParentTestData.Sql,
				Parameters.Empty,
				results =>
				{
					ParentTestData.Verify(new List<ParentTestData>() { results }, withGraph: true);
				},
				withGraph: typeof(Graph<ParentTestData, TestData>));
		}
		#endregion

		#region Multiple Recordset WithGraph Tests
		[Test]
		public void RecordsetWithGraphIsReturned()
		{
			var results = Connection().QueryResultsSql<ParentTestData, TestData2>(
				ParentTestData.Sql + TestData2.Sql,
				null,
				withGraphs: new[] { typeof(Graph<ParentTestData, TestData>) });

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: true);
			TestData2.Verify(results.Set2);
		}
		#endregion
	}
}
