using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;
using Insight.Database.Providers.MiniProfiler;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace Insight.Tests
{
	/// <summary>
	/// Tests that the mini-profiler connection wrapper does not interfere with SQL parameter detection.
	/// </summary>
	[TestFixture]
	class MiniProfilerTests : BaseTest
	{
		[OneTimeSetUp]
		public static void OneTimeSetup()
		{
			TestSetup.CreateTestDatabase();
		}

		[Test]
		public void TestProfiledSqlQuery()
		{
			var profiled = new ProfiledDbConnection((DbConnection)Connection(), MiniProfiler.Current);
			var result = profiled.QuerySql<int>("SELECT @p --MiniProfiler", new { p = 1 }).First();

			ClassicAssert.AreEqual((int)1, result);
		}

		/// <summary>
		/// Make sure that a profiled connection still can auto-detect the parameters.
		/// </summary>
		[Test]
		public void TestProfiledStoredProcWithParameters()
		{
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProcMiniProfiler (@Value int = 5) AS SELECT Value=@Value");

				var profiled = new ProfiledDbConnection((DbConnection)connection, MiniProfiler.Current);
				var result = profiled.Query<int>("InsightTestProcMiniProfiler", new { Value = 1 }).First();

				ClassicAssert.AreEqual((int)1, result);
			}
		}
	}
}
