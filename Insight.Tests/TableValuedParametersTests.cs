using System;
using System.Data;
using System.Linq;
using Insight.Database;
using NUnit.Framework;

namespace Insight.Tests
{
	public class TableValuedParameterWithClassesTests : BaseTest
	{
		[TearDown]
		public void TearDown()
		{
			DbSerializationRule.ResetRules();
		}

		class InsightTestDataString
		{
			public MyCustomClass String { get; set; }
		}

		class MyCustomClass : IHasStringValue
		{
			public string Value { get; set; }
		}

		[Test]
		public void Given_a_type_which_requires_serialisation_When_using_the_type_in_an_sql_statement_Then_it_does_not_explode()
		{
			DbSerializationRule.Serialize<MyCustomClass>(new MyCustomSerialiser<MyCustomClass>());

			var roundtrippedValue = Connection().ExecuteScalarSql<string>("select @value", new { value = new MyCustomClass { Value = "classes are best" } });

			Assert.That(roundtrippedValue, Is.EqualTo("classes are best"));
		}

		[Test]
		public void Given_a_table_type_with_a_property_that_is_a_class_which_requires_serialisation_When_using_the_type_in_an_sql_statement_Then_it_does_not_explode()
		{
			DbSerializationRule.Serialize<MyCustomClass>(new MyCustomSerialiser<MyCustomClass>());

			var roundtrippedValue = Connection().ExecuteScalarSql<string>("select top 1 String from @values", new { values = new[] { new InsightTestDataString() { String = new MyCustomClass { Value = "classes are better" } } } });

			Assert.That(roundtrippedValue, Is.EqualTo("classes are better"));
		}

		[Test]
		public void Given_a_table_type_with_a_property_that_is_a_class_which_requires_serialisation_When_using_the_type_in_an_sql_statement_in_a_transaction_Then_it_does_not_explode()
		{
			DbSerializationRule.Serialize<MyCustomClass>(new MyCustomSerialiser<MyCustomClass>());

			using (var connection = ConnectionWithTransaction())
			{
				var roundtrippedValue = connection.ExecuteScalarSql<string>("select top 1 String from @values", new {values = new[] {new InsightTestDataString() {String = new MyCustomClass {Value = "classes are better"}}}});

				Assert.That(roundtrippedValue, Is.EqualTo("classes are better"));
			}
		}
	}

	public class TableValuedParameterWithStructsTests : BaseTest
	{
		[TearDown]
		public void TearDown()
		{
			DbSerializationRule.ResetRules();
		}

		struct MyCustomStruct : IHasStringValue
		{
			public string Value { get; set; }
		}

		class InsightTestDataString
		{
			public MyCustomStruct String { get; set; }
		}

		[Test]
		public void Given_a_type_which_requires_serialisation_When_using_the_type_in_an_sql_statement_Then_it_does_not_explode()
		{
			DbSerializationRule.Serialize<MyCustomStruct>(new MyCustomSerialiser<MyCustomStruct>());

			var roundtrippedValue = Connection().ExecuteScalarSql<string>("select @value", new { value = new MyCustomStruct { Value = "structs are best" } });

			Assert.That(roundtrippedValue, Is.EqualTo("structs are best"));
		}

		[Test]
		public void Given_a_table_type_with_a_property_that_is_a_struct_which_requires_serialisation_When_using_the_type_in_an_sql_statement_Then_it_does_not_explode()
		{
			DbSerializationRule.Serialize<MyCustomStruct>(new MyCustomSerialiser<MyCustomStruct>());

			var roundtrippedValue = Connection().ExecuteScalarSql<string>("select top 1 String from @values", new { values = new[] { new InsightTestDataString { String = new MyCustomStruct { Value = "structs are better" } } } });

			Assert.That(roundtrippedValue, Is.EqualTo("structs are better"));
		}
	}

	interface IHasStringValue
	{
		string Value { get; }
	}

	class MyCustomSerialiser<T> : IDbObjectSerializer where T : IHasStringValue
	{
		public bool CanSerialize(Type type, DbType dbType)
		{
			return type == typeof(T);
		}

		public bool CanDeserialize(Type sourceType, Type targetType)
		{
			throw new NotImplementedException();
		}

		public DbType GetSerializedDbType(Type type, DbType dbType)
		{
			return dbType;
		}

		public object SerializeObject(Type type, object value)
		{
			return ((IHasStringValue)value).Value;
		}

		public object DeserializeObject(Type type, object encoded)
		{
			throw new NotImplementedException();
		}
	}

	#region Issue 354 Tests
	[TestFixture]
	public class Issue354Tests : BaseTest
	{
		[SetUp]
		public void SetUp()
		{
			Connection().ExecuteSql("create type SimpleIntTable as table (Value int primary key)");
		}

		[TearDown]
		public void TearDown()
		{
			Connection().ExecuteSql("drop type SimpleIntTable");
		}

		[Test]
		public void TVPsShouldBeCached()
		{
			var sql = "select count(*) from @values";
			var values = Enumerable.Range(1, 4).Select(v => new SimpleInt(v)).ToArray();

			void RunQuery() => Connection().SingleSql<int>(sql, new { values });

			//Run the query twice
			RunQuery();
			RunQuery();
		}

		public class SimpleInt
		{
			public int Value { get; }

			public SimpleInt(int value)
			{
				Value = value;
			}
		}
	}
	#endregion
}
