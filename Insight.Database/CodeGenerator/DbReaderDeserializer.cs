using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Insight.Database.CodeGenerator;

namespace Insight.Database.CodeGenerator
{
	#region Single Class Deserializer
	/// <summary>
	/// Class for deserializing different types objects from IDataReader.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class DbReaderDeserializer
	{
		#region Private Properties
		/// <summary>
		/// The cache for the deserializers that deserialize into new objects.
		/// </summary>
        private static ConcurrentDictionary<SchemaMappingIdentity, Delegate> _deserializers = new ConcurrentDictionary<SchemaMappingIdentity, Delegate>();

		/// <summary>
		/// The cache for mergers that deserialize into existing objects.
		/// </summary>
		private static ConcurrentDictionary<SchemaMappingIdentity, Delegate> _mergers = new ConcurrentDictionary<SchemaMappingIdentity, Delegate>();

        /// <summary>
        /// The cache for deserializers that return a type from a single column.
        /// </summary>
        private static ConcurrentDictionary<Type, Delegate> _valueDeserializers = new ConcurrentDictionary<Type, Delegate>();

        /// <summary>
        /// The method to get values from an IDataRecord.
        /// </summary>
        private static MethodInfo _getValueMethod = typeof(IDataRecord).GetMethod("GetValue");
		#endregion

		#region Code Cache Members
        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, T> GetDeserializer<T>(IDataReader reader, Type withGraph = null, Dictionary<Type, string> idColumns = null)
        {
            return (Func<IDataReader, T>)GetDeserializer(reader, typeof(T), withGraph, idColumns, useCallback: false);
        }

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, Action<object[]>, T> GetDeserializerWithCallback<T>(IDataReader reader, Type withGraph = null, Dictionary<Type, string> idColumns = null)
        {
            return (Func<IDataReader, Action<object[]>, T>)GetDeserializer(reader, typeof(T), withGraph, idColumns, useCallback: true);
        }

        /// <summary>
		/// Get a deserializer to read the fields of class T from the given reader into an existing object.
		/// </summary>
        /// <remarks>This is the same as a deserializer, except it is passed an existing object rather than creating a new object.</remarks>
		/// <param name="reader">The reader to read from.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, T, T> GetMerger<T>(IDataReader reader)
		{
			// get the class deserializer
			SchemaMappingIdentity identity = new SchemaMappingIdentity(reader, withGraph: typeof(Graph<T>), useCallback: false);

			// try to get the deserializer. if not found, create one.
            return (Func<IDataReader, T, T>)_mergers.GetOrAdd(
				identity,
				key =>
				{
                    // TODO: it would be nice to be able to deserialize sub-objects
                    return ClassDeserializerGenerator.CreateDeserializer(reader, typeof(T), null, createNewObject: false, withGraph: typeof(Graph<T>), useCallback: false);
				});
		}

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="type">The type of object to deserialize.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <param name="useCallback">True to generate a delegate that can take an object callback.</param>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        private static Delegate GetDeserializer(IDataReader reader, Type type, Type withGraph = null, Dictionary<Type, string> idColumns = null, bool useCallback = false)
        {
            // get the class deserializer
            SchemaMappingIdentity identity = new SchemaMappingIdentity(reader, withGraph ?? type, useCallback);

            // try to get the deserializer. if not found, create one.
            return _deserializers.GetOrAdd(
                identity,
                key =>
                {
                    if (type == typeof(FastExpando))
                        return GetDynamicDeserializer<FastExpando>(reader);
                    else if (type == typeof(ExpandoObject))
                        return GetDynamicDeserializer<ExpandoObject>(reader);
                    else if (type == typeof(XmlDocument))
                        return GetXmlDocumentDeserializer();
                    else if (type == typeof(XDocument))
                        return GetXDocumentDeserializer();
                    else if (type == typeof(char))
                        return new Func<IDataReader, char>(r => TypeConverterGenerator.ReadChar(r.GetValue(0)));
                    else if (type == typeof(char?))
                        return new Func<IDataReader, char?>(r => TypeConverterGenerator.ReadNullableChar(r.GetValue(0)));
                    else if (type == typeof(string))
                        return GetValueDeserializer(typeof(string));
                    else if (type.IsValueType)
                        return GetValueDeserializer(type);
                    else if (type == typeof(byte[]))
                        return GetByteArrayDeserializer();
                    else
                        return ClassDeserializerGenerator.CreateDeserializer(reader, type, idColumns, createNewObject: true, withGraph: withGraph, useCallback: useCallback);
                });
        }
		#endregion

		#region Value Methods
		/// <summary>
		/// Get a deserializer that returns a single value from the return result.
		/// </summary>
        /// <param name="type">The type of object to deserialize.</param>
		/// <returns>The deserializer to use.</returns>
        private static Delegate GetValueDeserializer(Type type)
		{
            return _valueDeserializers.GetOrAdd(type, t => CreateValueDeserializer(t));
        }

        /// <summary>
        /// Creates a deserializer that returns a single value from the return result.
        /// </summary>
        /// <param name="type">The type of object to deserialize.</param>
        /// <returns>The deserializer to use.</returns>
        private static Delegate CreateValueDeserializer(Type type)
        {
            // This is equivalent to
            //  object o = r.GetValue(0); return (o == DbNull.Value) ? default(T) : (T)o;
            // Another way of doing this would be
            //  if IDataReader.Get[thistype] exists, then this is equivalent to:
            //      r.IsDbNull(0) ? default(T) : r.Get[thistype](0)
            //  otherwise it's
            //      r.IsDbNull(0) ? default(T) : (T)r.GetValue(0)
            // But since IsDbNull isn't as trivial as you would think, performance tests show that there is a little bit of extra overhead making that extra call.
            // NOTE: this also just used to be a lambda expression in a generic method, but getting rid of the generic lets us make more flexible code.
            var dm = new DynamicMethod(
                String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
                type,
                new[] { typeof(IDataReader) },
                true);

            var il = dm.GetILGenerator();
            Label isNull = il.DefineLabel();
            if (type.IsValueType)
                il.DeclareLocal(type);

            // get the value from the reader
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Callvirt, _getValueMethod);

            // see if it's null
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value"));
            il.Emit(OpCodes.Beq, isNull);

            // not null, so unbox it and return it
            il.Emit(OpCodes.Unbox_Any, type);
            il.Emit(OpCodes.Ret);

            // is null, so return a default
            il.MarkLabel(isNull);
            il.Emit(OpCodes.Pop);
            if (type.IsValueType)
            {
                // return default(T)
                il.Emit(OpCodes.Ldloca_S, (byte)0);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                // return null
                il.Emit(OpCodes.Ldnull);
            }

            il.Emit(OpCodes.Ret);

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IDataReader), type);
            return dm.CreateDelegate(delegateType);
		}

		/// <summary>
		/// Get a deserializer that returns a single byte array value from the return result.
		/// </summary>
		/// <returns>The deserializer to use.</returns>
        private static Func<IDataReader, byte[]> GetByteArrayDeserializer()
		{
			return r =>
			{
				object value = r.GetValue(0);
				if (value == DBNull.Value)
					value = null;

				return (byte[])value;
			};
		}
		#endregion

		#region Dynamic Object Methods
		/// <summary>
		/// Get a deserializer for dynamic objects.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <returns>A deserializer that returns dynamic objects.</returns>
        private static Func<IDataReader, T> GetDynamicDeserializer<T>(IDataReader reader) where T : IDictionary<String, Object>, new()
		{
			// create a dictionary for the fields and names
			// this will be stored in a closure on the method so it will be reused
			Dictionary<int, string> fields = new Dictionary<int, string>();
			int length = reader.FieldCount;
			for (int i = 0; i < length; i++)
				fields[i] = reader.GetName(i);

			return r =>
			{
				// we need it to implement IDictionary so we can set the properties
                T t = new T();
				IDictionary<String, Object> o = (IDictionary<String, Object>)t;

				foreach (var field in fields)
				{
					object value = r.GetValue(field.Key);

					// handle null translation
					if (value == DBNull.Value)
						value = null;

					o.Add(field.Value, value);
				}

				return t;
			};
		}
		#endregion

		#region Xml Deserialization Methods
		/// <summary>
		/// Returns a deserializer for an XmlDocument.
		/// </summary>
		/// <returns>A deserializer for an XmlDocument.</returns>
        private static Func<IDataReader, XmlDocument> GetXmlDocumentDeserializer()
		{
			return reader =>
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(reader.GetString(0));
				return doc;
			};
		}

		/// <summary>
		/// Returns a deserializer for an XDocument.
		/// </summary>
		/// <returns>A deserializer for an XDocument.</returns>
        private static Func<IDataReader, XDocument> GetXDocumentDeserializer()
		{
			return reader =>
			{
				return XDocument.Parse(reader.GetString(0));
			};
		}
		#endregion
	}
	#endregion
}
