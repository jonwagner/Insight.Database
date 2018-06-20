using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Insight.Database;
using System.Data.Common;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using Insight.Tests.Cases;
using Insight.Database.Reliable;
using System.Data.SqlClient;

// since the interface and types are private, we have to let insight have access to them
[assembly: InternalsVisibleTo("Insight.Database.DynamicAssembly")]

namespace Insight.Tests
{
	#region Test Interfaces
	interface ITest1 : IDbConnection, IDbTransaction
	{
		// all execution modes
		void ExecuteSomething();
		void ExecuteSomethingWithParameters(int p, string q);
		int ExecuteSomethingScalar(int p);
		ParentTestData SingleObject();
		ParentTestData SingleObjectWithNoData();
		IList<int> QueryValue(int p);
		List<int> QueryValueList(int p);
		IEnumerable<int> QueryValueEnumerable(int p);
		IList<ParentTestData> QueryObject();
		int ObjectAsParameter(ParentTestData data);
		IList<int> ObjectListAsParameter(IEnumerable<ParentTestData> data);
		Results<ParentTestData, int> QueryResults(int p);

		// same procs, asynchronously
		Task ExecuteSomethingAsync();
		Task ExecuteSomethingWithParametersAsync(int p, string q);
		Task<int> ExecuteSomethingScalarAsync(int p);
		Task<ParentTestData> SingleObjectAsync();
		Task<IList<int>> QueryValueAsync(int p);
		Task<IEnumerable<int>> QueryValueEnumerableAsync(int p);
		Task<List<int>> QueryValueListAsync(int p);
		Task<IList<ParentTestData>> QueryObjectAsync();
		Task<int> ObjectAsParameterAsync(ParentTestData data);
		Task<IList<int>> ObjectListAsParameterAsync(IEnumerable<ParentTestData> data);
		Task<Results<ParentTestData, int>> QueryResultsAsync(int p);

		// inline overrides
		[Sql("SELECT X=CONVERT(varchar(128), @p)")]
		string InlineSql(int p);
		[Sql("ExecuteSomethingScalar", CommandType.StoredProcedure)]
		int InlineSqlProcOverride(int p);
		[Sql(Schema="dbo", Sql="ExecuteSomethingScalar")]
		int InlineSqlWithSchema(int p);
	}

	interface ITestWithSpecialParameters
	{
		void ExecuteSomething(int? commandTimeout);
		Task ExecuteSomethingAsync(CancellationToken cancellationToken);

		[Sql("ExecuteSomething")]
		void ExecuteSomethingWithTransaction(IDbTransaction transaction);
	}

	interface ITestInsertUpdate
	{
		void InsertTestData(TestData data);
		void UpdateTestData(TestData data);
		void UpsertTestData(TestData data);
		void InsertMultipleTestData(IEnumerable<TestData> data);
		void UpsertMultipleTestData(IEnumerable<TestData> data);

		Task InsertTestDataAsync(TestData data);
		Task UpsertTestDataAsync(TestData data);
		Task InsertMultipleTestDataAsync(IEnumerable<TestData> data);
		Task UpsertMultipleTestDataAsync(IEnumerable<TestData> data);
	}

	interface ITestOutputParameters
	{
		void ExecuteWithOutputParameter(out int p);
		int ExecuteScalarWithOutputParameter(out int p);
		IList<int> QueryWithOutputParameter(out int p);
		Results<ParentTestData, int> QueryResultsWithOutputParameter(out int p);
		void InsertWithOutputParameter(IEnumerable<TestData> data, out int p);
	}

	[Sql(Schema = "dbo")]
	interface ITestWithSqlAttribute
	{
		int ExecuteSomethingScalar(int p);
	}

	interface IBaseSql
	{
		[Sql("SELECT 'base'")]
		string Base();
	}

	interface IDerivedSql : IBaseSql
	{
		[Sql("SELECT 'derived'")]
		string Derived();
	}

	interface IMergeOutputs
	{
		[MergeOutputAttribute]
		[Sql("SELECT X=4, Z=6")]
		void DoAMerge(TestData data);
	}
	#endregion

