using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using Insight.Database;
using Insight.Database.Mapping;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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

	#region TVP With Date Tests
	[TestFixture]
	public class TvpWithDefaultDateTimeDataTypeIssueTests : BaseTest
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
			var sql    = "select count(*) from @values";
			var values = new[]
			{
				WithDate(new DateTime(2020, 3, 17)),
				WithDate(new DateTime()), // default(DateTime)
				WithDate(new DateTime(2020, 3, 19)),
				WithDate(new DateTime(2020, 3, 20))
			};

			var result = Connection().SingleSql<int>(sql, new { values });

			ClassicAssert.AreEqual(result, 4);
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
			public DateTime Value4 { get; }
		}
	}
	#endregion

	#region TVP With Deep Member Mapping Tests
	[TestFixture]
	public class TvpWithDeepMemberTests : BaseTest
	{
		private class Parent
		{
			public ChildA A { get; set; }
			public ChildB B { get; set; }

			public Parent()
			{
				A = new ChildA();
				B = new ChildB();
			}
		}

		private class ChildA : Child { }
		private class ChildB : Child { }

		private abstract class Child
		{
			public int X { get; set; }
		}

		private class MyMapper : IColumnMapper
		{
			public string MapColumn(Type type, IDataReader reader, int column)
			{
				if (type != typeof(Parent))
					return null;	

				if (reader.GetName(column) == "A_X")
					return "A.X";

				if (reader.GetName(column) == "B_X")
					return "B.X";

				return null;
			}
		}

		[SetUp]
		public void SetUp()
		{
			// This is needed for mapping object to TVP
			ColumnMapping.Tables.AddMapper(new MyMapper());

			// This is needed for mapping result to object
			// Is it a bug that these are required?
			ColumnMapping.Tables.RemovePrefixes("A_");
			ColumnMapping.Tables.RemovePrefixes("B_");
		}

		[TearDown]
		public void TearDown()
		{
			ColumnMapping.Tables.ResetMappers();
			ColumnMapping.Tables.ResetTransforms();
		}

		[Test]
		public void TvpWithDeepMembersCanRoundTrip()
		{
			try
			{
				Connection().ExecuteSql(@"
					CREATE TYPE dbo.TvpWithDeepMembers AS TABLE (
						A_X int NOT NULL,
						B_X int NOT NULL
					);
				");

				Connection().ExecuteSql(@"
					CREATE PROCEDURE dbo.TestTvpWithDeepMembers
						@Rows dbo.TvpWithDeepMembers READONLY
					AS
						SELECT * FROM @Rows;
				");

				var rowsIn = new[]
				{
					new Parent
					{
						A = new ChildA { X = 1 },
						B = new ChildB { X = 2 },
					},
				};

				var rowsOut = Connection().Query<Parent, ChildA, ChildB>(
					"dbo.TestTvpWithDeepMembers",
					new { Rows = rowsIn }
				);

				Assert.That(rowsOut,        Is.Not.Null.With.Count.EqualTo(1));
				Assert.That(rowsOut[0],     Is.Not.Null);
				Assert.That(rowsOut[0].A,   Is.Not.Null);
				Assert.That(rowsOut[0].A.X, Is.EqualTo(1));
				Assert.That(rowsOut[0].B,   Is.Not.Null);
				Assert.That(rowsOut[0].B.X, Is.EqualTo(2));
			}
			finally
			{
				Connection().ExecuteSql("DROP PROCEDURE IF EXISTS dbo.TestTvpWithDeepMembers;");
				Connection().ExecuteSql("DROP TYPE      IF EXISTS dbo.TvpWithDeepMembers;");
			}
		}
	}
	#endregion

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
					Errors = new[] { new { ErrorJson = "test"} },
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

	#region Issue 456 Tests
	[TestFixture]
	public class Issue456Tests : BaseTest
	{

		[Test]
		public void TestIssue456()
		{
			try
			{
				Connection().ExecuteSql(
					@"CREATE TYPE [UniqueIdTable] AS TABLE(
						[ItemId] [uniqueidentifier] NOT NULL
					)");

				Connection().ExecuteSql(
					@"CREATE PROCEDURE [SampleTable_GetOrderedItems]
						@TableEntryIDs [UniqueIdTable] READONLY
					AS BEGIN
						SELECT COUNT(*) FROM @TableEntryIDs
					END");		

				var orderedList = new[] { "a72863cf-c573-4bf8-9a0b-02212f84698a", "56a0c8ef-c826-45a5-bbce-fb334e59f4b7", "26525d03-1a64-4843-bab4-9daf88e9ae02" };
				var result = Connection().ExecuteScalar<int>("SampleTable_GetOrderedItems", new { TableEntryIDs = orderedList });

				ClassicAssert.AreEqual(result, 3);
			}
			finally
			{
				Connection().ExecuteSql("DROP PROC [SampleTable_GetOrderedItems]");
				Connection().ExecuteSql("DROP TYPE [UniqueIdTable]");
			}
		}
	}
	#endregion

	#region Issue 516 Tests
	[TestFixture]
	public class Issue516Tests : BaseTest
	{
		public class UpdateDate
		{
			public DateTimeOffset CreatedAt { get; set; }
			public DateTimeOffset? ModifiedAt { get; set; }
		}

		public interface Issue516TestsIConnection : IDbConnection
		{
			[Sql("UpdateDates")]
			void UpdateDates(IEnumerable<UpdateDate> dates);
		}

		[Test]
		public void TestIssue516()
		{
			try
			{
				Connection().ExecuteSql(
					@"CREATE TYPE DatesTable AS TABLE (
						CreatedAt datetimeoffset(0) NULL,
						ModifiedAt datetimeoffset(0) NULL
					)");

				Connection().ExecuteSql(
					@"CREATE PROCEDURE UpdateDates
						(@DatesTable DatesTable READONLY)
						AS
						BEGIN
							SELECT NULL
						END
					");

					// Calling code
					var dates = new List<UpdateDate>
					{
						new UpdateDate
						{
							CreatedAt = DateTimeOffset.Now,
							ModifiedAt = null
						}
					};

					Connection().As<Issue516TestsIConnection>().UpdateDates(dates);
			}
			finally
			{
				Connection().ExecuteSql("DROP PROC [UpdateDates]");
				Connection().ExecuteSql("DROP TYPE [DatesTable]");
			}
		}
	}
	#endregion

	#region Issue 523 Tests
	[TestFixture]
	public class Issue532Tests : BaseTest
	{
		[SetUp]
		public void SetUp()
		{
			Connection().ExecuteSql(@"
				CREATE TYPE dbo.My_Type AS TABLE (Value INT)
			");
			Connection().ExecuteSql(@"
				CREATE OR ALTER FUNCTION dbo.testing(
					@my_type My_Type READONLY
				)
				RETURNS @result TABLE (result VARCHAR(MAX))
				AS
				BEGIN
					INSERT INTO @result (result)
					SELECT 'it works'
					FROM @my_type;

					RETURN;				
				END
			");
		}

		[TearDown]
		public void TearDown()
		{
			try { Connection().ExecuteSql("DROP FUNCTION dbo.testing");} catch {}
			try { Connection().ExecuteSql("DROP TYPE dbo.My_Type"); } catch {}
		}

		public class SqlMyType
		{
			public int Value { get; set; }
		}

		public interface IIssue523TestsConnection : IDbConnection
		{
			[Sql("SELECT * FROM dbo.testing (@my_type)", commandType: CommandType.Text)]
			string Testing(IEnumerable<SqlMyType> my_type);
		}

		[Test]
		public void TVPShouldBeAutomaticallyDetected()
		{
			var result = Connection().As<IIssue523TestsConnection>().Testing(new[]
			{
				new SqlMyType { Value = 1 },
				new SqlMyType { Value = 2 },
				new SqlMyType { Value = 3 },
			});

			ClassicAssert.AreEqual("it works", result);
		}
	}
	#endregion
}
