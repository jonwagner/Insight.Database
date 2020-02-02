using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using NUnit.Framework;
using Insight.Database;
using System.Threading.Tasks;

#if !NO_SQL_TYPES
using Microsoft.SqlServer.Types;
#endif

namespace Insight.Tests
{
	[TestFixture]
	class BulkCopyTests : BaseTest
	{
		#region SetUp and TearDown
		[SetUp]
		public void SetUp()
		{
			Cleanup();
		}

		private void Cleanup(string tableName = "BulkCopyData")
		{
			Connection().ExecuteSql(String.Format("DELETE FROM [{0}]", tableName));
		}
		#endregion

		#region Helper Class
		class InsightTestData
		{
			public int Int;
			public int? IntNull { get; set; }
		}
		#endregion

		[Test]
		public void TestBulkLoad()
		{
			var connection = Connection();

			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// bulk load the data
				Cleanup();
				connection.BulkCopy("BulkCopyData", array);

				VerifyRecordsInserted(connection, i);
			}
		}

		[Test]
		public void TestBulkLoadAsync()
		{
			var connection = Connection();

			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// bulk load the data
				Cleanup();
				connection.BulkCopyAsync("BulkCopyData", array).Wait();

				VerifyRecordsInserted(connection, i);
			}
		}

		[Test]
		public void TestBulkLoadAsyncCancel()
		{
			var connection = Connection();
			var cancellationToken = new System.Threading.CancellationTokenSource();

			// cancel the token
			cancellationToken.Cancel();

			// try bulk load the data
			Cleanup();
			Assert.CatchAsync<OperationCanceledException>(
				async () => await 
				connection.BulkCopyAsync(
					"BulkCopyData",
					new InsightTestData[10],
					cancellationToken: cancellationToken.Token)
				);
		}

#if !NO_SQL_TYPES
		#region SqlTypes
		class InsightTestDataWithSqlTypes : InsightTestData
		{
			public Microsoft.SqlServer.Types.SqlGeometry Geometry;
		}

		[Test]
		public void TestBulkLoadSqlTypes()
		{
			var connection = Connection();

			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestDataWithSqlTypes[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestDataWithSqlTypes() { Int = j, Geometry = new Microsoft.SqlServer.Types.SqlGeometry() };

				// bulk load the data
				Cleanup("BulkCopyWithUdt");
				connection.BulkCopy("BulkCopyWithUdt", array);

				VerifyRecordsInserted(connection, array.Length, tableName: "BulkCopyWithUdt");
			}
		}
		#endregion
