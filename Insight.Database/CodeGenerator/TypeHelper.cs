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
			// we convert nulls to dbnull. So if the type is nullable, then we look at the underlying type.
			type = Nullable.GetUnderlyingType(type) ?? type;

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
			if (type == typeof(Decimal)) return true;
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
			if (o == null)
				return null;

			if (type == null)
				type = o.GetType();

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
					new DataContractSerializer(type).WriteObject(xw, o);
				}

				return sw.ToString();
			}
			finally
			{
				if (disposable != null)
					disposable.Dispose();
			}
		}
		#endregion
	}
}
