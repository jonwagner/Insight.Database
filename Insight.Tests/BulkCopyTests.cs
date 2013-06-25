using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data.SqlClient;

namespace Insight.Tests
{
	[TestFixture]
	class BulkCopyTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			// clean up old stuff first
			CleanupObjects();

			base.SetUpFixture();

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
				_sqlConnection.BulkCopy("InsightTestData", array);

				// run the query
				var items = _connection.QuerySql<InsightTestData>("SELECT * FROM InsightTestData");
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j, items[j].Int);

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
			_sqlConnection.BulkCopy("InsightTestData", array, configure: (SqlBulkCopy bulkCopy) =>
			{
				bulkCopy.NotifyAfter = 1;
				bulkCopy.SqlRowsCopied += (sender, args) => totalRows = args.RowsCopied;
			});

			// run the query
			var items = _connection.QuerySql<InsightTestData>("SELECT * FROM InsightTestData");
			Assert.IsNotNull(items);
			Assert.AreEqual(ItemCount, items.Count);
			Assert.AreEqual(ItemCount, totalRows);
			for (int j = 0; j < ItemCount; j++)
				Assert.AreEqual(j, items[j].Int);

			_connection.ExecuteSql("DELETE FROM InsightTestData");
		}
	}
}
