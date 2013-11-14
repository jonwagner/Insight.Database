using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insight.Database.Json
{
	/// <summary>
	/// Handles serializing objects to JSON by using the JSON.NET serializer.
	/// </summary>
	public static class JsonNetObjectSerializer
	{
		/// <summary>
		/// Initializes the JSON.NET serializer.
		/// </summary>
		public static void Initialize()
		{
			// override the type used to serialize
			Insight.Database.JsonObjectSerializer.SerializerType = typeof(JsonNetObjectSerializer);
		}

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

			return JsonConvert.SerializeObject(value, type, Formatting.None, null);
		}

		/// <summary>
		/// Deserializes an object from a string.
		/// </summary>
		/// <param name="encoded">The encoded value of the object.</param>
		/// <param name="type">The type of object to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static object Deserialize(string encoded, Type type)
		{
			return JsonConvert.DeserializeObject(encoded, type);
		}
	}
}
