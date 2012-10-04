using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Generates the IL that is needed to convert a value from an input type to an output type.
	/// This handles automatic conversion to/from nullables, Xml, strings, etc.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class TypeConverterGenerator
	{
		#region Private Members
		internal static readonly MethodInfo CreateDataExceptionMethod = typeof(TypeConverterGenerator).GetMethod("CreateDataException");
		private static readonly MethodInfo _enumParse = typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) });
		private static readonly ConstructorInfo _linqBinaryCtor = typeof(System.Data.Linq.Binary).GetConstructor(new Type[] { typeof(byte[]) });
		private static readonly MethodInfo _typeGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
		private static readonly MethodInfo _readChar = typeof(TypeConverterGenerator).GetMethod("ReadChar");
		private static readonly MethodInfo _readNullableChar = typeof(TypeConverterGenerator).GetMethod("ReadNullableChar");
		private static readonly MethodInfo _readXmlDocument = typeof(TypeConverterGenerator).GetMethod("ReadXmlDocument");
		private static readonly MethodInfo _readXDocument = typeof(TypeConverterGenerator).GetMethod("ReadXDocument");
		private static readonly MethodInfo _deserializeXml = typeof(TypeConverterGenerator).GetMethod("DeserializeXml");
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes static members of the TypeConverterGenerator class.
		/// </summary>
		static TypeConverterGenerator()
		{
			// check to see whether setting DbType to Time is broken. In .NET 4.5, it gets set to DateTime when you set it to Time.
			SqlParameter p = new SqlParameter("p", new TimeSpan());
			p.DbType = DbType.Time;
			if (p.DbType != DbType.Time)
				SqlClientTimeIsBroken = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether this version of .NET does not properly set DbType to Time.
		/// </summary>
		internal static bool SqlClientTimeIsBroken { get; private set; }
		#endregion

		#region Code Generation Members
		/// <summary>
		/// Emit the IL to convert the current value on the stack and set the value of the object.
		/// </summary>
		/// <param name="il">The IL generator to output to.</param>
		/// <param name="sourceType">The current type of the value.</param>
		/// <param name="method">The set property method to call.</param>
		/// <remarks>
		///	Expects the stack to contain:
		///		Target Object
		///		Value to set
		/// The value is first converted to the type required by the method parameter, then sets the property.
		/// </remarks>
		/// <returns>A label that needs to be marked at the end of a succesful set.</returns>
		public static Label EmitConvertAndSetValue(ILGenerator il, Type sourceType, ClassPropInfo method)
		{
			// targetType - the target type we need to convert to
			// underlyingTargetType - if the target type is nullable, we need to look at the underlying target type
			// rawTargetType - if the underlying target type is enum, we need to look at the underlying target type for that
			// sourceType - this is the type of the data in the data set
			Type targetType = method.MemberType;
			Type underlyingTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
			Type rawTargetType = underlyingTargetType.IsEnum ? Enum.GetUnderlyingType(underlyingTargetType) : underlyingTargetType;

			// some labels that we need
			Label isDbNullLabel = il.DefineLabel();
			Label finishLabel = il.DefineLabel();

			// if the value is DbNull, then we continue to the next item
			il.Emit(OpCodes.Dup);								// dup value, stack => [target][value][value]
			il.Emit(OpCodes.Isinst, typeof(DBNull));			// isinst DBNull:value, stack => [target][value-as-object][DBNull or null]
			il.Emit(OpCodes.Brtrue_S, isDbNullLabel);			// br.true isDBNull, stack => [target][value-as-object]

			// handle the special target types first
			if (targetType == typeof(char))
			{
				// char
				il.EmitCall(OpCodes.Call, _readChar, null);
			}
			else if (targetType == typeof(char?))
			{
				// char?
				il.EmitCall(OpCodes.Call, _readNullableChar, null);
			}
			else if (targetType == typeof(System.Data.Linq.Binary))
			{
				// unbox sql byte arrays to Linq.Binary

				// before: stack => [target][object-value]
				// after: stack => [target][byte-array-value]
				il.Emit(OpCodes.Unbox_Any, typeof(byte[])); // stack is now [target][byte-array]
				// before: stack => [target][byte-array-value]
				// after: stack => [target][Linq.Binary-value]
				il.Emit(OpCodes.Newobj, _linqBinaryCtor);
			}
			else if (targetType == typeof(XmlDocument))
			{
				// special handler for XmlDocuments

				// before: stack => [target][object-value]
				il.Emit(OpCodes.Call, _readXmlDocument);

				// after: stack => [target][xmlDocument]
			}
			else if (targetType == typeof(XDocument))
			{
				// special handler for XDocuments

				// before: stack => [target][object-value]
				il.Emit(OpCodes.Call, _readXDocument);

				// after: stack => [target][xDocument]
			}
			else if (sourceType == typeof(XmlDocument) && targetType != typeof(string) && !targetType.IsValueType)
			{
				// the column is an xml data type and the member is not a string

				// before: stack => [target][object-value]
				il.Emit(OpCodes.Ldtoken, targetType);
				il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));

				// after: stack => [target][object-value][memberType]
				il.Emit(OpCodes.Call, _deserializeXml);
				il.Emit(OpCodes.Unbox_Any, targetType);
			}
			else if (underlyingTargetType.IsEnum && sourceType == typeof(string))
			{
				// if we are converting a string to an enum, then parse it.
				// see if the value from the database is a string. if so, we need to parse it. If not, we will just try to unbox it.
				il.Emit(OpCodes.Isinst, typeof(string));			// is string, stack => [target][string]
				il.Emit(OpCodes.Stloc_2);							// pop loc.2 (enum), stack => [target]

				// call enum.parse (type, value, true)
				il.Emit(OpCodes.Ldtoken, underlyingTargetType);				// push type-token, stack => [target][enum-type-token]
				il.EmitCall(OpCodes.Call, _typeGetTypeFromHandle, null);

				// call GetType, stack => [target][enum-type]
				il.Emit(OpCodes.Ldloc_2);							// push enum, stack => [target][enum-type][string]
				il.Emit(OpCodes.Ldc_I4_1);							// push true, stack => [target][enum-type][string][true]
				il.EmitCall(OpCodes.Call, _enumParse, null);		// call Enum.Parse, stack => [target][enum-as-object]

				// Enum.Parse returns an object, which we need to unbox to the enum value
				il.Emit(OpCodes.Unbox_Any, underlyingTargetType);
			}
			else
			{
				// at this point, we can't treat it as an XmlDocument, we just have to treat it as a string
				if (sourceType == typeof(XmlDocument))
					sourceType = typeof(String);

				// if timespan is broken and we have an object, then it's probably a timespan
				if (SqlClientTimeIsBroken && sourceType == typeof(object) && underlyingTargetType == typeof(TimeSpan))
					sourceType = typeof(TimeSpan);

				// this isn't a system value type, so unbox to the type the reader is giving us (this is a system type, hopefully)
				// now we have an unboxed sourceType
				il.Emit(OpCodes.Unbox_Any, sourceType);

				// look for a constructor that takes the type as a parameter
				Type[] sourceTypes = new Type[] { sourceType };
				ConstructorInfo ci = targetType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, sourceTypes, null);
				if (ci != null)
				{
					// if the constructor only takes nullable types, warn the programmer
					if (Nullable.GetUnderlyingType(ci.GetParameters()[0].ParameterType) != null)
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Class {0} must provide a constructor taking a parameter of type {1}. Nullable parameters are not supported.", targetType, sourceType));

					il.Emit(OpCodes.Newobj, ci);
				}
				else
				{
					// see if there is an conversion operator on either the source or target types
					MethodInfo mi = FindConversionMethod(sourceType, targetType);

					// either emit the call to the conversion operator or try another strategy
					if (mi != null)
					{
						il.Emit(OpCodes.Call, mi);
					}
					else if (!TypeConverterGenerator.EmitCoersion(il, sourceType, rawTargetType))
					{
						if (sourceType != targetType)
						{
							throw new InvalidOperationException(String.Format(
								CultureInfo.InvariantCulture,
								"Field {0} cannot be converted from {1} to {2}. Create a conversion constructor or conversion operator.",
								method.Name,
								sourceType,
								targetType));
						}
					}

					// if the target is nullable, then construct the nullable from the data
					if (Nullable.GetUnderlyingType(targetType) != null)
						il.Emit(OpCodes.Newobj, targetType.GetConstructor(new[] { underlyingTargetType }));
				}
			}

			/////////////////////////////////////////////////////////////////////
			// now the stack has [target][value-unboxed]. we can set the value now
			method.EmitSetValue(il);

			// stack is now EMPTY
			/////////////////////////////////////////////////////////////////////

			/////////////////////////////////////////////////////////////////////
			// jump over our DBNull handler
			il.Emit(OpCodes.Br_S, finishLabel);
			/////////////////////////////////////////////////////////////////////

			/////////////////////////////////////////////////////////////////////
			// cleanup after IsDBNull.
			/////////////////////////////////////////////////////////////////////
			il.MarkLabel(isDbNullLabel);							// stack => [target][value]
			il.Emit(OpCodes.Pop);									// pop value, stack => [target]

			// if the type is an object, set the value to null
			// this is necessary for overwriting output parameters,
			// as well as overwriting any properties that may be set in the constructor of the object
			if (!method.MemberType.IsValueType)
			{
				il.Emit(OpCodes.Ldnull);							// push null
				method.EmitSetValue(il);
			}
			else
			{
				// we didn't call setvalue, so pop the target object off the stack
				il.Emit(OpCodes.Pop);								// pop target, stack => [empty]
			}

			return finishLabel;
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Wrap an exception with a DataException that contains more information.
		/// </summary>
		/// <param name="ex">The inner exception.</param>
		/// <param name="index">The index of the column.</param>
		/// <param name="reader">The data reader.</param>
		/// <returns>An exception that can be thrown.</returns>
		public static Exception CreateDataException(Exception ex, int index, IDataReader reader)
		{
			string name = "n/a";
			string value = "n/a";

			if (reader != null && index >= 0 && index < reader.FieldCount)
			{
				name = reader.GetName(index);
				object val = reader.GetValue(index);
				if (val == null || val is DBNull)
					value = "<null>";
				else
					value = val.ToString() + " - " + Type.GetTypeCode(val.GetType());
			}

			return new DataException(string.Format(CultureInfo.InvariantCulture, "Error parsing column {0} ({1}={2})", index, name, value), ex);
		}

		/// <summary>
		/// Convert an object value to a char.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A single character.</returns>
		public static char ReadChar(object value)
		{
			if (value == null || value is DBNull)
				throw new ArgumentNullException("value");

			string s = value as string;
			if (s == null || s.Length != 1)
				throw new ArgumentException("A single character was expected", "value");
			return s[0];
		}

		/// <summary>
		/// Convert an object value to a nullable char.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A single character.</returns>
		public static char? ReadNullableChar(object value)
		{
			if (value == null || value is DBNull)
				return null;

			string s = value as string;
			if (s == null || s.Length != 1)
				throw new ArgumentException("A single character was expected", "value");

			return s[0];
		}

		/// <summary>
		/// Reads an XmlDocument from a column.
		/// </summary>
		/// <param name="value">The value to convert to an XmlDocument.</param>
		/// <returns>The XmlDocument.</returns>
		public static XmlDocument ReadXmlDocument(object value)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(value.ToString());
			return doc;
		}

		/// <summary>
		/// Reads an XDocument from a column.
		/// </summary>
		/// <param name="value">The value to convert to an XDocument.</param>
		/// <returns>The XDocument.</returns>
		public static XDocument ReadXDocument(object value)
		{
			return XDocument.Parse(value.ToString(), LoadOptions.None);
		}

		/// <summary>
		/// Reads an object from an Xml column.
		/// </summary>
		/// <param name="value">The value to deserialize.</param>
		/// <param name="type">The type to deserialize to.</param>
		/// <returns>The deserialized object.</returns>
		public static object DeserializeXml(object value, Type type)
		{
			DataContractSerializer serializer = new DataContractSerializer(type);

			StringReader reader = new StringReader(value.ToString());
			XmlTextReader xr = new XmlTextReader(reader);
			return serializer.ReadObject(xr);
		}
		#endregion

		#region Code Generation Helpers
		/// <summary>
		/// Attempt to find a valid conversion method.
		/// </summary>
		/// <param name="sourceType">The source type for the conversion.</param>
		/// <param name="targetType">The target type for the conversion.</param>
		/// <returns>A conversion method or null if none could be found.</returns>
		private static MethodInfo FindConversionMethod(Type sourceType, Type targetType)
		{
			MethodInfo mi = null;
			if (mi == null)
				mi = FindConversionMethod("op_Explicit", targetType, sourceType, targetType);
			if (mi == null)
				mi = FindConversionMethod("op_Implicit", targetType, sourceType, targetType);
			if (mi == null)
				mi = FindConversionMethod("op_Explicit", sourceType, sourceType, targetType);
			if (mi == null)
				mi = FindConversionMethod("op_Implicit", sourceType, sourceType, targetType);

			// if the target type is an enum or nullable, try converting to that
			if (mi == null && Nullable.GetUnderlyingType(targetType) != null)
				mi = FindConversionMethod(sourceType, Nullable.GetUnderlyingType(targetType));
			if (mi == null && targetType.IsEnum)
				return FindConversionMethod(sourceType, Enum.GetUnderlyingType(targetType));

			return mi;
		}

		/// <summary>
		/// Look up a conversion method from a type.
		/// </summary>
		/// <param name="methodName">The name of the method to find.</param>
		/// <param name="searchType">The type to search through.</param>
		/// <param name="sourceType">The source type for the conversion.</param>
		/// <param name="targetType">The target type for the conversion.</param>
		/// <returns>A conversion method or null if none could be found.</returns>
		private static MethodInfo FindConversionMethod(string methodName, Type searchType, Type sourceType, Type targetType)
		{
			var members = searchType.FindMembers(
				MemberTypes.Method,
				BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
				new MemberFilter(
				(_m, filter) =>
				{
					MethodInfo m = _m as MethodInfo;
					if (m.Name != methodName) return false;
					if (m.ReturnType != targetType) return false;
					ParameterInfo[] pi = m.GetParameters();
					if (pi.Length != 1) return false;
					if (pi[0].ParameterType != sourceType) return false;
					return true;
				}),
				null);

			return (MethodInfo)members.FirstOrDefault();
		}

		/// <summary>
		/// Assuming the source and target types are primitives, coerce the types and handle nullable conversions.
		/// </summary>
		/// <param name="il">The il generator.</param>
		/// <param name="sourceType">The source type of data.</param>
		/// <param name="targetType">The type to coerce to.</param>
		/// <returns>True if a coersion was emitted, false otherwise.</returns>
		private static bool EmitCoersion(ILGenerator il, Type sourceType, Type targetType)
		{
			// support auto-converting strings to other types
			if (sourceType == typeof(string))
			{
				if (targetType == typeof(TimeSpan))
				{
					il.Emit(OpCodes.Call, typeof(TimeSpan).GetMethod("Parse", new Type[] { typeof(string) }));
					return true;
				}
				else if (targetType == typeof(DateTime))
				{
					il.Emit(OpCodes.Call, typeof(DateTime).GetMethod("Parse", new Type[] { typeof(string) }));
					return true;
				}
				else if (targetType == typeof(DateTimeOffset))
				{
					il.Emit(OpCodes.Call, typeof(DateTimeOffset).GetMethod("Parse", new Type[] { typeof(string) }));
					return true;
				}
			}

			if (!sourceType.IsPrimitive) return false;
			if (!targetType.IsPrimitive) return false;

			// if the enum is based on a different type of integer than returned, then do the conversion
			if (targetType == typeof(Int32) && sourceType != typeof(Int32)) il.Emit(OpCodes.Conv_I4);
			else if (targetType == typeof(Int64) && sourceType != typeof(Int64)) il.Emit(OpCodes.Conv_I8);
			else if (targetType == typeof(Int16) && sourceType != typeof(Int16)) il.Emit(OpCodes.Conv_I2);
			else if (targetType == typeof(char) && sourceType != typeof(char)) il.Emit(OpCodes.Conv_I1);
			else if (targetType == typeof(sbyte) && sourceType != typeof(sbyte)) il.Emit(OpCodes.Conv_I1);
			else if (targetType == typeof(UInt32) && sourceType != typeof(UInt32)) il.Emit(OpCodes.Conv_U4);
			else if (targetType == typeof(UInt64) && sourceType != typeof(UInt64)) il.Emit(OpCodes.Conv_U8);
			else if (targetType == typeof(UInt16) && sourceType != typeof(UInt16)) il.Emit(OpCodes.Conv_U2);
			else if (targetType == typeof(byte) && sourceType != typeof(byte)) il.Emit(OpCodes.Conv_U1);
			else if (targetType == typeof(bool) && sourceType != typeof(bool)) il.Emit(OpCodes.Conv_U1);
			else if (targetType == typeof(double) && sourceType != typeof(double)) il.Emit(OpCodes.Conv_R8);
			else if (targetType == typeof(float) && sourceType != typeof(float)) il.Emit(OpCodes.Conv_R4);

			return true;
		}
		#endregion
	}
}
