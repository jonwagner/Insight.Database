using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Insight.Database;
using System.Data.Common;
using System.Data;

// since the interface and types are private, we have to let insight have access to them
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Insight.Database")]

namespace Insight.Tests
{
	interface ITest1
	{
		void ExecuteSomething();
		void ExecuteSomethingWithParameters(int p, string q);
		int ExecuteSomethingScalar(int p);
		TestDataClasses.ParentTestData SingleObject();
		IList<int> QueryValue(int p);
		IList<TestDataClasses.ParentTestData> QueryObject();
		int ObjectAsParameter(TestDataClasses.ParentTestData data);
		IList<int> ObjectListAsParameter(IEnumerable<TestDataClasses.ParentTestData> data);
		Results<TestDataClasses.ParentTestData, int> QueryResults(int p);

		Task ExecuteSomethingAsync();
		Task ExecuteSomethingWithParametersAsync(int p, string q);
		Task<int> ExecuteSomethingScalarAsync(int p);
		Task<TestDataClasses.ParentTestData> SingleObjectAsync();
		Task<IList<int>> QueryValueAsync(int p);
		Task<IList<TestDataClasses.ParentTestData>> QueryObjectAsync();
		Task<int> ObjectAsParameterAsync(TestDataClasses.ParentTestData data);
		Task<IList<int>> ObjectListAsParameterAsync(IEnumerable<TestDataClasses.ParentTestData> data);
		Task<Results<TestDataClasses.ParentTestData, int>> QueryResultsAsync(int p);

		[Sql("SELECT X=CONVERT(varchar(128), @p)")]
		string InlineSql(int p);
		[Sql("ExecuteSomethingScalar", CommandType = CommandType.StoredProcedure)]
		int InlineSqlProcOverride(int p);
	}

	#region Test 1
	[TestFixture]
	public class InterfaceTests : BaseDbTest
	{
		[Test]
		public void InterfaceIsGenerated()
		{
			try
			{
				_connection.ExecuteSql("CREATE TYPE ObjectTable AS TABLE (ParentX [int])");

				using (var connection = _connectionStringBuilder.OpenWithTransaction())
				{
					// make sure that we can create an interface
					ITest1 i = connection.As<ITest1>();
					Assert.IsNotNull(i);

					// make sure that the wrapper is still a connection
					DbConnection c = i as DbConnection;
					Assert.IsNotNull(c);

					// create some procs to call
					connection.ExecuteSql("CREATE PROC ExecuteSomething AS SELECT NULL");
					connection.ExecuteSql("CREATE PROC ExecuteSomethingWithParameters @p int, @q [varchar](128) AS SELECT @p, @q");
					connection.ExecuteSql("CREATE PROC ExecuteSomethingScalar @p int AS SELECT @p");
					connection.ExecuteSql("CREATE PROC QueryValue @p int AS SELECT @p UNION ALL SELECT @p");
					connection.ExecuteSql("CREATE PROC QueryObject AS " + TestDataClasses.ParentTestData.Sql);
					connection.ExecuteSql("CREATE PROC SingleObject AS " + TestDataClasses.ParentTestData.Sql);
					connection.ExecuteSql("CREATE PROC ObjectAsParameter @ParentX [int] AS SELECT @ParentX");
					connection.ExecuteSql("CREATE PROC ObjectListAsParameter (@objects [ObjectTable] READONLY) AS SELECT ParentX FROM @objects");
					connection.ExecuteSql("CREATE PROC QueryResults @p int AS " + TestDataClasses.ParentTestData.Sql + " SELECT @p");
					
					// let's call us some methods
					i.ExecuteSomething();
					i.ExecuteSomethingWithParameters(5, "6");
					Assert.AreEqual(9, i.ExecuteSomethingScalar(9));
					i.SingleObject().Verify(false);
					Assert.AreEqual(2, i.QueryValue(9).Count());
					TestDataClasses.ParentTestData.Verify(i.QueryObject(), false);
					Assert.AreEqual(11, i.ObjectAsParameter(new TestDataClasses.ParentTestData() { ParentX = 11 }));
					Assert.AreEqual(11, i.ObjectListAsParameter(new[] { new TestDataClasses.ParentTestData() { ParentX = 11 } }).First());

					var results = i.QueryResults(7);
					TestDataClasses.ParentTestData.Verify(results.Set1, false);
					Assert.AreEqual(7, results.Set2.First());

					// let's call them asynchronously
					i.ExecuteSomethingAsync().Wait();
					i.ExecuteSomethingWithParametersAsync(5, "6").Wait();
					Assert.AreEqual(9, i.ExecuteSomethingScalarAsync(9).Result);
					i.SingleObjectAsync().Result.Verify(false);
					Assert.AreEqual(2, i.QueryValueAsync(9).Result.Count());
					TestDataClasses.ParentTestData.Verify(i.QueryObjectAsync().Result, false);
					Assert.AreEqual(11, i.ObjectAsParameterAsync(new TestDataClasses.ParentTestData() { ParentX = 11 }).Result);
					Assert.AreEqual(11, i.ObjectListAsParameterAsync(new[] { new TestDataClasses.ParentTestData() { ParentX = 11 } }).Result.First());

					results = i.QueryResultsAsync(7).Result;
					TestDataClasses.ParentTestData.Verify(results.Set1, false);
					Assert.AreEqual(7, results.Set2.First());

					// inline SQL!
					Assert.AreEqual("42", i.InlineSql(42));
					Assert.AreEqual(99, i.InlineSqlProcOverride(99));
				}
			}
			finally
			{
				_connection.ExecuteSql("DROP TYPE ObjectTable");
			}
		}
		#endregion
	}
}
