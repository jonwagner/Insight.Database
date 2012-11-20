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
	/// <typeparam name="T">The type to deserialize.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class DbReaderDeserializer<T>
	{
		#region Private Properties
		/// <summary>
		/// The cache for the deserializers that deserialize into new objects.
		/// </summary>
        private static ConcurrentDictionary<SchemaMappingIdentity, Delegate> _deserializers = new ConcurrentDictionary<SchemaMappingIdentity, Delegate>();

		/// <summary>
		/// The cache for mergers that deserialize into existing objects.
		/// </summary>
		private static ConcurrentDictionary<SchemaMappingIdentity, Func<IDataReader, T, T>> _mergers = new ConcurrentDictionary<SchemaMappingIdentity, Func<IDataReader, T, T>>();
		#endregion

		#region Code Cache Members
        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, T> GetDeserializer(IDataReader reader, Type withGraph = null, Dictionary<Type, string> idColumns = null)
        {
            return (Func<IDataReader, T>)GetDeserializer(reader, withGraph, idColumns, useCallback: false);
        }

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, Action<object[]>, T> GetDeserializerWithCallback(IDataReader reader, Type withGraph = null, Dictionary<Type, string> idColumns = null)
        {
            return (Func<IDataReader, Action<object[]>, T>)GetDeserializer(reader, withGraph, idColumns, useCallback: true);
        }

        /// <summary>
		/// Get a deserializer to read the fields of class T from the given reader into an existing object.
		/// </summary>
        /// <remarks>This is the same as a deserializer, except it is passed an existing object rather than creating a new object.</remarks>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>A function that can deserialize a T from the reader.</returns>
		public static Func<IDataReader, T, T> GetMerger(IDataReader reader)
		{
			// get the class deserializer
			SchemaMappingIdentity identity = new SchemaMappingIdentity(reader, withGraph: typeof(T), useCallback: false);

			// try to get the deserializer. if not found, create one.
			return _mergers.GetOrAdd(
				identity,
				key =>
				{
                    // TODO: it would be nice to be able to deserialize sub-objects
                    return (Func<IDataReader, T, T>)ClassDeserializerGenerator.CreateDeserializer<T>(reader, null, createNewObject: false, withGraph: null, useCallback: false);
				});
		}

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="withGraph">An optional type representing the object graph to deserialize.</param>
        /// <param name="idColumns">An optional dictionary of the ID columns of the types.</param>
        /// <param name="useCallback">True to generate a delegate that can take an object callback.</param>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        private static Delegate GetDeserializer(IDataReader reader, Type withGraph = null, Dictionary<Type, string> idColumns = null, bool useCallback = false)
        {
            // get the class deserializer
            SchemaMappingIdentity identity = new SchemaMappingIdentity(reader, withGraph, useCallback);

            // try to get the deserializer. if not found, create one.
            return _deserializers.GetOrAdd(
                identity,
                key =>
                {
                    if (typeof(T) == typeof(FastExpando))
                        return GetDynamicDeserializer(reader, () => new FastExpando());
                    else if (typeof(T) == typeof(ExpandoObject))
                        return GetDynamicDeserializer(reader, () => new ExpandoObject());
                    else if (typeof(T) == typeof(XmlDocument))
                        return GetXmlDocumentDeserializer();
                    else if (typeof(T) == typeof(XDocument))
                        return GetXDocumentDeserializer();
                    else if (typeof(T).IsValueType || typeof(T) == typeof(string))
                        return GetValueDeserializer();
                    else if (typeof(T) == typeof(byte[]))
                        return GetByteArrayDeserializer();
                    else
                        return ClassDeserializerGenerator.CreateDeserializer<T>(reader, idColumns, createNewObject: true, withGraph: withGraph, useCallback: useCallback);
                });
        }
		#endregion

		#region Value Methods
		/// <summary>
		/// Get a deserializer that returns a single value from the return result.
		/// </summary>
		/// <returns>The deserializer to use.</returns>
		private static Func<IDataReader, T> GetValueDeserializer()
		{
			if (typeof(T) == typeof(char))
				return (Func<IDataReader, T>)(object)new Func<IDataReader, char>(r => TypeConverterGenerator.ReadChar(r.GetValue(0)));

			if (typeof(T) == typeof(char?))
				return (Func<IDataReader, T>)(object)new Func<IDataReader, char?>(r => TypeConverterGenerator.ReadNullableChar(r.GetValue(0)));

			return r =>
			{
				object value = r.GetValue(0);
				if (value == DBNull.Value)
					value = null;

				return (T)value;
			};
		}

		/// <summary>
		/// Get a deserializer that returns a single byte array value from the return result.
		/// </summary>
		/// <returns>The deserializer to use.</returns>
		private static Func<IDataReader, T> GetByteArrayDeserializer()
		{
			return r =>
			{
				object value = r.GetValue(0);
				if (value == DBNull.Value)
					value = null;

				return (T)value;
			};
		}
		#endregion

		#region Dynamic Object Methods
		/// <summary>
		/// Get a deserializer for dynamic objects.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="constructor">A constructor to use for new objects.</param>
		/// <returns>A deserializer that returns dynamic objects.</returns>
		private static Func<IDataReader, T> GetDynamicDeserializer(IDataReader reader, Func<object> constructor)
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
				IDictionary<String, Object> o = constructor() as IDictionary<String, Object>;
				if (o == null)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Type {0} must implement IDictionary<String, Object>,", typeof(T).FullName));

				foreach (var field in fields)
				{
					object value = r.GetValue(field.Key);

					// handle null translation
					if (value == DBNull.Value)
						value = null;

					o.Add(field.Value, value);
				}

				return (T)o;
			};
		}
		#endregion

		#region Xml Deserialization Methods
		/// <summary>
		/// Returns a deserializer for an XmlDocument.
		/// </summary>
		/// <returns>A deserializer for an XmlDocument.</returns>
		private static Func<IDataReader, T> GetXmlDocumentDeserializer()
		{
			return reader =>
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(reader.GetString(0));
				return (T)(object)doc;
			};
		}

		/// <summary>
		/// Returns a deserializer for an XDocument.
		/// </summary>
		/// <returns>A deserializer for an XDocument.</returns>
		private static Func<IDataReader, T> GetXDocumentDeserializer()
		{
			return reader =>
			{
				return (T)(object)XDocument.Parse(reader.GetString(0));
			};
		}
		#endregion
	}
	#endregion
}
