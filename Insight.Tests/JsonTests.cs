using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;

#if !NET35
namespace Insight.Tests
{
	[TestFixture]
	public class JsonTests : BaseDbTest
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
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC ClassAsJsonParameter @SubClass [varchar](max) AS SELECT SubClass=@SubClass");

				var input = new JsonClass();
				input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

				Assert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", connection.Single<string>("ClassAsJsonParameter", input));

				var result = connection.Query<JsonClass, JsonSubClass>("ClassAsJsonParameter", input).First();
				Assert.IsNotNull(result.SubClass);
				Assert.AreEqual(input.SubClass.Foo, result.SubClass.Foo);
				Assert.AreEqual(input.SubClass.Bar, result.SubClass.Bar);
			}
		}

		[Test]
		public void StructCanBeSerializedAsJsonParameter()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC StructAsJsonParameter @SubStruct [varchar](max) AS SELECT SubStruct=@SubStruct");

				var input = new JsonClass();
				input.SubStruct = new JsonSubStruct() { Foo = "foo", Bar = 5 };

				Assert.AreEqual("{\"Bar\":5,\"Foo\":\"foo\"}", connection.Single<string>("StructAsJsonParameter", input));

				var result = connection.Query<JsonClass, JsonSubStruct>("StructAsJsonParameter", input).First();
				Assert.AreEqual(input.SubStruct.Foo, result.SubStruct.Foo);
				Assert.AreEqual(input.SubStruct.Bar, result.SubStruct.Bar);
			}
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void AtomicFieldWithSerializerShouldThrow()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC FieldAsJsonParameter @DateTimeField [varchar](max) AS SELECT DateTimeField=@DateTimeField");

				var date = DateTime.Now;
				var input = new InvalidClass();
				input.DateTimeField = date;

				Assert.AreEqual(date.ToString(), connection.Single<string>("FieldAsJsonParameter", input));

				var result = connection.Query<InvalidClass>("FieldAsJsonParameter", input).First();
				Assert.AreEqual(input.DateTimeField, result.DateTimeField);
			}
		}

		[Test]
		public void ListOfClassCanBeSerializedAsJsonParameter()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC ListAsJsonParameter @ListOfClass [varchar](max) AS SELECT ListOfClass=@ListOfClass");

				var input = new JsonClass();
				input.ListOfClass = new List<JsonSubClass>();
				input.ListOfClass.Add(new JsonSubClass() { Foo = "foo", Bar = 5 });
				input.ListOfClass.Add(new JsonSubClass() { Foo = "foo2", Bar = 6 });

				Assert.AreEqual("[{\"Bar\":5,\"Foo\":\"foo\"},{\"Bar\":6,\"Foo\":\"foo2\"}]", connection.Single<string>("ListAsJsonParameter", input));

				var result = connection.Query<JsonClass, JsonSubClass>("ListAsJsonParameter", input).First();
				Assert.IsNotNull(result.ListOfClass);
				Assert.AreEqual(input.ListOfClass.Count, result.ListOfClass.Count);
				Assert.AreEqual(input.ListOfClass[0].Foo, result.ListOfClass[0].Foo);
			}
		}

		[Test]
		public void TestNullValue()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				var result = connection.QuerySql<JsonClass, JsonSubClass>("SELECT SubClass=CONVERT (varchar(MAX),NULL)").First();
				Assert.IsNull(result.SubClass);
			}
		}
	}
}
#endif