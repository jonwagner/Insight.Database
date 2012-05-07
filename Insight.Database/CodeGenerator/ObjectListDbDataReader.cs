using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Reads an object list as a data reader.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "This class only implements certain members")]
	class ObjectListDbDataReader : DbDataReader
	{
		#region Private Fields
		/// <summary>
		/// The DataTable representing the structure of the data.
		/// </summary>
		private DataTable _schemaTable;

		/// <summary>
		/// The type of object in the list.
		/// </summary>
		private Type _listType;

		/// <summary>
		/// The list of objects to enumerate through.
		/// </summary>
		private IEnumerator _list;

		/// <summary>
		/// The object at the current position.
		/// </summary>
		private object _current;

		/// <summary>
		/// True when we are processing a list of value types (or strings).
		/// </summary>
		private bool _isValueType;

		private FieldReaderData _readerData;

		/// <summary>
		/// Global cache of accessors.
		/// </summary>
		private static ConcurrentDictionary<Tuple<Type, SchemaIdentity>, FieldReaderData> _readerDataCache = new ConcurrentDictionary<Tuple<Type, SchemaIdentity>, FieldReaderData>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ObjectListDbDataReader class.
		/// </summary>
		/// <param name="schemaTable">The IDbDataReader schema table to use (get it from Sql Server).</param>
		/// <param name="listType">The type of the list to bind to.</param>
		/// <param name="list">The list of objects.</param>
		public ObjectListDbDataReader(DataTable schemaTable, Type listType, IEnumerable list)
		{
			_schemaTable = schemaTable;
			_listType = listType;
			_list = list.GetEnumerator();

			_isValueType = _listType.IsValueType || listType == typeof(string);

			// SQL Server tells us the precision of the columns
			// but the TDS parser doesn't like the ones set on money, smallmoney and date
			// so we have to override them
			_schemaTable.Columns["NumericScale"].ReadOnly = false;
			foreach (DataRow row in _schemaTable.Rows)
			{
				string dataType = row["DataTypeName"].ToString();
				if (String.Equals(dataType, "money", StringComparison.OrdinalIgnoreCase))
					row["NumericScale"] = 4;
				else if (String.Equals(dataType, "smallmoney", StringComparison.OrdinalIgnoreCase))
					row["NumericScale"] = 4;
				else if (String.Equals(dataType, "date", StringComparison.OrdinalIgnoreCase))
					row["NumericScale"] = 0;
			}
 
			// if this is not a value type, then we need to have methods to pull values out of the object
			if (!_isValueType)
			{
				// generate an identity for the schema and a key for our accessors
				SchemaIdentity identity = new SchemaIdentity(schemaTable);
				Tuple<Type, SchemaIdentity> key = new Tuple<Type, SchemaIdentity>(listType, identity);

				_readerData = _readerDataCache.GetOrAdd(key, k => CreateFieldReaderData(k.Item1, k.Item2));
			}
		}
		#endregion

		#region Reflection Members
		/// <summary>
		/// Create accessors to pull data from the object of the given type for the given schema.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="identity">The schema identity to analyze.</param>
		/// <returns>A list of accessor functions to get values from the type.</returns>
		private FieldReaderData CreateFieldReaderData(Type type, SchemaIdentity identity)
		{
			FieldReaderData readerData = new FieldReaderData();

			readerData.Accessors = new List<Func<object, object>>();
			readerData.MemberTypes = new List<Type>();

			for (int i = 0; i < identity.SchemaTable.Rows.Count; i++)
			{
				// get the name of the column
				string name = _schemaTable.Rows[i]["ColumnName"].ToString();

				// create a new anonymous method that takes an object and returns the value
				var dm = new DynamicMethod(string.Format(CultureInfo.InvariantCulture, "GetValue-{0}-{1}", type.FullName, Guid.NewGuid()), typeof(object), new[] { typeof(object) }, true);
				var il = dm.GetILGenerator();

				il.Emit(OpCodes.Ldarg_0);						// push object argument
				il.Emit(OpCodes.Isinst, type);					// cast object -> type

				// get the value from the object
				ClassPropInfo propInfo = new ClassPropInfo(type, name);
				propInfo.EmitGetValue(il);
				propInfo.EmitBox(il);

				il.Emit(OpCodes.Ret);

				readerData.MemberTypes.Add(propInfo.MemberType);
				readerData.Accessors.Add((Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>)));
			}

			return readerData;
		}
		#endregion

		#region Implemented Methods
		/// <summary>
		/// Returns the schema table for the data.
		/// </summary>
		/// <returns>The schema table for the data.</returns>
		public override DataTable GetSchemaTable()
		{
			return _schemaTable;
		}

		/// <summary>
		/// Read the next item from the reader.
		/// </summary>
		/// <returns>True if an item was read, false if there were no more items.</returns>
		public override bool Read()
		{
			// move the enumerator to the next item
			bool hasItems = _list.MoveNext();
			if (hasItems)
			{
				_current = _list.Current;

				// not allowed to have nulls in the list
				if (_current == null && !_isValueType)
					throw new InvalidOperationException("Cannot send a list of objects to a table-valued parameter when the list contains a null value");
			}

			return hasItems;
		}

		/// <summary>
		/// Get the value of the given column.
		/// </summary>
		/// <param name="ordinal">The index of the column.</param>
		/// <returns>The value of the column.</returns>
		public override object GetValue(int ordinal)
		{
			// value types just need to be converted
			if (_isValueType)
			{
				if (ordinal == 0)
					return _current;
				else
					throw new ArgumentOutOfRangeException("ordinal");
			}

			// this is a class type, so look up the accessor and call it
			return _readerData.Accessors[ordinal](_current);
		}

		/// <summary>
		/// Is the value of a column null.
		/// </summary>
		/// <param name="ordinal">The ordinal of the column.</param>
		/// <returns>True if the column value is null.</returns>
		public override bool IsDBNull(int ordinal)
		{
			return GetValue(ordinal) == null;
		}

		/// <summary>
		/// Get the next result set.
		/// </summary>
		/// <returns>False, since this only supports one result set.</returns>
		public override bool NextResult()
		{
			return false;
		}
		#endregion

		#region Stub Methods
		public override int FieldCount
		{
			get { return _schemaTable.Rows.Count; }
		}

		public override bool GetBoolean(int ordinal)
		{
			return Convert.ToBoolean(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override byte GetByte(int ordinal)
		{
			return Convert.ToByte(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override char GetChar(int ordinal)
		{
			return Convert.ToChar(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override DateTime GetDateTime(int ordinal)
		{
			return Convert.ToDateTime(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override decimal GetDecimal(int ordinal)
		{
			return Convert.ToDecimal(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override double GetDouble(int ordinal)
		{
			return Convert.ToDouble(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override float GetFloat(int ordinal)
		{
			return Convert.ToSingle(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override Guid GetGuid(int ordinal)
		{
			return (Guid)GetValue(ordinal);
		}

		public override short GetInt16(int ordinal)
		{
			return (short)GetValue(ordinal);
		}

		public override int GetInt32(int ordinal)
		{
			return (int)GetValue(ordinal);
		}

		public override long GetInt64(int ordinal)
		{
			return (long)GetValue(ordinal);
		}

		public override string GetString(int ordinal)
		{
			object value = GetValue(ordinal);

			// if this is a string, then return it
			string s = value as string;
			if (value == null || s != null)
				return s;

			return (string)TypeHelper.SerializeObjectToXml(value, _readerData.MemberTypes[ordinal]);
		}
		#endregion

		#region Not Implemented Members
		public override string GetName(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override int GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			throw new NotImplementedException();
		}

		public override int Depth
		{
			get { throw new NotImplementedException(); }
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

		public override string GetDataTypeName(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override Type GetFieldType(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public override bool HasRows
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public override int RecordsAffected
		{
			get { throw new NotImplementedException(); }
		}

		public override object this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		public override object this[int ordinal]
		{
			get { throw new NotImplementedException(); }
		}
		#endregion

		private class FieldReaderData
		{
			public List<Func<object, object>> Accessors { get; set; }

			public List<Type> MemberTypes { get; set; }
		}
	}
}
