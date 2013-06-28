using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;
using Insight.Database.Providers;
using NUnit.Framework;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace Insight.Tests
{
	/// <summary>
	/// Tests that the mini-profiler connection wrapper does not interfere with SQL parameter detection.
	/// </summary>
	[TestFixture]
	class MiniProfilerTests : BaseDbTest
	{
		public override void SetUpFixture()
		{
			MiniProfilerInsightDbProvider.RegisterProvider();

			base.SetUpFixture();
		}

		[Test]
		public void TestProfiledSqlQuery()
		{
			var profiled = new ProfiledDbConnection(_connection, MiniProfiler.Current);
			var result = profiled.QuerySql<int>("SELECT @p", new { p = 1 }).First();

			Assert.AreEqual((int)1, result);
		}

		/// <summary>
		/// Make sure that a profiled connection still can auto-detect the parameters.
		/// </summary>
		[Test]
		public void TestProfiledStoredProcWithParameters()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value");

				var profiled = new ProfiledDbConnection(connection, MiniProfiler.Current);
				var result = profiled.Query<int>("InsightTestProc", new { Value = 1 }).First();

				Assert.AreEqual((int)1, result);
			}
		}
	}
}
