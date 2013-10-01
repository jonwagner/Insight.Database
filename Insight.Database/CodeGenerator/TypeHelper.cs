using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
			if (type == null) throw new ArgumentNullException("type");

			// we convert nulls to dbnull. So if the type is nullable, then we look at the underlying type.
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type == typeof(string))	return true;
			if (type.IsArray) return true;
			if (type.IsEnum) return true;

			// anything marked as a user-defined type is atomic for our purposes
			if (IsSqlUserDefinedType(type))
				return true;

			// value references are atomic
			if (type.IsByRef)
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

		/// <summary>
		/// Determines if a given DbType represents a string.
		/// </summary>
		/// <param name="dbType">The dbType to test.</param>
		/// <returns>True if the type is a string type.</returns>
		public static bool IsDbTypeAString(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Determines if a type is a sql user defined type.
		/// </summary>
		/// <param name="type">The type to examine.</param>
		/// <returns>True if it is a Sql UDT.</returns>
		public static bool IsSqlUserDefinedType(Type type)
		{
			return type.GetCustomAttributes(true).Any(a => a.GetType().Name == "SqlUserDefinedTypeAttribute");
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

			// don't double-encode strings. assume the string is xml.
			if (type == typeof(string))
				return (string)o;

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

		#region Method Implementation Helpers
		/// <summary>
		/// Copy the generic attributes of a method.
		/// </summary>
		/// <param name="sourceMethod">The source method.</param>
		/// <param name="targetMethod">The target method.</param>
		public static void CopyGenericSignature(MethodInfo sourceMethod, MethodBuilder targetMethod)
		{
			if (sourceMethod.IsGenericMethod)
			{
				// get the interface's generic types and make our own
				var oldTypes = sourceMethod.GetGenericArguments();
				var newTypes = targetMethod.DefineGenericParameters(oldTypes.Select(t => t.Name).ToArray());
				for (int i = 0; i < newTypes.Length; i++)
				{
					var oldType = oldTypes[i];
					var newType = newTypes[i];

					newType.SetGenericParameterAttributes(oldType.GenericParameterAttributes);
					newType.SetInterfaceConstraints(oldType.GetGenericParameterConstraints());
				}
			}
		}

		/// <summary>
		/// Copies the method signature from one method to another.
		/// This includes generic parameters, constraints and parameters.
		/// </summary>
		/// <param name="sourceMethod">The source method.</param>
		/// <param name="targetMethod">The target method.</param>
		public static void CopyMethodSignature(MethodInfo sourceMethod, MethodBuilder targetMethod)
		{
			CopyGenericSignature(sourceMethod, targetMethod);

			targetMethod.SetReturnType(sourceMethod.ReturnType);

			// copy the parameters and attributes
			// it seems that we can use the source parameters directly because the target method is derived
			// from the source method
			var parameters = sourceMethod.GetParameters();
			targetMethod.SetParameters(parameters.Select(p => p.ParameterType).ToArray());

			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i];
				targetMethod.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
			}
		}
		#endregion
	}
}
