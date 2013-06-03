using System.Data;
using System.Data.SqlClient;
using Insight.Database;
using Insight.Tests.TestDataClasses;
using Microsoft.SqlServer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests
{
	[TestFixture]
	public class MappingTests : BaseDbTest
	{
		[TearDown]
		public override void TearDown()
		{
			ColumnMapping.Tables.ResetHandlers();

			base.TearDown();
		}

		#region Table Tests
		[Test]
		public void RegexReplaceShouldAlterColumnName()
		{
			ColumnMapping.Tables.ReplaceRegex("_", String.Empty);

			var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = _connection.QuerySql<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>));
			ParentTestData.Verify(results);
		}

		[Test]
		public void PrefixRemoveShouldAlterColumnName()
		{
			ColumnMapping.Tables.RemovePrefixes("int");

			var sql = ParentTestData.Sql.Replace("ParentX", "intParentX");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = _connection.QuerySql<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>));
			ParentTestData.Verify(results);
		}

		[Test]
		public void SuffixRemoveShouldAlterColumnName()
		{
			ColumnMapping.Tables.RemoveSuffixes("int");

			var sql = ParentTestData.Sql.Replace("ParentX", "ParentXint");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = _connection.QuerySql<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>));
			ParentTestData.Verify(results);
		}

		[Test]
		public void ReplaceCanBeChained()
		{
			ColumnMapping.Tables.RemovePrefixes("int").RemoveSuffixes("Foo").RemoveStrings("_");

			var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X_Foo");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = _connection.QuerySql<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>));
			ParentTestData.Verify(results);
		}
		#endregion
	}


	/// <summary>
	/// Tests dynamic connection.
	/// </summary>
	[TestFixture]
	public class MappingProcTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("CREATE TYPE [InsightTestDataTable] AS TABLE ([IntParentX] [int], [IntX][int])");
			_connection.ExecuteSql("CREATE PROCEDURE [TestProc] @p [InsightTestDataTable] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql("CREATE TABLE [InsightTestDataTable2] ([IntParentX] [int], [IntX][int])");
			_connection.ExecuteSql("CREATE PROCEDURE [TestProc2] @intParentX [int] AS SELECT @intParentX");
			_connection.ExecuteSql("CREATE PROCEDURE [TestProc3] @geo [geography] AS SELECT @geo");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			ColumnMapping.Tables.ResetHandlers();

			CleanupObjects();

			base.TearDownFixture();
		}

		private void CleanupObjects()
		{
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'TestProc3') DROP PROCEDURE [TestProc3]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'TestProc2') DROP PROCEDURE [TestProc2]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'TestProc') DROP PROCEDURE [TestProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataTable') DROP TYPE [InsightTestDataTable]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataTable2') DROP TABLE [InsightTestDataTable2]");
		}
		#endregion

		#region Table Valued Parameter Tests
		[Test]
		public void MappingsAreAppliedToTableValuedParameters()
		{
			// get a stanard set of objects from the server
			var original = _connection.QuerySql<ParentTestData>(ParentTestData.Sql);
			ParentTestData.Verify(original, false);

			ColumnMapping.Tables.RemovePrefixes("int");

			// send the object up to the server and get them back
			var results = _connection.Query<ParentTestData>("TestProc", original);
			ParentTestData.Verify(results, false);
		}
		#endregion

		#region BulkCopy Tests
		[Test]
		public void MappingsAreAppliedToBulkCopy()
		{
			ColumnMapping.Tables.RemovePrefixes("int");

			for (int i = 0; i < 3; i++)
			{
				// build test data
				ParentTestData[] array = new ParentTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new ParentTestData() { ParentX = j };

				// bulk load the data
				_sqlConnection.BulkCopy("InsightTestDataTable2", array);

				// run the query
				var items = _connection.QuerySql<ParentTestData>("SELECT * FROM InsightTestDataTable2");
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j, items[j].ParentX);

				_connection.ExecuteSql("DELETE FROM InsightTestDataTable2");
			}
		}
		#endregion

		[Test]
		public void MappingsAreAppliedToParameters()
		{
			ColumnMapping.Parameters.RemovePrefixes("int");

			var parentTestData = new ParentTestData() { ParentX = 5 };

			var results = _connection.Query<int>("TestProc2", parentTestData);
			int data = results.First();

			Assert.AreEqual(parentTestData.ParentX, data);
		}

		[Test]
		public void GeographyParametersArePassedCorrectly()
		{
			var point = SqlGeography.Point(0, 0, 4326);

			// The code below works, and demonstrates how
			// geography parameters would have been passed manually
//			using (var command = _connection.CreateCommand())
//			{
//				command.CommandType = CommandType.StoredProcedure;
//				command.CommandText = "TestProc3";
//				command.Parameters.Add(
//					new SqlParameter("@geo", point)
//					{
//						SqlDbType = SqlDbType.Udt,
//						UdtTypeName = "sys.geography"
//					}
//				);
//				var r1 = (SqlGeography)command.ExecuteScalar();
//				Assert.That(r1.STEquals(point).IsTrue);
//			}

			var results = _connection.Query<SqlGeography>("TestProc3", new { geo = point });
			Assert.That(results[0].STEquals(point).IsTrue);
		}
	}
}
