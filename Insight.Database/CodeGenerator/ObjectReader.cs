using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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
		/// Gets an array containing the types of the members of the given type.
		/// </summary>
		private Type[] _memberTypes;

		/// <summary>
		/// Gets an array of methods that can be called to access private members of the given type.
		/// </summary>
		private Func<object, object>[] _accessors;
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

			// copy the schema and fix it
			SchemaTable = reader.GetSchemaTable().Copy();
			FixupSchemaNumericScale();
			FixupSchemaRemoveReadOnlyColumns();

		    IsAtomicType = TypeHelper.IsAtomicType(type);
			if (!IsAtomicType)
			{
				// create a mapping, and only keep mappings that match our modified schema
				var mappings = ColumnMapping.Tables.CreateMapping(type, reader, null, null, null, 0, reader.FieldCount, true)
					.Where(m => m != null).ToArray();

				int columnCount = SchemaTable.Rows.Count;
				_accessors = new Func<object, object>[columnCount];
				_memberTypes = new Type[columnCount];

				for (int i = 0; i < columnCount; i++)
				{
					var columnName = SchemaTable.Rows[i]["ColumnName"].ToString();
					var mapping = mappings.FirstOrDefault(m => String.Compare(m.ColumnName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
					if (mapping == null)
						continue;
					ClassPropInfo propInfo = mapping.ClassPropInfo;

					// create a new anonymous method that takes an object and returns the value
					var dm = new DynamicMethod(string.Format(CultureInfo.InvariantCulture, "GetValue-{0}-{1}", type.FullName, Guid.NewGuid()), typeof(object), new[] { typeof(object) }, true);
					var il = dm.GetILGenerator();

					// convert the object reference to the desired type
					il.Emit(OpCodes.Ldarg_0);
					if (type.IsValueType)
					{
						// access the field/property of a value type
						var valueHolder = il.DeclareLocal(type);
						il.Emit(OpCodes.Unbox_Any, type);
						il.Emit(OpCodes.Stloc, valueHolder);
						il.Emit(OpCodes.Ldloca_S, valueHolder);
					}
					else
						il.Emit(OpCodes.Isinst, type);					// cast object -> type

					// get the value from the object
					propInfo.EmitGetValue(il);

					// if the type is nullable, handle nulls
					Type sourceType = propInfo.MemberType;
					Type targetType = (Type)SchemaTable.Rows[i]["DataType"];
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

					if (sourceType != targetType && !sourceType.IsValueType && sourceType != typeof(string))
					{
						// if the provider type is Xml, then serialize the value
						var serializerMethod = mapping.Serializer.GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(Type) }, null);
						if (serializerMethod == null)
							throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Serializer type {0} needs the method 'public static string Serialize(object, Type)'", mapping.Serializer.Name));

						il.EmitLoadType(sourceType);
						il.Emit(OpCodes.Call, serializerMethod);
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

					il.Emit(OpCodes.Ret);

					_memberTypes[i] = propInfo.MemberType;
					_accessors[i] = (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
				}
			}
			else
			{
				// we are working off a single-column atomic type
				_memberTypes = new Type[1] { type };
				_accessors = new Func<object, object>[] { o => o };
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a DataTable containing the expected schema for the type.
		/// </summary>
		public DataTable SchemaTable { get; private set; }

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
			var row = (DataRow)SchemaTable.Rows[ordinal];
			return (string)row["ColumnName"];
		}

		/// <summary>
		/// Returns the ordinal of the first column with the given name.
		/// </summary>
		/// <param name="name">The name to look up.</param>
		/// <returns>The matching ordinal.</returns>
		public int GetOrdinal(string name)
		{
			for (int i = 0; i < SchemaTable.Rows.Count; i++)
				if (String.Compare(name, GetName(i), StringComparison.OrdinalIgnoreCase) == 0)
					return i;

			return -1;
		}

		/// <summary>
		/// Returns the type of the column with the given ordinal.
		/// </summary>
		/// <param name="ordinal">The ordinal to look up.</param>
		/// <returns>The type of the column, or null if there is no mapping.</returns>
		public Type GetType(int ordinal)
		{
			return _memberTypes[ordinal];
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
		/// SQL Server tells us the precision of the columns
		/// but the TDS parser doesn't like the ones set on money, smallmoney and date
		/// so we have to override them
		/// </summary>
		private void FixupSchemaNumericScale()
		{
			if (SchemaTable.Columns.Contains("DataTypeName"))
			{
				SchemaTable.Columns["NumericScale"].ReadOnly = false;
				foreach (DataRow row in SchemaTable.Rows)
				{
					string dataType = row["DataTypeName"].ToString();
					if (String.Equals(dataType, "money", StringComparison.OrdinalIgnoreCase))
						row["NumericScale"] = 4;
					else if (String.Equals(dataType, "smallmoney", StringComparison.OrdinalIgnoreCase))
						row["NumericScale"] = 4;
					else if (String.Equals(dataType, "date", StringComparison.OrdinalIgnoreCase))
						row["NumericScale"] = 0;
				}
			}
		}

		/// <summary>
		/// Fix the schema by removing any readonly columns and adjusting the column ordinal
		/// </summary>
		private void FixupSchemaRemoveReadOnlyColumns()
		{
			const string ColumnOrdinal = "ColumnOrdinal";

			var isReadOnlyColumn = SchemaTable.Columns.IndexOf("IsReadOnly");
			var isIdentityColumn = SchemaTable.Columns.IndexOf("IsIdentity");

			// remove any mappings for readonly columns, except identities, which we may want to insert
			if (isReadOnlyColumn != -1)
			{
				SchemaTable.Columns[ColumnOrdinal].ReadOnly = false;

				for (int i = 0; i < SchemaTable.Rows.Count; i++)
				{
					var row = SchemaTable.Rows[i];
					row[ColumnOrdinal] = i;

					bool isReadOnly = (isReadOnlyColumn == -1) ? false : row.IsNull(isReadOnlyColumn) ? false : Convert.ToBoolean(row[isReadOnlyColumn], CultureInfo.InvariantCulture);
					bool isIdentity = (isIdentityColumn == -1) ? false : row.IsNull(isIdentityColumn) ? false : Convert.ToBoolean(row[isIdentityColumn], CultureInfo.InvariantCulture);

					if (isReadOnly && !isIdentity)
					{
						SchemaTable.Rows.Remove(row);
						i--;
					}
				}
			}
		}
	}
}
