using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Generates dynamic IL methods to deserialize an object or object graph from an IDataReader.
	/// </summary>
	static class ClassDeserializerGenerator
	{
		/// <summary>
		/// The IDataReader.GetItem method. This is initialized in the class constructor.
		/// </summary>
		private static readonly MethodInfo _iDataReaderGetItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
							.Where(p => p.GetIndexParameters().Any() && p.GetIndexParameters()[0].ParameterType == typeof(int))
							.Select(p => p.GetGetMethod()).First();

		/// <summary>
		/// Creates a deserializer to deserialize an object from an IDataReader.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="type">The type of object to deserialize from the IDataReader.</param>
		/// <param name="withGraph">The type of the graph of object to be returned, or null/typeof(T) to deserialize just the top-level object.</param>
		/// <param name="idColumns">An optional mapping of type to Id columns used for splitting and object graph.</param>
		/// <param name="mappingType">The type of mapping to create.</param>
		/// <returns>
		/// A function that takes an IDataReader and deserializes an object of type T.
		/// The first parameter will be an IDataReader.
		/// If createNewObject is true, the next parameter will be of type T.
		/// If useCallback is true, the next parameter will be an Action&lt;object[]&gt;.
		/// </returns>
		public static Delegate CreateDeserializer(IDataReader reader, Type type, Type withGraph, Dictionary<Type, string> idColumns, SchemaMappingType mappingType)
		{
			// if the graph isn't specified, look for a defaultgraphattribute, or just do a one-level graph
			if (withGraph == null)
			{
				DefaultGraphAttribute defaultGraph = type.GetCustomAttributes(typeof(DefaultGraphAttribute), true).OfType<DefaultGraphAttribute>().FirstOrDefault();
				if (defaultGraph != null)
					withGraph = defaultGraph.GetGraphTypes()[0];
				else
					withGraph = typeof(Graph<>).MakeGenericType(type);
			}

			// make sure that withGraph is a graph
			if (!withGraph.IsSubclassOf(typeof(Graph)))
				throw new ArgumentException("withGraph passed in must be of Graph<T>", "withGraph");

			// process the object graph types
            Type[] subTypes = Graph.GetGenericArguments(withGraph);
			if (subTypes[0] != type)
				throw new ArgumentException("The top-level type of the object graph must match the return type of the object.", "withGraph");

			// if the graph type is not a graph, or just the object, and we don't want a callback function
			// then just return a one-level graph.
			if (subTypes.Length == 1 && !mappingType.HasFlag(SchemaMappingType.WithCallback))
				return CreateClassDeserializer(type, reader, 0, (reader.IsClosed) ? 0 : reader.FieldCount, mappingType.HasFlag(SchemaMappingType.NewObject));

			// we can't deserialize an object graph in an insert/merge because we don't know whether to create subobjects or leave them null.
			if (!mappingType.HasFlag(SchemaMappingType.NewObject))
				throw new ArgumentException("mappingType must be set to NewObject when deserializing an object graph.", "mappingType");

			// create the graph deserializer
			if (mappingType.HasFlag(SchemaMappingType.WithCallback))
				return CreateGraphDeserializerWithCallback(subTypes, reader, idColumns);
			else
				return CreateGraphDeserializer(subTypes, reader, idColumns);
		}

		#region Single Class Deserialization
		/// <summary>
		/// Compiles and returns a method that deserializes class type from the subset of fields of an IDataReader record.
		/// </summary>
		/// <param name="type">The type of object to deserialize.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="startColumn">The index of the first column to read.</param>
		/// <param name="columnCount">The number of columns to read.</param>
		/// <param name="createNewObject">True if the method should create a new instance of an object, false to have the object passed in as a parameter.</param>
		/// <returns>If createNewObject=true, then Func&lt;IDataReader, T&gt;.</returns>
		private static Delegate CreateClassDeserializer(Type type, IDataReader reader, int startColumn, int columnCount, bool createNewObject)
		{
			var method = CreateClassDeserializerDynamicMethod(type, reader, startColumn, columnCount, createNewObject, true);

			// create a generic type for the delegate we are returning
			Type delegateType;
			if (createNewObject)
				delegateType = typeof(Func<,>).MakeGenericType(typeof(IDataReader), type);
			else
				delegateType = typeof(Func<,,>).MakeGenericType(typeof(IDataReader), type, type);

			return method.CreateDelegate(delegateType);
		}

		/// <summary>
		/// Compiles and returns a method that deserializes class type from the subset of fields of an IDataReader record.
		/// </summary>
		/// <param name="type">The type of object to deserialize.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="startColumn">The index of the first column to read.</param>
		/// <param name="columnCount">The number of columns to read.</param>
		/// <param name="createNewObject">True if the method should create a new instance of an object, false to have the object passed in as a parameter.</param>
		/// <param name="isRootObject">True if this object is the root object and should always be created.</param>
		/// <returns>If createNewObject=true, then Func&lt;IDataReader, T&gt;.</returns>
		/// <remarks>This returns a DynamicMethod so that the graph deserializer can call the methods using IL. IL cannot call the dm after it is converted to a delegate.</remarks>
		private static DynamicMethod CreateClassDeserializerDynamicMethod(Type type, IDataReader reader, int startColumn, int columnCount, bool createNewObject, bool isRootObject)
		{
			// if there are no columns detected for the class, then the deserializer is null
			if (columnCount == 0 && !isRootObject)
				return null;

			// get the mapping from the reader to the type
			ClassPropInfo[] mapping = ColumnMapping.Tables.CreateMapping(type, reader, null, null, null, startColumn, columnCount, true);

			// need to know the constructor for the object (except for structs)
			bool isStruct = type.IsValueType;
			ConstructorInfo constructor = null;
			if (!isStruct)
			{
				constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				if (constructor == null)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a default constructor for type {0}", type.FullName));
			}

			// the method can either be:
			// createNewObject => Func<IDataReader, T>
			// !createNewObject => Func<IDataReader, T, T>
			// create a new anonymous method that takes an IDataReader and returns the given type
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
				type,
				createNewObject ? new[] { typeof(IDataReader) } : new[] { typeof(IDataReader), type },
				true);

			// get the il generator and put some local variables on the stack
			var il = dm.GetILGenerator();
			var localIndex = il.DeclareLocal(typeof(int));
			var localResult = il.DeclareLocal(type);
			var localValue = il.DeclareLocal(typeof(object));

			// initialize index = 0
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, localIndex);

			// emit a call to the constructor of the object
			il.BeginExceptionBlock();

			// if we are supposed to create a new object, then new that up, otherwise use the object passed in as an argument
			// this block sets loc.1 with the return value
			if (isStruct)
			{
				if (createNewObject)
				{
					il.Emit(OpCodes.Ldloca_S, localResult);	// load the pointer to the result on the stack
					il.Emit(OpCodes.Initobj, type);			// initialize the object on the stack
				}
				else
				{
					il.Emit(OpCodes.Ldarg_1);				// store arg.1 => loc.1
					il.Emit(OpCodes.Stloc, localResult);
				}
			}
			else
			{
				if (createNewObject)
					il.Emit(OpCodes.Newobj, constructor);	// push new T, stack => [target]
				else
					il.Emit(OpCodes.Ldarg_1);				// push arg.1 (T), stack => [target]

				il.Emit(OpCodes.Stloc, localResult);		// pop loc.1 (result), stack => [empty]
			}

			var returnLabel = il.DefineLabel();

			for (int index = 0; index < columnCount; index++)
			{
				var method = mapping[index];

				// if there is no matching property for this column, then continue
				if (method == null || !method.CanSetMember)
					continue;

				// store the value as a local variable in case type conversion fails
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Stloc, localValue);

				// load the address of the object we are working on
				if (isStruct)
					il.Emit(OpCodes.Ldloca_S, localResult);			// push pointer to object
				else
					il.Emit(OpCodes.Ldloc, localResult);			// push loc.1 (target), stack => [target]

				// need to call IDataReader.GetItem to get the value of the field
				il.Emit(OpCodes.Ldarg_0);							// push arg.0 (reader), stack => [target][reader]
				IlHelper.EmitLdInt32(il, index + startColumn);		// push index, stack => [target][reader][index]
				// before we call it, put the current index into the index local variable
				il.Emit(OpCodes.Dup);								// dup index, stack => [target][reader][index][index]
				il.Emit(OpCodes.Stloc, localIndex);					// pop loc.0 (index), stack => [target][reader][index]
				// now call it
				il.Emit(OpCodes.Callvirt, _iDataReaderGetItem);		// call getItem, stack => [target][value-as-object]
				// store the value as a local variable in case type conversion fails
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Stloc, localValue);

				// determine the type of the object in the recordset
				Type sourceType = reader.GetFieldType(index + startColumn);

				// emit the code to convert the value and set the value on the field
				Label finishLabel = TypeConverterGenerator.EmitConvertAndSetValue(il, sourceType, method);

				/////////////////////////////////////////////////////////////////////
				// if this is the first column of a sub-object and the value is null, then return a null object
				/////////////////////////////////////////////////////////////////////
				if (startColumn > 0 && index == 0)
				{
					il.Emit(OpCodes.Ldnull);							// push null
					il.Emit(OpCodes.Stloc, localResult);				// store null => loc.1 (target)
					il.Emit(OpCodes.Br, returnLabel);					// exit the loop
				}

				/////////////////////////////////////////////////////////////////////
				// stack should be [target] and ready for the next column
				/////////////////////////////////////////////////////////////////////
				il.MarkLabel(finishLabel);
			}

			il.MarkLabel(returnLabel);

			/////////////////////////////////////////////////////////////////////
			// catch exceptions and rethrow
			/////////////////////////////////////////////////////////////////////
			il.BeginCatchBlock(typeof(Exception));						// stack => [Exception]
			il.Emit(OpCodes.Ldloc, localIndex);							// push loc.0, stack => [Exception][index]
			il.Emit(OpCodes.Ldarg_0);									// push arg.0, stack => [Exception][index][reader]
			il.Emit(OpCodes.Ldloc, localValue);							// push loc.3, stack => [Exception][index][reader][value]
			il.Emit(OpCodes.Call, TypeConverterGenerator.CreateDataExceptionMethod);
			il.Emit(OpCodes.Throw);									// stack => DataException
			il.EndExceptionBlock();

			/////////////////////////////////////////////////////////////////////
			// load the return value from the local variable
			/////////////////////////////////////////////////////////////////////
			il.Emit(OpCodes.Ldloc, localResult);						// ld loc.1 (target), stack => [target]
			il.Emit(OpCodes.Ret);

			// create the function
			return dm;
		}
		#endregion

		#region Object Graph Deserializer
		/// <summary>
		/// Creates a deserializer for a graph of objects.
		/// </summary>
		/// <param name="subTypes">The types of the subobjects.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="idColumns">An optional mapping of type to Id columns used for splitting.</param>
		/// <returns>A function that takes an IDataReader and deserializes an object of type T.</returns>
		private static Delegate CreateGraphDeserializer(Type[] subTypes, IDataReader reader, Dictionary<Type, string> idColumns)
		{
			Type type = subTypes[0];
			bool isStruct = type.IsValueType;

			// go through each of the subtypes
			var deserializers = CreateDeserializersForSubObjects(subTypes, reader, idColumns);

			// create a new anonymous method that takes an IDataReader and returns T
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
				type,
				new[] { typeof(IDataReader) },
				true);
			var il = dm.GetILGenerator();
			var localObject = il.DeclareLocal(type);

			///////////////////////////////////////////////////
			// emit the method
			///////////////////////////////////////////////////
			for (int i = 0; i < deserializers.Length; i++)
			{
				// if there is no deserializer for this object, then skip it
				if (deserializers[i] == null)
					continue;

				// for subobjects, dup the core object so we can set values on it
				if (i > 0)
				{
					if (isStruct)
						il.Emit(OpCodes.Ldloca_S, localObject);
					else
						il.Emit(OpCodes.Ldloc, localObject);
				}

				// if we don't have a callback, then we are going to store the value directly into the field on T or one of the subobjects
				// here we determine the proper set method to store into.
				// we are going to look into all of the types in the graph and find the first parameter that matches our current type
				ClassPropInfo setMethod = null;
				for (int parent = 0; parent < i; parent++)
				{
					// find the set method on the current parent
					setMethod = GetFirstMatchingMethod(ClassPropInfo.GetMembersForType(subTypes[parent]).Where(m => m.CanSetMember), subTypes[i]);

					// if we didn't find a matching set method, then continue on to the next type in the graph
					if (setMethod == null)
						continue;

					// if the parent is not the root object, we have to drill down to the parent, then set the value
					// the root object is already on the stack, so emit a get method to get the object to drill down into
					for (int p = 0; p < parent; p++)
					{
						var getMethod = GetFirstMatchingMethod(ClassPropInfo.GetMembersForType(subTypes[p]).Where(m => m.CanGetMember && m.CanSetMember), subTypes[p + 1]);
						if (getMethod == null)
							throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "In order to deserialize sub-objects, {0} must have a get/set method for type {1}", subTypes[p].FullName, subTypes[p + 1].FullName));
						getMethod.EmitGetValue(il);
					}

					break;
				}

				// call the deserializer for the subobject
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, deserializers[i]);

				// other than the root object, set the value on the parent object
				if (i == 0)
				{
					// store root object in loc.0
					il.Emit(OpCodes.Stloc, localObject);
				}
				else
				{
					if (setMethod == null)
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find set method for type {0} into type {1}", subTypes[i].FullName, subTypes[0].FullName));

					setMethod.EmitSetValue(il);
				}
			}

			// return the object from loc.0
			il.Emit(OpCodes.Ldloc, localObject);
			il.Emit(OpCodes.Ret);

			// convert the dynamic method to a delegate
			var delegateType = typeof(Func<,>).MakeGenericType(typeof(IDataReader), type);
			return dm.CreateDelegate(delegateType);
		}

		/// <summary>
		/// Gets the first get/set method out of a list that has a given membertype.
		/// </summary>
		/// <param name="properties">The list of properties to look through.</param>
		/// <param name="type">The type to look for.</param>
		/// <returns>The first method that has the given type.</returns>
		private static ClassPropInfo GetFirstMatchingMethod(IEnumerable<ClassPropInfo> properties, Type type)
		{
			return properties.FirstOrDefault(s => type == s.MemberType) ??
				properties.FirstOrDefault(s => type.IsSubclassOf(s.MemberType));
		}
		#endregion

		#region Object Graph Deserializer With Custom Object-Assembly Callback
		/// <summary>
		/// Creates a deserializer for a graph of objects. The objects are allocated to an array of objects and passed to a callback that assembles the objects.
		/// </summary>
		/// <param name="subTypes">The types of the subobjects.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="idColumns">An optional mapping of type to Id columns used for splitting.</param>
		/// <returns>A function that takes an IDataReader and deserializes an object of type T.</returns>
		private static Delegate CreateGraphDeserializerWithCallback(Type[] subTypes, IDataReader reader, Dictionary<Type, string> idColumns)
		{
			Type type = subTypes[0];

			// go through each of the subtypes
			var deserializers = CreateDeserializersForSubObjects(subTypes, reader, idColumns);

			// create a new anonymous method that takes an IDataReader and returns T
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
				type,
				new[] { typeof(IDataReader), typeof(Action<object[]>) },
				true);
			var il = dm.GetILGenerator();

			// we have a callback function to call with our objects, so set up the delegate and the array
			var localObject = il.DeclareLocal(type);                        // store the result
			il.Emit(OpCodes.Ldarg_1);                                       // push the delegate
			il.Emit(OpCodes.Ldc_I4, deserializers.Length);                  // create a new array
			il.Emit(OpCodes.Newarr, typeof(object));

			///////////////////////////////////////////////////
			// emit the method
			///////////////////////////////////////////////////
			for (int i = 0; i < deserializers.Length; i++)
			{
				il.Emit(OpCodes.Dup);                                       // duplicate the array 
				il.Emit(OpCodes.Ldc_I4, i);                                 // the index to store to

				// call the deserializer for the subobject
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, deserializers[i]);

				// for the root object, store it in our local variable
				if (i == 0)
				{
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Stloc, localObject);
				}

				il.Emit(OpCodes.Stelem, typeof(object));                    // store the element in the array
			}

			// we have a callback function to call with our objects, so set up the delegate and the array
			il.Emit(OpCodes.Call, typeof(Action<object[]>).GetMethod("Invoke"));        // invoke the delegate

			// return the result
			il.Emit(OpCodes.Ldloc, localObject);							// put the root object back in for return
			il.Emit(OpCodes.Ret);

			// convert the dynamic method to a delegate
			var delegateType = typeof(Func<,,>).MakeGenericType(typeof(IDataReader), typeof(Action<object[]>), type);
			return dm.CreateDelegate(delegateType);
		}
		#endregion

		#region Object Graph Helper Functions
		/// <summary>
		/// Create the deserializers for all of the sub-object types in the graph.
		/// </summary>
		/// <param name="subTypes">The list of sub-object types to parse.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="idColumns">A mapping from type to names of ID columns to be used for splitting.</param>
		/// <returns>An array of delegates.</returns>
		private static DynamicMethod[] CreateDeserializersForSubObjects(Type[] subTypes, IDataReader reader, Dictionary<Type, string> idColumns)
		{
			// take up to the first NoClass
			int subTypeCount = subTypes.Length;
			DynamicMethod[] deserializers = new DynamicMethod[subTypeCount];

			int column = 0;
			for (int i = 0; i < subTypeCount; i++)
			{
				// determine the end of this type and the beginning of the next
				int endColumn = DetectEndColumn(reader, idColumns, column, subTypes, i);

				// generate a deserializer for the class
				deserializers[i] = CreateClassDeserializerDynamicMethod(subTypes[i], reader, column, endColumn - column, createNewObject: true, isRootObject: (i == 0));

				column = endColumn;
			}

			return deserializers;
		}

		/// <summary>
		/// Detect the boundary between tOther and tNext in the reader.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="idColumns">Caller-specified column to look for.</param>
		/// <param name="columnIndex">The index of the next column to look at.</param>
		/// <param name="types">The list of types to be deserialized.</param>
		/// <param name="typeIndex">The index of the current type being deserialized.</param>
		/// <returns>The end boundary for the current object.</returns>
		private static int DetectEndColumn(IDataReader reader, Dictionary<Type, string> idColumns, int columnIndex, Type[] types, int typeIndex)
		{
			Type currentType = types[typeIndex];
			Type nextType = (typeIndex + 1 < types.Length) ? types[typeIndex + 1] : typeof(object);

			// if the caller specified columns to split on then use that
			if (idColumns != null)
			{
				// go through all of the remaining types
				for (int t = typeIndex + 1; t < types.Length; t++)
				{
					// get the column name for the id of the next type
					string columnName;
					if (!idColumns.TryGetValue(types[t], out columnName))
						continue;
						
					for (; columnIndex < reader.FieldCount; columnIndex++)
					{
						if (String.Compare(columnName, reader.GetName(columnIndex), StringComparison.OrdinalIgnoreCase) == 0)
							return columnIndex;
					}
				}
			}

			// get the setters for the class and the next class
			// for the current set, we want to simulate what we will actually use, so we only want to use unique matches
			// for the next set, we want to find all applicable matches, so we can detect the transition to the next object
			int fieldCount = reader.FieldCount;
			int columnsLeft = fieldCount - columnIndex;
			var currentSetters = ColumnMapping.Tables.CreateMapping(currentType, reader, null, null, null, columnIndex, columnsLeft, uniqueMatches: true);

			// go through the remaining types to see if anything will claim the column
			int i = 0;
			for (; columnIndex + i < fieldCount; i++)
			{
				// if there is a setter for the current column, keep going
				if (currentSetters[i] != null)
					continue;

				// there isn't a setting for the column, so see if any other type can claim the column
				for (int t = typeIndex + 1; t < types.Length; t++)
				{
					// one of the next types can claim the column, so quit now
					var nextSetters = ColumnMapping.Tables.CreateMapping(types[t], reader, null, null, null, columnIndex + i, 1, uniqueMatches: false);
					if (nextSetters[0] != null)
						return columnIndex + i;
				}
			}

			return columnIndex + i;
		}
		#endregion
	}
}
