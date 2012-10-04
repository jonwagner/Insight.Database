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
	/// Code generator class for deserializing objects from IDataReader.
	/// </summary>
	/// <typeparam name="T">The type to deserialize.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class DbReaderDeserializer<T>
	{
		#region Private Properties
		/// <summary>
		/// The IDataReader.GetItem method. This is initialized in the class constructor.
		/// </summary>
		private static readonly MethodInfo _iDataReaderGetItem;

		/// <summary>
		/// List of all of the methods that can set values.
		/// </summary>
		private static Dictionary<string, ClassPropInfo> _setMethods = new Dictionary<string, ClassPropInfo>();

		/// <summary>
		/// The constructor for type T.
		/// </summary>
		private static ConstructorInfo _constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

		/// <summary>
		/// The cache for the deserializers that deserialize into new objects.
		/// </summary>
		private static ConcurrentDictionary<SchemaIdentity, Func<IDataReader, T>> _deserializers = new ConcurrentDictionary<SchemaIdentity, Func<IDataReader, T>>();

		/// <summary>
		/// The cache for mergers that deserialize into existing objects.
		/// </summary>
		private static ConcurrentDictionary<SchemaIdentity, Func<IDataReader, T, T>> _mergers = new ConcurrentDictionary<SchemaIdentity, Func<IDataReader, T, T>>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes static members of the DbReaderDeserializer class.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "These methods are too complicated to inline")]
		static DbReaderDeserializer()
		{
			// get method info for the method calls we will emit
			_iDataReaderGetItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
							.Where(p => p.GetIndexParameters().Any() && p.GetIndexParameters()[0].ParameterType == typeof(int))
							.Select(p => p.GetGetMethod()).First();

			// if this is an object (and not a value type) get the get properties for the types that we pass in
			if (!typeof(T).IsValueType
				&& typeof(T) != typeof(XmlDocument)
				&& typeof(T) != typeof(XDocument))
			{
				// get all fields in the class
				foreach (var f in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					// see if there is a column attribute defined on the field
					var attribute = f.GetCustomAttributes(typeof(ColumnAttribute), true).OfType<ColumnAttribute>().FirstOrDefault();
					string name = (attribute != null) ? attribute.ColumnName : f.Name;
					name = name.ToUpperInvariant();

					if (!_setMethods.ContainsKey(name))
						_setMethods.Add(name, new ClassPropInfo(typeof(T), f.Name));
				}

				// get all properties in the class
				foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					// see if there is a column attribute defined on the field
					var attribute = p.GetCustomAttributes(typeof(ColumnAttribute), true).OfType<ColumnAttribute>().FirstOrDefault();
					string name = (attribute != null) ? attribute.ColumnName : p.Name;
					name = name.ToUpperInvariant();

					if (!_setMethods.ContainsKey(name))
						_setMethods.Add(name, new ClassPropInfo(typeof(T), p.Name));
				}
			}
		}
		#endregion

		#region Code Cache Members
		/// <summary>
		/// Get a deserializer to read class T from the given reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>A function that can deserialize a T from the reader.</returns>
		public static Func<IDataReader, T> GetDeserializer(IDataReader reader)
		{
			// get the class deserializer
			SchemaIdentity identity = new SchemaIdentity(reader);

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
						return (Func<IDataReader, T>)GetClassDeserializer(reader, createNewObject: true);
				});
		}

		/// <summary>
		/// Get a deserializer to read the fields of class T from the given reader into an existing object.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>A function that can deserialize a T from the reader.</returns>
		public static Func<IDataReader, T, T> GetMerger(IDataReader reader)
		{
			// get the class deserializer
			SchemaIdentity identity = new SchemaIdentity(reader);

			// try to get the deserializer. if not found, create one.
			return _mergers.GetOrAdd(
				identity,
				key =>
				{
					return (Func<IDataReader, T, T>)GetClassDeserializer(reader, createNewObject: false);
				});
		}
		#endregion

		#region Code Generator Methods
		/// <summary>
		/// Compiles and returns a method that deserializes class T from an IDataReader record.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="startBound">The index of the first column to read.</param>
		/// <param name="endBound">The index of the last column to read.</param>
		/// <param name="createNewObject">True if the method should create a new instance of an object, false to have the object passed in.</param>
		/// <returns>If createNewObject=true, then Func&lt;IDataReader, T&gt;.</returns>
		internal static Delegate GetClassDeserializer(IDataReader reader, int startBound, int endBound, bool createNewObject)
		{
			// get the schema table in case we need it
			DataTable schemaTable = reader.GetSchemaTable();

			// get the names of all of the fields in the range
			var names = Enumerable.Range(startBound, endBound - startBound).Select(i => reader.GetName(i)).ToList();

			// convert the list of names into a list of set reflections
			// clone the methods list, since we are only going to use each setter once (i.e. if you return two ID columns, we will only use the first one)
			var setMethods = new Dictionary<string, ClassPropInfo>(_setMethods);
			var setters = new List<ClassPropInfo>();
			foreach (string name in names)
			{
				string n = name.ToUpperInvariant();

				if (setMethods.ContainsKey(n))
				{
					setters.Add(setMethods[n]);
					setMethods.Remove(n);
				}
				else
					setters.Add(null);
			}

			// the method can either be:
			// createNewObject => Func<IDataReader, T>
			// !createNewObject => Func<IDataReader, T, T>
			// create a new anonymous method that takes an IDataReader and returns T
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", typeof(T).FullName, Guid.NewGuid()), 
				typeof(T),
				createNewObject ? new[] { typeof(IDataReader) } : new[] { typeof(IDataReader), typeof(T) }, 
				true);

			// get the il generator and put some local variables on the stack
			var il = dm.GetILGenerator();
			il.DeclareLocal(typeof(int));					// loc.0 = index
			il.DeclareLocal(typeof(T));						// loc.1 = result
			il.DeclareLocal(typeof(string));				// loc.2 = string (for enum processing)

			// initialize index = 0
			il.Emit(OpCodes.Ldc_I4_0);						// push 0
			il.Emit(OpCodes.Stloc_0);						// loc.0 (index) = 0

			// emit a call to the constructor of the object
			if (_constructor == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find default constructor for type {0}", typeof(T).FullName));
			il.BeginExceptionBlock();

			// if we are supposed to create a new object, then new that up, otherwise use the object passed in as an argument 
			if (createNewObject)
				il.Emit(OpCodes.Newobj, _constructor);		// push new T, stack => [target]
			else
				il.Emit(OpCodes.Ldarg_1);					// push arg.1 (T), stack => [target]
			il.Emit(OpCodes.Stloc_1);						// pop loc.1 (result), stack => [empty]

			for (int index = startBound; index < endBound; index++)
			{
				var method = setters[index - startBound];

				// if there is no matching property for this column, then continue
				if (method == null)
					continue;

				// we will need to store the value on the target at the end of the method
				il.Emit(OpCodes.Ldloc_1);							// push loc.1 (target), stack => [target]

				// need to call IDataReader.GetItem to get the value of the field
				il.Emit(OpCodes.Ldarg_0);							// push arg.0 (reader), stack => [target][reader]
				IlHelper.EmitLdInt32(il, index);					// push index, stack => [target][reader][index]
				// before we call it, put the current index into the index local variable
				il.Emit(OpCodes.Dup);								// dup index, stack => [target][reader][index][index]
				il.Emit(OpCodes.Stloc_0);							// pop loc.0 (index), stack => [target][reader][index]
				// now call it
				il.Emit(OpCodes.Callvirt, _iDataReaderGetItem);		// call getItem, stack => [target][value-as-object]

				// determine the type of the object in the recordset
				Type sourceType;
				if ((string)schemaTable.Rows[index]["DataTypeName"] == "xml")
					sourceType = typeof(XmlDocument);
				else
					sourceType = (Type)schemaTable.Rows[index]["DataType"];

				// emit the code to convert the value and set the value on the field
				Label finishLabel = TypeConverterGenerator.EmitConvertAndSetValue(il, sourceType, method);

				/////////////////////////////////////////////////////////////////////
				// if this is the first column of a sub-object and the value is null, then return a null object
				/////////////////////////////////////////////////////////////////////
				if (startBound > 0 && index == startBound)
				{
					il.Emit(OpCodes.Ldnull);							// push null
					il.Emit(OpCodes.Stloc_1);							// store null => loc.1 (target)
				}

				/////////////////////////////////////////////////////////////////////
				// stack should be [target] and ready for the next column
				/////////////////////////////////////////////////////////////////////
				il.MarkLabel(finishLabel);
			}

			/////////////////////////////////////////////////////////////////////
			// catch exceptions and rethrow
			/////////////////////////////////////////////////////////////////////
			il.BeginCatchBlock(typeof(Exception));						// stack => [Exception]
			il.Emit(OpCodes.Ldloc_0);									// push loc.0, stack => [Exception][index]
			il.Emit(OpCodes.Ldarg_0);									// push arg.0, stack => [Exception][index][reader]
			il.Emit(OpCodes.Call, TypeConverterGenerator.CreateDataExceptionMethod);
			il.Emit(OpCodes.Throw);									// stack => DataException
			il.EndExceptionBlock();

			/////////////////////////////////////////////////////////////////////
			// load the return value from the local variable
			/////////////////////////////////////////////////////////////////////
			il.Emit(OpCodes.Ldloc_1);									// ld loc.1 (target), stack => [target]
			il.Emit(OpCodes.Ret);

			// create the function
			return dm.CreateDelegate(createNewObject ? typeof(Func<IDataReader, T>) : typeof(Func<IDataReader, T, T>));
		}

		/// <summary>
		/// Gets the set methods for the type T.
		/// </summary>
		/// <returns>The set methods for type T.</returns>
		internal static Dictionary<string, ClassPropInfo> GetSetMethods() { return new Dictionary<string, ClassPropInfo>(_setMethods); }

		/// <summary>
		/// Gets the constructor for the type T.
		/// </summary>
		/// <returns>The constructor for type T.</returns>
		internal static ConstructorInfo GetConstructor() { return _constructor; }

		/// <summary>
		/// Compiles and returns a method that deserializes class T from an IDataReader record.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="createNewObject">True to create a deserializer that creates new objects, False to take an object as input.</param>
		/// <returns>A function that reads a T from an IDataReader.</returns>
		private static Delegate GetClassDeserializer(IDataReader reader, bool createNewObject)
		{
			return GetClassDeserializer(reader, 0, reader.FieldCount, createNewObject);
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

	#region Multi-Class Deserializer
	/// <summary>
	/// Deserializes multiple classes from a record.
	/// </summary>
	/// <typeparam name="T">The main type to deserialize and return.</typeparam>
	/// <typeparam name="TSub1">The first subtype.</typeparam>
	/// <typeparam name="TSub2">The second subtype.</typeparam>
	/// <typeparam name="TSub3">The third subtype.</typeparam>
	/// <typeparam name="TSub4">The fourth subtype.</typeparam>
	/// <typeparam name="TSub5">The fifth subtype.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	static class DbReaderDeserializer<T, TSub1, TSub2, TSub3, TSub4, TSub5>
	{
		#region Private Members
		/// <summary>
		/// The set methods for the return class.
		/// </summary>
		private static Dictionary<string, ClassPropInfo> _setMethods = DbReaderDeserializer<T>.GetSetMethods();

		/// <summary>
		/// The cache for the deserializers.
		/// </summary>
		private static ConcurrentDictionary<SchemaIdentity, Func<IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T>> _deserializers =
			new ConcurrentDictionary<SchemaIdentity, Func<IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T>>();
		#endregion

		/// <summary>
		/// Compiles and returns a method that deserializes objects from an IDataReader.
		/// </summary>
		/// <param name="reader">The reader with a schema to analyze.</param>
		/// <param name="useCallback">True to create a generator that links the classes together with a callback function, false to auto-connect them.</param>
		/// <param name="idColumns">A dictionary naming columns that define the start of a new object.</param>
		/// <returns>A function that deserializes T and its sub-objects from an IDataReader.</returns>
		public static Func<IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T> GetDeserializer(
			IDataReader reader,
			bool useCallback = false,
			Dictionary<Type, string> idColumns = null)
		{
			// get an identity for the schema of the reader
			SchemaIdentity identity = new SchemaIdentity(reader);

			// try to get the deserializer. if not found, create one.
			return _deserializers.GetOrAdd(
				identity,
				key =>
				{
					return GetMultiDeserializer(reader, useCallback, idColumns);
				});
		}

		/// <summary>
		/// Returns a multi-class deserializer for the given reader.
		/// </summary>
		/// <param name="reader">The data reader to analyze.</param>
		/// <param name="useCallback">True to use a callback method for sub-object link-up.</param>
		/// <param name="idColumns">The list of column names that mark the beginning of the next object.</param>
		/// <returns>A deserializer.</returns>
		[SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Readability")]
		private static Func<IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T> GetMultiDeserializer(
			IDataReader reader,
			bool useCallback,
			Dictionary<Type, string> idColumns)
		{
			// detect all of the columns and generate the method holder
			int column = 0;
			int nextColumn = column;

			MethodHolder foo = new MethodHolder();
			foo.T = (Func<IDataReader, T>)DbReaderDeserializer<T>.GetClassDeserializer(reader, column, DetectNextColumn<T, TSub1>(idColumns, reader, ref nextColumn), true); column = nextColumn;
			foo.T1 = (Func<IDataReader, TSub1>)((typeof(TSub1) != typeof(NoClass)) ? DbReaderDeserializer<TSub1>.GetClassDeserializer(reader, column, DetectNextColumn<TSub1, TSub2>(idColumns, reader, ref nextColumn), true) : null); column = nextColumn;
			foo.T2 = (Func<IDataReader, TSub2>)((typeof(TSub2) != typeof(NoClass)) ? DbReaderDeserializer<TSub2>.GetClassDeserializer(reader, column, DetectNextColumn<TSub2, TSub3>(idColumns, reader, ref nextColumn), true) : null); column = nextColumn;
			foo.T3 = (Func<IDataReader, TSub3>)((typeof(TSub3) != typeof(NoClass)) ? DbReaderDeserializer<TSub3>.GetClassDeserializer(reader, column, DetectNextColumn<TSub3, TSub4>(idColumns, reader, ref nextColumn), true) : null); column = nextColumn;
			foo.T4 = (Func<IDataReader, TSub4>)((typeof(TSub4) != typeof(NoClass)) ? DbReaderDeserializer<TSub4>.GetClassDeserializer(reader, column, DetectNextColumn<TSub4, TSub5>(idColumns, reader, ref nextColumn), true) : null); column = nextColumn;
			foo.T5 = (Func<IDataReader, TSub5>)((typeof(TSub5) != typeof(NoClass)) ? DbReaderDeserializer<TSub5>.GetClassDeserializer(reader, column, reader.FieldCount, true) : null);

			// create a new anonymous method that takes an IDataReader and returns T
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", typeof(T).FullName, Guid.NewGuid()),
				typeof(T),
				new[] { typeof(MethodHolder), typeof(IDataReader), typeof(Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>) },
				true);
			var il = dm.GetILGenerator();

			///////////////////////////////////////////////////
			// Create a new object and store it into loc.0
			///////////////////////////////////////////////////
			// loc.0 = GetT
			il.DeclareLocal(typeof(T));													// loc.0 = result

			// if we have a callback, then at the end we are going to call: callback (target, t, t1, t2, t3, t4, t5);
			if (useCallback)
				il.Emit(OpCodes.Ldarg_2);												// push callback

			///////////////////////////////////////////////////
			// Read the main object
			///////////////////////////////////////////////////
			if (foo.T != null)
			{
				FieldInfo fieldT = typeof(MethodHolder).GetField("T");
				il.Emit(OpCodes.Ldarg_0);												// push MethodHolder
				il.Emit(OpCodes.Ldfld, fieldT);											// get T delegate
				il.Emit(OpCodes.Ldarg_1);												// push reader
				il.Emit(OpCodes.Callvirt, fieldT.FieldType.GetMethod("Invoke"));		// call f (reader), stack is now T

				il.Emit(OpCodes.Dup);													// stack has T T
				il.Emit(OpCodes.Stloc_0);												// stack now has T
			}

			///////////////////////////////////////////////////
			// emit the code to read sub-objects and set them
			///////////////////////////////////////////////////
			if (foo.T1 != null) EmitReadAndSet<TSub1>(il, "T1", useCallback); else if (useCallback) il.Emit(OpCodes.Ldnull);
			if (foo.T2 != null) EmitReadAndSet<TSub2>(il, "T2", useCallback); else if (useCallback) il.Emit(OpCodes.Ldnull);
			if (foo.T3 != null) EmitReadAndSet<TSub3>(il, "T3", useCallback); else if (useCallback) il.Emit(OpCodes.Ldnull);
			if (foo.T4 != null) EmitReadAndSet<TSub4>(il, "T4", useCallback); else if (useCallback) il.Emit(OpCodes.Ldnull);
			if (foo.T5 != null) EmitReadAndSet<TSub5>(il, "T5", useCallback); else if (useCallback) il.Emit(OpCodes.Ldnull);

			// if we have a callback, then the stack is target, t, t1, t2, t3, t4, t5, so we can call the method			
			// if not, we have an extra T on the stack (we needed it for calling the other get methods anyway)
			if (useCallback)
				il.Emit(OpCodes.Call, typeof(Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>).GetMethod("Invoke"));
			else
				il.Emit(OpCodes.Pop);												// this is the extra t

			///////////////////////////////////////////////////
			// Return the object
			///////////////////////////////////////////////////
			il.Emit(OpCodes.Ldloc_0);												// push loc.1 (result)
			il.Emit(OpCodes.Ret);

			// create the function
			var del = (Func<MethodHolder, IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T>)dm.CreateDelegate(typeof(Func<MethodHolder, IDataReader, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5>, T>));
			return (IDataReader r, Action<T, TSub1, TSub2, TSub3, TSub4, TSub5> callback) => del(foo, r, callback);
		}

		/// <summary>
		/// Emits code to read an object from the stream and then store it in a member.
		/// </summary>
		/// <typeparam name="TOther">The sub type we are deserializing.</typeparam>
		/// <param name="il">The il generator.</param>
		/// <param name="field">The name of the method to invoke.</param>
		/// <param name="hasCallback">True if the operation has a callback.</param>
		private static void EmitReadAndSet<TOther>(ILGenerator il, string field, bool hasCallback)
		{
			// if we don't have a callback, then we are going to store the value directly into the field on T
			if (!hasCallback)
				il.Emit(OpCodes.Dup);

			// invoke the delegate to get the sub-object
			FieldInfo fieldInfo = typeof(MethodHolder).GetField(field);
			il.Emit(OpCodes.Ldarg_0);											// push MethodHolder
			il.Emit(OpCodes.Ldfld, fieldInfo);									// get the delegate
			il.Emit(OpCodes.Ldarg_1);											// push reader
			il.Emit(OpCodes.Call, fieldInfo.FieldType.GetMethod("Invoke"));		// invoke the delegate

			// if we don't have a callback, then we are going to store the value directly into the field on T
			if (!hasCallback)
			{
				ClassPropInfo setMethod = _setMethods.Values.FirstOrDefault(s => s.MemberType == typeof(TOther) && s.SetMethodInfo != null) ??
					_setMethods.Values.FirstOrDefault(s => s.MemberType == typeof(TOther) && s.FieldInfo != null);

				if (setMethod == null)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find set method for type {0} into type {1}", typeof(TOther).FullName, typeof(T).FullName));

				setMethod.EmitSetValue(il);
			}

			// if we DO have a callback, then this just pushed Tx onto the stack, so we have T, T1, T2... building up
		}

		/// <summary>
		/// Detect the boundary between TOther and TNext in the reader.
		/// </summary>
		/// <typeparam name="TOther">The type at index in the reader.</typeparam>
		/// <typeparam name="TNext">The next type we expect.</typeparam>
		/// <param name="idColumns">Caller-specified column to look for.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="index">The index of the next column to look at.</param>
		/// <returns>The end boundary for the current object.</returns>
		private static int DetectNextColumn<TOther, TNext>(Dictionary<Type, string> idColumns, IDataReader reader, ref int index)
		{
			// if the caller specified columns to split on then use that
			string columnName;
			if (idColumns != null && idColumns.TryGetValue(typeof(TNext), out columnName))
			{
				for (; index < reader.FieldCount; index++)
				{
					if (String.Compare(columnName, reader.GetName(index), StringComparison.OrdinalIgnoreCase) == 0)
						return index;
				}

				return index;
			}

			// get the setters for the class and the next class
			var otherSetters = DbReaderDeserializer<TOther>.GetSetMethods();
			var nextSetters = DbReaderDeserializer<TNext>.GetSetMethods();

			// go through the fields and find the next field NOT ALREADY used in other, and ready to use in next
			for (; index < reader.FieldCount; index++)
			{
				string name = reader.GetName(index).ToUpperInvariant();

				// if other still hasn't set the field, then mark it as used and go to the next column
				// if other doesn't need it, but next does, then we found our split
				if (otherSetters.ContainsKey(name))
					otherSetters.Remove(name);
				else if (nextSetters.ContainsKey(name))
					break;
			}

			return index;
		}

		#region Method Holder Class
		/// <summary>
		/// Holds callback methods to invoke.
		/// </summary>
		/// <remarks>
		/// We need to invoke the sub-deserializers. These are dynamic methods. Unfortunately, reflection emit won't let us bind to those directly,
		/// only to compile-time members. So this is a small wrapper to let us call those methods through the delegates.
		/// It should only be a few extra instructions per sub-object.
		/// UPDATE: now that I know how to invoke delegates from IL, we could pass in these delegate as parameters to the deserializer
		/// rather than the method holder, but then the code becomes unreadable. It would only save 2 instructions (i.e. change MethodHolder.get_t to ldarg_3),
		/// so it's not worth it.
		/// </remarks>
		[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Fields have better performance in this case.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This is an internal class.")]
		class MethodHolder
		{
			public Func<IDataReader, T> T;
			public Func<IDataReader, TSub1> T1;
			public Func<IDataReader, TSub2> T2;
			public Func<IDataReader, TSub3> T3;
			public Func<IDataReader, TSub4> T4;
			public Func<IDataReader, TSub5> T5;
		}
		#endregion
	}
	#endregion

	/// <summary>
	/// Represents an empty slot in the deserializer.
	/// </summary>
	class NoClass { }
}