#endif

		[Test]
        public void TestBulkLoadCount()
        {
			var connection = Connection();
			
			const int RowCount = 3;
            for (int i = 0; i < RowCount; i++)
            {
                // build test data
                InsightTestData[] array = new InsightTestData[i];
                for (int j = 0; j < i; j++)
                    array[j] = new InsightTestData() { Int = j };

                // bulk load the data
				Cleanup();
				var count = connection.BulkCopy("BulkCopyData", array);

                Assert.AreEqual(i, count);
            }
        }

		[Test]
		public void TestBulkLoadWithConfiguration()
		{
			var connection = Connection();

			const int ItemCount = 10;

			// build test data
			InsightTestData[] array = new InsightTestData[ItemCount];
			for (int j = 0; j < ItemCount; j++)
				array[j] = new InsightTestData() { Int = j };

			// bulk load the data
			long totalRows = 0;
			connection.BulkCopy("BulkCopyData", array, configure: (InsightBulkCopy bulkCopy) =>
			{
				bulkCopy.NotifyAfter = 1;
				bulkCopy.RowsCopied += (sender, args) => totalRows = args.RowsCopied;
			});

			VerifyRecordsInserted(connection, ItemCount);
			Assert.AreEqual(ItemCount, totalRows);
		}

		[Test]
		public void TestBulkLoadWithAutoTransaction()
		{
			for (int i = 0; i < 3; i++)
			{
				using (var connection = ConnectionWithTransaction())
				{
					// build test data
					InsightTestData[] array = new InsightTestData[i];
					for (int j = 0; j < i; j++)
						array[j] = new InsightTestData() { Int = j };

					// bulk load the data
					Cleanup();
					connection.BulkCopy("BulkCopyData", array);

					// records should be there
					VerifyRecordsInserted(connection, i);
				}

				// records should be gone
				VerifyRecordsInserted(Connection(), 0);
			}
		}

		[Test]
		public void TestBulkLoadWithSqlTransaction()
		{
			for (int i = 0; i < 3; i++)
			{
				using (var connection = Connection())
				{
					connection.Open();

					using (var tx = connection.BeginTransaction())
					{
						// build test data
						InsightTestData[] array = new InsightTestData[i];
						for (int j = 0; j < i; j++)
							array[j] = new InsightTestData() { Int = j };

						// bulk load the data
						Cleanup();
						connection.BulkCopy("BulkCopyData", array, transaction: tx);

						// records should be there
						VerifyRecordsInserted(connection, i, tx);
					}

					// records should be gone
					VerifyRecordsInserted(connection, 0);
				}
			}
		}

		private void VerifyRecordsInserted(IDbConnection connection, int count, IDbTransaction transaction = null, string tableName = "BulkCopyData")
		{
			// run the query
			var items = connection.QuerySql<InsightTestData>(String.Format("SELECT * FROM {0} ORDER BY Int", tableName), transaction: transaction);
			Assert.IsNotNull(items);
			Assert.AreEqual(count, items.Count);
			for (int j = 0; j < count; j++)
				Assert.AreEqual(j, items[j].Int);
		}

		class MerchNameTermTransaction
		{
			public int Id { get; set; }
			public int TranId { get; set; }
			public int TermId { get; set; }
		}

		[Test]
		public void TestIssue103()
		{
			var m = new MerchNameTermTransaction()
			{
				TranId = 2,
				TermId = 3
			};

			using (var connection = ConnectionWithTransaction())
			{
				connection.BulkCopy("MerchNameTermsTransactions", new List<MerchNameTermTransaction>() { m });

				var items = connection.QuerySql<MerchNameTermTransaction>("SELECT * FROM MerchNameTermsTransactions");
				Assert.AreEqual(1, items.Count);
				Assert.AreNotEqual(0, items[0].Id);
				Assert.AreEqual(2, items[0].TranId);
				Assert.AreEqual(3, items[0].TermId);
			}
		}

		[Test]
		public void TestIssue103WithKeepIdentity()
		{
			var m = new MerchNameTermTransaction()
			{
				Id = 99,
				TranId = 2,
				TermId = 3
			};

			using (var connection = ConnectionWithTransaction())
			{
				connection.BulkCopy("MerchNameTermsTransactions", new List<MerchNameTermTransaction>() { m }, options: InsightBulkCopyOptions.KeepIdentity);

				var items = connection.QuerySql<MerchNameTermTransaction>("SELECT * FROM MerchNameTermsTransactions");
				Assert.AreEqual(1, items.Count);
				Assert.AreEqual(99, items[0].Id);
				Assert.AreEqual(2, items[0].TranId);
				Assert.AreEqual(3, items[0].TermId);
			}
		}

        public interface ICanBulkCopy : IDbConnection, IDbTransaction
        {
            [Sql("SELECT X=1")]
            int MyProc();
        }

        [Test]
        public void TestBulkCopyWithInterface()
        {
			const int ItemCount = 10;

			// build test data
			InsightTestData[] array = new InsightTestData[ItemCount];
			for (int j = 0; j < ItemCount; j++)
				array[j] = new InsightTestData() { Int = j };

            using (var i = Connection().OpenWithTransactionAs<ICanBulkCopy>())
            {
                i.BulkCopy("BulkCopyData", array);
            }
        }

		#region BulkCopyWithColumnMapping
		class InsightTestDataWithColumnMapping
		{
			[Column("Int")]
			public int Foo;
			[Column("IntNull")]
			public int? FooNull { get; set; }
		}

		[Test]
		public void TestBulkWithColumnMapping()
		{
			var connection = Connection();

			// build test data
			var data = new InsightTestDataWithColumnMapping() { Foo = 1 };
			var array = new InsightTestDataWithColumnMapping[1] { data };

			// bulk load the data
			Cleanup();
			var count = connection.BulkCopy("BulkCopyData", array);

			Assert.AreEqual(1, count);
			var result = connection.QuerySql<InsightTestDataWithColumnMapping>("SELECT * FROM BulkCopyData").First();
			Assert.AreEqual(1, result.Foo);
		}
		#endregion

		#region Tests for Issue 162
		public class BulkCopyDataWithComputedColumn
        {
            public int Int1;
            public int Computed;
            public int Int2;
        }

        [Test]
        public void TestBulkCopyWithComputedColumn()
        {
            var array = new BulkCopyDataWithComputedColumn[1]
            {
                new BulkCopyDataWithComputedColumn()
                {
                    Int1 = 1,
                    Computed = 0,
                    Int2 = 3
                }
            };

            using (var i = Connection().OpenWithTransaction())
            {
                i.BulkCopy("BulkCopyWithComputedData", array);

                var inserted = i.QuerySql<BulkCopyDataWithComputedColumn>("SELECT * FROM BulkCopyWithComputedData").First();
                Assert.AreEqual(array[0].Int1, inserted.Int1);
                Assert.AreEqual(array[0].Int2, inserted.Int2);
                Assert.AreEqual(array[0].Int1 + 1, inserted.Computed);
            }
        }
#endregion
    }
}
