﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Insight.Tests
{
	/// <summary>
	/// Tests that dynamic objects can be sent to the database and retrieved.
	/// </summary>
	[TestFixture]
	public class DynamicParameterTests : BaseTest
	{
		[Test]
		public void TestThatDynamicObjectCanBeUsedAsParameters()
		{
			var p = new FastExpando();
			dynamic pd = p;
			pd["Int"] = 1;
			pd["Text"] = "foo";

			var list = Connection().QuerySql("SELECT Int=CONVERT (int, @Int), Text=@Text", p);

			ClassicAssert.IsNotNull(list);
			ClassicAssert.AreEqual(1, list.Count);

			dynamic result = list[0];
			ClassicAssert.AreEqual(pd["Int"], result["Int"]);
			ClassicAssert.AreEqual(pd["Text"], result["Text"]);
		}

		[Test]
		public void FastExpandoCanBeCreatedFromObject()
		{
			FastExpando o = FastExpando.FromObject(new { Id = 1, Text = "foo" });
			dynamic d = o;

			ClassicAssert.AreEqual(4, o.Count());
			ClassicAssert.AreEqual(1, d["ID"]);
			ClassicAssert.AreEqual("foo", d["Text"]);
		}

		[Test]
		public void FastExpandoCanExpandByObject()
		{
			FastExpando o = FastExpando.FromObject(new { Id = 1, Text = "foo" });
			o.Expand(new { Id = 2, Guest = "boo" });
			dynamic d = o;

			ClassicAssert.AreEqual(6, o.Count());
			ClassicAssert.AreEqual(2, d["ID"]);
			ClassicAssert.AreEqual("foo", d["Text"]);
			ClassicAssert.AreEqual("boo", d["GUEST"]);
		}

		[Test]
		public void FastExpandoCanBeUsedAsDynamic()
		{
			dynamic d = FastExpando.FromObject(new { Id = 1, Text = "foo" });
			d["Property"] = "prop";
			d.Expand(new { Id = 2, Guest = "boo" });

			FastExpando o = (FastExpando)d;
			ClassicAssert.AreEqual(7, o.Count());
			ClassicAssert.AreEqual(2, d["ID"]);
			ClassicAssert.AreEqual("foo", d["Text"]);
			ClassicAssert.AreEqual("prop", d["Property"]);
		}

		[Test]
		public void TestConvenienceExpandoQueries()
		{
			// we have two objects with parameters in them
			var o = new { Id = 1, Text = "foo" };
			var p = new { Id = 2, Guest = "boo" };

			var list = Connection().QuerySql("SELECT ID=CONVERT (int, @ID), Text=@Text, Guest=@Guest", o.Expand(p));
			FastExpando f = list[0];
			dynamic d = f;

			ClassicAssert.AreEqual(3, f.Count());
			ClassicAssert.AreEqual(2, d["ID"]);
			ClassicAssert.AreEqual("foo", d["Text"]);
			ClassicAssert.AreEqual("boo", d["GUEST"]);
		}

		[Test]
		public void TestConvenienceExpandoQueriesWithDynamicObject()
		{
			// start with a dynamic object
			dynamic o = new FastExpando();
			o["ID"] = 1;
			o["Text"] = "foo";

			// expand the dynamic
			var p = new { Id = 2, Guest = "boo" };

			var list = Connection().QuerySql("SELECT ID=CONVERT (int, @ID), Text=@Text, Guest=@Guest", (object)o.Expand(p));
			FastExpando f = list[0];
			dynamic d = f;

			ClassicAssert.AreEqual(3, f.Count());
			ClassicAssert.AreEqual(2, d["ID"]);
			ClassicAssert.AreEqual("foo", d["Text"]);
			ClassicAssert.AreEqual("boo", d["GUEST"]);
		}

		[Test]
		public void TestExpandoNullFields()
		{
			// an expando with a null value passes in null
			dynamic f = new FastExpando();
			f["Value"] = null;
			dynamic result = Connection().Query("ReflectInt", (object)f).FirstOrDefault();
			ClassicAssert.IsNull(result["Value"]);

			// an expando with a NO value passes in DEFAULT
			f = new FastExpando();
			result = Connection().Query("ReflectInt", (object)f).FirstOrDefault();
			ClassicAssert.AreEqual(5, result["Value"]);
		}

		[Test]
		public void TestThatDictionaryCanBeUsedAsParameters()
		{
			var p = new Dictionary<string, object>() { { "Int", 1 }, { "Text", "foo" } };
			var list = Connection().QuerySql("SELECT Int=CONVERT (int, @Int), Text=@Text", p);

			ClassicAssert.IsNotNull(list);
			ClassicAssert.AreEqual(1, list.Count);

			dynamic result = list[0];
			ClassicAssert.AreEqual(p["Int"], result["Int"]);
			ClassicAssert.AreEqual(p["Text"], result["Text"]);
		}

		[Test]
		public void TestGuidConversionInDynamicParameter()
		{
			var p = new FastExpando();
			dynamic pd = p;
			pd["g"] = Guid.NewGuid();

			var list = Connection().QuerySql("SELECT G=@g", p);

			ClassicAssert.IsNotNull(list);
			ClassicAssert.AreEqual(1, list.Count);

			dynamic result = list[0];
			ClassicAssert.AreEqual(pd["g"], result["g"]);
		}
    }

    #region Tests for Issue #167
    [TestFixture]
    public class Issue167 : BaseTest
    {
        [Test]
        public void DynamicsCanContainEnumerables()
        {
            // put a list into a dynamic
            var list = new List<string> { "one", "two", "three" };
            var dyn = new { ID = 1 }.Expand(new { p = list });

            Connection().Execute("VarCharProc", dyn);
        }
    }
    #endregion 

	#region Tests for Issue #193
	[TestFixture]
	public class Issue193 : BaseTest
	{
		public class TestData
		{
			public int X;
			public int Z;
		}

		[Test]
		public void DynamicParametersShouldPopulateListElements()
		{
			using (var c = Connection())
			{
				c.Open();
				var parameters = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

				parameters.Add("data", new List<TestData>() { new TestData() { X = 1, Z = 2 } });

				var cmd = c.CreateCommand("ReflectMultipleTestData", parameters);
				var results = cmd.ExecuteReader().ToList<TestData>();

				// this was failing because the list parameter was not being added to the command
				ClassicAssert.AreEqual(1, results.Count());
				ClassicAssert.AreEqual(2, results[0].Z);
			}

			// this was failing the second time because there was a closure with a command with an old connection
			// and the old connection's connection string goes blank
			using (var c = Connection())
			{
				c.Open();
				var parameters = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

				parameters.Add("data", new List<TestData>() { new TestData() { X = 1, Z = 2 } });

				var cmd = c.CreateCommand("ReflectMultipleTestData", parameters);
				var results = cmd.ExecuteReader().ToList<TestData>();
				ClassicAssert.AreEqual(1, results.Count());
				ClassicAssert.AreEqual(2, results[0].Z);
			}
		}
	}
	#endregion
}
