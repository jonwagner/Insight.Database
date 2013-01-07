using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
		/// Determines whether the given type is is an atomic type that does not have members.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if this is an atomic type that does not have members.</returns>
		public static bool IsAtomicType(Type type)
		{
			// treat strings as atomic
			if (type == typeof(string))
				return true;

			// arrays are atomic
			if (type.IsArray)
				return true;

			// enums are atomic
			if (type.IsEnum)
				return true;

			// treat all references as non-atomic
			if (!type.IsValueType)
				return false;

			// these are structures, but we want to treat them as atomic
			if (type == typeof(DateTime)) return true;
			if (type == typeof(DateTimeOffset)) return true;
			if (type == typeof(Guid)) return true;
			if (type == typeof(TimeSpan)) return true;

			// all of the primitive types, array, etc. are atomic
			return type.IsPrimitive;
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
