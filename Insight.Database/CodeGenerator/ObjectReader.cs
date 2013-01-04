using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Encapsulates the information needed to read from an object list into a given schema.
	/// </summary>
	class ObjectReader
	{
		#region Fields
		/// <summary>
		/// Global cache of accessors.
		/// </summary>
		private static ConcurrentDictionary<SchemaMappingIdentity, ObjectReader> _readerDataCache = new ConcurrentDictionary<SchemaMappingIdentity, ObjectReader>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ObjectReader class.
		/// </summary>
		/// <param name="identity">The schema identity to analyze.</param>
		/// <param name="reader">The reader that contains the schema.</param>
		/// <returns>A list of accessor functions to get values from the type.</returns>
		private ObjectReader(SchemaMappingIdentity identity, IDataReader reader)
		{
			// SQL Server tells us the precision of the columns
			// but the TDS parser doesn't like the ones set on money, smallmoney and date
			// so we have to override them
			SchemaTable = reader.GetSchemaTable();
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

			IsValueType = identity.Graph.IsValueType || identity.Graph == typeof(string);
			if (!IsValueType)
			{
				// get the type we are binding to
				Type type = identity.Graph.IsSubclassOf(typeof(Graph)) ? identity.Graph.GetGenericArguments()[0] : identity.Graph;

				var mapping = ColumnMapping.Tables.CreateMapping(type, reader, null, null, null, 0, reader.FieldCount, uniqueMatches: true);

				Accessors = new Func<object, object>[mapping.Length];
				MemberTypes = new Type[mapping.Length];

				for (int i = 0; i < mapping.Length; i++)
				{
					ClassPropInfo propInfo = mapping[i];
					if (propInfo == null)
						continue;

					// create a new anonymous method that takes an object and returns the value
					var dm = new DynamicMethod(string.Format(CultureInfo.InvariantCulture, "GetValue-{0}-{1}", type.FullName, Guid.NewGuid()), typeof(object), new[] { typeof(object) }, true);
					var il = dm.GetILGenerator();

					il.Emit(OpCodes.Ldarg_0);						// push object argument
					il.Emit(OpCodes.Isinst, type);					// cast object -> type

					// get the value from the object
					propInfo.EmitGetValue(il);
					propInfo.EmitBox(il);

					il.Emit(OpCodes.Ret);

					MemberTypes[i] = propInfo.MemberType;
					Accessors[i] = (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets an array of methods that can be called to access private members of the given type.
		/// </summary>
		public Func<object, object>[] Accessors { get; private set; }

		/// <summary>
		/// Gets an array containing the types of the members of the given type.
		/// </summary>
		public Type[] MemberTypes { get; private set; }

		/// <summary>
		/// Gets a DataTable containing the expected schema for the type.
		/// </summary>
		public DataTable SchemaTable { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the given type is a value type.
		/// </summary>
		public bool IsValueType { get; private set; }
		#endregion

		/// <summary>
		/// Returns an object reader for the given type that matches the given schema.
		/// </summary>
		/// <param name="reader">The reader containing the schema to analyze.</param>
		/// <param name="type">The type to analyze.</param>
		/// <returns>An ObjectReader for the schema and type.</returns>
		public static ObjectReader GetObjectReader(IDataReader reader, Type type)
		{
			SchemaIdentity schemaIdentity = new SchemaIdentity(reader);
			SchemaMappingIdentity mappingIdentity = new SchemaMappingIdentity(schemaIdentity, type, null, SchemaMappingType.ExistingObject);

			return _readerDataCache.GetOrAdd(mappingIdentity, i => new ObjectReader(i, reader));
		}
	}
}
