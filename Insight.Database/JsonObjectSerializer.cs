using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if !NET35
using System.Runtime.Serialization.Json;
#endif
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Handles serializing objects to and from JSON by using DataContractSerializer.
	/// </summary>
	public static class JsonObjectSerializer
	{
		/// <summary>
		/// The serializer to use for JSON objects. This is overridden by Insight.Database.Json.
		/// </summary>
		private static Type _serializerType = typeof(JsonObjectSerializer);

		/// <summary>
		/// Gets or sets the serializer to use for JSON objects. By default, this is JsonObjectSerializer.
		/// </summary>
		internal static Type SerializerType { get { return _serializerType; } set { _serializerType = value; } }

		/// <summary>
		/// Serializes an object to a string.
		/// </summary>
		/// <param name="value">The object to serialize.</param>
		/// <param name="type">The type of the object.</param>
		/// <returns>The serialized representation of the object.</returns>
		public static string Serialize(object value, Type type)
		{
			if (value == null)
				return null;

#if NET35
			throw new InvalidOperationException(".NET 3.5 does not have a built-in JSON serializer. Please add Insight.Database.Json to your project and call Initialize.");
#else
			// serialize the parameters
			using (MemoryStream stream = new MemoryStream())
			{
				new DataContractJsonSerializer(type).WriteObject(stream, value);

				return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			}
#endif
		}

		/// <summary>
		/// Deserializes an object from a string.
		/// </summary>
		/// <param name="encoded">The encoded value of the object.</param>
		/// <param name="type">The type of object to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static object Deserialize(string encoded, Type type)
		{
#if NET35
			throw new InvalidOperationException(".NET 3.5 does not have a built-in JSON serializer. Please add Insight.Database.Json to your project and call Initialize.");
#else
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(encoded)))
			{
				return serializer.ReadObject(stream);
			}
#endif
		}
	}
}
