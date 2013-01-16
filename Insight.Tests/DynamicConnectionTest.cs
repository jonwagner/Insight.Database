using Insight.Database;
using Insight.Tests.TestDataClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

#pragma warning disable 0649

namespace Insight.Tests
{
	public class DynamicConnectionTest : BaseDbTest
	{
		class Data
		{
			public int Value;
		}

		/// <summary>
		/// Make sure that using dynamic on an unopened connection properly auto-opens the connection when getting the procedure.
		/// </summary>
		[Test]
		public void TestUnopenedConnection()
		{
			// make sure the connection is closed first
			_connection.Close();
			Assert.AreEqual(ConnectionState.Closed, _connection.State);

			// call a proc that we know exists and make sure we get data back
			var result = _connection.Dynamic().sp_Who();
			Assert.IsTrue(result.Count > 0);
			Assert.AreEqual(ConnectionState.Closed, _connection.State);

			// call a proc with no results
			var result2 = _connection.Dynamic().sp_validname("foo");
			Assert.AreEqual(ConnectionState.Closed, _connection.State);

			// call a proc that we know exists and make sure we get data back
			var result3 = _connection.Dynamic().sp_WhoAsync().Result;
			Assert.IsTrue(result3.Count > 0);
			Assert.AreEqual(ConnectionState.Closed, _connection.State);

			// call a proc async with no results
			var result4 = _connection.Dynamic().sp_validnameAsync("foo").Result;
			Assert.AreEqual(ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void Test()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value");

				List<Data> result = connection.Dynamic<Data>().InsightTestProc(value: 5);
			}
		}

		[Test]
		public void TestUnnamedParameter()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value");

				List<Data> result = connection.Dynamic<Data>().InsightTestProc(5);
			}
		}

		[Test]
		public void TestExecute()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS PRINT 'foo'");

				connection.Dynamic().InsightTestProc(value: 5);
			}
		}

		[Test]
		public void TestObjectParameter()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value");

				Data d = new Data() { Value = 10 };
				IList<Data> list = connection.Dynamic<Data>().InsightTestProc(d);
				Data result = list.First();

				Assert.AreEqual(d.Value, result.Value);
			}
		}

		[Test]
		public void TestSingleStringParameter()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS SELECT Value=@Value");

				string value = "foo";
				IList<string> list = connection.Dynamic<string>().InsightTestProc(value);
				string result = list.First();

				Assert.AreEqual(value, result);
			}
		}

		[Test]
		public void TestDynamicWithMultipleResultSets()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128), @Value2 varchar(128)) AS SELECT Value=@Value SELECT Value=@Value2");

				string value = "foo";
				string value2 = "foo2";
				Results<string, string> results = connection.Dynamic<Results<string, string>>().InsightTestProc(value, value2);

				Assert.AreEqual(value, results.Set1.First());
				Assert.AreEqual(value2, results.Set2.First());
			}
		}

		[Test]
		public void TestReturnTypeOverride()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS SELECT Value=@Value");

				string value = "foo";

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				IList<string> results = dc.InsightTestProc(value, returnType: typeof(string));

				Assert.AreEqual(value, results.First());
			}
		}

		[Test]
		public void TestReturnTypeOverrideAsync()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS SELECT Value=@Value");

				string value = "foo";

				var dc = connection.Dynamic();

				// going to infer the return type of the stored procedure rather than specifying it
				Task<IList<string>> task = dc.InsightTestProcAsync(value, returnType: typeof(string));

				var results = task.Result;

				Assert.AreEqual(value, results.First());
			}
		}

		[Test]
		public void TestReturnTypeOverrideWithJustWithGraph()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
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
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
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
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
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
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
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
	}

	/// <summary>
	/// Tests dynamic connection.
	/// </summary>
	[TestFixture]
	public class DynamicConnectionProcTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("CREATE TYPE [Int32Table] AS TABLE ([Value] [int])");
			_connection.ExecuteSql("CREATE PROCEDURE [Int32TestProc] @p [Int32Table] READONLY AS SELECT * FROM @p");
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
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'Int32Table') DROP TYPE [Int32Table]");
		}
		#endregion

		/// <summary>
		/// Make sure that using dynamic on an unopened connection properly auto-opens the connection when getting the procedure.
		/// </summary>
		[Test]
		public void TestUnopenedConnection()
		{
			// make sure the connection is closed first
			_connection.Close();
			Assert.AreEqual(ConnectionState.Closed, _connection.State);

			// call a proc that requires input parameters from a list
			var result5 = _connection.Dynamic().Int32TestProc(new List<int>() { 5, 7 });
			Assert.IsTrue(result5.Count == 2);
			Assert.AreEqual(ConnectionState.Closed, _connection.State);
		}

		/// <summary>
		/// Tests GitHub Issue #13
		/// </summary>
		[Test]
		public void SqlExceptionShouldNotBeHiddenByDynamicCalls()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProcWithError (@Value varchar(128)) AS raiserror ('test', 18, 1)");

				// in v2.0.1, we were using method.Invoke to execute the sql. 
				// This would wrap the results in a TargetInvocationException and hide the SQL error.
				Assert.Throws(typeof(SqlException), () => connection.Dynamic().InsightTestProcWithError(value: 4));
				Assert.Throws(typeof(SqlException), () => connection.Dynamic().InsightTestProcWithError(value: 4, returnType: typeof(Results<int>)));

				Assert.Throws(typeof(SqlException), () =>
					{
						try
						{
							connection.Dynamic().InsightTestProcWithErrorAsync(value: 4).Wait();
						}
						catch (AggregateException e)
						{
							throw e.Flatten().InnerExceptions.OfType<SqlException>().First();
						}
					}
				);

				Assert.Throws(typeof(SqlException), () =>
					{
						try
						{
							connection.Dynamic().InsightTestProcWithErrorAsync(value: 4, returnType: typeof(Results<int>)).Wait();
						}
						catch (AggregateException e)
						{
							throw e.Flatten().InnerExceptions.OfType<SqlException>().First();
						}
					}
				);
			}
		}
	}
}
