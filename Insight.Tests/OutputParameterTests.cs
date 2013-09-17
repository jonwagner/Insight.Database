using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
using System.Transactions;
using System.Data.Common;

#pragma warning disable 0649

namespace Insight.Tests
{
	/// <summary>
	/// Tests that output parameters function properly.
	/// </summary>
	[TestFixture]
	public class OutputParameterTests : BaseDbTest
	{
		#region Tests to Verify Parameter Returns
		class OutputData
		{
			public int p;
		}

		/// <summary>
		/// Test that an expando can receive an output parameter even if the value has not been set.
		/// </summary>
		[Test]
		public void TestClassOutputParameterWithDefault()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROCEDURE Insight_TestOutput @p int = 1 OUTPUT AS SET @p = 9");

				var command = connection.CreateCommand("Insight_TestOutput", new { p = 5 });
				var result = command.ExecuteNonQuery();

				var outputData = new OutputData();
				command.OutputParameters(outputData);

				Assert.AreEqual(9, outputData.p);
			}
		}

		/// <summary>
		/// Test that an expando can receive an output parameter even if the value has not been set.
		/// </summary>
		[Test]
		public void TestExpandoOutputParameterWithDefault()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROCEDURE Insight_TestOutput @p int = 1 OUTPUT AS SET @p = 11");

				var command = connection.CreateCommand("Insight_TestOutput");
				var result = command.ExecuteNonQuery();
				var output = command.OutputParameters();

				Assert.AreEqual(11, output["p"]);
			}
		}

		/// <summary>
		/// Test that an expando can receive an output parameter when the input value has been set.
		/// </summary>
		[Test]
		public void TestExpandoOutputParameterWithoutDefault()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROCEDURE Insight_TestOutput @p int OUTPUT AS SET @p = 6");

				var command = connection.CreateCommand("Insight_TestOutput", new { p = 5 });
				var result = command.ExecuteNonQuery();
				var output = command.OutputParameters();

				Assert.AreEqual(6, output["p"]);
			}
		}
		#endregion

		#region Tests to Verify Different Parameter Return Types
		class ReturnTypeTest<T>
		{
			public T p;

			public static void Test(T value, DbConnection connection, string sqlType)
			{
				using (DbTransaction t = connection.BeginTransaction())
				{
					// special case for formatting enums in our proc definition
					string stringValue = value.ToString();
					if (typeof(T).IsEnum)
						stringValue = Enum.Format(typeof(T), value, "D");

					// generate the test proc name
					string procName = typeof(T).Name;
					Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
					if (underlyingType != null)
						procName = "Nullable_" + underlyingType.Name;
					procName += sqlType.Split(new char[] { '(' }, 2)[0];

					connection.ExecuteSql(String.Format(
						"CREATE PROCEDURE Insight_TestOutput_{2} @p {0} = NULL OUTPUT AS SET @p = CONVERT({0}, '{1}')",
						sqlType,
						stringValue,
						procName),
						transaction: t);

					var command = connection.CreateCommand("Insight_TestOutput_" + procName, transaction: t);
					var result = command.ExecuteNonQuery();

					var outputData = new ReturnTypeTest<T>();
					command.OutputParameters(outputData);

					Assert.AreEqual(value.ToString(), outputData.p.ToString());
				}
			}
		}

		/// <summary>
		/// A test enum
		/// </summary>
		enum TestEnum
		{
			One = 1,
			Two = 2
		}

		/// <summary>
		/// Test support for all of the types in/out of the database.
		/// </summary>
		[Test]
		public void TestTypes()
		{
			// value types
			// NOTE: unsigned types are not supported by sql so we won't test them here
			ReturnTypeTest<string>.Test("string", _connection, "varchar(32)");
			ReturnTypeTest<string>.Test("string", _connection, "nvarchar(32)");
			ReturnTypeTest<string>.Test("string", _connection, "char(6)");
			ReturnTypeTest<string>.Test("string", _connection, "nchar(6)");

			ReturnTypeTest<byte>.Test(1, _connection, "tinyint");
			ReturnTypeTest<short>.Test(-2, _connection, "smallint");
			ReturnTypeTest<int>.Test(-120, _connection, "int");
			ReturnTypeTest<long>.Test(-120000000, _connection, "bigint");
			ReturnTypeTest<float>.Test(123.456f, _connection, "float");
			ReturnTypeTest<double>.Test(567, _connection, "real");
			ReturnTypeTest<decimal>.Test(890.12345m, _connection, "decimal(18,5)");
			ReturnTypeTest<bool>.Test(false, _connection, "bit");
			ReturnTypeTest<bool>.Test(true, _connection, "bit");
			ReturnTypeTest<char>.Test('c', _connection, "char");
			ReturnTypeTest<Guid>.Test(Guid.NewGuid(), _connection, "uniqueidentifier");
			ReturnTypeTest<DateTime>.Test(DateTime.Now.Date, _connection, "date");				// SQL will round the time, so need to knock off some milliseconds 
			ReturnTypeTest<DateTimeOffset>.Test(DateTimeOffset.Now, _connection, "datetimeoffset");
			ReturnTypeTest<TimeSpan>.Test(TimeSpan.Parse("00:01:15"), _connection, "time");
			ReturnTypeTest<TimeSpan>.Test(TimeSpan.Parse("00:01:15"), _connection, "datetime");

			ReturnTypeTest<Nullable<byte>>.Test(1, _connection, "tinyint");
			ReturnTypeTest<Nullable<short>>.Test(-2, _connection, "smallint");
			ReturnTypeTest<Nullable<int>>.Test(-120, _connection, "int");
			ReturnTypeTest<Nullable<long>>.Test(-120000000, _connection, "bigint");
			ReturnTypeTest<Nullable<float>>.Test(123.456f, _connection, "float");
			ReturnTypeTest<Nullable<double>>.Test(567, _connection, "real");
			ReturnTypeTest<Nullable<decimal>>.Test(890.12345m, _connection, "decimal(18,5)");
			ReturnTypeTest<Nullable<bool>>.Test(false, _connection, "bit");
			ReturnTypeTest<Nullable<bool>>.Test(true, _connection, "bit");
			ReturnTypeTest<Nullable<char>>.Test('c', _connection, "char");
			ReturnTypeTest<Nullable<Guid>>.Test(Guid.NewGuid(), _connection, "uniqueidentifier");
			ReturnTypeTest<Nullable<DateTime>>.Test(DateTime.Now.Date, _connection, "date");				// SQL will round the time, so need to knock off some milliseconds 
			ReturnTypeTest<Nullable<DateTimeOffset>>.Test(DateTimeOffset.Now, _connection, "datetimeoffset");
			ReturnTypeTest<Nullable<TimeSpan>>.Test(TimeSpan.Parse("00:01:15"), _connection, "time");
			ReturnTypeTest<Nullable<TimeSpan>>.Test(TimeSpan.Parse("00:01:15"), _connection, "datetime");

			// enums
			ReturnTypeTest<TestEnum>.Test(TestEnum.Two, _connection, "int");
		}
		#endregion

		#region Tests To Verify Query Results
		/// <summary>
		/// Test that an expando can receive an output parameter even if the value has not been set.
		/// </summary>
		[Test]
		public void TestQueryResults()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROCEDURE Insight_TestOutput @p int = 1 OUTPUT AS SET @p = 9 SELECT 1");

				var result = connection.QueryResults<Results<int>>("Insight_TestOutput", new { p = 5 });

				Assert.IsNotNull(result.Outputs);
				Assert.AreEqual(9, result.Outputs["p"]);
			}
		}

		/// <summary>
		/// Test that an expando can receive an output parameter even if the value has not been set.
		/// </summary>
		[Test]
		public void TestAsyncQueryResults()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROCEDURE Insight_TestOutput @p int = 1 OUTPUT AS SET @p = 9 SELECT 1");

				var result = connection.QueryResultsAsync<Results<int>>("Insight_TestOutput", new { p = 5 }).Result;

				Assert.IsNotNull(result.Outputs);
				Assert.AreEqual(9, result.Outputs["p"]);
			}
		}
		#endregion

		#region Return Value Tests
		class ReturnValue
		{
			public int Return_Value;
		}

		[Test]
		public void ExecuteProcShouldReturnValue()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC ReturnAValue AS RETURN 11");

				var output = new ReturnValue();
				connection.Execute("ReturnAValue", outputParameters: output);
				Assert.AreEqual(11, output.Return_Value);
			}
		}

		[Test]
		public void ReturnValueCanFillInADynamic()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC ReturnAValue AS RETURN 11");

				dynamic output = new FastExpando();
				connection.Execute("ReturnAValue", outputParameters: (object)output);
				Assert.AreEqual(11, output["Return_Value"]);
			}
		}

		[Test]
		public void ReturnValueCanFillQueryResults()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE PROC ReturnAValue AS RETURN 11");

				var results = connection.QueryResults("ReturnAValue");
				Assert.AreEqual(11, results.Outputs["Return_Value"]);
			}
		}
		#endregion
	}
}
