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
using System.Collections.ObjectModel;

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

		#region Multiple Level Mapping Tests
		public class FlattenedParameters
		{
			public int Y1;
			public int Y2;
		}

		public class FlattenedParameters2
		{
			public int Z1;
			public int Z2;
		}

		public interface IFlattenParameters
		{
			[Sql("SELECT x=@x, y1=@y1, y2=@y2")]
			FastExpando Foo(int x, FlattenedParameters y);

			[Sql("SELECT x=@x, y1=@y1, y2=@y2, z1=@z1, z2=@z2")]
			FastExpando Foo(int x, FlattenedParameters y, FlattenedParameters2 z);
		}

		[Test]
		public void IntefaceMethodsFlattenParameters()
		{
			FastExpando result = Connection().As<IFlattenParameters>().Foo(1, new FlattenedParameters() { Y1 = 2, Y2 = 3 });

			Assert.AreEqual(1, result["x"]);
			Assert.AreEqual(2, result["y1"]);
			Assert.AreEqual(3, result["y2"]);
		}

		[Test]
		public void IntefaceMethodsFlattenParameters2()
		{
			FastExpando result = Connection().As<IFlattenParameters>().Foo(1, new FlattenedParameters() { Y1 = 2, Y2 = 3 }, new FlattenedParameters2() { Z1 = 4, Z2 = 5 });

			Assert.AreEqual(1, result["x"]);
			Assert.AreEqual(2, result["y1"]);
			Assert.AreEqual(3, result["y2"]);
			Assert.AreEqual(4, result["z1"]);
			Assert.AreEqual(5, result["z2"]);
		}

		[Test]
		public void InputParametersCanMapChildFields()
		{
			// we should be able to read in child fields
			FastExpando result = (FastExpando)Connection().QuerySql("SELECT x=@x, y1=@y1, y2=@y2", new { X = 1, Y = new { Y1 = 2, Y2 = 3 } }).First();

			Assert.AreEqual(1, result["x"]);
			Assert.AreEqual(2, result["y1"]);
			Assert.AreEqual(3, result["y2"]);
		}

		[Test]
		public void InputParametersCanMapChildFields2()
		{
			// we should be able to read in child fields
			FastExpando result = (FastExpando)Connection().QuerySql("SELECT x=@x, y1=@y1, y2=@y2, z1=@z1, z2=@z2", new { X = 1, Y = new { Y1 = 2, Y2 = 3 }, Z = new { Z1 = 4, Z2 = 5 } }).First();

			Assert.AreEqual(1, result["x"]);
			Assert.AreEqual(2, result["y1"]);
			Assert.AreEqual(3, result["y2"]);
			Assert.AreEqual(4, result["z1"]);
			Assert.AreEqual(5, result["z2"]);
		}

		[Test, ExpectedException(typeof(SqlException),  ExpectedMessage="The parameterized query '(@parent int,@foo int)SELECT parent=@parent, child=@foo' expects the parameter '@foo', which was not supplied.")]
		public void InputParametersAreUndefinedWhenChildrenAreNull()
		{
			var parent = new ParameterParent() { parent = 3 };

			// we should be able to read in child fields
			Connection().QuerySql("SELECT parent=@parent, child=@foo", parent).First();
		}

		class ParameterParent
		{
			public int parent;
			public OutputParameters Child;
		}

		[Test]
		public void OutputParametersCanMapChildFields()
		{
			var output = new ParameterParent()
			{
				Child = new OutputParameters()
			};

			// we should be able to read in child fields
			Connection().Execute("OutputParameterParentMappingTest", outputParameters: output);

			Assert.AreEqual(1, output.parent);
			Assert.AreEqual(2, output.Child.foo);
		}

		public class ParentOfMissingChild<T>
		{
			public MissingChild<T> Child;
		}

		public class MissingChild<T>
		{
			public T Member;
		}

		private void TestMissingChildCanBeNulled<T>()
		{
			var p = new ParentOfMissingChild<T>();

			try
			{
				Connection().ExecuteSql("SELECT @Member", p);
			}
			catch (SqlException)
			{
				// should get a missing parameter exception
			}

			// this should map a DbNull, since the child isn't missing
			p.Child = new MissingChild<T>();
			Assert.AreEqual(default(T), Connection().ExecuteScalarSql<T>("SELECT @Member", p));
		}

		[Test]
		public void MissingChildrenAreNulledSuccessfully()
		{
			TestMissingChildCanBeNulled<int>();			// value
			TestMissingChildCanBeNulled<int?>();		// value?
			TestMissingChildCanBeNulled<Guid>();		// struct
			TestMissingChildCanBeNulled<Guid?>();		// struct?
			TestMissingChildCanBeNulled<TestData>();	// object
		}

		public class ParentWithConflictedField
		{
			public int ID;
			public ChildWithConflictedField Child;
		}

		public class ChildWithConflictedField
		{
			public int ID;
			public int OtherFieldToTriggerDepthSearch;
		}

		[Test]
		public void ParentFieldOverridesChildField()
		{
			var p = new ParentWithConflictedField() { ID = 1 };
			p.Child = new ChildWithConflictedField() { ID = 2 };

			Assert.AreEqual(1, Connection().ExecuteScalarSql<int>("SELECT @ID, @OtherFieldToTriggerDepthSearch", p));
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
