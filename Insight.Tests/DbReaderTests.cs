using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Dynamic;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class DbReaderTests : BaseDbTest
	{
		#region Helper Classes
		class Data
		{
			public int Int;
			public string String;
			public TimeSpan TimeSpan;
		}
		#endregion

		[Test]
		public void TestThatSingleClassIsDeserialized()
		{
			var list = _connection.QuerySql<Data> ("SELECT Int=1, String='foo'", new { });

			Assert.IsNotNull (list);
			Assert.AreEqual (1, list.Count);
			var item = list[0];
			Assert.IsNotNull (item);
			Assert.AreEqual (1, item.Int);
			Assert.AreEqual ("foo", item.String);
		}

		[Test]
		public void TestThatMultipleClassIsDeserialized ()
		{
			var list = _connection.QuerySql<Data> ("SELECT Int=1, String='foo' UNION SELECT Int=2, String='moo'", new { });

			Assert.IsNotNull (list);
			Assert.AreEqual (2, list.Count);

			var item = list[0];
			Assert.IsNotNull (item);
			Assert.AreEqual (1, item.Int);
			Assert.AreEqual ("foo", item.String);

			item = list[1];
			Assert.IsNotNull (item);
			Assert.AreEqual (2, item.Int);
			Assert.AreEqual ("moo", item.String);
		}

		[Test]
		public void TestThatDynamicObjectIsDeserialized ()
		{
			var list = _connection.QuerySql ("SELECT Int=1, String='foo' UNION SELECT Int=2, String='moo'", new { });

			Assert.IsNotNull (list);
			Assert.AreEqual (2, list.Count);

			dynamic item = list[0];
			Assert.IsNotNull (item);
			Assert.AreEqual (1, item.Int);
			Assert.AreEqual ("foo", item.String);

			item = list[1];
			Assert.IsNotNull (item);
			Assert.AreEqual (2, item.Int);
			Assert.AreEqual ("moo", item.String);
		}

		[Test]
		public void TestMultipleDynamicObjectQueriesAgainstReader ()
		{
			// this failed at one point because the expando converter was holding onto a reader in a closure and calling the wrong one.

			string sql = "SELECT p FROM (SELECT p=0 UNION SELECT 1 UNION SELECT 2) as v WHERE p IN (@p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range (0, i).ToArray ();
				var items = _connection.QuerySql (sql, new { p = array });
				Assert.IsNotNull (items);
				Assert.AreEqual (i, items.Count);
			}
		}

		[Test]
		public void ReaderShouldNotCloseWhenMultipleRecordsetsAreReturned()
		{
			string sql = "SELECT p=1 SELECT p=2";

			using (var reader = _connection.GetReaderSql(sql, Parameters.Empty))
			{
				var first = reader.AsEnumerable<int>().ToList();
				var next = reader.AsEnumerable<int>().ToList();
			}
		}

		[Test]
		public void ReaderShouldAdvanceWhenSingleIsCalled()
		{
			string sql = "SELECT p=1 SELECT p=2";

			using (var reader = _connection.GetReaderSql(sql, Parameters.Empty))
			{
				var first = reader.Single<int>();
				var next = reader.Single<int>();
				Assert.Throws<InvalidOperationException>(() => reader.NextResult());
			}
		}

		#region Specific Type Tests
		[Test]
		public void ReaderShouldTranslateTimeColumnsToTimestamp()
		{
			var list = _connection.QuerySql<Data>("SELECT TimeSpan=CONVERT(time, '00:01:01')", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TimeSpan.Parse("00:01:01"), item.TimeSpan);
		}
		#endregion
	}
}
