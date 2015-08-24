using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insight.Database.Json
{
	/// <summary>
	/// Handles serializing objects to JSON by using the JSON.NET serializer.
	/// </summary>
	public class JsonNetObjectSerializer : DbObjectSerializer
	{
		/// <summary>
		/// Initializes the JSON.NET serializer.
		/// </summary>
		public static void Initialize()
		{
			// override the type used to serialize
			Insight.Database.JsonObjectSerializer.OverrideSerializer = new JsonNetObjectSerializer();
		}

		/// <inheritdoc/>
		public override bool CanSerialize(Type type, DbType dbType)
		{
			return base.CanSerialize(type, dbType) || dbType == DbType.Object;
		}

		/// <summary>
		/// Serializes an object to a string.
		/// </summary>
		/// <param name="type">The type of the object.</param>
		/// <param name="value">The object to serialize.</param>
		/// <returns>The serialized representation of the object.</returns>
		public override object SerializeObject(Type type, object value)
		{
			if (value == null)
				return null;

			return JsonConvert.SerializeObject(value, type, Formatting.None, null);
		}

		/// <summary>
		/// Deserializes an object from a string.
		/// </summary>
		/// <param name="type">The type of object to deserialize.</param>
		/// <param name="encoded">The encoded value of the object.</param>
		/// <returns>The deserialized object.</returns>
		public override object DeserializeObject(Type type, object encoded)
		{
			return JsonConvert.DeserializeObject((string)encoded, type);
		}
	}
}