	[TestFixture]
	public class InterfaceTests : BaseTest
	{
		#region Test Interface Is Generated
		[Test]
		public void InterfaceIsGenerated()
		{
			var connection = Connection();

			// make sure that we can create an interface
			ITest1 i = connection.As<ITest1>();
			Assert.IsNotNull(i);

			// make sure that the wrapper is still a connection
			DbConnection c = i as DbConnection;
			Assert.IsNotNull(c);

			// let's call us some methods
			i.ExecuteSomething();
			i.ExecuteSomethingWithParameters(5, "6");
			Assert.AreEqual(9, i.ExecuteSomethingScalar(9));
			i.SingleObject().Verify(false);
			Assert.IsNull(i.SingleObjectWithNoData());
			Assert.AreEqual(2, i.QueryValue(9).Count());
			ParentTestData.Verify(i.QueryObject(), false);
			Assert.AreEqual(11, i.ObjectAsParameter(new ParentTestData() { ParentX = 11 }));
			Assert.AreEqual(11, i.ObjectListAsParameter(new[] { new ParentTestData() { ParentX = 11 } }).First());

			var results = i.QueryResults(7);
			ParentTestData.Verify(results.Set1, false);
			Assert.AreEqual(7, results.Set2.First());

			// let's call them asynchronously
			i.ExecuteSomethingAsync().Wait();
			i.ExecuteSomethingWithParametersAsync(5, "6").Wait();
			Assert.AreEqual(9, i.ExecuteSomethingScalarAsync(9).Result);
			i.SingleObjectAsync().Result.Verify(false);
			Assert.AreEqual(2, i.QueryValueAsync(9).Result.Count());
			ParentTestData.Verify(i.QueryObjectAsync().Result, false);
			Assert.AreEqual(11, i.ObjectAsParameterAsync(new ParentTestData() { ParentX = 11 }).Result);
			Assert.AreEqual(11, i.ObjectListAsParameterAsync(new[] { new ParentTestData() { ParentX = 11 } }).Result.First());

			results = i.QueryResultsAsync(7).Result;
			ParentTestData.Verify(results.Set1, false);
			Assert.AreEqual(7, results.Set2.First());

			// inline SQL!
			Assert.AreEqual("42", i.InlineSql(42));
			Assert.AreEqual(99, i.InlineSqlProcOverride(99));
			Assert.AreEqual(98, i.InlineSqlWithSchema(98));
			Assert.AreEqual(98, connection.As<ITestWithSqlAttribute>().ExecuteSomethingScalar(98));
		}
		#endregion

		#region Test Interface Special Parameters
		[Test]
		public void InterfaceWithSpecialParametersIsGenerated()
		{
			using (var connection = Connection().OpenConnection())
			{
				// make sure that we can create an interface
				ITestWithSpecialParameters i = connection.As<ITestWithSpecialParameters>();
				Assert.IsNotNull(i);

				// let's call us some methods

				// commandTimeout
				i.ExecuteSomething(30);

				// a cancelled cancellation token
				CancellationTokenSource cts = new CancellationTokenSource();
				cts.Cancel();
				Assert.Throws<AggregateException>(() => { i.ExecuteSomethingAsync(cts.Token).Wait(); });

				// override of the transaction
				// NOTE: if you use OpenWithTransaction, the transaction is propagated automatically, so you don't need to do this
				using (var tx = connection.BeginTransaction())
				{
					i.ExecuteSomethingWithTransaction(tx);
				}
			}
		}
		#endregion

		#region Test Interface with Transaction
		[Test]
		public void ConnectionOpenedWithInterfaceAndTransaction()
		{
			using (var connection = Connection().OpenWithTransactionAs<ITest1>())
			{
				connection.ExecuteSomething();
				connection.Rollback();
			}
		}
		#endregion

		#region Test Insert as TVP
		[Test]
		public void TestInsert()
		{
			using (var connection = Connection().OpenWithTransaction())
			{
				var i = connection.As<ITestInsertUpdate>();
				connection.Execute("ResetTestDataTable");

				// single insert
				TestData data = new TestData() { Z = 4 };
				i.InsertTestData(data);
				Assert.AreEqual(1, data.X, "ID should be returned");

				// single update
				i.UpdateTestData(data);
				Assert.AreEqual(0, data.X, "ID should be reset");

				// single upsert
				data = new TestData() { Z = 4 };
				i.InsertTestData(data);
				Assert.AreEqual(2, data.X, "ID should be returned");
				i.UpsertTestData(data);
				Assert.AreEqual(0, data.X, "ID should be reset");

				// multiple insert
				var list = new[]
				{
					new TestData() { Z = 5 },
					new TestData() { Z = 6 }
				};
				i.InsertMultipleTestData(list);
				Assert.AreEqual(3, list[0].X, "ID should be returned");
				Assert.AreEqual(4, list[1].X, "ID should be returned");

				// multiple update
				i.UpsertMultipleTestData(list);
				Assert.AreEqual(0, list[0].X, "ID should be reset");
				Assert.AreEqual(0, list[1].X, "ID should be reset");

				// single insert
				data = new TestData() { Z = 4 };
				i.InsertTestDataAsync(data).Wait();
				Assert.AreEqual(5, data.X, "ID should be returned");

				// single update
				i.UpsertTestDataAsync(data).Wait();
				Assert.AreEqual(0, data.X, "ID should be reset");

				// multiple insert
				list = new[]
				{
					new TestData() { Z = 5 },
					new TestData() { Z = 6 }
				};
				i.InsertMultipleTestDataAsync(list).Wait();
				Assert.AreEqual(6, list[0].X, "ID should be returned");
				Assert.AreEqual(7, list[1].X, "ID should be returned");

				// multiple update
				i.UpsertMultipleTestDataAsync(list).Wait();
				Assert.AreEqual(0, list[0].X, "ID should be reset");
				Assert.AreEqual(0, list[1].X, "ID should be reset");
			}
		}
		#endregion

