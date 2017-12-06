using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
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
using Insight.Database.Structure;

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
        /// The cache for the simple deserializers that deserialize into new objects.
        /// </summary>
        private static ConcurrentDictionary<Type, Delegate> _simpleDeserializers = new ConcurrentDictionary<Type, Delegate>();

        /// <summary>
        /// The method to get values from an IDataRecord.
        /// </summary>
        private static MethodInfo _getValueMethod = typeof(IDataRecord).GetMethod("GetValue");

        /// <summary>
        /// The DBNull.Value field.
        /// </summary>
        private static FieldInfo _dbNullField = typeof(DBNull).GetField("Value");

        /// <summary>
        /// Initializes static members of the DbReaderDeserializer class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DbReaderDeserializer()
        {
            // pre-initialize all of the simple serializers for known types so they can be quickly returned.
            _simpleDeserializers.TryAdd(typeof(object), GetDynamicDeserializer<FastExpando, FastExpando>());
            _simpleDeserializers.TryAdd(typeof(FastExpando), GetDynamicDeserializer<FastExpando, FastExpando>());
            _simpleDeserializers.TryAdd(typeof(ExpandoObject), GetDynamicDeserializer<ExpandoObject, ExpandoObject>());
            _simpleDeserializers.TryAdd(typeof(XmlDocument), GetXmlDocumentDeserializer());
            _simpleDeserializers.TryAdd(typeof(XDocument), GetXDocumentDeserializer());
            _simpleDeserializers.TryAdd(typeof(byte[]), GetByteArrayDeserializer());
            _simpleDeserializers.TryAdd(typeof(char), new Func<IDataReader, char>(r => TypeConverterGenerator.ReadChar(r.GetValue(0))));
            _simpleDeserializers.TryAdd(typeof(char?), new Func<IDataReader, char?>(r => TypeConverterGenerator.ReadNullableChar(r.GetValue(0))));
            _simpleDeserializers.TryAdd(typeof(string), GetValueDeserializer<string>());
            _simpleDeserializers.TryAdd(typeof(Guid), GetGuidDeserializer());
            _simpleDeserializers.TryAdd(typeof(Guid?), GetNullableGuidDeserializer());

            _simpleDeserializers.TryAdd(typeof(byte), GetValueDeserializer<byte>());
            _simpleDeserializers.TryAdd(typeof(short), GetValueDeserializer<short>());
            _simpleDeserializers.TryAdd(typeof(int), GetValueDeserializer<int>());
            _simpleDeserializers.TryAdd(typeof(long), GetValueDeserializer<long>());
            _simpleDeserializers.TryAdd(typeof(decimal), GetValueDeserializer<decimal>());
            _simpleDeserializers.TryAdd(typeof(float), GetValueDeserializer<float>());
            _simpleDeserializers.TryAdd(typeof(double), GetValueDeserializer<double>());
            _simpleDeserializers.TryAdd(typeof(DateTime), GetValueDeserializer<DateTime>());
            _simpleDeserializers.TryAdd(typeof(DateTimeOffset), GetValueDeserializer<DateTimeOffset>());
            _simpleDeserializers.TryAdd(typeof(TimeSpan), GetValueDeserializer(typeof(TimeSpan)));

            _simpleDeserializers.TryAdd(typeof(byte?), GetValueDeserializer<byte?>());
            _simpleDeserializers.TryAdd(typeof(short?), GetValueDeserializer<short?>());
            _simpleDeserializers.TryAdd(typeof(int?), GetValueDeserializer<int?>());
            _simpleDeserializers.TryAdd(typeof(long?), GetValueDeserializer<long?>());
            _simpleDeserializers.TryAdd(typeof(decimal?), GetValueDeserializer<decimal?>());
            _simpleDeserializers.TryAdd(typeof(float?), GetValueDeserializer<float?>());
            _simpleDeserializers.TryAdd(typeof(double?), GetValueDeserializer<double?>());
            _simpleDeserializers.TryAdd(typeof(DateTime?), GetValueDeserializer<DateTime?>());
            _simpleDeserializers.TryAdd(typeof(DateTimeOffset?), GetValueDeserializer<DateTimeOffset?>());
            _simpleDeserializers.TryAdd(typeof(TimeSpan?), GetValueDeserializer(typeof(TimeSpan?)));
        }
        #endregion

        #region Code Cache Members
        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="structure">The structure of the objects in the record.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, T> GetDeserializer<T>(IDataReader reader, IRecordStructure structure)
        {
            return (Func<IDataReader, T>)GetDeserializer(reader, typeof(T), structure, SchemaMappingType.NewObject);
        }

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="structure">The structure of the objects in the record.</param>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        public static Func<IDataReader, Action<object[]>, T> GetDeserializerWithCallback<T>(IDataReader reader, IRecordStructure structure)
        {
            return (Func<IDataReader, Action<object[]>, T>)GetDeserializer(reader, typeof(T), structure, SchemaMappingType.NewObjectWithCallback);
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
            var merger = GetDeserializer(reader, typeof(T), OneToOne<T>.Records, SchemaMappingType.ExistingObject);

            // if we have returned a value deserializer, then we can't do a merge
            if (merger is Func<IDataReader, T>)
                return null;

            return (Func<IDataReader, T, T>)merger;
        }

        /// <summary>
        /// Get a deserializer to read class T from the given reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="type">The type of object to deserialize.</param>
        /// <param name="structure">The structure of the objects in the record.</param>
        /// <param name="mappingType">The type of mapping to return.</param>
        /// <returns>A function that can deserialize a T from the reader.</returns>
        private static Delegate GetDeserializer(IDataReader reader, Type type, IRecordStructure structure, SchemaMappingType mappingType)
        {
            if (structure == null) throw new ArgumentNullException("structure");

            // This method should try to return the deserializer with as little work as possible.
            // Calculating the SchemaMappingIdentity is relatively expensive, so we will take care of the simple cases first,
            // Where we can just look up a type in a dictionary.
            // since these types are single column types, deserializing these types do not depend on the schema that comes back from the database
            // we don't need to keep a schema identity for these
            Delegate deserializer = null;
            if (!_simpleDeserializers.TryGetValue(type, out deserializer) && TypeHelper.IsAtomicType(type))
                deserializer = GetValueDeserializer(type);

            // we have a simple deserializer
            if (deserializer != null)
            {
                var genericArgs = structure.GetObjectTypes();
                if (genericArgs.Length != 1 || genericArgs[0] != type)
                    throw new ArgumentException("Column Mapper does not match single column deserialization", "structure");
                return deserializer;
            }

            // at this point, we know that we aren't returning a value type or simple object that doesn't depend on the schema.
            // so we need to calculate a mapping identity and then create or return a deserializer.
            SchemaMappingIdentity identity = new SchemaMappingIdentity(reader, structure, mappingType);

            // try to get the deserializer. if not found, create one.
            return _deserializers.GetOrAdd(
                identity,
                key => ClassDeserializerGenerator.CreateDeserializer(reader, type, structure, mappingType));
        }
        #endregion

        #region Value Methods
        /// <summary>
        /// Get a deserializer that returns a single value from the return result.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <returns>The deserializer to use.</returns>
        private static Func<IDataReader, T> GetValueDeserializer<T>()
        {
            if (typeof(T).GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                return r => (T)r.GetValue(0);
            }
            else
            {
                return r =>
                {
                    object o = r.GetValue(0);
                    if (o == DBNull.Value)
                        return default(T);
                    else
                        return (T)o;
                };
            }
        }

        /// <summary>
        /// Get a deserializer that returns a single value from the return result.
        /// </summary>
        /// <param name="type">The type of object to deserialize.</param>
        /// <returns>The deserializer to use.</returns>
        private static Delegate GetValueDeserializer(Type type)
        {
            return _simpleDeserializers.GetOrAdd(type, t => CreateValueDeserializer(t));
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

            Type underlyingType = Nullable.GetUnderlyingType(type);

            var il = dm.GetILGenerator();
            Label isNull = il.DefineLabel();
            var result = il.DeclareLocal(type);

            // get the value from the reader
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Callvirt, _getValueMethod);

            // see if it's null
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldsfld, _dbNullField);
            il.Emit(OpCodes.Beq, isNull);

            // convert timespans from SQL values to timespans
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                // convert whatever object it is to a boxed timespan
                il.Emit(OpCodes.Call, typeof(TypeConverterGenerator).GetMethod("SqlObjectToTimeSpan"));
            }

            // not null, so unbox it and return it
            if (underlyingType != null)
            {
                // a nullable type, so unbox to the underlying type
                il.Emit(OpCodes.Unbox_Any, underlyingType);
                var temp = il.DeclareLocal(underlyingType);
                il.Emit(OpCodes.Stloc, temp);

                // now create the nullable
                il.Emit(OpCodes.Ldloca, result);
                il.Emit(OpCodes.Ldloc, temp);
                il.Emit(OpCodes.Call, type.GetConstructor(new Type[] { underlyingType }));

                // return the nullable
                il.Emit(OpCodes.Ldloc, result);
            }
            else
            {
                // not a nullable type, so just unbox the type
                il.Emit(OpCodes.Unbox_Any, type);
            }

            il.Emit(OpCodes.Ret);

            // is null, so return a default
            il.MarkLabel(isNull);
            il.Emit(OpCodes.Pop);
            if (type.GetTypeInfo().IsValueType)
            {
                // return default(T)
                il.Emit(OpCodes.Ldloca, result);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, result);
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

        /// <summary>
        /// Get a deserializer that returns a single Guid value from the return result.
        /// </summary>
        /// <returns>The deserializer to use.</returns>
        private static Func<IDataReader, Guid> GetGuidDeserializer()
        {
            return r =>
            {
                if (r.IsDBNull(0))
                    return Guid.Empty;

                object value = r.GetValue(0);
                if (value is Guid)
                    return (Guid)value;

                return Guid.Parse(value.ToString());
            };
        }

        /// <summary>
        /// Get a deserializer that returns a single Guid value from the return result.
        /// </summary>
        /// <returns>The deserializer to use.</returns>
        private static Func<IDataReader, Guid?> GetNullableGuidDeserializer()
        {
            Func<IDataReader, Guid> guidDeserializer = GetGuidDeserializer();

            return r =>
            {
                if (r.IsDBNull(0))
                    return null;

                return guidDeserializer(r);
            };
        }
        #endregion

        #region Dynamic Object Methods
        /// <summary>
        /// Get a deserializer for dynamic objects.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <typeparam name="TImpl">The type of object to use to implement the requested type.</typeparam>
        /// <returns>A deserializer that returns dynamic objects.</returns>
        private static Func<IDataReader, T> GetDynamicDeserializer<T, TImpl>() where TImpl : T, IDictionary<String, Object>, new()
        {
            return r =>
            {
                // we need it to implement IDictionary so we can set the properties
                T t = new TImpl();
                IDictionary<String, Object> o = (IDictionary<String, Object>)t;

                // get all of the values at once to avoid overhead
                int fieldCount = r.FieldCount;
                object[] values = new object[fieldCount];
                r.GetValues(values);

                // stick the values into the array
                for (int i = 0; i < fieldCount; i++)
                {
                    // handle dbnull conversion
                    object value = values[i];
                    if (value == DBNull.Value)
                        value = null;

                    // set the value on the dictionary
                    // GetName is pretty efficient
                    o.Add(r.GetName(i), value);
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
