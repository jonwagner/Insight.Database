using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Serialization;
using System.Data;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Data.Common;
using System.IO;

namespace Insight.Tests
{
	[TestFixture]
	public class SerializationTests : BaseTest
	{
		public class CustomSerializerClass
		{
			[Column(SerializationMode = SerializationMode.Custom, Serializer = typeof(StringTrimDeserializer))]
			public string Trimmed;
		}

		public class StringTrimDeserializer : DbObjectSerializer
		{
			public override bool CanDeserialize(Type sourceType, Type targetType)
			{
				return targetType == typeof(String);
			}
			public override object SerializeObject(Type type, object o)
			{
				return (string)o;
			}

			public override object DeserializeObject(Type type, object o)
			{
				return ((string)o).TrimEnd();
			}
		}

		// Issue #133
		[Test]
		public void CustomDeserializerCanBeAppliedToStrings()
		{
			var result = Connection().QuerySql<CustomSerializerClass>("SELECT Trimmed='Trim      '").First();
			Assert.AreEqual("Trim", result.Trimmed);
		}
	}

    [TestFixture]
    public class CustomSerializationTests : BaseTest
    {
        public class HasBool
        {
            public bool IsBool;
            public bool? IsNullableBool;
        }

        [TearDown]
        public void TearDown()
        {
            DbSerializationRule.ResetRules();
        }

        [Test]
        public void BooleanSerializerWorksWithStrings()
        {
            DbSerializationRule.Serialize<bool>(new BooleanYNSerializer());
            DbSerializationRule.Serialize<bool?>(new BooleanYNSerializer());

            using (var c = Connection().OpenWithTransaction())
            {
                var b = c.QuerySql<HasBool>("SELECT IsBool='Y', IsNullableBool=NULL").First();

                c.ExecuteSql("CREATE PROC TestBool(@IsBool varchar(10), @IsNullableBool varchar(10)) AS SELECT IsBool=@IsBool, IsNullableBool=@IsNullableBool;");
                var b2 = c.Query<HasBool>("TestBool", b).First();
            }
        }

		[DataContract]
        public class EncodedInt
        {
			[DataMember]
            public string Encoded;
        }

        public class EncodedIntSerializer : DbObjectSerializer
        {
            public override bool CanDeserialize(Type sourceType, Type targetType)
            {
                return sourceType == typeof(int) && targetType == typeof(string);
            }

            public override bool CanSerialize(Type type, DbType dbType)
            {
                return type == typeof(string) && dbType == DbType.Int32;
            }

            public override DbType GetSerializedDbType(Type type, DbType dbType)
            {
                return DbType.Int32;
            }

            public override object SerializeObject(Type type, object o)
            {
				switch (o.ToString())
                {
                    case "One":
                        return 1;
                    case "Two":
                        return 2;
                    default:
                        return null;
                }
            }

            public override object DeserializeObject(Type type, object encoded)
            {
                if (encoded == null)
                    return null;

                switch((int)encoded)
                {
                    case 1:
                        return "One";
                    case 2:
                        return "Two";
                    default:
                        return null;
                }
            }
        }

        [Test]
        public void CustomSerializerWorksWithOtherTypes()
        {
            DbSerializationRule.Serialize<EncodedInt>("Encoded", new EncodedIntSerializer());

            using (var c = Connection().OpenWithTransaction())
            {
                var e = new EncodedInt() { Encoded = "Two" };

                c.ExecuteSql("CREATE PROC TestEncoded(@Encoded int) AS SELECT Encoded=@Encoded");
                var e2 = c.Query<EncodedInt>("TestEncoded", e).First();
                Assert.AreEqual(e.Encoded, e2.Encoded);
            }
        }

		public class EncodedTypeSerializer : DbObjectSerializer
		{
			public override bool CanDeserialize(Type sourceType, Type targetType)
			{
				return sourceType == typeof(int);
			}

			public override bool CanSerialize(Type type, DbType dbType)
			{
				return dbType == DbType.Int32;
			}

			public override DbType GetSerializedDbType(Type type, DbType dbType)
			{
				return DbType.Int32;
			}

			public override object SerializeObject(Type type, object o)
			{
				var encoded = (EncodedInt)o;
				switch (encoded.Encoded)
				{
					case "One":
						return 1;
					case "Two":
						return 2;
					default:
						return null;
				}
			}

			public override object DeserializeObject(Type type, object encoded)
			{
				switch ((int)encoded)
				{
					case 1:
						return new EncodedInt() { Encoded = "One" };
					case 2:
						return new EncodedInt() { Encoded = "Two" };
					default:
						return null;
				}
			}
		}

		public class TestWithSerializedObject
		{
			public EncodedInt Encoded;
		}

		[Test]
		public void CustomSerializerWorksWithTVPs()
		{
			DbSerializationRule.Serialize<EncodedInt>(new EncodedTypeSerializer());

			var e = new EncodedInt() { Encoded = "Two" };
			var o = new TestWithSerializedObject() { Encoded = e };
			var data = new List<TestWithSerializedObject>() { o };

			try
			{
				Connection().ExecuteSql("CREATE TYPE TestTableWithEncodedInt AS TABLE (Encoded int)");

				using (var c = Connection().OpenWithTransaction())
				{
					c.ExecuteSql("CREATE PROC TestEncoded(@Encoded [TestTableWithEncodedInt] READONLY) AS SELECT * FROM @Encoded");
					var e2 = c.Query<TestWithSerializedObject>("TestEncoded", new { Encoded = data }).First();
					Assert.AreEqual(e.Encoded, e2.Encoded.Encoded);
				}
			}
			finally
			{
				Connection().ExecuteSql("DROP TYPE TestTableWithEncodedInt");
			}
		}

		class CustomXmlSerializer : Insight.Database.XmlObjectSerializer
		{
			public bool DidSerialize { get; private set; }
			public bool DidDeserialize { get; private set; }

			public override object SerializeObject(Type type, object value)
			{
				DidSerialize = true;
				return base.SerializeObject(type, value);
			}

			public override object DeserializeObject(Type type, object encoded)
			{
				DidDeserialize = true;
				return base.DeserializeObject(type, encoded);
			}
		}

		[Test]
        public void CustomSerializerWorksWithXmlFields()
        {
			var customSerializer = new CustomXmlSerializer();
			DbSerializationRule.Serialize<EncodedInt>(customSerializer);

			var e = new EncodedInt() { Encoded = "Two" };

			using (var c = Connection().OpenWithTransaction())
			{
				c.ExecuteSql("CREATE PROC TestEncodedXml(@Encoded [Xml]) AS SELECT Encoded=@Encoded");
				var e2 = c.Query<TestWithSerializedObject>("TestEncodedXml", new { Encoded = e }).First();
				Assert.AreEqual(e.Encoded, e2.Encoded.Encoded);
			}

			Assert.IsTrue(customSerializer.DidSerialize);
			Assert.IsTrue(customSerializer.DidDeserialize);
        }
    }
}
