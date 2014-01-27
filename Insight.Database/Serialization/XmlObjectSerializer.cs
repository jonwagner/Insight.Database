using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Insight.Database
{
	/// <summary>
	/// Handles serialization of objects by using the DataContractSerializer.
	/// </summary>
	public static class XmlObjectSerializer
	{
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

			if (type == null)
				type = value.GetType();

			// don't double-encode strings. assume the string is xml.
			if (type == typeof(string))
				return (string)value;

			// serialize the parameters
			StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
			StringWriter disposable = sw;
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.OmitXmlDeclaration = true;
				using (XmlWriter xw = XmlWriter.Create(sw, settings))
				{
					disposable = null;
					new DataContractSerializer(type).WriteObject(xw, value);
				}

				return sw.ToString();
			}
			finally
			{
				if (disposable != null)
					disposable.Dispose();
			}
		}

		/// <summary>
		/// Deserializes an object from a string.
		/// </summary>
		/// <param name="encoded">The encoded value of the object.</param>
		/// <param name="type">The type of object to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static object Deserialize(string encoded, Type type)
		{
			DataContractSerializer serializer = new DataContractSerializer(type);

			StringReader reader = new StringReader(encoded);
			try
			{
				using (XmlTextReader xr = new XmlTextReader(reader))
				{
					reader = null;
					return serializer.ReadObject(xr);
				}
			}
			finally
			{
				if (reader != null)
					reader.Dispose();
			}
		}
	}
}
