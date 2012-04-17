using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Helper methods for dealing with system types.
	/// </summary>
	static class TypeHelper
	{
		/// <summary>
		/// Determines whether the given type is a system type that can be easily converted to by the unbox or cast operations.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if this is a system type that unbox or cast can convert to.</returns>
		public static bool IsSystemType(Type type)
		{
			// enums are just ints in disguise
			if (type.IsEnum)
				return true;

			// if it's in the system assembly, it's a good bet.
			if (type.Assembly == typeof(System.Byte).Assembly)
				return true;

			// just need this for byte[]
			if (type.IsArray)
				return true;

			// another special case
			if (type == typeof(System.Data.Linq.Binary))
				return true;

			return false;
		}

		#region Xml Serialization Helpers
		/// <summary>
		/// Serialize an object to Xml.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <param name="type">The type of the object to deserialize.</param>
		/// <returns>The serialized xml.</returns>
		public static string SerializeObjectToXml(object o, Type type)
		{
			// serialize the parameters
			StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			using (XmlWriter xw = XmlWriter.Create(sw, settings))
			{
				new DataContractSerializer(type).WriteObject(xw, o);
			}

			return sw.ToString();
		}
		#endregion
	}
}
