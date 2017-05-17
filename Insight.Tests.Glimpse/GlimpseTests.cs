using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Glimpse.Ado.AlternateType;
using Insight.Database;
using Insight.Database.Providers.Glimpse;
using NUnit.Framework;
using System.Data;

namespace Insight.Tests
{
	/// <summary>
	/// Tests that the mini-profiler connection wrapper does not interfere with SQL parameter detection.
	/// </summary>
	[TestFixture]
	public class GlimpseTests : BaseTest
	{
		[OneTimeSetUp]
		public static void OneTimeSetup()
		{
			TestSetup.CreateTestDatabase();
		}

		/// <summary>
		/// Make sure that we can connect to the database
		/// </summary>
		[Test]
		public void TestProfiledSqlQuery()
		{
			var profiled = new GlimpseDbConnection((DbConnection)Connection());
			var result = profiled.QuerySql<int>("SELECT @p --GLIMPSE", new { p = 1 }).First();

			Assert.AreEqual((int)1, result);
		}

		/// <summary>
		/// Make sure that a profiled connection still can auto-detect the parameters.
		/// </summary>
		[Test]
		public void TestProfiledStoredProcWithParameters()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProcGlimpse (@Value int = 5) AS SELECT Value=@Value");

				var profiled = new GlimpseDbConnection((DbConnection)connection);
				var result = profiled.Query<int>("InsightTestProcGlimpse", new { Value = 1 }).First();

				Assert.AreEqual((int)1, result);
			}
		}

        #region Tests for Issue #158
        public interface ICanBulkCopy : IDbConnection, IDbTransaction
        {
            [Sql("SELECT X=1")]
            int MyProc();
        }

        public class InsightTestData
        {
            public int Int;
            public int? IntNull { get; set; }
        }

        [Test]
        public void TestBulkCopyWithInterface()
        {
            const int ItemCount = 10;

            // build test data
            InsightTestData[] array = new InsightTestData[ItemCount];
            for (int j = 0; j < ItemCount; j++)
                array[j] = new InsightTestData() { Int = j };

            var profiled = new GlimpseDbConnection((DbConnection)Connection());

            using (var i = profiled.OpenWithTransactionAs<ICanBulkCopy>())
            {
                i.BulkCopy("BulkCopyData", array);
            }
        }
        #endregion
    }
}
