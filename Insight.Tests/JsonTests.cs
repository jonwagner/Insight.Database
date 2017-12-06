using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

			Assert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", Connection().Single<string>("ClassAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubClass>("ClassAsJsonParameter", input).First();
			Assert.IsNotNull(result.SubClass);
			Assert.AreEqual(input.SubClass.Foo, result.SubClass.Foo);
			Assert.AreEqual(input.SubClass.Bar, result.SubClass.Bar);
		}

		[Test]
		public void StructCanBeSerializedAsJsonParameter()
		{
			var input = new JsonClass();
			input.SubStruct = new JsonSubStruct() { Foo = "foo", Bar = 5 };

			Assert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", Connection().Single<string>("StructAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubStruct>("StructAsJsonParameter", input).First();
			Assert.AreEqual(input.SubStruct.Foo, result.SubStruct.Foo);
			Assert.AreEqual(input.SubStruct.Bar, result.SubStruct.Bar);
		}

		[Test]
		public void ListOfClassCanBeSerializedAsJsonParameter()
		{
			var input = new JsonClass();
			input.ListOfClass = new List<JsonSubClass>();
			input.ListOfClass.Add(new JsonSubClass() { Foo = "foo", Bar = 5 });
			input.ListOfClass.Add(new JsonSubClass() { Foo = "foo2", Bar = 6 });

			Assert.AreEqual("[{\"Bar\":5,\"Foo\":\"foo\"},{\"Bar\":6,\"Foo\":\"foo2\"}]", Connection().Single<string>("ListAsJsonParameter", input));

			var result = Connection().Query<JsonClass, JsonSubClass>("ListAsJsonParameter", input).First();
			Assert.IsNotNull(result.ListOfClass);
			Assert.AreEqual(input.ListOfClass.Count, result.ListOfClass.Count);
			Assert.AreEqual(input.ListOfClass[0].Foo, result.ListOfClass[0].Foo);
		}

		[Test]
		public void TestNullValue()
		{
			var result = Connection().QuerySql<JsonClass, JsonSubClass>("SELECT SubClass=CONVERT (varchar(MAX),NULL)").First();
			Assert.IsNull(result.SubClass);
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

				Assert.AreEqual(entity.Definition.Name, result.Definition.Name);
			}
		}
		#endregion
	}
}
