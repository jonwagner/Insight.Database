using System;
using System.Collections.Generic;
using System.Data;
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
	/// Serializes objects to JSON using the DataContractJsonSerializer or an overridden serializer (usually Newtonsoft.JSON).
	/// </summary>
	public class JsonObjectSerializer : DbObjectSerializer
	{
		/// <summary>
		/// The singleton Serializer.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly JsonObjectSerializer Serializer = new JsonObjectSerializer();

		/// <summary>
		/// Gets or sets a JSON Serializer to replace the DataContractJsonSerializer.
		/// </summary>
		public static DbObjectSerializer OverrideSerializer { get; set; }

		/// <inheritdoc/>
		public override object SerializeObject(Type type, object value)
		{
			if (OverrideSerializer != null)
				return OverrideSerializer.SerializeObject(type, value);

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

		/// <inheritdoc/>
		public override object DeserializeObject(Type type, object encoded)
		{
			if (OverrideSerializer != null)
				return OverrideSerializer.DeserializeObject(type, encoded);

#if NET35
			throw new InvalidOperationException(".NET 3.5 does not have a built-in JSON serializer. Please add Insight.Database.Json to your project and call Initialize.");
#else
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string)encoded)))
			{
				return serializer.ReadObject(stream);
			}
#endif
		}
	}
}
