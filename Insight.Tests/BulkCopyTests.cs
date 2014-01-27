using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data.SqlClient;
using System.Data;

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

		private void Cleanup()
		{
			Connection().ExecuteSql("DELETE FROM [BulkCopyData]");
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

		private void VerifyRecordsInserted(IDbConnection connection, int count, IDbTransaction transaction = null)
		{
			// run the query
			var items = connection.QuerySql<InsightTestData>("SELECT * FROM BulkCopyData ORDER BY Int", transaction: transaction);
			Assert.IsNotNull(items);
			Assert.AreEqual(count, items.Count);
			for (int j = 0; j < count; j++)
				Assert.AreEqual(j, items[j].Int);
		}
	}
}