		#region Test Interface Multi-Threaded
		/// <summary>
		/// Tests GitHub Issue #41 - connection was not being disposed properly.
		/// </summary>
		[Test]
		public void TestInterfaceMultithreaded()
		{
			// this only works in 4.0 and later
			Parallel.For(0, 100, _ => TryInterfaceCall(100));
		}

		public void TryInterfaceCall(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Connection()
					.As <ITest1>()
					.ExecuteSomething();
			}
		}
		#endregion

		#region Test Output Parameters
		[Test]
		public void TestOutputParameters()
		{
			using (var connection = Connection().OpenWithTransaction())
			{
				var i = connection.As<ITestOutputParameters>();

				// test execute with output parameter
				int original = 2;
				int p = original;
				i.ExecuteWithOutputParameter(out p);
				Assert.AreEqual(original + 1, p);

				// test executescalar with output parameter
				p = original;
				var scalar = i.ExecuteScalarWithOutputParameter(out p);
				Assert.AreEqual(original + 1, p);
				Assert.AreEqual(7, scalar);

				// test query with output parameters
				p = original;
				var results = i.QueryWithOutputParameter(out p);
				Assert.AreEqual(original + 1, p);
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual(5, results[0]);

				// test query results with output parameters
				p = original;
				i.QueryResultsWithOutputParameter(out p);
				Assert.AreEqual(original + 1, p);

				// test insert with output parameters
				TestData data = new TestData() { Z = 4 };
				var list = new List<TestData>() { data };
				p = original;
				i.InsertWithOutputParameter(list, out p);
				Assert.AreEqual(original + 1, p);
			}
		}
		#endregion

		#region List Return Tests
		public interface IReturnItems
		{
			[Sql("SELECT 1 UNION SELECT 2")] IEnumerable<int> QueryEnumerable();
			[Sql("SELECT 1 UNION SELECT 2")] IList<int> QueryIList();
			[Sql("SELECT 1 UNION SELECT 2")] List<int> QueryList();
			[Sql("SELECT 1 UNION SELECT 2")] ICollection<int> QueryCollection();
			[Sql("SELECT 1 UNION SELECT 2")] Results<int> QueryResults();
			[Sql("SELECT 1 UNION SELECT 2")] Task<IEnumerable<int>> QueryEnumerableAsync();
			[Sql("SELECT 1 UNION SELECT 2")] Task<IList<int>> QueryIListAsync();
			[Sql("SELECT 1 UNION SELECT 2")] Task<List<int>> QueryListAsync();
			[Sql("SELECT 1 UNION SELECT 2")] Task<ICollection<int>> QueryCollectionAsync();
			[Sql("SELECT 1 UNION SELECT 2")] Task<Results<int>> QueryResultsAsync();
		}

