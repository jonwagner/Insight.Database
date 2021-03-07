using Insight.Database;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;

namespace Insight.Tests.MsSqlClient
{
	[TestFixture]
	public class TableValuedParameterWithClassesTests : MsSqlClientBaseTest
	{
		[TearDown]
		public void TearDown()
		{
			DbSerializationRule.ResetRules();
		}

		private class InsightTestDataString
		{
			public MyCustomClass String { get; set; }
		}

		private class MyCustomClass : IHasStringValue
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
				var roundtrippedValue = connection.ExecuteScalarSql<string>("select top 1 String from @values", new { values = new[] { new InsightTestDataString() { String = new MyCustomClass { Value = "classes are better" } } } });

				Assert.That(roundtrippedValue, Is.EqualTo("classes are better"));
			}
		}
	}

	public class TableValuedParameterWithStructsTests : MsSqlClientBaseTest
	{
		[TearDown]
		public void TearDown()
		{
			DbSerializationRule.ResetRules();
		}

		private struct MyCustomStruct : IHasStringValue
		{
			public string Value { get; set; }
		}

		private class InsightTestDataString
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

	internal interface IHasStringValue
	{
		string Value { get; }
	}

	internal class MyCustomSerialiser<T> : IDbObjectSerializer where T : IHasStringValue
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

	#region TVP With Date Tests
	[TestFixture]
	public class TvpWithDefaultDateTimeDataTypeIssueTests : MsSqlClientBaseTest
	{
		[SetUp]
		public void SetUp()
		{
			Connection().ExecuteSql("create type SimpleDateTable as table (Value date, Value2 datetime not null, Value3 datetime null, Value4 datetime2)");
		}

		[TearDown]
		public void TearDown()
		{
			Connection().ExecuteSql("drop type SimpleDateTable");
		}

		[Test]
		public void TVPs_WithDefaultDateTime_DoesNotBlowUp()
		{
			var sql = "select count(*) from @values";
			var values = new[]
			{
				WithDate(new DateTime(2020, 3, 17)),
				WithDate(new DateTime()), // default(DateTime)
				WithDate(new DateTime(2020, 3, 19)),
				WithDate(new DateTime(2020, 3, 20))
			};

			var result = Connection().SingleSql<int>(sql, new { values });

			Assert.AreEqual(result, 4);
		}

		private SimpleDate WithDate(DateTime value)
			=> new SimpleDate(value);

		public class SimpleDate
		{
			public SimpleDate(DateTime value)
			{
				Value = value;
				Value2 = default == value ? (DateTime)SqlDateTime.MinValue : value;
				Value3 = default != value ? (DateTime?)value : null;
				Value4 = value;
			}

			public DateTime Value { get; }
			public DateTime Value2 { get; }
			public DateTime? Value3 { get; }
			public DateTime Value4 { get;  }
		}
	}
	#endregion

	#region Issue 354 Tests
	[TestFixture]
	public class Issue354Tests : MsSqlClientBaseTest
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

	#region Issue 448 Tests
	[TestFixture]
	public class Issue448Tests : BaseTest
	{
		[Test]
		public void TestIssue448()
		{
			try
			{
				Connection().ExecuteSql(
					@"CREATE TYPE [ErrorRecord] AS TABLE(
					[Id] [int] NULL,
					[TableId] [int] NULL,
					[DocTypeRowPK] [int] NULL,
					[ErrorJson] varchar(4001) NULL
					)");

				Connection().ExecuteSql(
					@"CREATE PROCEDURE [InsertPdsData] (
					@errors [ErrorRecord] READONLY,
					@someid INT)

					AS BEGIN
						SELECT COUNT(*) FROM @errors
					END");



				Connection().ExecuteScalar<int>("[InsertPdsData]", new
				{
					Errors = new[] { new { ErrorJson = "test" } },
					SomeId = 1
				});
			}
			finally
			{
				Connection().ExecuteSql("DROP PROC [InsertPdsData]");
				Connection().ExecuteSql("DROP TYPE [ErrorRecord]");
			}
		}
	}
	#endregion
}
