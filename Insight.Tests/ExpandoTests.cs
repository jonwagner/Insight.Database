using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

			ClassicAssert.AreNotSame(o, other);
			ClassicAssert.IsFalse(otherDict.ContainsKey("A"));
			ClassicAssert.IsNotNull(otherD["B"]);
			ClassicAssert.AreEqual(otherD["Z"], d["A"]);
		}

		[Test]
		public void TransformSucceedsWithExtraMap()
		{
			FastExpando o = new FastExpando();

			FastExpando other = o.Transform(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> otherDict = other;

			ClassicAssert.AreNotSame(o, other);
			ClassicAssert.IsFalse(otherDict.ContainsKey("A"));
			ClassicAssert.IsFalse(otherDict.ContainsKey("Z"));
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

			ClassicAssert.IsFalse(dict.ContainsKey("A"));
			ClassicAssert.IsNotNull(d["B"]);
			ClassicAssert.AreEqual(d["Z"], "foo");
		}

		[Test]
		public void MutateSucceedsWithExtraMap()
		{
			FastExpando o = new FastExpando();
			dynamic d = o;
			d["B"] = "goo";

			o.Mutate(new Dictionary<string, string>() { { "A", "Z" } });
			IDictionary<string, object> dict = o;

			ClassicAssert.IsFalse(dict.ContainsKey("A"));
			ClassicAssert.IsTrue(dict.ContainsKey("B"));
		}

		[Test]
		public void ExpandoShouldPreserveCase()
		{
			string key = "UlUl";

			FastExpando o = new FastExpando();
			var d = (IDictionary<string, object>)o;
			o[key] = "goo";

			ClassicAssert.AreEqual(key, d.Keys.First());
		}
	}
}
