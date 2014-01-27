using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Microsoft.SqlServer.Types;
using NUnit.Framework;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class MappingTests : BaseTest
	{
		[TearDown]
		public void TearDown()
		{
			ColumnMapping.All.ResetHandlers();
		}

		#region Table Tests
		[Test]
		public void RegexReplaceShouldAlterColumnName()
		{
			ColumnMapping.Tables.ReplaceRegex("_", String.Empty);

			var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = Connection().QuerySql<ParentTestData, TestData>(sql);
			ParentTestData.Verify(results);
		}

		[Test]
		public void PrefixRemoveShouldAlterColumnName()
		{
			ColumnMapping.Tables.RemovePrefixes("int");

			var sql = ParentTestData.Sql.Replace("ParentX", "intParentX");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = Connection().QuerySql<ParentTestData, TestData>(sql);
			ParentTestData.Verify(results);
		}

		[Test]
		public void SuffixRemoveShouldAlterColumnName()
		{
			ColumnMapping.Tables.RemoveSuffixes("int");

			var sql = ParentTestData.Sql.Replace("ParentX", "ParentXint");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = Connection().QuerySql<ParentTestData, TestData>(sql);
			ParentTestData.Verify(results);
		}

		[Test]
		public void ReplaceCanBeChained()
		{
			ColumnMapping.Tables.RemovePrefixes("int").RemoveSuffixes("Foo").RemoveStrings("_");

			var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X_Foo");
			Assert.AreNotEqual(sql, ParentTestData.Sql);
			var results = Connection().QuerySql<ParentTestData, TestData>(sql);
			ParentTestData.Verify(results);
		}
		#endregion

		#region Output Parameter Tests
		class OutputParameters
		{
			public int out_foo;
			public int foo;
		}

		[Test]
		public void OutputParameterShouldHonorParameterMappings()
		{
			ColumnMapping.Parameters.RemovePrefixes("out_");

			var output = new OutputParameters();
			Connection().Execute("OutputParameterMappingTest", outputParameters: output);
			Assert.AreEqual(0, output.out_foo);
			Assert.AreEqual(5, output.foo);
		}
		#endregion

		#region Serialization Tests
		public class JsonClass
		{
			[Column(SerializationMode = SerializationMode.Json)]
			public JsonSubClass SubClass;
		}

		public class JsonSubClass
		{
			public string Foo;
			public int Bar;
		}

		[Test]
		public void SerializationHandlerSwitchesObjectToXmlWhenNotFiltered()
		{
			ColumnMapping.All.AddHandler(new SerializationMappingHandler() { SerializationMode = SerializationMode.Xml });

			var input = new JsonClass();
			input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

			var s = Connection().Single<string>("MappingAsJson1", input);
			Assert.IsTrue(Connection().Single<string>("MappingAsJson1", input).StartsWith("<MappingTests."));
		}

		[Test]
		public void SerializationHandlerSwitchesObjectToXmlWhenNameMatches()
		{
			ColumnMapping.All.AddHandler(new SerializationMappingHandler()
			{ 
				FieldName = "Subclass",
				SerializationMode = SerializationMode.Xml
			});

			var input = new JsonClass();
			input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

			var s = Connection().Single<string>("MappingAsJson2", input);
			Assert.IsTrue(Connection().Single<string>("MappingAsJson2", input).StartsWith("<MappingTests."));
		}

		[Test]
		public void SerializationHandlerSwitchesObjectToXmlWhenTypeMatches()
		{
			ColumnMapping.All.AddHandler(new SerializationMappingHandler()
			{
				FieldType = typeof(JsonSubClass),
				SerializationMode = SerializationMode.Xml
			});

			var input = new JsonClass();
			input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

			var s = Connection().Single<string>("MappingAsJson3", input);
			Assert.IsTrue(Connection().Single<string>("MappingAsJson3", input).StartsWith("<MappingTests."));
		}

#if !NET35
		[Test]
		public void SerializationHandlerDoesNotSwitchObjectToXmlWhenNameDoesNotMatch()
		{
			ColumnMapping.All.AddHandler(new SerializationMappingHandler()
			{ 
				FieldName = "foo",
				SerializationMode = SerializationMode.Xml
			});

			var input = new JsonClass();
			input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

			var s = Connection().Single<string>("MappingAsJson4", input);
			Assert.IsFalse(Connection().Single<string>("MappingAsJson4", input).StartsWith("<MappingTests."));
		}
#endif
		#endregion
	}
	
	/// <summary>
	/// Tests dynamic connection.
	/// </summary>
	[TestFixture]
	public class MappingProcTests : BaseTest
	{
		#region SetUp and TearDown
		[TearDown]
		public void TearDownFixture()
		{
			ColumnMapping.Tables.ResetHandlers();
		}
		#endregion

		#region Table Valued Parameter Tests
		[Test]
		public void MappingsAreAppliedToTableValuedParameters()
		{
			// get a stanard set of objects from the server
			var original = Connection().QuerySql<ParentTestData>(ParentTestData.Sql);
			ParentTestData.Verify(original, false);

			ColumnMapping.Tables.RemovePrefixes("int");

			// send the object up to the server and get them back
			var results = Connection().Query<ParentTestData>("MappingTestProc", original);
			ParentTestData.Verify(results, false);
		}

		class ParentTestDataWithColumn
		{
			[Column("IntParentX")]
			public int ParentX { get; set; }

			[Column("IntX")]
			public int X { get; set; }
		}

		[Test]
		public void ColumnAttributesAreAppliedToTableValuedParameters()
		{
			var original = new ParentTestDataWithColumn() { ParentX = 5, X = 7 };
			var list = new List<ParentTestDataWithColumn>() { original };

			// send the object up to the server and get them back
			var results = Connection().Query<ParentTestDataWithColumn>("MappingTestProc", list).First();
			Assert.AreEqual(original.ParentX, results.ParentX);
			Assert.AreEqual(original.X, results.X);
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
				Connection().ExecuteSql("DELETE FROM MappingBulkCopyTestTable");
				Connection().BulkCopy("MappingBulkCopyTestTable", array);

				// run the query
				var items = Connection().QuerySql<ParentTestData>("SELECT * FROM MappingBulkCopyTestTable");
				Assert.IsNotNull(items);
				Assert.AreEqual(i, items.Count);
				for (int j = 0; j < i; j++)
					Assert.AreEqual(j, items[j].ParentX);
			}
		}
		#endregion

		[Test]
		public void MappingsAreAppliedToParameters()
		{
			ColumnMapping.Parameters.RemovePrefixes("int");

			var parentTestData = new ParentTestData() { ParentX = 5 };

			var results = Connection().Query<int>("MappingTestProc2", parentTestData);
			int data = results.First();

			Assert.AreEqual(parentTestData.ParentX, data);
		}

		#region Geography Tests
		class TestGeography
		{
			public SqlGeography Geo;
		}

		[Test]
		public void GeographyParametersArePassedCorrectly()
		{
			// single value query
			var point = SqlGeography.Point(0, 0, 4326);
			var results = Connection().Query<SqlGeography>("MappingTestProcGeography", new { geo = point });
			Assert.That(results[0].STEquals(point).IsTrue);

			// class return value
			var list = Connection().Query<TestGeography>("MappingTestProcGeography", new { geo = point });
			Assert.That(list[0].Geo.STEquals(point).IsTrue);

			// dynamic parameter
			dynamic p = new FastExpando();
			p["Geo"] = point;
			results = Connection().Query<SqlGeography>("MappingTestProcGeography", (object)p);
			Assert.That(results[0].STEquals(point).IsTrue);

			// dynamic results
			var dynamicList = Connection().Query("MappingTestProcGeography", new { geo = point });
			Assert.That(results[0].STEquals(point).IsTrue);
		}
		#endregion
	}
}
