using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Mapping;
using Insight.Database.Providers;
using Insight.Database.Structure;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Encapsulates the information needed to read from an object list into a given schema.
	/// Not intended to be used by application code.
	/// </summary>
	public class ObjectReader
	{
		#region Fields
		/// <summary>
		/// Global cache of accessors.
		/// </summary>
		private static ConcurrentDictionary<Tuple<SchemaIdentity, Type>, ObjectReader> _readerDataCache = new ConcurrentDictionary<Tuple<SchemaIdentity, Type>, ObjectReader>();

		/// <summary>
		/// Gets an array of methods that can be called to access private members of the given type.
		/// </summary>
		private Func<object, object>[] _accessors;

		/// <summary>
		/// The columns in the virtual table.
		/// </summary>
		private List<ColumnInfo> _columns;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ObjectReader class.
		/// </summary>
		/// <param name="command">The command associated with the reader.</param>
		/// <param name="type">The type of object to read.</param>
		/// <param name="reader">The reader that contains the schema.</param>
		/// <returns>A list of accessor functions to get values from the type.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
		private ObjectReader(IDbCommand command, Type type, IDataReader reader)
		{
			var provider = InsightDbProvider.For(command);
			_columns = new List<ColumnInfo>();

			// create a mapping, and only keep mappings that match our modified schema
			var sourceMappings = ColumnMapping.MapColumns(type, reader).ToList();
			var sourceColumns = ColumnInfo.FromDataReader(reader);

			IsAtomicType = TypeHelper.IsAtomicType(type);
			if (!IsAtomicType)
			{
				int columnCount = sourceMappings.Count;

				var accessors = new List<Func<object, object>>();

				for (int i = 0; i < columnCount; i++)
				{
					var mapping = sourceMappings[i];
					var column = sourceColumns[i];
					if (column.IsReadOnly && !column.IsIdentity)
						continue;

					_columns.Add(column);
					accessors.Add(GenerateAccessor(type, mapping, column));
				}

				_accessors = accessors.ToArray();
			}
			else
			{
				_columns.Add(sourceColumns.First());
				_accessors = new Func<object, object>[] { o => o };
			}
		}
        #endregion

        #region Properties
#if !NO_SCHEMA_TABLE
        /// <summary>
        /// Gets a DataTable containing the expected schema for the type.
        /// </summary>
        public DataTable SchemaTable { get { return ColumnInfo.ToSchemaTable(_columns); } }
#endif

        /// <summary>
        /// Gets the number of fields in the table.
        /// </summary>
        public int FieldCount { get { return _columns.Count; } }

		/// <summary>
		/// Gets a value indicating whether the given type is a value type.
		/// </summary>
		public bool IsAtomicType { get; private set; }
		#endregion

		/// <summary>
		/// Returns an object reader for the given type that matches the given schema.
		/// </summary>
		/// <param name="command">The command associated with the reader.</param>
		/// <param name="reader">The reader containing the schema to analyze.</param>
		/// <param name="type">The type to analyze.</param>
		/// <returns>An ObjectReader for the schema and type.</returns>
		public static ObjectReader GetObjectReader(IDbCommand command, IDataReader reader, Type type)
		{
			SchemaIdentity schemaIdentity = new SchemaIdentity(reader);

			var key = Tuple.Create(schemaIdentity, type);

			return _readerDataCache.GetOrAdd(key, k => new ObjectReader(command, k.Item2, reader));
		}

		/// <summary>
		/// Returns the name of the column with the given ordinal.
		/// </summary>
		/// <param name="ordinal">The ordinal to look up.</param>
		/// <returns>The name of the column, or null if there is no mapping.</returns>
		public string GetName(int ordinal)
		{
			return _columns[ordinal].Name;
		}

		/// <summary>
		/// Returns the ordinal of the first column with the given name.
		/// </summary>
		/// <param name="name">The name to look up.</param>
		/// <returns>The matching ordinal.</returns>
		public int GetOrdinal(string name)
		{
			return _columns.FindIndex(c => String.Compare(name, c.Name, StringComparison.OrdinalIgnoreCase) == 0);
		}

		/// <summary>
		/// Returns the type of the column with the given ordinal.
		/// </summary>
		/// <param name="ordinal">The ordinal to look up.</param>
		/// <returns>The type of the column, or null if there is no mapping.</returns>
		public Type GetType(int ordinal)
		{
			return _columns[ordinal].DataType;
		}

		/// <summary>
		/// Returns a delegate that can access the given data point on an object.
		/// </summary>
		/// <param name="ordinal">The ordinal to look up.</param>
		/// <returns>A delegate that can return the given column.</returns>
		public Func<object, object> GetAccessor(int ordinal)
		{
			if (ordinal >= _accessors.Length)
				return null;

			return _accessors[ordinal];
		}

		/// <summary>
		/// Serializes an object. This is a stub method that reorders parameters.
		/// </summary>
		/// <param name="value">The value to serialize.</param>
		/// <param name="type">The type to serialize.</param>
		/// <param name="serializer">The serializer to use.</param>
		/// <returns>The serialized object.</returns>
		private static object SerializeObject(object value, Type type, IDbObjectSerializer serializer)
		{
			return serializer.SerializeObject(type, value);
		}

		/// <summary>
		/// Generates an accessor for a property in the object.
		/// </summary>
		/// <param name="type">The type of object being read.</param>
		/// <param name="mapping">The column mapping.</param>
		/// <param name="columnInfo">The column information.</param>
		/// <returns>An accessor function or null if there is no mapping.</returns>
		private static Func<object, object> GenerateAccessor(Type type, FieldMapping mapping, ColumnInfo columnInfo)
		{
			if (mapping == null)
				return null;

			ClassPropInfo propInfo = mapping.Member;

			// create a new anonymous method that takes an object and returns the value
			var dm = new DynamicMethod(string.Format(CultureInfo.InvariantCulture, "GetValue-{0}-{1}", type.FullName, Guid.NewGuid()), typeof(object), new[] { typeof(object) }, true);
			var il = dm.GetILGenerator();

			// convert the object reference to the desired type
			il.Emit(OpCodes.Ldarg_0);
            if (type.GetTypeInfo().IsValueType)
            {
                // access the field/property of a value type
                var valueHolder = il.DeclareLocal(type);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc, valueHolder);
                il.Emit(OpCodes.Ldloca_S, valueHolder);
            }
            else
            {
                il.Emit(OpCodes.Isinst, type);                  // cast object -> type
            }

			// get the value from the object
			var readyToSetLabel = il.DefineLabel();
			ClassPropInfo.EmitGetValue(type, mapping.PathToMember, il, readyToSetLabel);
			if (mapping.Member.MemberType.GetTypeInfo().IsValueType)
				il.Emit(OpCodes.Unbox_Any, mapping.Member.MemberType);

			// if the type is nullable, handle nulls
			Type sourceType = propInfo.MemberType;
			Type targetType = columnInfo.DataType;
			Type underlyingType = Nullable.GetUnderlyingType(sourceType);
			if (underlyingType != null)
			{
				// check for not null
				Label notNullLabel = il.DefineLabel();

				var nullableHolder = il.DeclareLocal(propInfo.MemberType);
				il.Emit(OpCodes.Stloc, nullableHolder);
				il.Emit(OpCodes.Ldloca_S, nullableHolder);
				il.Emit(OpCodes.Call, sourceType.GetProperty("HasValue").GetGetMethod());
				il.Emit(OpCodes.Brtrue_S, notNullLabel);

				// it's null, just return null
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ret);

				il.MarkLabel(notNullLabel);

				// it's not null, so unbox to the underlyingtype
				il.Emit(OpCodes.Ldloca, nullableHolder);
				il.Emit(OpCodes.Call, sourceType.GetProperty("Value").GetGetMethod());

				// at this point we have de-nulled value, so use those converters
				sourceType = underlyingType;
			}

			// if the serializer isn't the default ToStringSerializer, and it can serialize the type properly, then use it
			// otherwise we'll use coersion or conversion
			var targetDbType = DbParameterGenerator.LookupDbType(targetType);
			bool canSerialize = sourceType != typeof(string) &&
				mapping.Serializer.GetType() != typeof(ToStringObjectSerializer) &&
				mapping.Serializer.CanSerialize(sourceType, targetDbType);

			if (sourceType != targetType && canSerialize)
			{
				if (sourceType.GetTypeInfo().IsValueType)
					il.Emit(OpCodes.Box, sourceType);
				il.EmitLoadType(sourceType);
				StaticFieldStorage.EmitLoad(il, mapping.Serializer);
				il.Emit(OpCodes.Call, typeof(ObjectReader).GetMethod("SerializeObject", BindingFlags.NonPublic | BindingFlags.Static));
			}
			else
			{
				// attempt to convert the value
				// either way, we are putting it in an object variable, so box it
				if (TypeConverterGenerator.EmitConversionOrCoersion(il, sourceType, targetType))
					il.Emit(OpCodes.Box, targetType);
				else
					il.Emit(OpCodes.Box, sourceType);
			}

			il.MarkLabel(readyToSetLabel);
			il.Emit(OpCodes.Ret);

			return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
		}
	}
}
