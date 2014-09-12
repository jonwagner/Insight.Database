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
	public class SerializationTests : BaseTest
	{
		public class CustomSerializerClass
		{
			[Column(SerializationMode = SerializationMode.Custom, Serializer = typeof(StringTrimDeserializer))]
			public string Trimmed;
		}

		public class StringTrimDeserializer : DbObjectSerializer
		{
			public override bool CanDeserialize(Type type)
			{
				return type == typeof(String);
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
}
