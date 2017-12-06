using Insight.Database;
using Insight.Database.Reliable;
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
	public class DynamicConnectionTest : BaseTest
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
			var _connection = Connection();

			_connection.Dynamic().sp_Who(commandTimeout: 100);

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
			List<Data> result = Connection().Dynamic<Data>().ReflectInt(value: 5);
		}

		[Test]
		public void TestSchema()
		{
			List<Data> result = Connection().Dynamic<Data>().dbo.ReflectInt(value: 5);
		}

		[Test]
		public void TestUnnamedParameter()
		{
			List<Data> result = Connection().Dynamic<Data>().ReflectInt(5);
		}

		[Test]
		public void TestExecute()
		{
			Connection().Dynamic().ReflectInt(value: 5);
		}

		[Test]
		public void TestObjectParameter()
		{
			Data d = new Data() { Value = 10 };
			IList<Data> list = Connection().Dynamic<Data>().ReflectInt(d);
			Data result = list.First();

			Assert.AreEqual(d.Value, result.Value);
		}

		[Test]
		public void TestSingleStringParameter()
		{
			string value = "foo";
			IList<string> list = Connection().Dynamic<string>().ReflectString(value);
			string result = list.First();

			Assert.AreEqual(value, result);
		}

		[Test]
		public void TestDynamicWithMultipleResultSets()
		{
			string value = "foo";
			string value2 = "foo2";
			Results<string, string> results = Connection().Dynamic<Results<string, string>>().ReflectTwoRecordsets(value, value2);

			Assert.AreEqual(value, results.Set1.First());
			Assert.AreEqual(value2, results.Set2.First());
		}

		[Test]
		public void TestReturnTypeOverride()
		{
			string value = "foo";

			var dc = Connection().Dynamic();
			IList<string> results = dc.ReflectString(value, returnType: typeof(string));

			Assert.AreEqual(value, results.First());
		}

		[Test]
		public void TestReturnTypeOverrideAsync()
		{
			string value = "foo";

			var dc = Connection().Dynamic();

			Task<IList<string>> task = dc.ReflectStringAsync(value, returnType: typeof(string));
			var results = task.Result;

			Assert.AreEqual(value, results.First());
		}

		[Test]
		public void TestReturnTypeOverrideWithJustWithReturns()
		{
			IList<ParentTestData> results = Connection().Dynamic().GetParentTestData(returns: Query.Returns(Some<ParentTestData>.Records));
			ParentTestData.Verify(results, false);
		}

		[Test]
		public void TestReturnTypeOverrideWithUnnamedParameter()
		{
			IList<ParentTestData> results = Connection().Dynamic().GetParentTestData(Query.Returns(Some<ParentTestData>.Records));
			ParentTestData.Verify(results, false);
		}

		[Test]
		public void TestMultipleRecordsets()
		{
			// going to infer the return type of the stored procedure rather than specifying it
			Results<ParentTestData, TestData2> results = Connection().Dynamic().GetParentAndChildTestData(
				returns: Query.Returns(Some<ParentTestData>.Records)
							.Then(Some<TestData2>.Records));

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);
			TestData2.Verify(results.Set2);
		}

		[Test]
		public void TestMultipleRecordsetsAsync()
		{
			// going to infer the return type of the stored procedure rather than specifying it
			Results<ParentTestData, TestData2> results = Connection().Dynamic().GetParentAndChildTestDataAsync(
				returns: Query.Returns(Some<ParentTestData>.Records)
							.Then(Some<TestData2>.Records))
							.Result;

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);
			TestData2.Verify(results.Set2);
		}

		[Test]
		public void TestMultipleRecordsetsWithGraph()
		{
			Results<ParentTestData, TestData2> results = Connection().Dynamic().GetParentAndChildTestData(
				returns: Query.Returns(OneToOne<ParentTestData, TestData>.Records)
						.Then(Some<TestData2>.Records));

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: true);
			TestData2.Verify(results.Set2);
		}

		/// <summary>
		/// Tests GitHub Issue #41 - ReliableConnection was not being disposed properly.
		/// </summary>
		[Test]
		public void TestDynamicAsync()
		{
			Parallel.For(0, 10, _ => TryDynamicAsync(10));
		}

		public void TryDynamicAsync(int count)
		{
			for (int i = 0; i < count; i++)
			{
				new SqlConnectionStringBuilder(ConnectionString)
					.ReliableDynamic<int>()
					.ReflectInt32TableAsync(new List<int>() { 5, 7 })
					.Wait();
			}
		}

		/// <summary>
		/// Tests GitHub Issue #13
		/// </summary>
		[Test]
		public void SqlExceptionShouldNotBeHiddenByDynamicCalls()
		{
			// in v2.0.1, we were using method.Invoke to execute the sql. 
			// This would wrap the results in a TargetInvocationException and hide the SQL error.
			Assert.Throws(typeof(SqlException), () => Connection().Dynamic().RaiseAnError(value: 4));
			Assert.Throws(typeof(SqlException), () => Connection().Dynamic().RaiseAnError(value: 4, returnType: typeof(Results<int>)));

			Assert.Throws(typeof(SqlException), () =>
				{
					try
					{
						Connection().Dynamic().RaiseAnErrorAsync(value: 4).Wait();
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
						Connection().Dynamic().RaiseAnErrorAsync(value: 4, returnType: typeof(Results<int>)).Wait();
					}
					catch (AggregateException e)
					{
						throw e.Flatten().InnerExceptions.OfType<SqlException>().First();
					}
				}
			);
		}

		#region Dynamic Proc with Table Parameters
		public class DynamicTableType
		{
			public int Value;
		}

		[Test]
		public void DynamicProcCanHaveTableParameters()
		{
			Assert.AreEqual(0, Connection().Dynamic().DynamicProcWithTable(new { Table = Parameters.EmptyList }).Count);
			Assert.AreEqual(0, Connection().Dynamic().DynamicProcWithTable(Table: Parameters.EmptyList).Count);

			var results = Connection().Dynamic().DynamicProcWithTable(Table: new List<DynamicTableType>() { new DynamicTableType() { Value = 9 } });
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(9, results[0].Value);
		}
		#endregion
	}
}
