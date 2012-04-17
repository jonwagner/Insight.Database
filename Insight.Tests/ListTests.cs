using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data;
using System.Dynamic;

namespace Insight.Tests
{
	[TestFixture]
	public class ListTests :BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture ();

			// clean up old stuff first
			CleanupObjects ();

			_connection.ExecuteSql ("CREATE TYPE [Int32Table] AS TABLE ([Value] [int])");
			_connection.ExecuteSql ("CREATE TYPE [InsightTestDataTable] AS TABLE ([Int] [int] NOT NULL, [IntNull][int])");
			_connection.ExecuteSql ("CREATE TYPE [InsightTestDataTable2] AS TABLE ([Int] [decimal] NOT NULL, [IntNull][decimal])");
			_connection.ExecuteSql ("CREATE PROCEDURE [Int32TestProc] @p [Int32Table] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql ("CREATE PROCEDURE [InsightTestDataTestProc] @p [InsightTestDataTable] READONLY AS SELECT * FROM @p");
			_connection.ExecuteSql ("CREATE PROCEDURE [InsightTestDataTestProc2] @p [InsightTestDataTable2] READONLY AS SELECT * FROM @p");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			CleanupObjects ();

			base.TearDownFixture ();
		}

		private void CleanupObjects()
		{
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'Int32TestProc') DROP PROCEDURE [Int32TestProc]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataTestProc') DROP PROCEDURE [InsightTestDataTestProc]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestDataTestProc2') DROP PROCEDURE [InsightTestDataTestProc2]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.types WHERE name = 'Int32Table') DROP TYPE [Int32Table]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataTable') DROP TYPE [InsightTestDataTable]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql ("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestDataTable2') DROP TYPE [InsightTestDataTable2]");
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

		#region List Tests
		/// <summary>
		/// Test support for enumerable parameters (a parameter is IEnumerable ValueType)
		/// </summary>
		[Test]
		public void TestEnumerableValueParameters ()
		{
			string sql = "SELECT p FROM (SELECT p=0 UNION SELECT 1 UNION SELECT 2) as v WHERE p IN @p";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range (0, i).ToArray ();
				var items = _connection.QuerySql (sql, new { p = array });
				Assert.IsNotNull (items);
				Assert.AreEqual (i, items.Count);
			}
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableClassArrayParameters ()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData () { Int = j };

				// run the query
				var items = _connection.QuerySql<InsightTestData> ("SELECT * FROM @p", new { p = array });
				Assert.IsNotNull (items);
				Assert.AreEqual (i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual (j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<InvalidOperationException> (() => _connection.QuerySql<InsightTestData> ("SELECT * FROM @p", new { p = new InsightTestData[] { new InsightTestData (), null, new InsightTestData () } }));
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableClassListParameters ()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				InsightTestData[] array = new InsightTestData[i];
				for (int j = 0; j < i; j++)
					array[j] = new InsightTestData () { Int = j };

				// run the query
				var items = _connection.QuerySql<InsightTestData> ("SELECT * FROM @p", new { p = array.ToList () });
				Assert.IsNotNull (items);
				Assert.AreEqual (i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual (j, items[j].Int);
			}

			// make sure that we cannot send up a null item in the list
			Assert.Throws<InvalidOperationException> (() => _connection.QuerySql<InsightTestData> ("SELECT * FROM @p", new { p = new List<InsightTestData> () { new InsightTestData (), null, new InsightTestData () } }));
		}

		/// <summary>
		/// Test support for enumerable parameters of classes sent as table value parameters.
		/// </summary>
		[Test]
		public void TestEnumerableValueParametersToStoredProc ()
		{
			for (int i = 0; i < 3; i++)
			{
				// build test data
				int[] array = Enumerable.Range (0, i).ToArray ();

				// run the query
				var items = _connection.Query<InsightTestData> ("Int32TestProc", new { p = array });
				Assert.IsNotNull (items);
				Assert.AreEqual (i, items.Count);
			}

			// make sure that we CAN send up a null item in the list
			Assert.AreEqual (3, _connection.Query<InsightTestData> ("Int32TestProc", new { p = new int?[] { 0, null, 1 } }).Count);
		}
		#endregion

		#region Mismatch Tests
		[Test]
		public void TestTypeMismatchInObjectReader ()
		{
			InsightTestData[] array = new InsightTestData[] { new InsightTestData () };

			// run the query
			_connection.QuerySql<InsightTestData> ("InsightTestDataTestProc2", new { p = array.ToList () });
		}
		#endregion
	}
}