		[Test]
		public void DifferentTypesOfListsAreSupportedAsReturnTypes()
		{
			using (var connection = Connection().OpenWithTransaction())
			{
				IReturnItems i = connection.As<IReturnItems>();

				IEnumerable<int> result = i.QueryEnumerable();
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryIList();
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryList();
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryCollection();
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryResults().Set1;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryEnumerableAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryIListAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryListAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryCollectionAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());

				result = i.QueryResultsAsync().Result.Set1;
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.Count());
			}
		}

		public interface IReturnNothing
		{
			[Sql("print ''")] IEnumerable<int> EmptyEnumerable();
			[Sql("print ''")] IList<int> EmptyIList();
			[Sql("print ''")] List<int> EmptyList();
			[Sql("print ''")] ICollection<int> EmptyCollection();
			[Sql("print ''")] Results<int> EmptyResults();
			[Sql("print ''")] Task<IEnumerable<int>> EmptyEnumerableAsync();
			[Sql("print ''")] Task<IList<int>> EmptyIListAsync();
			[Sql("print ''")] Task<List<int>> EmptyListAsync();
			[Sql("print ''")] Task<ICollection<int>> EmptyCollectionAsync();
			[Sql("print ''")] Task<Results<int>> EmptyResultsAsync();
		}

		[Test]
		public void MissingResultSetShouldReturnEmptyListRegardlessOfReturnType()
		{
			using (var connection = Connection().OpenWithTransaction())
			{
				var i = connection.As<IReturnNothing>();

				IEnumerable<int> result = i.EmptyEnumerable();
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyIList();
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyList();
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyCollection();
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyResults().Set1;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyEnumerableAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyIListAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyListAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyCollectionAsync().Result;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());

				result = i.EmptyResultsAsync().Result.Set1;
				Assert.IsNotNull(result);
				Assert.AreEqual(0, result.Count());
			}
		}
		#endregion

		#region Schema Tests
		[Sql(Schema="MySchema")]
		public interface IBeerRepositoryWithSchema
		{
			[Sql("MyOtherInsertProc")]
			void InsertBeer(int beer);
		}

		[Test]
		public void SchemaShouldBeInherited()
		{
			Assert.Throws<InvalidOperationException>(() => Connection().As<IBeerRepositoryWithSchema>().InsertBeer(1), "The stored procedure 'MySchema.MyOtherInsertProc' doesn't exist.");
		}
		#endregion

		#region Structure Tests
		internal interface IHaveStructure
		{
			[Sql("SELECT ID=1, ID=2")]
			IList<InfiniteBeer> GetBeerAndMoreWithExplicitStructure(Insight.Database.Structure.IQueryReader<IList<InfiniteBeer>> returns);

			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			IList<InfiniteBeer> GetBeerAndMoreWithAttributeIList();
			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			ICollection<InfiniteBeer> GetBeerAndMoreWithAttributeICollection();
			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			IEnumerable<InfiniteBeer> GetBeerAndMoreWithAttributeIEnumerable();
			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			List<InfiniteBeer> GetBeerAndMoreWithAttributeList();

			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			Task<IList<InfiniteBeer>> GetBeerAndMoreWithAttributeAsync();

			[Sql("SELECT ID=1, ID=2; SELECT ID=2, ID=2")]
			[Recordset(1, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			Results<InfiniteBeer, InfiniteBeer> GetBeerResultsWithAttribute();

			[Sql("SELECT ParentX=1; SELECT TotalCount=70")]
			PageData<ParentTestData> GetDerivedRecordset();

			[Sql("SELECT ID=1; SELECT ParentID=1, ID=1")]
			[Recordset(0, typeof(InfiniteBeerList))]
			[Recordset(1, typeof(InfiniteBeerList), IsChild = true)]
			IList<InfiniteBeerList> GetBeerWithChildrenList();
			[Sql("SELECT ID=1; SELECT ParentID=1, ID=1")]
			[Recordset(0, typeof(InfiniteBeerList))]
			[Recordset(1, typeof(InfiniteBeerList), IsChild = true)]
			Results<InfiniteBeerList> GetBeerWithChildrenResults();

			[Sql("SELECT ID=1; SELECT ParentID=1, ID=2; SELECT ParentID=1, ID=3")]
			[Recordset(1, typeof(Beer), IsChild = true)]
			[Recordset(2, typeof(Wine), IsChild = true)]
			IList<LiquorStore> GetLiquorStoreWithMultipleChildren();

			[Sql("SELECT ID=1; SELECT ParentID=1, ID=2; SELECT ParentID=1, ID=3; SELECT ID=4")]
			[Recordset(1, typeof(Beer), IsChild = true)]
			[Recordset(2, typeof(Wine), IsChild = true)]
			[Recordset(3, typeof(Beer))]
			Results<LiquorStore, Beer> GetLiquorStoreWithChildrenAndMore();

			[Sql("SELECT Foo=1; SELECT ParentID=1, ID=2; SELECT ParentID=1, ID=3; SELECT ID=4")]
			[Recordset(1, typeof(Beer), IsChild = true, Id = "Foo", Into = "OtherBeer")]
			[Recordset(2, typeof(Wine), IsChild = true, Id = "Foo")]
			[Recordset(3, typeof(Beer))]
			Results<LiquorStoreNeedingFieldOverrides, Beer> GetLiquorStoreWithChildrenAndMoreWithOverrides();

			[Sql("SELECT ID=1, ID=2")]
			[Recordset(0, typeof(InfiniteBeer), typeof(InfiniteBeer))]
			InfiniteBeer GetSingleBeerWithAttribute();
		}

		internal interface IHaveStructure2
		{
			[Sql("SELECT ID=1; SELECT ID=2 UNION SELECT ID=3")]
			[Recordset(0, typeof(InfiniteBeerList))]
			[Recordset(1, typeof(InfiniteBeerList), IsChild = true)]
			InfiniteBeerList GetSingleBeerWithChildren();

			[Sql("SELECT ID=1; SELECT ID=2 UNION SELECT ID=3")]
			[Recordset(0, typeof(InfiniteBeerList))]
			[Recordset(1, typeof(InfiniteBeerList), IsChild = true, Id="ID")]
			InfiniteBeerList GetSingleBeerWithChildrenWithIDOverride();

			[Sql("SELECT ID=1; SELECT ParentID=1, ID=2 UNION SELECT ParentID=1, ID=3; SELECT ParentID=2, ID=4")]
			[Recordset(0, typeof(InfiniteBeerList))]
			[Recordset(1, typeof(InfiniteBeerList), IsChild = true, Id = "ID")]
			[Recordset(2, typeof(InfiniteBeerList), IsChild = true, Id = "ID", Parents="List")]
			InfiniteBeerList GetMultiLevelChildren();			
		}

		[Test]
		public void CanPassStructureToInterface()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetBeerAndMoreWithExplicitStructure(Insight.Database.Structure.ListReader<InfiniteBeer, InfiniteBeer>.Default);

			Assert.AreEqual(1, result.Count);
			var beer = result[0];
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.IsNotNull(beer.More);
			Assert.AreEqual(2, beer.More.ID);
		}

		[Test]
		public void CanUseAttributeToGetList()
		{
			var i = Connection().As<IHaveStructure>();

			var calls = new Func<IEnumerable<InfiniteBeer>>[] {
				() => i.GetBeerAndMoreWithAttributeIList(),
				() => i.GetBeerAndMoreWithAttributeIEnumerable(),
				() => i.GetBeerAndMoreWithAttributeICollection(),
				() => i.GetBeerAndMoreWithAttributeList(),
			};

			foreach (var call in calls)
			{
				var result = call();

				Assert.AreEqual(1, result.Count());
				var beer = result.First();
				Assert.IsNotNull(beer);
				Assert.AreEqual(1, beer.ID);
				Assert.IsNotNull(beer.More);
				Assert.AreEqual(2, beer.More.ID);
			}
		}

		[Test]
		public void CanUseAttributeToGetListAsync()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetBeerAndMoreWithAttributeAsync().Result;

			Assert.AreEqual(1, result.Count);
			var beer = result[0];
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.IsNotNull(beer.More);
			Assert.AreEqual(2, beer.More.ID);
		}

		[Test]
		public void CanUseAttributeToGetResults()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetBeerResultsWithAttribute();

			Assert.AreEqual(1, result.Set1.Count);
			Assert.AreEqual(1, result.Set1[0].ID);
			Assert.IsNull(result.Set1[0].More);

			Assert.AreEqual(1, result.Set2.Count);
			Assert.AreEqual(2, result.Set2[0].ID);
			Assert.IsNotNull(result.Set2[0].More);
			Assert.AreEqual(2, result.Set2[0].More.ID);
		}

		[Test]
		public void CanGetDerivedRecordset()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetDerivedRecordset();

			Assert.AreEqual(1, result.Set1.Count);
			Assert.AreEqual(1, result.Set1[0].ParentX);

			Assert.AreEqual(70, result.TotalCount);
		}

		[Test]
		public void CanGetChildrenInList()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetBeerWithChildrenList();

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0].List.Count);
			Assert.AreEqual(1, result[0].List[0].ID);
		}

		[Test]
		public void CanGetChildrenInResults()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetBeerWithChildrenResults().Set1;

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0].List.Count);
			Assert.AreEqual(1, result[0].List[0].ID);
		}

		[Test]
		public void CanGetMultipleChildren()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetLiquorStoreWithMultipleChildren();

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0].Beer.Count);
			Assert.AreEqual(2, result[0].Beer[0].ID);
			Assert.AreEqual(1, result[0].Wine.Count);
			Assert.AreEqual(3, result[0].Wine[0].ID);
		}

		[Test]
		public void CanGetMultipleChildrenAndThenMore()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetLiquorStoreWithChildrenAndMore();

			Assert.AreEqual(1, result.Set1.Count);
			Assert.AreEqual(1, result.Set1[0].Beer.Count);
			Assert.AreEqual(2, result.Set1[0].Beer[0].ID);
			Assert.AreEqual(1, result.Set1[0].Wine.Count);
			Assert.AreEqual(3, result.Set1[0].Wine[0].ID);

			Assert.AreEqual(1, result.Set2.Count);
			Assert.AreEqual(4, result.Set2[0].ID);
		}

		[Test]
		public void CanOverrideParentChildRelationships()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetLiquorStoreWithChildrenAndMoreWithOverrides().Set1;

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0].OtherBeer.Count);
			Assert.AreEqual(2, result[0].OtherBeer[0].ID);
			Assert.AreEqual(1, result[0].Wine.Count);
			Assert.AreEqual(3, result[0].Wine[0].ID);
		}

		[Test]
		public void CanGetSingleWithAttribute()
		{
			var i = Connection().As<IHaveStructure>();
			var result = i.GetSingleBeerWithAttribute();

			var beer = result;
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.IsNotNull(beer.More);
			Assert.AreEqual(2, beer.More.ID);
		}

		[Test]
		public void CanGetSingleWithChildren()
		{
			var i = Connection().As<IHaveStructure2>();
			var result = i.GetSingleBeerWithChildren();

			var beer = result;
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.AreEqual(2, beer.List.Count);
			Assert.AreEqual(2, beer.List[0].ID);
			Assert.AreEqual(3, beer.List[1].ID);
		}

		[Test]
		public void SingleWithChildrenAndIDFiltersOutChildren()
		{
			var i = Connection().As<IHaveStructure2>();
			var result = i.GetSingleBeerWithChildrenWithIDOverride();

			var beer = result;
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.AreEqual(0, beer.List.Count);
		}
		
		[Test]
		public void CanGetMultiLevelChildren()
		{
			var i = Connection().As<IHaveStructure2>();
			var result = i.GetMultiLevelChildren();

			var beer = result;
			Assert.IsNotNull(beer);
			Assert.AreEqual(1, beer.ID);
			Assert.AreEqual(2, beer.List.Count);
			Assert.AreEqual(2, beer.List[0].ID);
			Assert.AreEqual(3, beer.List[1].ID);

			var beer2 = beer.List[0];
			Assert.AreEqual(2, beer2.ID);
			Assert.AreEqual(1, beer2.List.Count);
			Assert.AreEqual(4, beer2.List[0].ID);
		}

		#endregion

		#region Inheritance Tests
		[Test]
		public void TestInheritedInterface()
		{
			// make sure that we can create an interface
			var i = Connection().As<IDerivedSql>();
			Assert.IsNotNull(i);
			Assert.AreEqual("base", i.Base());
			Assert.AreEqual("derived", i.Derived());

            i = Connection().AsParallel<IDerivedSql>();
			Assert.IsNotNull(i);
			Assert.AreEqual("base", i.Base());
			Assert.AreEqual("derived", i.Derived());
		}
		#endregion

		[Test]
		public void TestMergeOutputAttribute()
		{
			var data = new TestData();
			var i = Connection().As<IMergeOutputs>();
			i.DoAMerge(data);

			Assert.AreEqual(4, data.X);
			Assert.AreEqual(6, data.Z);
		}

		#region TVP Test
		public interface IHaveTVPWithSQL
		{
			[Sql("SELECT * FROM dbo.ReflectTableFunction(@p)")]
			IEnumerable<Beer> ReflectBeer(IEnumerable<Beer> p);

			[Sql("SELECT * FROM dbo.ReflectTableFunction2(@p, @q)")]
			IEnumerable<Beer> ReflectBeer2(IEnumerable<Beer> p, int q);
		}

		[Test]
		public void CanPassTVPToFunctionInSQLText()
		{
			var beer = new List<Beer>()
			{
				new Beer()
			};

			var i = Connection().As<IHaveTVPWithSQL>();
			var result = i.ReflectBeer(beer);
			Assert.AreEqual(1, result.Count());

			result = i.ReflectBeer2(beer, 2);
			Assert.AreEqual(1, result.Count());
		}
		#endregion

		#region Friendly Error Message Test
		interface IAmAPrivateInterface
		{
		}

		[Test]
		public void PrivateInterfaceThrowsFriendlierError()
		{
			Assert.Throws<InvalidOperationException>(() => Connection().As<IAmAPrivateInterface>());
		}
		#endregion

		#region DynamicAssembly Test
        [Test]
        public void InterfacesShouldBeInSameAssembly()
        {
			// we reuse the assemblyname for our dynamic interfaces
			// some proxy systems (e.g. castle.dynamicproxy), have a problem if two dynamic assemblies share the same name
			// so here we make sure that the two interfaces come from the same assembly

            var i1 = Connection().As<ITest1>();
            var i2 = Connection().As<ITestInsertUpdate>();

			Assert.AreEqual(i1.GetType().GetTypeInfo().Assembly, i2.GetType().GetTypeInfo().Assembly);
        }
		#endregion

		#region Ambiguous Match Test
		public interface IHaveAmbiguousMatch
		{
			// this method had a conflict with the create method that converts a connection to an interface
			void Create(int installation);
		}

		[Test]
		public void TestInterfaceWithMethodNamedCreate()
		{
			var c = Connection().As<IHaveAmbiguousMatch>();
		}
		#endregion
	}

	#region Interface Update Tests
	[TestFixture]
	public class InterfaceUpdateTests : BaseTest
	{
		public interface IEmailRepository
		{
			[Sql("print ''")]
			void UpsertByInt(int id);

			[Sql("print ''")]
			void UpsertByString(string id);

			[Sql("print ''")]
			void UpsertByT<T>(T id);
		}

		[Test]
		public void UpsertShouldNotFailWhenFirstParameterIsAtomic()
		{
			var repo = Connection().As<IEmailRepository>();
			repo.UpsertByInt(0);
			repo.UpsertByString("");
			repo.UpsertByT<int>(0);
			repo.UpsertByT<string>("");
		}
	}
	#endregion

	#region Multi-Threaded Interface Tests
	interface IMultiThreaded
	{
		[Sql("SELECT ParentX=@p")]
		Task<ParentTestData> FooAsync(int p);
	}

	[TestFixture]
	public class MultiThreadedInterfaceTests : BaseTest
	{
		[Test]
		public void CanCallInterfaceMultiThreaded()
		{
			var foo = Connection().AsParallel<IMultiThreaded>();

			var tasks = new List<Task>();
			for (int i = 0; i < 100; i++)
				tasks.Add (foo.FooAsync(i));

			Task.WaitAll(tasks.ToArray());
		}

		[Test]
		public void CanCallWrappedConnectionMultiThreaded()
		{
			var reliable = new ReliableConnection((DbConnection)Connection());
			var foo = reliable.AsParallel<IMultiThreaded>();

			var tasks = new List<Task>();
			for (int i = 0; i < 100; i++)
				tasks.Add(foo.FooAsync(i));

			Task.WaitAll(tasks.ToArray());
		}
	}
	#endregion

	#region Abstract Class Implementation Tests
	public abstract class AbstractClass
	{
		[Sql("SELECT 'abstract'")]
		public abstract string AbstractMethod();
	}

	public abstract class DerivedAbstractClass : AbstractClass
	{
		[Sql("SELECT 'derived'")]
		public abstract string DerivedAbstractMethod();
	}

	public abstract class AbstractClassWithProtectedGetConnection
	{
		protected abstract IDbConnection GetConnection();

		public void Test()
		{
			Assert.IsNotNull(GetConnection());
		}
	}

	public abstract class AbstractClassWithPublicGetConnection
	{
		public abstract IDbConnection GetConnection();
	}

	public abstract class AbstractClassOfDbConnectionWrapper : DbConnectionWrapper
	{
		public abstract IDbConnection GetConnection();

		protected AbstractClassOfDbConnectionWrapper(IDbConnection connection)
			: base(connection)
		{
		}

		public void Test()
		{
			Assert.AreNotEqual(this, InnerConnection);
			Assert.IsNotNull(InnerConnection);
			Assert.AreEqual(this, GetConnection());
		}
	}

	public abstract class AbstractClassWithProtectedMethod
	{
		public string PublicMethod()
		{ return AbstractMethod(); }

		[Sql("SELECT 'abstract'")]
		protected abstract string AbstractMethod();
	}

    public abstract class AbstractClassWithDefaultConstructor
    {
        public bool DefaultConstructorCalled { get; set; }

        protected AbstractClassWithDefaultConstructor() { DefaultConstructorCalled = true; }

        [Sql("SELECT 'abstract'")]
        public abstract string AbstractMethod();
    }

    public abstract class AbstractClassWithNonDefaultConstructor
    {
        protected AbstractClassWithNonDefaultConstructor(int x) {}

        [Sql("SELECT 'abstract'")]
        public abstract string AbstractMethod();
    }

	[TestFixture]
	public class AbstractClassTests : BaseTest
	{
		[Test]
		public void TestAbstractClass()
		{
			// make sure that we can create an interface
            var i = Connection().As<AbstractClass>();
			Assert.IsNotNull(i);
			Assert.AreEqual("abstract", i.AbstractMethod());
            i = Connection().AsParallel<AbstractClass>();
			Assert.IsNotNull(i);
			Assert.AreEqual("abstract", i.AbstractMethod());

            var derived = Connection().As<DerivedAbstractClass>();
			Assert.IsNotNull(derived);
			Assert.AreEqual("abstract", derived.AbstractMethod());
			Assert.AreEqual("derived", derived.DerivedAbstractMethod());
            derived = Connection().AsParallel<DerivedAbstractClass>();
			Assert.IsNotNull(derived);
			Assert.AreEqual("abstract", derived.AbstractMethod());
			Assert.AreEqual("derived", derived.DerivedAbstractMethod());
		}

		[Test]
		public void ProtectedGetConnectionIsImplemented()
		{
			var connection = Connection();

			// make sure that we can create an interface
			var i = connection.As<AbstractClassWithProtectedGetConnection>();
			i.Test();
		}

		[Test]
		public void PublicGetConnectionIsImplemented()
		{
			var connection = Connection();

			// make sure that we can create an interface
			var i = connection.As<AbstractClassWithPublicGetConnection>();
			Assert.IsNotNull(i.GetConnection());
		}

		[Test]
		public void DbConnectionWrapperIsImplemented()
		{
			var connection = Connection();

			// make sure that we can create an interface
			var i = connection.As<AbstractClassOfDbConnectionWrapper>();
			i.Test();
		}

		[Test]
		public void DbConnectionWrapperCannotBeInvokedAsParallel()
		{
			var connection = Connection();

			Assert.Throws<InvalidOperationException>(() => connection.AsParallel<AbstractClassOfDbConnectionWrapper>());
		}

		[Test]
		public void ProtectedAbstractMethodIsImplemented()
		{
			var connection = Connection();

			var i = connection.As<AbstractClassWithProtectedMethod>();
			Assert.AreEqual("abstract", i.PublicMethod());
		}

		[Test]
		public void BaseClassDefaultConstructorIsInvoked()
		{
			var connection = Connection();

            var i = connection.As<AbstractClassWithDefaultConstructor>();
			Assert.IsTrue(i.DefaultConstructorCalled);

            var i2 = connection.AsParallel<AbstractClassWithDefaultConstructor>();
			Assert.IsTrue(i2.DefaultConstructorCalled);
		}

		[Test]
		public void WhenBaseClassHasNoValidConstructorsThrows()
		{
			var connection = Connection();

			Assert.Throws<InvalidOperationException>(() => connection.As<AbstractClassWithNonDefaultConstructor>());
			Assert.Throws<InvalidOperationException>(() => connection.AsParallel<AbstractClassWithNonDefaultConstructor>());
		}
	}
	#endregion

	public interface IDontReturnAScalar
	{
		[Sql("PRINT 1")]
		int Foo();
	}

	public class NullableReturnTests : BaseTest
	{
		[Test]
		public void NonNullableReturnShouldThrowWhenNoRowsAreReturned()
		{
			var i = Connection().As<IDontReturnAScalar>();
			Assert.Throws<InvalidOperationException>(() => i.Foo());
		}
	}

	[TestFixture]
	public class TestBitReturn : BaseTest
	{
		public interface Bit
		{
			[Sql("SELECT CONVERT(bit, 1)")]
			Task<bool> GetBit();
		}

		[Test]
		public void BitAsync()
		{
			var i = Connection().As<Bit>();

			var b = i.GetBit().Result;

			Assert.AreEqual(true, b);
		}
	}

    #region Interface Transaction Tests
    [TestFixture]
    public class InterfaceTransactionsTest : BaseTest
    {
        public interface I1
        {
            [Sql("CREATE TABLE #tmpFoo(id int); INSERT INTO #tmpFoo VALUES(1)")]
            void Do();
        }

        public interface I2
        {
            [Sql("SELECT TOP 1 * FROM #tmpFoo")]
            int Do();
        }

        [Test]
        public void InterfacesCanShareTransactions()
        {
            using (var c = Connection().OpenWithTransaction())
            {
                c.As<I1>().Do();
                int i = c.As<I2>().Do();
                Assert.AreEqual(1, i);
            }
        }

		[Test]
        public void InterfacesCanUseExternalTransactions_Issue370()
        {
			using (var c = Connection().OpenConnection())
			using (var sqlTransaction = c.BeginTransaction())
            {
                c.UsingTransaction(sqlTransaction).As<I1>().Do();
                int i = c.UsingTransaction(sqlTransaction).As<I2>().Do();
                Assert.AreEqual(1, i);
            }
        }
    }
    #endregion  

    #region Parameter Tests
    [TestFixture]
    public class ParameterWithAttributeTest : BaseTest
    {
        public interface IHaveColumnAttribute
        {
            [Sql("SELECT @p")]
            int RenameParameter([Column("p")]int q);
        }

        [Test]
        public void ParameterNameCanBeOverridden()
        {
            var p = Connection().As<IHaveColumnAttribute>().RenameParameter(5);
            Assert.AreEqual(5, p);
        }
    }
	#endregion

	#region Multi-Level Child Support
	[TestFixture]
	public class TestMultiLevelChildren : BaseTest
	{
		public class Root
		{
			public int ID;
			public List<Level2> List;
		}

		public class Level2
		{
			public int ID;
			public List<Level3> List;
		}

		public class Level3
		{
			public int ID;
		}

		internal interface IHaveGrandchildren
		{
			[Sql("SELECT ID=1; SELECT ParentID=1, ID=2 UNION SELECT ParentID=1, ID=3; SELECT ParentID=2, ID=4")]
			[Recordset(0, typeof(Root))]
			[Recordset(1, typeof(Level2), IsChild = true, Id = "ID")]
			[Recordset(2, typeof(Level3), IsChild = true, Id = "ID", Parents="List")]
			Root GetMultiLevelChildren();			
		}
		
		[Test]
		public void CanGetMultiLevelChildren()
		{
			var i = Connection().As<IHaveGrandchildren>();
			var result = i.GetMultiLevelChildren();

			var root = result;
			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ID);
			Assert.AreEqual(2, root.List.Count);
			Assert.AreEqual(2, root.List[0].ID);
			Assert.AreEqual(3, root.List[1].ID);

			var level2 = root.List[0];
			Assert.AreEqual(2, level2.ID);
			Assert.AreEqual(1, level2.List.Count);
			Assert.AreEqual(4, level2.List[0].ID);
		}
	}
	#endregion
}
