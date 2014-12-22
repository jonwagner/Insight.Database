using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Serialization;
using System.Data;

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
                Console.WriteLine(b.IsBool);
                Console.WriteLine(b.IsNullableBool);

                c.ExecuteSql("CREATE PROC TestBool(@IsBool varchar(10), @IsNullableBool varchar(10)) AS SELECT IsBool=@IsBool, IsNullableBool=@IsNullableBool;");
                var b2 = c.Query<HasBool>("TestBool", b).First();
                Console.WriteLine(b2.IsBool);
                Console.WriteLine(b2.IsNullableBool);
            }
        }

        public class EncodedInt
        {
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
    }
}
