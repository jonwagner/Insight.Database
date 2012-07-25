using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class TypeTests : BaseDbTest
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

			public static void Test (T value, SqlConnection connection)
			{
				// make sure we can read the values
				var data = connection.QuerySql<NullableData<T>> ("SELECT Field=@p, Property=@p, FieldNullable=@p, PropertyNullable=@p, FieldNull=NULL, PropertyNull=NULL", new { p = value }).First ();
				Assert.AreEqual (value, data.Field);
				Assert.AreEqual (value, data.Property);
				Assert.AreEqual (value, data.FieldNullable);
				Assert.AreEqual (value, data.PropertyNullable);
				Assert.IsNull (data.FieldNull);
				Assert.IsNull (data.PropertyNull);

				// make sure we can query with a null parameter
				data = connection.QuerySql<NullableData<T>> ("SELECT Field=@p, Property=@p, FieldNull=NULL, PropertyNull=NULL", new { p = (T?)null }).First ();
				Assert.AreEqual (default (T), data.Field);
				Assert.AreEqual (default (T), data.Property);
				Assert.IsNull (data.FieldNullable);
				Assert.IsNull (data.PropertyNullable);
				Assert.IsNull (data.FieldNull);
				Assert.IsNull (data.PropertyNull);

				// make sure that we can return a list of the types
				var data2 = connection.QuerySql<T> ("SELECT @p UNION ALL SELECT @p", new { p = value });
				Assert.AreEqual (2, data2.Count);
				Assert.AreEqual (value, data2[0]);
				Assert.AreEqual (value, data2[1]);

				var data3 = connection.QuerySql<T?> ("SELECT @p UNION SELECT @p", new { p = (T?)null });
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

			public static void Test (T value, SqlConnection connection)
			{
				// make sure we can read the values
				var data = connection.QuerySql<Data<T>> ("SELECT Field=@p, Property=@p, FieldNull=NULL, PropertyNull=NULL", new { p = value }).First ();
				Assert.AreEqual (value, data.Field);
				Assert.AreEqual (value, data.Property);
				Assert.IsNull (data.FieldNull);
				Assert.IsNull (data.PropertyNull);

				// make sure we can query with a null parameter
				data = connection.QuerySql<Data<T>> ("SELECT Field=@p, Property=@p, FieldNull=NULL, PropertyNull=NULL", new { p = (T)null }).First ();
				Assert.IsNull (data.Field);
				Assert.IsNull (data.Property);
				Assert.IsNull (data.FieldNull);
				Assert.IsNull (data.PropertyNull);
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
			// value types
			// NOTE: unsigned types are not supported by sql so we won't test them here
			NullableData<byte>.Test(1, _connection);
			NullableData<short>.Test(-2, _connection);
			NullableData<int>.Test(-120, _connection);
			NullableData<long>.Test(-120000000, _connection);
			NullableData<float>.Test(123.456f, _connection);
			NullableData<double>.Test(567.134567, _connection);
			NullableData<decimal>.Test(890.12345m, _connection);
			NullableData<bool>.Test(false, _connection);
			NullableData<bool>.Test(true, _connection);
			NullableData<char>.Test('c', _connection);
			NullableData<Guid>.Test(Guid.NewGuid(), _connection);
			NullableData<DateTime>.Test(DateTime.Now.Date, _connection);				// SQL will round the time, so need to knock off some milliseconds 
			NullableData<DateTimeOffset>.Test(DateTimeOffset.Now, _connection);

			// class types
			Data<string>.Test("foo", _connection);
			Data<byte[]>.Test(new byte[] { 1, 2, 3, 4 }, _connection);
			Data<System.Data.Linq.Binary>.Test(new System.Data.Linq.Binary(new byte[] { 1, 2, 3, 4 }), _connection);

			// enums
			NullableData<TestEnum>.Test (TestEnum.One, _connection);

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
			var results = _connection.QuerySql<byte[]>("SELECT CONVERT(varbinary(MAX), '1234')");
		}

		[Test]
		public void TestIntAsSingleColumn()
		{
			var results = _connection.QuerySql<int>("SELECT 1234");
		}
		#endregion

		#region Enum Conversion Tests
		[Test]
		public void EnumConversions()
		{
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<EnumContainer<Int64Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<EnumContainer<Int64Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<EnumContainer<Int64Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int64Enum.One, _connection.QuerySql<EnumContainer<Int64Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<EnumContainer<UInt64Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<EnumContainer<UInt64Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<EnumContainer<UInt64Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt64Enum.One, _connection.QuerySql<EnumContainer<UInt64Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<EnumContainer<Int32Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<EnumContainer<Int32Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<EnumContainer<Int32Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int32Enum.One, _connection.QuerySql<EnumContainer<Int32Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<EnumContainer<UInt32Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<EnumContainer<UInt32Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<EnumContainer<UInt32Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt32Enum.One, _connection.QuerySql<EnumContainer<UInt32Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<EnumContainer<Int16Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<EnumContainer<Int16Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<EnumContainer<Int16Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int16Enum.One, _connection.QuerySql<EnumContainer<Int16Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<EnumContainer<UInt16Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<EnumContainer<UInt16Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<EnumContainer<UInt16Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt16Enum.One, _connection.QuerySql<EnumContainer<UInt16Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<EnumContainer<Int8Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<EnumContainer<Int8Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<EnumContainer<Int8Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(Int8Enum.One, _connection.QuerySql<EnumContainer<Int8Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<EnumContainer<UInt8Enum>>("SELECT Value=CONVERT(tinyint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<EnumContainer<UInt8Enum>>("SELECT Value=CONVERT(smallint, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<EnumContainer<UInt8Enum>>("SELECT Value=CONVERT(int, 1)").FirstOrDefault().Value);
			Assert.AreEqual(UInt8Enum.One, _connection.QuerySql<EnumContainer<UInt8Enum>>("SELECT Value=CONVERT(bigint, 1)").FirstOrDefault().Value);
		}

		class EnumContainer<T>
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

			public FooID (int i)
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
			var data = _connection.QuerySql<Foo>("SELECT ID=1, Name='foo'", Parameters.Empty);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("foo", data[0].Name);
		}

		[Test]
		public void TestThatSimpleClassesCanBeProcParametersByToString()
		{
			Foo f = new Foo() { ID = new FooID(1), Name = "goo" };

			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@ID int, @Name varchar(128)) AS SELECT ID=@ID, Name=@Name", transaction: t);

				var data = _connection.Query<Foo>("InsightTestProc", f, transaction: t);
				Assert.AreEqual(1, data.Count);
				Assert.IsNotNull(data[0].ID);
				Assert.AreEqual(1, data[0].ID.Value);
				Assert.AreEqual("goo", data[0].Name);
			}
		}

		[Test]
		public void TestThatNullSimpleClassesCanBeProcParametersByToString()
		{
			Foo f = new Foo() { ID = null, Name = "goo" };

			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@ID int, @Name varchar(128)) AS SELECT ID=@ID, Name=@Name", transaction: t);

				var data = _connection.Query<Foo>("InsightTestProc", f, transaction: t);
				Assert.AreEqual(1, data.Count);
				Assert.IsNull(data[0].ID);
				Assert.AreEqual("goo", data[0].Name);
			}
		}

		[Test]
		public void TestThatSimpleClassesCanBeSqlParametersByToString()
		{
			Foo f = new Foo() { ID = new FooID(1), Name = "goo" };

			var data = _connection.QuerySql<Foo>("SELECT ID=CONVERT (int, @ID), Name=@Name", f);
			Assert.AreEqual(1, data.Count);
			Assert.IsNotNull(data[0].ID);
			Assert.AreEqual(1, data[0].ID.Value);
			Assert.AreEqual("goo", data[0].Name);
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
			var data = _connection.QuerySql<FooByConversion>("SELECT ID=1, Name='foo'", Parameters.Empty);
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
			var data = _connection.QuerySql<FooStruct>("SELECT ID=1, Name='foo'", Parameters.Empty);
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
			var data = _connection.QuerySql<FooStructByConversion>("SELECT ID=1, Name='foo'", Parameters.Empty);
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

			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@ID int, @Name varchar(128)) AS SELECT ID=@ID, Name=@Name", transaction: t);

				var data = _connection.Query<FooConvertible>("InsightTestProc", f, transaction: t);
				Assert.AreEqual(1, data.Count);
				Assert.IsNotNull(data[0].ID);
				Assert.AreEqual(1, data[0].ID.Value);
				Assert.AreEqual("goo", data[0].Name);
			}
		}

		[Test]
		public void TestThatSimpleClassesCanBeSqlParametersByIConvertible()
		{
			FooConvertible f = new FooConvertible() { ID = new FooConvertibleID(1), Name = "goo" };

			var data = _connection.QuerySql<FooConvertible>("SELECT ID=CONVERT (int, @ID), Name=@Name", f);
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
			var data = _connection.QuerySql<FooNullable>("SELECT ID=1, Name='foo'", Parameters.Empty);
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
			var data = _connection.QuerySql<FooNullable>("SELECT ID=CONVERT (int, null), Name='foo'", Parameters.Empty);
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

			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@ID int, @Name varchar(128)) AS SELECT ID=@ID, Name=@Name", transaction: t);

				var data = _connection.Query<FooNullable>("InsightTestProc", f, transaction: t);
				Assert.AreEqual(1, data.Count);
				Assert.IsNull(data[0].ID);
				Assert.AreEqual("goo", data[0].Name);
			}
		}
		#endregion
		#endregion
	}
}
