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
	class BulkCopyTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestData') DROP TABLE [InsightTestData]");
			_connection.ExecuteSql("CREATE TABLE [InsightTestData] ([Int] [int])");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			CleanupObjects();

			base.TearDownFixture();
		}

		private void CleanupObjects()
		{
			try
			{
				_connection.ExecuteSql("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestData') DROP TABLE [InsightTestData]");
			}
			catch { }
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
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData() { Int = j };

				// bulk load the data
				_connection.BulkCopy("InsightTestData", array);

				VerifyRecordsInserted(_connection, i);

				_connection.ExecuteSql("DELETE FROM InsightTestData");
			}
		}

		[Test]
		public void TestBulkLoadWithConfiguration()
		{
			const int ItemCount = 10;

			// build test data
			InsightTestData[] array = new InsightTestData[ItemCount];
			for (int j = 0; j < ItemCount; j++)
				array[j] = new InsightTestData() { Int = j };

			// bulk load the data
			long totalRows = 0;
			_connection.BulkCopy("InsightTestData", array, configure: (InsightBulkCopy bulkCopy) =>
			{
				bulkCopy.NotifyAfter = 1;
				bulkCopy.RowsCopied += (sender, args) => totalRows = args.RowsCopied;
			});

			VerifyRecordsInserted(_connection, ItemCount);
			Assert.AreEqual(ItemCount, totalRows);

			_connection.ExecuteSql("DELETE FROM InsightTestData");
		}

		[Test]
		public void TestBulkLoadWithAutoTransaction()
		{
			for (int i = 0; i < 3; i++)
			{
				using (var connection = _connectionStringBuilder.OpenWithTransaction())
				{
					// build test data
					InsightTestData[] array = new InsightTestData[i];
					for (int j = 0; j < i; j++)
						array[j] = new InsightTestData() { Int = j };

					// bulk load the data
					connection.BulkCopy("InsightTestData", array);

					// records should be there
					VerifyRecordsInserted(connection, i);
				}

				// records should be gone
				VerifyRecordsInserted(_connection, 0);

				_connection.ExecuteSql("DELETE FROM InsightTestData");
			}
		}

		[Test]
		public void TestBulkLoadWithSqlTransaction()
		{
			for (int i = 0; i < 3; i++)
			{
				using (var tx = _connection.BeginTransaction())
				{
					// build test data
					InsightTestData[] array = new InsightTestData[i];
					for (int j = 0; j < i; j++)
						array[j] = new InsightTestData() { Int = j };

					// bulk load the data
					_connection.BulkCopy("InsightTestData", array, transaction: tx);

					// records should be there
					VerifyRecordsInserted(_connection, i, tx);
				}

				// records should be gone
				VerifyRecordsInserted(_connection, 0);

				_connection.ExecuteSql("DELETE FROM InsightTestData");
			}
		}

		private void VerifyRecordsInserted(IDbConnection connection, int count, IDbTransaction transaction = null)
		{
			// run the query
			var items = connection.QuerySql<InsightTestData>("SELECT * FROM InsightTestData", transaction: transaction);
			Assert.IsNotNull(items);
			Assert.AreEqual(count, items.Count);
			for (int j = 0; j < count; j++)
				Assert.AreEqual(j, items[j].Int);
		}
	}
}
