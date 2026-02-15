using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;

namespace Insight.Tests
{
	[TestFixture]
	public class JsonTests : BaseTest
	{
		public class JsonClass
		{
			[Column(SerializationMode = SerializationMode.Json)]
			public JsonSubClass SubClass;

			[Column(SerializationMode = SerializationMode.Json)]
			public JsonSubStruct SubStruct;
	
			[Column(SerializationMode = SerializationMode.Json)]
			public List<JsonSubClass> ListOfClass;
		}

		public class JsonSubClass
		{
			public string Foo;
			public int Bar;
		}

		public struct JsonSubStruct
		{
			public string Foo;
			public int Bar;
		}

		public class InvalidClass
		{
			// this should throw
			[Column(SerializationMode = SerializationMode.Json)]
			public DateTime DateTimeField;
		}

		[Test]
		public void ClassCanBeSerializedAsJsonParameter()
		{
			var input = new JsonClass();
			input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

			ClassicAssert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", Connection().Single<string>("ClassAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubClass>("ClassAsJsonParameter", input).First();
			ClassicAssert.IsNotNull(result.SubClass);
			ClassicAssert.AreEqual(input.SubClass.Foo, result.SubClass.Foo);
			ClassicAssert.AreEqual(input.SubClass.Bar, result.SubClass.Bar);
		}

		[Test]
		public void StructCanBeSerializedAsJsonParameter()
		{
			var input = new JsonClass();
			input.SubStruct = new JsonSubStruct() { Foo = "foo", Bar = 5 };

			ClassicAssert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", Connection().Single<string>("StructAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubStruct>("StructAsJsonParameter", input).First();
			ClassicAssert.AreEqual(input.SubStruct.Foo, result.SubStruct.Foo);
			ClassicAssert.AreEqual(input.SubStruct.Bar, result.SubStruct.Bar);
		}

		[Test]
		public void ListOfClassCanBeSerializedAsJsonParameter()
		{
			var input = new JsonClass();
			input.ListOfClass = new List<JsonSubClass>();
			input.ListOfClass.Add(new JsonSubClass() { Foo = "foo", Bar = 5 });
			input.ListOfClass.Add(new JsonSubClass() { Foo = "foo2", Bar = 6 });

			ClassicAssert.AreEqual("[{\"Bar\":5,\"Foo\":\"foo\"},{\"Bar\":6,\"Foo\":\"foo2\"}]", Connection().Single<string>("ListAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubClass>("ListAsJsonParameter", input).First();
			ClassicAssert.IsNotNull(result.ListOfClass);
			ClassicAssert.AreEqual(input.ListOfClass.Count, result.ListOfClass.Count);
			ClassicAssert.AreEqual(input.ListOfClass[0].Foo, result.ListOfClass[0].Foo);
		}

		[Test]
		public void TestNullValue()
		{
			var result = Connection().QuerySql<JsonClass, JsonSubClass>("SELECT SubClass=CONVERT (varchar(MAX),NULL)").First();
			ClassicAssert.IsNull(result.SubClass);
		}

		[DataContract]
		public class NativeJsonData
		{
			[DataMember]
			public int P;

			[DataMember]
			public string Text;
		}

		public class NativeJsonParameters
		{
			[Column(SerializationMode = SerializationMode.Json)]
			public NativeJsonData Data;
		}

		[Test]
		public void ObjectCanAutoSerializeToNativeJsonParameterAndStoreInJsonColumn()
		{
			using (var connection = ConnectionWithTransaction())
			{
				if (!SupportsNativeJson(connection))
					Assert.Ignore("Native json type is not available on this SQL Server instance.");

				connection.ExecuteSql("CREATE TABLE NativeJsonDataTable (Data json NOT NULL)");
				connection.ExecuteSql("CREATE PROC InsertNativeJsonData (@Data json) AS INSERT INTO NativeJsonDataTable (Data) VALUES (@Data)");

				var input = new NativeJsonParameters() { Data = new NativeJsonData() { P = 5, Text = "foo" } };
				connection.Execute("InsertNativeJsonData", input);

				ClassicAssert.AreEqual("5", connection.ExecuteScalarSql<string>("SELECT JSON_VALUE(CONVERT(nvarchar(max), Data), '$.P') FROM NativeJsonDataTable"));
				ClassicAssert.AreEqual("foo", connection.ExecuteScalarSql<string>("SELECT JSON_VALUE(CONVERT(nvarchar(max), Data), '$.Text') FROM NativeJsonDataTable"));
			}
		}

		[Test]
		public void ObjectCanAutoDeserializeFromNativeJsonResult()
		{
			using (var connection = ConnectionWithTransaction())
			{
				if (!SupportsNativeJson(connection))
					Assert.Ignore("Native json type is not available on this SQL Server instance.");

				connection.ExecuteSql("CREATE PROC ReflectNativeJsonResult (@Data json) AS SELECT Data=CONVERT(nvarchar(max), @Data)");

				var input = new NativeJsonParameters() { Data = new NativeJsonData() { P = 7, Text = "bar" } };
				var output = connection.Query<NativeJsonParameters>("ReflectNativeJsonResult", input).Single();

				ClassicAssert.IsNotNull(output.Data);
				ClassicAssert.AreEqual(7, output.Data.P);
				ClassicAssert.AreEqual("bar", output.Data.Text);
			}
		}

		private static bool SupportsNativeJson(System.Data.IDbConnection connection)
		{
			return connection.ExecuteScalarSql<int>(
				@"SELECT COUNT(*) FROM sys.types t
					INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
					WHERE s.name = 'sys' AND t.name = 'json'") > 0;
		}

		#region Test Case for Issue 140
		public class EntityDefinition
		{
			public string Id;
			public string Name;
			public string Title;
			public bool IsEnabled;
		}

		public class EntityDefinitionDataModel
		{
			[Column(SerializationMode = SerializationMode.Json)]
			public EntityDefinition Definition { get; set; }
			public Boolean IsEnabled { get; set; }

			public EntityDefinitionDataModel()
			{
			}
		}

		[Test]
		public void LargeSerialization()
		{
			using (var connection = Connection().OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC LargeSerialization(@Definition nvarchar(MAX)) AS SELECT Definition=@Definition");

				var entity = new EntityDefinitionDataModel()
				{
					Definition = new EntityDefinition()
					{
						Name = new String('a', 16384)
					}
				};

				var result = connection.Single<EntityDefinitionDataModel>("LargeSerialization", entity);

				ClassicAssert.AreEqual(entity.Definition.Name, result.Definition.Name);
			}
		}
		#endregion
	}
}
