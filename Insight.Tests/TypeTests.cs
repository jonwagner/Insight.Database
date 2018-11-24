using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database;
#if !NO_SQL_TYPES
using Microsoft.SqlServer.Types;
#endif
using NUnit.Framework;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class TypeTests : BaseTest
	{
#region Helper Classes
		/// <summary>
		/// A test enum
		/// </summary>
		enum TestEnum
		{
			One = 1,
			Two = 2
		}

		/// <summary>
		/// Tests deserialization of value types
		/// </summary>
		/// <typeparam name="T">The type to test</typeparam>
		class NullableData<T> where T : struct
		{
			private T Field;
			private T Property { get; set; }
			private T? FieldNullable;
			private T? PropertyNullable { get; set; }
			private T? FieldNull;
			private T? PropertyNull { get; set; }

			public static void Test(T value, IDbConnection connection, string sqlType)
			{
				// make sure we can send values up to SQL
				// make sure we can deserialize properties and fields
				var data = connection.QuerySql<NullableData<T>>(String.Format("SELECT Field=CONVERT({0}, @p), Property=CONVERT({0}, @p), FieldNullable=CONVERT({0}, @p), PropertyNullable=CONVERT({0}, @p), FieldNull=CONVERT({0}, NULL), PropertyNull=CONVERT({0}, NULL)", sqlType), new { p = value }).First();
				Assert.AreEqual(value, data.Field);
				Assert.AreEqual(value, data.Property);
				Assert.AreEqual(value, data.FieldNullable);
				Assert.AreEqual(value, data.PropertyNullable);
				Assert.IsNull(data.FieldNull);
				Assert.IsNull(data.PropertyNull);

				// test deserializing without conversions. This is to check for special cases for Time/DateTime conversions
				data = connection.QuerySql<NullableData<T>>(String.Format("SELECT Field=@p, Property=@p, FieldNullable=@p, PropertyNullable=@p, FieldNull=CONVERT({0}, NULL), PropertyNull=CONVERT({0}, NULL)", sqlType), new { p = value }).First();
				Assert.AreEqual(value, data.Field);
				Assert.AreEqual(value, data.Property);
				Assert.AreEqual(value, data.FieldNullable);
				Assert.AreEqual(value, data.PropertyNullable);
				Assert.IsNull(data.FieldNull);
				Assert.IsNull(data.PropertyNull);

				// make sure we can query with a null parameter to SQL
				data = connection.QuerySql<NullableData<T>>(String.Format("SELECT Field=CONVERT({0}, @p), Property=CONVERT({0}, @p), FieldNull=CONVERT({0}, NULL), PropertyNull=CONVERT({0}, NULL)", sqlType), new { p = (T?)null }).First();
				Assert.AreEqual(default(T), data.Field);
				Assert.AreEqual(default(T), data.Property);
				Assert.IsNull(data.FieldNullable);
				Assert.IsNull(data.PropertyNullable);
				Assert.IsNull(data.FieldNull);
				Assert.IsNull(data.PropertyNull);

				// make sure that we can return a list of the T
				var data2 = connection.QuerySql<T>(String.Format("SELECT CONVERT({0}, @p) UNION ALL SELECT CONVERT({0}, @p)", sqlType), new { p = value });
				Assert.AreEqual(2, data2.Count);
				Assert.AreEqual(value, data2[0]);
				Assert.AreEqual(value, data2[1]);

				// make sure that we can return a list of nullable<T>
				var data3 = connection.QuerySql<T?>(String.Format("SELECT CONVERT({0}, @p) UNION SELECT CONVERT({0}, @p)", sqlType), new { p = (T?)null });
				data3 = connection.QuerySql<T?>(String.Format("SELECT CONVERT({0}, @p) UNION SELECT CONVERT({0}, @p)", sqlType), new { p = (T?)value });

				// make sure that we can convert the type to an proc parameter
				string procName = String.Format("InsightTestProc_{0}_{1}", sqlType.Split('(')[0], typeof(T).Name);
				try
				{
					connection.ExecuteSql(String.Format("CREATE PROC {1} @p {0} AS SELECT CONVERT({0}, @p)", sqlType, procName));

					// parameter as T => T
					var data4 = connection.Query<T>(procName, new { p = (T)value });
					Assert.AreEqual(value, data4.First());

					// parameter as (object)T => T
					var data5 = connection.Query<T>(procName, new { p = (object)value });
					Assert.AreEqual(value.ToString(), data5.First().ToString());

					// parameter as T?(value) => T
					var data6 = connection.Query<T>(procName, new { p = (T?)value });
					Assert.AreEqual(value, data6.First());

					// parameter as T(value) => T?
					var data7 = connection.Query<T?>(procName, new { p = (T)value });
					Assert.AreEqual(value.ToString(), data7.First().ToString());

					// parameter as (object)T=value => T?
					var data8 = connection.Query<T?>(procName, new { p = (object)value });
					Assert.AreEqual(value.ToString(), data8.First().ToString());

					// parameter as T?(value) => T?
					var data9 = connection.Query<T?>(procName, new { p = (T?)value });
					Assert.AreEqual(value.ToString(), data9.First().ToString());

					// parameter as T?=null => T?
					var data10 = connection.Query<T?>(procName, new { p = (T?)null });
					Assert.IsNull(data10.First());

					// parameter as (object)T=null => T?
					var data11 = connection.Query<T?>(procName, new { p = (object)null });
					Assert.IsNull(data11.First());
				}
				finally
				{
					try
					{
						connection.ExecuteSql("DROP PROC " + procName);
					}
					catch{}
				}
			}
		}

		/// <summary>
		/// Tests deserialization of class types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class Data<T> where T : class
		{
			private T Field;
			private T Property { get; set; }
			private T FieldNull;
			private T PropertyNull { get; set; }

			public static void Test(T value, IDbConnection connection, string sqlType)
			{
				// make sure we can read the values
				var data = connection.QuerySql<Data<T>>(String.Format("SELECT Field=@p, Property=@p, FieldNull=CONVERT({0}, NULL), PropertyNull=CONVERT({0}, NULL)", sqlType), new { p = value }).First();
				Assert.AreEqual(value, data.Field);
				Assert.AreEqual(value, data.Property);
				Assert.IsNull(data.FieldNull);
				Assert.IsNull(data.PropertyNull);

				// make sure we can query with a null parameter
				data = connection.QuerySql<Data<T>>(String.Format("SELECT Field=@p, Property=@p, FieldNull=CONVERT({0}, NULL), PropertyNull=CONVERT({0}, NULL)", sqlType), new { p = (T)null }).First();
				Assert.IsNull(data.Field);
				Assert.IsNull(data.Property);
				Assert.IsNull(data.FieldNull);
				Assert.IsNull(data.PropertyNull);

				// make sure that we can convert the type to a stored proc parameter
				try
				{
					connection.ExecuteSql(String.Format("CREATE PROC InsightTestProc @p {0} AS SELECT Field=CONVERT({0}, @p)", sqlType));

					var data4 = connection.Query<Data<T>>("InsightTestProc", new { p = value });
					Assert.AreEqual(value, data4.First().Field);
				}
				finally
				{
					try
					{
						connection.ExecuteSql("DROP PROC InsightTestProc");
					}
					catch { }
				}
			}
		}
#endregion

#region Type Tests
		/// <summary>
		/// Test support for all of the types in/out of the database.
		/// </summary>
		[Test]
		public void TestTypes()
		{
			var _connection = Connection();

			// value types
			// NOTE: unsigned types are not supported by sql so we won't test them here
			NullableData<byte>.Test(1, _connection, "tinyint");
			NullableData<short>.Test(-2, _connection, "smallint");
			NullableData<int>.Test(-120, _connection, "int");
			NullableData<long>.Test(-120000000, _connection, "bigint");
			NullableData<float>.Test(123.456f, _connection, "real");
			NullableData<double>.Test(567.134567, _connection, "float");
			NullableData<decimal>.Test(890.12345m, _connection, "decimal(18,5)");
			NullableData<bool>.Test(false, _connection, "bit");
			NullableData<bool>.Test(true, _connection, "bit");
			NullableData<char>.Test('c', _connection, "char(1)");
			NullableData<Guid>.Test(Guid.NewGuid(), _connection, "uniqueidentifier");
			NullableData<DateTime>.Test(DateTime.Now.Date, _connection, "date");				// SQL will round the time, so need to knock off some milliseconds 
			NullableData<DateTimeOffset>.Test(DateTimeOffset.Now, _connection, "datetimeoffset");
			NullableData<TimeSpan>.Test(TimeSpan.Parse("00:00:00"), _connection, "time");
			NullableData<TimeSpan>.Test(TimeSpan.Parse("00:00:00"), _connection, "datetime");

			// class types
			Data<string>.Test("foo", _connection, "varchar(128)");
			Data<byte[]>.Test(new byte[] { 1, 2, 3, 4 }, _connection, "varbinary(MAX)");
#if !NO_LINQ_BINARY
			Data<System.Data.Linq.Binary>.Test(new System.Data.Linq.Binary(new byte[] { 1, 2, 3, 4 }), _connection, "varbinary(MAX)");
#endif

			// enums
			NullableData<TestEnum>.Test(TestEnum.Two, _connection, "int");

			// make sure that we can return a list of strings
			var data2 = _connection.QuerySql<string>("SELECT @p UNION ALL SELECT @p", new { p = "foo" });
			Assert.AreEqual(2, data2.Count);
			Assert.AreEqual("foo", data2[0]);
			Assert.AreEqual("foo", data2[1]);
		}

		class TestData { public int P; }
#endregion

#region Single Column Tests
		[Test]
		public void TestVarBinaryMaxAsSingleColumn()
		{
			// this used to fail because we couldn't find the constructor for byte[]
			var results = Connection().QuerySql<byte[]>("SELECT CONVERT(varbinary(MAX), '1234')");
		}

		[Test]
		public void TestIntAsSingleColumn()
		{
			var results = Connection().QuerySql<int>("SELECT 1234");
		}
#endregion

#region Type Coersion Tests
		[Test]
		public void TypeCoersions()
		{
			var _connection = Connection();

			// string tests
			Assert.AreEqual(Guid.Empty.ToString(), _connection.QuerySql<TypeContainer<string>>(String.Format("SELECT Value=CONVERT(uniqueidentifier, '{0}')", Guid.Empty.ToString())).FirstOrDefault().Value);

			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(varchar(100), '1')").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(nvarchar(100), '1')").FirstOrDefault().Value);
			Assert.AreEqual("1", _connection.QuerySql<TypeContainer<string>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);

			// enum tests
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<TypeContainer<Int64Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<TypeContainer<UInt64Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<TypeContainer<Int32Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<TypeContainer<UInt32Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<TypeContainer<Int16Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<TypeContainer<UInt16Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<TypeContainer<Int8Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(varchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(nvarchar(100), 'One')").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<TypeContainer<UInt8Enum>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);

			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int64>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt64>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int32>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt32>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<Int16>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<UInt16>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<sbyte>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1, _connection.QuerySql<TypeContainer<byte>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0, _connection.QuerySql<TypeContainer<double>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(decimal, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(float, 1)").FirstOrDefault().Value);
			Assert.AreEqual(1.0f, _connection.QuerySql<TypeContainer<float>>("SELECT Value=CONVERT(real, 1)").FirstOrDefault().Value);
		}

		class TypeContainer<T>
		{
			public T Value;
		}
		enum Int64Enum : long
		{
			One = 1
		}
		enum UInt64Enum : ulong
		{
			One = 1
		}
		enum Int32Enum : int
		{
			One = 1
		}
		enum UInt32Enum : uint
		{
			One = 1
		}
		enum Int16Enum : short
		{
			One = 1
		}
		enum UInt16Enum : ushort
		{
			One = 1
		}
		enum Int8Enum : sbyte
		{
			One = 1
		}
		enum UInt8Enum : byte
		{
			One = 1
		}
#endregion

#region Class/Struct Field Deserialization Tests
#region Class By Constructor
		class FooID
		{
			public int Value;

			public FooID(int i)
			{
				Value = i;
			}

			public override string ToString()
			{
				return Value.ToString();
			}
		}

		class Foo
		{
			public FooID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleClassesCanBeDeserializedByConstructor()
		{
			var data = Connection().QuerySql<Foo>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}

		[Test]
		public void TestThatSimpleClassesCanBeProcParametersByToString()
		{
			Foo f = new Foo() { ID = new FooID(1), Name = "goo" };

			var data = Connection().Query<Foo>("ConvertClassToString", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("goo", data[0].Name);
		}

		[Test]
		public void TestThatNullSimpleClassesCanBeProcParametersByToString()
		{
			Foo f = new Foo() { ID = null, Name = "goo" };

			var data = Connection().Query<Foo>("ConvertClassToString", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNull(data[0].ID);
			Assert.AreEqual("goo", data[0].Name);
		}

		[Test]
		public void TestThatSimpleClassesCanBeSqlParametersByToString()
		{
			Foo f = new Foo() { ID = new FooID(1), Name = "goo" };

			var data = Connection().QuerySql<Foo>("SELECT ID=CONVERT (int, @ID), Name=@Name", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("goo", data[0].Name);
		}
#endregion

#region Type Tests for Conversion by Constructor
		public class ID<T>
		{
			public T Value;

			public ID(T value)
			{
				Value = value;
			}

			public override string ToString()
			{
				return Value.ToString();
			}
		}

		public class ObjectWithID<T>
		{
			public ID<T> ID;
		}

		public void TestConstructorConversion<T>(T value, string sqlType)
		{
			var data = Connection().QuerySql<ObjectWithID<T>>(String.Format("SELECT ID=CONVERT({0}, @p)", sqlType), new { p = value });
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(value, data[0].ID.Value);
		}

		[Test]
		public void TestThatClassesCanBeDeserializedByConstructor()
		{
			TestConstructorConversion<double>(5, "real");

			TestConstructorConversion<string>("s", "varchar(500)");
			TestConstructorConversion<byte>(5, "tinyint");
			TestConstructorConversion<short>(5, "smallint");
			TestConstructorConversion<int>(5, "int");
			TestConstructorConversion<long>(5, "bigint");
			TestConstructorConversion<float>(5, "real");
			TestConstructorConversion<double>(5, "real");
			TestConstructorConversion<decimal>(5, "decimal(18, 2)");
			TestConstructorConversion<bool>(true, "bit");
			TestConstructorConversion<Guid>(Guid.NewGuid(), "uniqueidentifier");
			TestConstructorConversion<DateTime>(DateTime.Now.Date, "datetime");
			TestConstructorConversion<DateTimeOffset>(DateTimeOffset.Now, "datetimeoffset");
			TestConstructorConversion<TimeSpan>(TimeSpan.Parse("00:00:00"), "time");

			// note: char is not compatible with string
		}
#endregion

#region Class By Conversion Operator
		class FooByConversionID
		{
			public int Value;

			public static implicit operator FooByConversionID(int i)
			{
				return new FooByConversionID() { Value = i };
			}
		}

		class FooByConversion
		{
			public FooByConversionID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleClassesCanBeDeserializedByConversion()
		{
			var data = Connection().QuerySql<FooByConversion>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}
#endregion

#region Struct By Constructor
		struct FooStructID
		{
			public int Value;

			public FooStructID(int i)
			{
				Value = i;
			}
		}

		class FooStruct
		{
			public FooStructID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleStructsCanBeDeserialized()
		{
			var data = Connection().QuerySql<FooStruct>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}
#endregion

#region Struct By Conversion
		struct FooStructByConversionID
		{
			public int Value;

			public static explicit operator FooStructByConversionID(int i)
			{
				return new FooStructByConversionID() { Value = i };
			}
		}

		class FooStructByConversion
		{
			public FooStructByConversionID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleStructsCanBeDeserializedByConversion()
		{
			var data = Connection().QuerySql<FooStructByConversion>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}
#endregion

#region ParameterByIConvertible
		class FooConvertibleID : IConvertible
		{
			public int Value;

			public FooConvertibleID(int i)
			{
				Value = i;
			}

			TypeCode IConvertible.GetTypeCode()
			{
				throw new NotImplementedException();
			}

			bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			byte IConvertible.ToByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			char IConvertible.ToChar(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			double IConvertible.ToDouble(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			short IConvertible.ToInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			int IConvertible.ToInt32(IFormatProvider provider)
			{
				return Value;
			}

			long IConvertible.ToInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			float IConvertible.ToSingle(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			string IConvertible.ToString(IFormatProvider provider)
			{
				return Value.ToString(provider);
			}

			object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}
		}

		class FooConvertible
		{
			public FooConvertibleID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleClassesCanBeProcParametersByIConvertible()
		{
			FooConvertible f = new FooConvertible() { ID = new FooConvertibleID(1), Name = "goo" };

			var data = Connection().Query<FooConvertible>("ConvertClassToString", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("goo", data[0].Name);
		}

		[Test]
		public void TestThatSimpleClassesCanBeSqlParametersByIConvertible()
		{
			FooConvertible f = new FooConvertible() { ID = new FooConvertibleID(1), Name = "goo" };

			var data = Connection().QuerySql<FooConvertible>("SELECT ID=CONVERT (int, @ID), Name=@Name", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("goo", data[0].Name);
		}
#endregion

#region NullableValueParameterByToString
		class FooNullableID
		{
			public int? Value;

			public FooNullableID()
			{
			}

			public FooNullableID(int i)
			{
				Value = i;
			}

			// add another constructor just to trick the engine
			public FooNullableID(int? i)
			{
				Value = i;
			}

			public override string ToString()
			{
				if (Value == null)
					return null;

				return Value.ToString();
			}
		}

		class FooNullable
		{
			public FooNullableID ID;
			public string Name;
		}

		[Test]
		public void TestThatSimpleNullableValueClassesCanBeDeserializedByConstructor()
		{
			var data = Connection().QuerySql<FooNullable>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}

		[Test]
		public void TestThatSimpleNullableValueClassesCanBeDeserializedByConstructorWhenNull()
		{
			// in this case, the db value is null, so when we deserialize, the OUTER object id comes back as null.
			// since the values are not round-trip, this configuration is NOT recommended
			var data = Connection().QuerySql<FooNullable>("SELECT ID=CONVERT (int, null), Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNull(data[0].ID);
			Assert.AreEqual("foo", data[0].Name);
		}

		[Test]
		public void TestThatNullableSimpleClassesCanBeProcParametersByToString()
		{
			// in this case, we send up an ID with a null interior
			// when it comes back, the ID will be null
			// since the values are not round-trip, this configuration is NOT recommended
			FooNullable f = new FooNullable() { ID = new FooNullableID(), Name = "goo" };

			var data = Connection().Query<FooNullable>("ConvertClassToString", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNull(data[0].ID);
			Assert.AreEqual("goo", data[0].Name);
		}
#endregion

#region Set Members on a Struct
		struct TestStruct
		{
			public int Foo { get; private set; }
		}

		struct TestParentStruct
		{
			public TestStruct Struct { get; private set; }
		}

		[Test]
		public void TestSettingMembersOfAStruct()
		{
			var results = Connection().QuerySql<TestStruct>("SELECT Foo = 4");
			Assert.AreEqual(4, results[0].Foo);
		}

		[Test]
		public void TestSettingNestedStructures()
		{
			var results = Connection().QuerySql<TestParentStruct, TestStruct>("SELECT Foo = 4");
			Assert.AreEqual(4, results[0].Struct.Foo);
		}
#endregion
#endregion

#region TimeSpan Tests
		[Test]
		public void TimeSpanShouldConvertProperlyToSqlTime()
		{
			// one hour should work
			TimeSpan oneHour = new TimeSpan(1, 0, 0);
			var time = Connection().ExecuteScalar<TimeSpan>("TimeInput", new { t = oneHour });
			Assert.AreEqual(oneHour, time);

			// > 1 day should throw
			Assert.Throws<OverflowException>(() => Connection().ExecuteScalar<TimeSpan>("TimeInput", new { t = new TimeSpan(1, 1, 0, 0) }));
		}

		[Test]
		public void TimeSpanShouldConvertProperlyToSqlDateTime()
		{
			// this is the sql server base 'zero-datetime'
			DateTime timeBase = DateTime.Parse("1/1/1900");
			TimeSpan oneHour = new TimeSpan(1, 0, 0);
			DateTime result;

			// one hour should work - and come back based on 'zero-datetime'
			result = Connection().ExecuteScalar<DateTime>("DateTimeInput", new { t = oneHour });
			Assert.AreEqual(oneHour, result - timeBase);

			// one hour should work - and come back round-tripped
			result = Connection().ExecuteScalar<DateTime>("DateTimeInput", new { t = oneHour });
			Assert.AreEqual(oneHour, result - timeBase);

			TimeSpan oneDayAndOneMinute = new TimeSpan(1, 0, 0, 1);

			// > 1 day should not fail because [datetime] is longer
			result = Connection().ExecuteScalar<DateTime>("DateTimeInput", new { t = oneDayAndOneMinute });
			Assert.AreEqual(oneDayAndOneMinute, result - timeBase);
		}

		[Test]
		public void DateTimeMathShouldWorkOnBothSides()
		{
			// make a time and a span and add them
			DateTime now = new DateTime (1970, 2, 1, 1, 0, 5);
			TimeSpan adjust = new TimeSpan(2, 1, 5, 6);

			var time = Connection().ExecuteScalar<DateTime>("TimeAdd", new { t = now, add = adjust });
			Assert.AreEqual(now + adjust, time);
		}

		[Test]
		public void TimeSpanMathShouldWorkOnBothSides()
		{
			// This seems to require SQL2012 SP1 - 11.0.3000
			// Fails on 11.0.2100
			// make a time and a span and add them
			DateTime now = new DateTime(1970, 2, 1, 1, 0, 5);
			TimeSpan oneHour = new TimeSpan(1, 0, 0);

			var time = Connection().ExecuteScalar<DateTime>("TimeAdd2", new { t = now, add = oneHour });
			Assert.AreEqual(now + oneHour, time);
		}
#endregion

#region Date Tests
		[Test]
		public void DateFieldsShouldConvertProperly()
		{
			var expected = DateTime.Now;

			// send datetime and datetime? to sql
			Connection().QuerySql<DateTime>("SELECT @date", new { date = expected });
			Connection().QuerySql<DateTime>("SELECT @date", new { date = (DateTime?)expected });

			Connection().Query<DateTime>("TestDateTime2", new { date = expected });
			var list = Connection().Query<DateTime>("TestDateTime2", new { date = (DateTime?)expected });

			var result = list.First();
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void DateFieldsShouldConvertFromString()
		{
			DateTime d = DateTime.Today;
			var results = Connection().Query<DateTime>("TestDateTimeConvert", new { p = d.ToString() }).First();
			Assert.AreEqual(d, results);
		}

		[Test]
		public void ZeroDateShouldConvertToDbNull()
		{
			DateTime d = DateTime.MinValue;
			var results = Connection().Query<DateTime>("TestDateTime2", new { date = d }).First();
			Assert.AreEqual(d, results);
		}

		public class DateTimeModel { public DateTime MyDatetime { get; set; } }
		public interface IDateTimeRepository
		{ 
			List<DateTime> InsertDateTimeList(IList<DateTimeModel> dateTimeTestList);
			List<DateTime> InsertDateTime2List(IList<DateTimeModel> dateTime2TestList);
		}

		[Test]
		public void DatetimeFieldsShouldConvertInTVP()
		{
			var c = Connection();
			try
			{
				c.ExecuteSql(@"create type [datetimeList] as table( [myDatetime] [datetime] not null )");
				c.ExecuteSql(@"create procedure [InsertDateTimeList] @datetimeList as datetimeList readonly as select myDatetime from @datetimeList");				

				var expected = new DateTime(2018, 11, 15, 14, 53, 48, 493);

				var model = new DateTimeModel { MyDatetime = expected };
				var list = new List<DateTimeModel> { model, model };
				var repo = c.As<IDateTimeRepository>();
				var results = repo.InsertDateTimeList(list);

				Assert.AreEqual(expected, results[0]);
			}
			finally
			{
				c.ExecuteSql("drop procedure [InsertDateTimeList]");
				c.ExecuteSql("drop type [datetimeList]");
			}
		}
		
		[Test]
		public void Datetime2FieldsShouldConvertInTVP()
		{
			var c = Connection();
			try
			{
				c.ExecuteSql(@"create type [datetime2List] as table( [myDatetime] [datetime2](7) not null )");
				c.ExecuteSql(@"create procedure [InsertDateTime2List] @datetime2List as datetime2List readonly as select myDatetime from @datetime2List");				

				var expected = DateTime.UtcNow;

				var model = new DateTimeModel { MyDatetime = expected };
				var list = new List<DateTimeModel> { model, model };
				var repo = c.As<IDateTimeRepository>();
				var results = repo.InsertDateTime2List(list);

				Assert.AreEqual(expected, results[0]);
			}
			finally
			{
				c.ExecuteSql("drop procedure [InsertDateTime2List]");
				c.ExecuteSql("drop type [datetime2List]");
			}
		}
#endregion

#region Guid Tests
		[Test]
		public void GuidsShouldConvertFromString()
		{
			Guid g = Guid.NewGuid();
			var results = Connection().Query<Guid>("TestGuidFromStringParam", new { p = g.ToString() }).First();
			Assert.AreEqual(g, results);
		}

        [Test]
        public void GuidsShouldConvertToString()
        {
            Guid g = Guid.NewGuid();
            var results = Connection().Query<Guid>("TestGuidToStringParam", new { p = g }).First();
            Assert.AreEqual(g, results);
        }

        [Test]
        public void GuidsShouldConvertAsValueResult()
        {
            var results = Connection().QuerySql<Guid>("SELECT CONVERT(varchar(200),NEWID())").First();
        }

        [Test]
        public void GuidsShouldBeSentAsGuidToUntypedParameter()
        {
            Guid g = Guid.NewGuid();
            var results = Connection().ExecuteScalarSql<object>("SELECT @p", new { p = g });
            Assert.AreEqual(g, results);
        }

        [Test]
        public void NullableGuidsShouldConvertToString()
        {
            Guid? g = Guid.NewGuid();
            var results = Connection().Query<Guid>("TestGuidToStringParam", new { p = g }).First();
            Assert.AreEqual(g, results);
        }

		[Test]
		public void NullGuidsShouldConvertToNullString()
		{
			Guid? g = null;
			var results = Connection().Query<Guid?>("TestGuidToStringParam", new { p = g }).First();
			Assert.AreEqual(g, results);
		}

		[Test]
		public void NullStringShouldConvertToEmptyGuid()
		{
			Guid? g = null;
			var results = Connection().Query<Guid>("TestGuidToStringParam", new { p = g }).First();
			Assert.AreEqual(Guid.Empty, results);
		}
		#endregion

#if !NO_SQL_TYPES
		#region SqlGeometry Tests
		public interface ITestGeometry
		{
			[Sql("GeometryProc", CommandType.StoredProcedure)]
			IList<SqlGeometry> GeometryProc(SqlGeometry geo);

			[Sql("SELECT @geo")]
			IList<SqlGeometry> GeometrySql(SqlGeometry geo);
		}

		[Test]
		public void GeographyParameterOnInterface()
		{
			SqlGeometry geo = SqlGeometry.STGeomFromText(new SqlChars("POINT (2568634.9700000007 1269220.2200000007)"), 102605);

			var i = Connection().As<ITestGeometry>();

			// call by a proc
			var result = i.GeometryProc(geo);
			Assert.AreEqual(geo.ToString(), result.First().ToString());

			// call by sql
			var result2 = i.GeometrySql(geo);
			Assert.AreEqual(geo.ToString(), result2.First().ToString());
		}
		#endregion

		#region SqlHierarchyTests
		public class HierarchyModel
		{
			public int Id;
			public SqlHierarchyId Hierarchy;
		}

		/// <summary>
		/// Tests Issue #70.
		/// </summary>
		[Test]
		public void Hierarchy()
		{
			var results = Connection().QuerySql<HierarchyModel>("SELECT ID=1, Hierarchy=CAST('/1/' AS hierarchyid)").FirstOrDefault();
		}
		#endregion
#endif
	}
}
