using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
#if !NO_CONNECTION_SETTINGS
using System.Configuration;
#endif

namespace Insight.Tests
{
	/// <summary>
	/// Tests the behavior of the ConnectionStringSettings extensions.
	/// </summary>
	[TestFixture]
	public class ExpandoTests
	{
		[Test]
		public void TransformShouldCreateANewObject()
		{
			FastExpando o = new FastExpando();
			dynamic d = o;
			d["A"] = "foo";
			d["B"] = "goo";

			FastExpando other = o.Transform(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> otherDict = other;
			dynamic otherD = other;

			Assert.AreNotSame(o, other);
			Assert.IsFalse(otherDict.ContainsKey("A"));
			Assert.IsNotNull(otherD["B"]);
			Assert.AreEqual(otherD["Z"], d["A"]);
		}

		[Test]
		public void TransformSucceedsWithExtraMap()
		{
			FastExpando o = new FastExpando();

			FastExpando other = o.Transform(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> otherDict = other;

			Assert.AreNotSame(o, other);
			Assert.IsFalse(otherDict.ContainsKey("A"));
			Assert.IsFalse(otherDict.ContainsKey("Z"));
		}

		[Test]
		public void MutateShouldModifyObject()
		{
			FastExpando o = new FastExpando();
			dynamic d = o;
			d["A"] = "foo";
			d["B"] = "goo";

			o.Mutate(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> dict = o;

			Assert.IsFalse(dict.ContainsKey("A"));
			Assert.IsNotNull(d["B"]);
			Assert.AreEqual(d["Z"], "foo");
		}

		[Test]
		public void MutateSucceedsWithExtraMap()
		{
			FastExpando o = new FastExpando();
			dynamic d = o;
			d["B"] = "goo";

			o.Mutate(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> dict = o;

			Assert.IsFalse(dict.ContainsKey("A"));
			Assert.IsTrue(dict.ContainsKey("B"));
		}

		[Test]
		public void ExpandoShouldPreserveCase()
		{
			string key = "UlUl";

			FastExpando o = new FastExpando();
			var d = (IDictionary<string, object>)o;
			o[key] = "goo";

			Assert.AreEqual(key, d.Keys.First());
		}
	}
}
