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

		public class StringTrimDeserializer
		{
			public static bool CanDeserialize(Type type)
			{
				return type == typeof(string);
			}
	
			public static object Deserialize(string encoded, Type type)
			{
				return encoded.TrimEnd();
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
