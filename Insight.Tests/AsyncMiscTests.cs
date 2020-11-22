using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Transactions;
using Insight.Database;
using Insight.Tests.Cases;

#pragma warning disable 0649

namespace Insight.Tests
{
	/// <summary>
	/// Tests asynchronous SQL features. Requires a local database with a trusted connection.
	/// </summary>
	[TestFixture]
	public class AsyncMiscTests : BaseTest
	{
		#region Tests
		/// <summary>
		/// Asynchronously run a stored procedure with parameter detection
		/// </summary>
		[Test]
		public void TestAsyncQueryDynamic()
		{
			var results = Connection().QueryAsync("SELECT X=1", Parameters.Empty, CommandType.Text).Result;

			Assert.AreEqual(1, results.Count);
			dynamic d = results[0];
			Assert.AreEqual(1, d["X"]);
		}

		/// <summary>
		/// Asynchronously run a command that returns a data reader
		/// </summary>
		[Test]
		public void TestAsyncReader()
		{
			var connection = Connection();

			var task = connection.QuerySqlAsync(
				"SELECT 1",
				Parameters.Empty,
				reader =>
				{
					using (reader)
					{
						reader.Read();
						Assert.AreEqual(1, reader.GetInt32(0));
					}
				},
				commandBehavior: CommandBehavior.CloseConnection);

			task.Wait();

			Assert.IsTrue(connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Make sure that we can chain sql tasks
		/// </summary>
		[Test]
		public void TestAsyncReaderContinuation()
		{
			var connection = Connection();

			int result = 0;

			var task = connection.QuerySqlAsync(
				"SELECT 1",
				Parameters.Empty,
				reader =>
				{
					using (reader)
					{
						reader.Read();
						result = reader.GetInt32(0);
					}
				},
				CommandBehavior.CloseConnection);

			task.Wait();

			Assert.AreEqual(1, result);
			Assert.IsTrue(connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Make sure that we can chain sql tasks
		/// </summary>
		[Test]
		public void TestSqlError()
		{
			var connection = Connection();
			int result = 0;

			var task = connection.QuerySqlAsync(
				"THIS IS INVALID SQL",
				Parameters.Empty,
				reader =>
				{
					using (reader)
					{
						reader.Read();
						result = reader.GetInt32(0);
					}
				},
				commandBehavior: CommandBehavior.CloseConnection);

			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.IsTrue(connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Asynchronously run a command that is not a query
		/// </summary>
		[Test]
		public void TestNonQuerySqlError()
		{
			var connection = Connection();
			var task = connection.ExecuteSqlAsync("THIS IS INVALID SQL", Parameters.Empty, closeConnection: true);

			Assert.Throws<AggregateException>(() => task.Wait());
			Assert.IsTrue(connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Run a multiple recordset expando query asynchronously.
		/// </summary>
		[Test]
		public void TestAsyncReaderMultiRecordsetWithExpandos()
		{
			var connection = Connection();
			IList<FastExpando> foo = null;
			IList<FastExpando> goo = null;

			var task = connection.QuerySqlAsync(
				"SELECT Foo=1 SELECT Goo='Text'",
				Parameters.Empty,
				reader =>
				{
					// note that ToList will automatically advance to the next recordset
					foo = reader.ToList();
					goo = reader.ToList();
				},
				commandBehavior: CommandBehavior.CloseConnection);

			task.Wait();

			Assert.IsTrue(connection.State == ConnectionState.Closed);
			Assert.IsNotNull(foo);
			Assert.AreEqual(1, foo[0]["Foo"]);
			Assert.IsNotNull(goo);
			Assert.AreEqual("Text", goo[0]["Goo"]);
		}
		#endregion

		#region Derived Recordsets Tests
		[Test]
		public void DerivedRecordsetsCanBeReturned()
		{
			var results = Connection().QueryResultsSqlAsync<PageData<ParentTestData>>(ParentTestData.Sql + "SELECT TotalCount=70").Result;

			Assert.IsNotNull(results);
			ParentTestData.Verify(results.Set1, withGraph: false);

			Assert.IsNotNull(results.Set2);
			Assert.AreEqual(1, results.Set2.Count);
			Assert.AreEqual(70, results.TotalCount);
		}
		#endregion

#if !NO_AMBIENT_TRANSACTIONS
		#region Issue 445
		[TestFixture]
		public class Issue445Tests : BaseTest
		{
			public interface IHaveMethods
			{
				[Sql("SELECT 1")]
				public Task<int> SelectOne();

				[Sql("SELECT 2")]
				public Task<int> SelectTwo();
			}

			[Test]
			public void AmbientTransactionShouldTransferAcrossThreads()
			{
				RunAsync().Wait();		
			}

			private async Task RunAsync()
			{
				var i = Connection().As<IHaveMethods>();

				var options = new TransactionOptions {
					IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
					Timeout = TransactionManager.DefaultTimeout,
					
				};

				using (var ts = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
				{
					await i.SelectOne();
					await i.SelectTwo();
					ts.Complete();
				}	
			}
		}
		#endregion
#endif
	}
}
