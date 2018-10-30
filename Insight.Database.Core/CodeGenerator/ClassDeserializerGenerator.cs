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
using Insight.Database.Mapping;
using Insight.Database.Structure;

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
		/// <param name="structure">The structure of the object.</param>
		/// <param name="mappingType">The type of mapping to create.</param>
		/// <returns>
		/// A function that takes an IDataReader and deserializes an object of type T.
		/// The first parameter will be an IDataReader.
		/// If createNewObject is true, the next parameter will be of type T.
		/// If useCallback is true, the next parameter will be an Action&lt;object[]&gt;.
		/// </returns>
		public static Delegate CreateDeserializer(IDataReader reader, Type type, IRecordStructure structure, SchemaMappingType mappingType)
		{
			// process the object graph types
			var subTypes = structure.GetObjectTypes();
			if (subTypes[0] != type)
				throw new ArgumentException("The top-level type of the object graph must match the return type of the object.", "structure");

			// if the graph type is not a graph, or just the object, and we don't want a callback function
			// then just return a one-level graph.
			if (subTypes.Length == 1 && !mappingType.HasFlag(SchemaMappingType.WithCallback))
				return CreateClassDeserializer(type, reader, structure, 0, (reader.IsClosed) ? 0 : reader.FieldCount, mappingType.HasFlag(SchemaMappingType.NewObject));

			// we can't deserialize an object graph in an insert/merge because we don't know whether to create subobjects or leave them null.
			if (!mappingType.HasFlag(SchemaMappingType.NewObject))
				throw new ArgumentException("mappingType must be set to NewObject when deserializing an object graph.", "mappingType");

			// create the graph deserializer
			if (mappingType.HasFlag(SchemaMappingType.WithCallback))
				return CreateGraphDeserializerWithCallback(subTypes, reader, structure, mappingType == SchemaMappingType.ExistingObject);
			else
				return CreateGraphDeserializer(subTypes, reader, structure, mappingType == SchemaMappingType.ExistingObject);
		}

		#region Single Class Deserialization
		/// <summary>
		/// Compiles and returns a method that deserializes class type from the subset of fields of an IDataReader record.
		/// </summary>
		/// <param name="type">The type of object to deserialize.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="structure">The structure of the record being read.</param>
		/// <param name="startColumn">The index of the first column to read.</param>
		/// <param name="columnCount">The number of columns to read.</param>
		/// <param name="createNewObject">True if the method should create a new instance of an object, false to have the object passed in as a parameter.</param>
		/// <returns>If createNewObject=true, then Func&lt;IDataReader, T&gt;.</returns>
		private static Delegate CreateClassDeserializer(Type type, IDataReader reader, IRecordStructure structure, int startColumn, int columnCount, bool createNewObject)
		{
			var method = CreateClassDeserializerDynamicMethod(type, reader, structure, startColumn, columnCount, createNewObject, true, !createNewObject);

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
		/// <param name="structure">The structure of the record being read.</param>
		/// <param name="startColumn">The index of the first column to read.</param>
		/// <param name="columnCount">The number of columns to read.</param>
		/// <param name="createNewObject">True if the method should create a new instance of an object, false to have the object passed in as a parameter.</param>
		/// <param name="isRootObject">True if this object is the root object and should always be created.</param>
		/// <param name="allowBindChild">True if the columns should be allowed to bind to children.</param>
		/// <returns>If createNewObject=true, then Func&lt;IDataReader, T&gt;.</returns>
		/// <remarks>This returns a DynamicMethod so that the graph deserializer can call the methods using IL. IL cannot call the dm after it is converted to a delegate.</remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static DynamicMethod CreateClassDeserializerDynamicMethod(Type type, IDataReader reader, IRecordStructure structure, int startColumn, int columnCount, bool createNewObject, bool isRootObject, bool allowBindChild)
		{
			// if there are no columns detected for the class, then don't deserialize it
			// exception: a parentandchild object in the middle of a child hierarchy
			var genericType = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : null;
			if (columnCount == 0 && !isRootObject && genericType != typeof(ParentAndChild<,>))
				return null;

			var mappings = MapColumns(type, reader, startColumn, columnCount, structure, allowBindChild && isRootObject);

			// need to know the constructor for the object (except for structs)
			bool isStruct = type.GetTypeInfo().IsValueType;
			ConstructorInfo constructor = createNewObject ? SelectConstructor(type) : null;

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
			var localIsNotAllDbNull = il.DeclareLocal(typeof(bool));

			// initialization
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, localIndex);

            /////////////////////////////////////////////////////////////////////
            // read all of the values into local variables
            /////////////////////////////////////////////////////////////////////
			il.BeginExceptionBlock();
			var hasAtLeastOneMapping = false;
            var localValues = new LocalBuilder[mappings.Count];
            for (int index = 0; index < columnCount; index++)
            {
                var mapping = mappings[index];
                if (mapping == null)
                    continue;
				hasAtLeastOneMapping = true;

                var member = mapping.Member;

                localValues[index] = il.DeclareLocal(member.MemberType);

                // need to call IDataReader.GetItem to get the value of the field
                il.Emit(OpCodes.Ldarg_0);
                IlHelper.EmitLdInt32(il, index + startColumn);

                // before we call it, put the current index into the index local variable
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc, localIndex);

                // now call it
                il.Emit(OpCodes.Callvirt, _iDataReaderGetItem);

                // if handling a subobject, we check to see if the value is null
                if (startColumn > 0)
                {
                    var afterNullCheck = il.DefineLabel();
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value"));
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brtrue, afterNullCheck);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Stloc, localIsNotAllDbNull);
                    il.MarkLabel(afterNullCheck);
                }

                // store the value as a local variable in case type conversion fails
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc, localValue);

				// convert the value and store it locally
				Type sourceType = reader.GetFieldType(index + startColumn);
                TypeConverterGenerator.EmitConvertValue(il, member.Name, sourceType, member.MemberType, mapping.Serializer);
                il.Emit(OpCodes.Stloc, localValues[index]);
            }

            /////////////////////////////////////////////////////////////////////
            // catch translation exceptions and rethrow
            /////////////////////////////////////////////////////////////////////
            il.BeginCatchBlock(typeof(Exception));						// stack => [Exception]
            il.Emit(OpCodes.Ldloc, localIndex);							// push loc.0, stack => [Exception][index]
            il.Emit(OpCodes.Ldarg_0);									// push arg.0, stack => [Exception][index][reader]
            il.Emit(OpCodes.Ldloc, localValue);							// push loc.3, stack => [Exception][index][reader][value]
            il.Emit(OpCodes.Call, TypeConverterGenerator.CreateDataExceptionMethod);
            il.Emit(OpCodes.Throw);									// stack => DataException
            il.EndExceptionBlock();

            /////////////////////////////////////////////////////////////////////
            // if this was a subobject and all of the values are null, then return the default for the object
            /////////////////////////////////////////////////////////////////////
            if (hasAtLeastOneMapping && startColumn > 0)
            {
                var afterNullExit = il.DefineLabel();
                il.Emit(OpCodes.Ldloc, localIsNotAllDbNull);
                il.Emit(OpCodes.Brtrue, afterNullExit);
                TypeHelper.EmitDefaultValue(il, type);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(afterNullExit);
            }

            /////////////////////////////////////////////////////////////////////
            // call the constructor
            /////////////////////////////////////////////////////////////////////
            if (createNewObject)
            {
                if (isStruct)
                {
					il.Emit(OpCodes.Ldloca_S, localResult);
                    il.Emit(OpCodes.Initobj, type);
                    if (constructor != null)
                        il.Emit(OpCodes.Ldloca_S, localResult);
                }

                // if there is a constructor, then populate the values
                if (constructor != null)
                {
                    foreach (var p in constructor.GetParameters())
                    {
                        var mapping = mappings.Where(m => m != null).SingleOrDefault(m => m.Member.Name.IsIEqualTo(p.Name));
                        if (mapping != null)
                            il.Emit(OpCodes.Ldloc, localValues[mappings.IndexOf(mapping)]);
                        else
                            TypeHelper.EmitDefaultValue(il, p.ParameterType);
                    }
                }

                if (isStruct)
                {
                    if (constructor != null)
                        il.Emit(OpCodes.Call, constructor);
                }
                else
                {
                    il.Emit(OpCodes.Newobj, constructor);
                    il.Emit(OpCodes.Stloc, localResult);
                }
            }
            else
            {
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stloc, localResult);
            }

            /////////////////////////////////////////////////////////////////////
            // for anything not passed to the constructor, copy the local values to the properties
            /////////////////////////////////////////////////////////////////////
			for (int index = 0; index < columnCount; index++)
			{
				var mapping = mappings[index];
				if (mapping == null)
					continue;

				var member = mapping.Member;
				if (!member.CanSetMember)
					continue;

                // don't set values that have already been set
                if (constructor != null && constructor.GetParameters().Any(p => p.Name.IsIEqualTo(mapping.Member.Name)))
                    continue;

				// load the address of the object we are working on
				if (isStruct)
					il.Emit(OpCodes.Ldloca_S, localResult);
				else
					il.Emit(OpCodes.Ldloc, localResult);

				// for deep mappings, go to the parent of the field that we are trying to set
				var nextLabel = il.DefineLabel();
				if (mapping.IsDeep)
				{
					ClassPropInfo.EmitGetValue(type, mapping.Prefix, il);

					// if the mapping parent is nullable, check to see if it is null.
					// if so, pop the parent off the stack and move to the next field
					if (!ClassPropInfo.FindMember(type, mapping.Prefix).MemberType.GetTypeInfo().IsValueType)
					{
						var notNullLabel = il.DefineLabel();
						il.Emit(OpCodes.Dup);
						il.Emit(OpCodes.Brtrue, notNullLabel);
						il.Emit(OpCodes.Pop);
						il.Emit(OpCodes.Br, nextLabel);
						il.MarkLabel(notNullLabel);
					}
				}

                // load the value from the local and set it on the object
                il.Emit(OpCodes.Ldloc, localValues[index]);
                member.EmitSetValue(il);
				il.MarkLabel(nextLabel);

				/////////////////////////////////////////////////////////////////////
				// stack should be [target] and ready for the next column
				/////////////////////////////////////////////////////////////////////
			}

			/////////////////////////////////////////////////////////////////////
			// load the return value from the local variable
			/////////////////////////////////////////////////////////////////////
			il.Emit(OpCodes.Ldloc, localResult);						// ld loc.1 (target), stack => [target]
			il.Emit(OpCodes.Ret);

			// create the function
			return dm;
		}

        /// <summary>
        /// Selects the constructor to use for a given type.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <returns>The constructor to use when creating instances of the object.</returns>
        private static ConstructorInfo SelectConstructor(Type type)
        {
            var allConstructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // allow the developer to specify a constructor to use when loading records from the database
            // or use a single constructor if there is only one
            // or use the default constructor
            var constructor = allConstructors.SingleOrDefault(c => c.GetCustomAttributes(true).OfType<SqlConstructorAttribute>().Any());
            if (constructor == null && allConstructors.Length == 1)
                constructor = allConstructors[0];
            if (constructor == null)
                constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (constructor == null && !type.GetTypeInfo().IsValueType)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a default constructor for type {0}, and there was more than one constructor, but no DbConstructorAttribute was specified.", type.FullName));

            return constructor;
        }

		/// <summary>
		/// Maps the columns.
		/// </summary>
		/// <param name="type">The type being mapped.</param>
		/// <param name="reader">The reader being read.</param>
		/// <param name="startColumn">The start column index.</param>
		/// <param name="columnCount">The number of columns.</param>
		/// <param name="structure">The record structure, which may contain custom mapping.</param>
		/// <param name="allowBindChild">True if the context allows binding children (e.g. Merge Outputs)</param>
		/// <returns>The mapping.</returns>
		private static List<FieldMapping> MapColumns(Type type, IDataReader reader, int startColumn, int columnCount, IRecordStructure structure, bool allowBindChild = false)
		{
			// get the mapping from the reader to the type, excluding deep mappings
			var mapping = ColumnMapping.MapColumns(type, reader, startColumn, columnCount, structure);

			if (!allowBindChild)
			{
                for (int i = 0; i < mapping.Count; i++)
                {
                    if (mapping[i] != null && mapping[i].IsDeep)
                        mapping[i] = null;
                }
			}

			return mapping;
		}
		#endregion

		#region Object Graph Deserializer
		/// <summary>
		/// Creates a deserializer for a graph of objects.
		/// </summary>
		/// <param name="subTypes">The types of the subobjects.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="structure">The structure of the record we are reading.</param>
		/// <param name="allowBindChild">True if the columns should be allowed to bind to children.</param>
		/// <returns>A function that takes an IDataReader and deserializes an object of type T.</returns>
		private static Delegate CreateGraphDeserializer(Type[] subTypes, IDataReader reader, IRecordStructure structure, bool allowBindChild)
		{
			Type type = subTypes[0];
			bool isStruct = type.GetTypeInfo().IsValueType;

			// go through each of the subtypes
			var deserializers = CreateDeserializersForSubObjects(subTypes, reader, structure, allowBindChild);

			// create a new anonymous method that takes an IDataReader and returns T
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
				type,
				new[] { typeof(IDataReader) },
				true);
			var il = dm.GetILGenerator();
			var localObject = il.DeclareLocal(type);

			// keep track of the properties that we have already used
			// the tuple is the level + property
			var usedMethods = new HashSet<Tuple<int, ClassPropInfo>>();

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
					setMethod = GetMatchingMethods(ClassPropInfo.GetMembersForType(subTypes[parent]).Where(m => m.CanSetMember), subTypes[i])
									.Where(m => !usedMethods.Contains(Tuple.Create(parent, m)))
									.OrderBy(m => m.Name)
									.FirstOrDefault();

					// if we didn't find a matching set method, then continue on to the next type in the graph
					if (setMethod == null)
						continue;

					// make sure that at a given level, we only use the method once
					usedMethods.Add(Tuple.Create(parent, setMethod));

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
			return GetMatchingMethods(properties, type).FirstOrDefault();
		}

		private static IEnumerable<ClassPropInfo> GetMatchingMethods(IEnumerable<ClassPropInfo> properties, Type type)
		{
			// NOTE: for a subtype match, we can't bind to object, it's not specific enough, it also prevents Guardian<T> from working
			return properties.Where(s => type == s.MemberType || (s.MemberType != typeof(object) && type.IsSubclassOf(s.MemberType)));
		}
		#endregion

		#region Object Graph Deserializer With Custom Object-Assembly Callback
		/// <summary>
		/// Creates a deserializer for a graph of objects. The objects are allocated to an array of objects and passed to a callback that assembles the objects.
		/// </summary>
		/// <param name="subTypes">The types of the subobjects.</param>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="structure">The structure of the record we are reading.</param>
		/// <param name="allowBindChild">True if the columns should be allowed to bind to children.</param>
		/// <returns>A function that takes an IDataReader and deserializes an object of type T.</returns>
		private static Delegate CreateGraphDeserializerWithCallback(Type[] subTypes, IDataReader reader, IRecordStructure structure, bool allowBindChild)
		{
			Type type = subTypes[0];

			// go through each of the subtypes
			var deserializers = CreateDeserializersForSubObjects(subTypes, reader, structure, allowBindChild);

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
		/// <param name="structure">The structure of the record we are reading.</param>
		/// <param name="allowBindChild">True if the columns should be allowed to bind to children.</param>
		/// <returns>An array of delegates.</returns>
		private static DynamicMethod[] CreateDeserializersForSubObjects(Type[] subTypes, IDataReader reader, IRecordStructure structure, bool allowBindChild)
		{
			// take up to the first NoClass
			int subTypeCount = subTypes.Length;
			DynamicMethod[] deserializers = new DynamicMethod[subTypeCount];

			int column = 0;
			for (int i = 0; i < subTypeCount; i++)
			{
				// determine the end of this type and the beginning of the next
				int endColumn = DetectEndColumn(reader, structure, column, subTypes, i);

				// generate a deserializer for the class
				var subType = subTypes[i];
				if (TypeHelper.IsAtomicType(subType))
					deserializers[i] = CreateValueDeserializer(subType, column);
				else
					deserializers[i] = CreateClassDeserializerDynamicMethod(subType, reader, structure, column, endColumn - column, true, (i == 0), (i == 0) && allowBindChild);

				column = endColumn;
			}

			return deserializers;
		}

		/// <summary>
		/// Detect the boundary between tOther and tNext in the reader.
		/// </summary>
		/// <param name="reader">The reader to analyze.</param>
		/// <param name="structure">The structure of the record we are reading.</param>
		/// <param name="columnIndex">The index of the next column to look at.</param>
		/// <param name="types">The list of types to be deserialized.</param>
		/// <param name="typeIndex">The index of the current type being deserialized.</param>
		/// <returns>The end boundary for the current object.</returns>
		private static int DetectEndColumn(
			IDataReader reader,
			IRecordStructure structure,
			int columnIndex,
			Type[] types,
			int typeIndex)
		{
			Type currentType = types[typeIndex];
			Type nextType = (typeIndex + 1 < types.Length) ? types[typeIndex + 1] : typeof(object);

			// if the current type is atomic, it's only one column wide
			if (TypeHelper.IsAtomicType(currentType))
				return columnIndex + 1;

			// if the caller specified columns to split on then use that
			var splitColumns = structure.GetSplitColumns();
			if (splitColumns != null)
			{
				// go through all of the remaining types
				for (int t = typeIndex + 1; t < types.Length; t++)
				{
					// get the column name for the id of the next type
					string columnName;
					if (!splitColumns.TryGetValue(types[t], out columnName))
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

			var currentSetters = MapColumns(currentType, reader, columnIndex, columnsLeft, structure);

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
					// if the next type is an atomic type, then read it
					if (TypeHelper.IsAtomicType(types[t]))
						return columnIndex + i;

					// one of the next types can claim the column, so quit now
					var nextSetters = MapColumns(types[t], reader, columnIndex + i, 1, structure);
					if (nextSetters[0] != null)
						return columnIndex + i;
				}
			}

			return columnIndex + i;
		}

		/// <summary>
		/// Create a deserializer that just reads a value.
		/// </summary>
		/// <param name="type">The type to read.</param>
		/// <param name="column">The index of column to read.</param>
		/// <returns>A method that reads the value.</returns>
		private static DynamicMethod CreateValueDeserializer(Type type, int column)
		{
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Deserialize-{0}-{1}", type.FullName, Guid.NewGuid()),
				type,
				new[] { typeof(IDataReader) },
				true);

			// get the il generator and put some local variables on the stack
			var il = dm.GetILGenerator();

			// need to call IDataReader.GetItem to get the value of the field
			il.Emit(OpCodes.Ldarg_0);							// push arg.0 (reader), stack => [target][reader]
			IlHelper.EmitLdInt32(il, column);					// push index, stack => [target][reader][index]
			il.Emit(OpCodes.Callvirt, _iDataReaderGetItem);		// call getItem, stack => [target][value-as-object]

			il.Emit(OpCodes.Call, typeof(ClassDeserializerGenerator).GetMethod("DBValueToT", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
			il.Emit(OpCodes.Ret);

			return dm;
		}

		/// <summary>
		/// Helper method to convert a database value to a given type.
		/// </summary>
		/// <typeparam name="T">The target type.</typeparam>
		/// <param name="value">The database value.</param>
		/// <returns>The converted value.</returns>
		private static T DBValueToT<T>(object value)
		{
			if (value == DBNull.Value)
				return default(T);
			else
				return (T)value;
		}
		#endregion
	}
}
