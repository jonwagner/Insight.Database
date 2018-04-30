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
	public class DbReaderTests : BaseTest
	{
		#region Helper Classes
		class Data
		{
			public int Int;
			public string String;
			public TimeSpan TimeSpan;
			public TestEnum TestEnum;
			public TestEnum? NullableTestEnum;
		}
		
		enum TestEnum
		{
			One = 1,
			Two = 2,
			Three = 3
		}
		#endregion

		[Test]
		public void TestThatEnumPropertyIsDeserialized()
		{
			var list = Connection().QuerySql<Data>("SELECT TestEnum='Two'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TestEnum.Two, item.TestEnum);
		}
		
		[Test]
		public void TestThatEnumPropertyIsDeserializedFromInt()
		{
			var list = Connection().QuerySql<Data>("SELECT TestEnum=2", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TestEnum.Two, item.TestEnum);
		}
		
		[Test]
		public void TestThatNullableEnumPropertyIsDeserialized()
		{
			var list = Connection().QuerySql<Data>("SELECT NullableTestEnum='Two'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TestEnum.Two, item.NullableTestEnum);
		}
		
		[Test]
		public void TestThatNullableEnumPropertyIsDeserializedFromInt()
		{
			var list = Connection().QuerySql<Data>("SELECT NullableTestEnum=2", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TestEnum.Two, item.NullableTestEnum);
		}
		
		[Test]
		public void TestThatSingleClassIsDeserialized()
		{
			var list = Connection().QuerySql<Data>("SELECT Int=1, String='foo'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(1, item.Int);
			Assert.AreEqual("foo", item.String);
		}

		[Test]
		public void TestThatMultipleClassIsDeserialized()
		{
			var list = Connection().QuerySql<Data>("SELECT Int=1, String='foo' UNION SELECT Int=2, String='moo'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);

			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(1, item.Int);
			Assert.AreEqual("foo", item.String);

			item = list[1];
			Assert.IsNotNull(item);
			Assert.AreEqual(2, item.Int);
			Assert.AreEqual("moo", item.String);
		}

		[Test]
		public void TestThatDynamicObjectIsDeserialized()
		{
			var list = Connection().QuerySql("SELECT Int=1, String='foo' UNION SELECT Int=2, String='moo'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);

			dynamic item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(1, item["Int"]);
			Assert.AreEqual("foo", item["String"]);

			item = list[1];
			Assert.IsNotNull(item);
			Assert.AreEqual(2, item["Int"]);
			Assert.AreEqual("moo", item["String"]);
		}

		[Test]
		public void TestThatDynamicImpliesFastExpando()
		{
			var list = Connection().QuerySql<dynamic>("SELECT Int=1, String='foo' UNION SELECT Int=2, String='moo'", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);

			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(1, item["Int"]);
			Assert.AreEqual("foo", item["String"]);

			item = list[1];
			Assert.IsNotNull(item);
			Assert.AreEqual(2, item["Int"]);
			Assert.AreEqual("moo", item["String"]);
		}

		[Test]
		public void TestMultipleDynamicObjectQueriesAgainstReader()
		{
			// this failed at one point because the expando converter was holding onto a reader in a closure and calling the wrong one.

			string sql = "SELECT p FROM (SELECT p=0 UNION SELECT 1 UNION SELECT 2) as v WHERE p IN (@p)";

			for (int i = 0; i < 3; i++)
			{
				int[] array = Enumerable.Range(0, i).ToArray();
				var items = Connection().QuerySql(sql, new { p = array });
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
			}
		}

		[Test]
		public void ReaderShouldNotCloseWhenMultipleRecordsetsAreReturned()
		{
			string sql = "SELECT p=1 SELECT p=2";

			using (var connection = Connection().OpenConnection())
			using (var reader = connection.GetReaderSql(sql, Parameters.Empty))
			{
				var first = reader.AsEnumerable<int>().ToList();
				var next = reader.AsEnumerable<int>().ToList();
			}
		}

		[Test]
		public void ReaderShouldAdvanceWhenSingleIsCalled()
		{
			string sql = "SELECT p=1 SELECT p=2";

			using (var connection = Connection().OpenConnection())
			using (var reader = connection.GetReaderSql(sql, Parameters.Empty))
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
			var list = Connection().QuerySql<Data>("SELECT TimeSpan=CONVERT(time, '00:01:01')", new { });

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);
			var item = list[0];
			Assert.IsNotNull(item);
			Assert.AreEqual(TimeSpan.Parse("00:01:01"), item.TimeSpan);
		}
		#endregion

		#region Private Property Tests
		public class ParentWithPrivate
		{
			public string Name { get; private set; }
			public virtual string VirtualName { get; private set; }

			public ParentWithPrivate()
			{
			}

			public ParentWithPrivate(string virtualName)
			{
				VirtualName = virtualName;
			}
		}

		public class ChildWithPrivate : ParentWithPrivate
		{
			public ChildWithPrivate()
			{
			}

			public ChildWithPrivate(string virtualName) : base(virtualName)
			{
			}
	
			public override string VirtualName
			{
				get
				{
					return "virtual" + base.VirtualName;
				}
			}
		}

		[Test]
		public void PrivateParentPropertiesCanBeSet()
		{
			var results = Connection().QuerySql<ChildWithPrivate>("SELECT Name='name'").First();
			Assert.AreEqual("name", results.Name);
		}

		[Test]
		public void VirtualOverridesAreCalled()
		{
			var child = new ChildWithPrivate("name");
			var results = Connection().ExecuteScalarSql<string>("SELECT @VirtualName", child);
			Assert.AreEqual("virtualname", results);
		}
		#endregion
	}
}
