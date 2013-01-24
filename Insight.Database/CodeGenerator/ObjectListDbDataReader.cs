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
		/// The list of objects to enumerate through.
		/// </summary>
		private IEnumerator _list;

		/// <summary>
		/// The object at the current position.
		/// </summary>
		private object _current;

		/// <summary>
		/// The ordinal of the last column retrieved.
		/// </summary>
		private int _currentOrdinal;

		/// <summary>
		/// Caches the value of the last column retrieved.
		/// </summary>
		private object _currentValue;

		/// <summary>
		/// Caches the ordinal of the last string column retrieved.
		/// </summary>
		private int _currentStringOrdinal;

		/// <summary>
		/// Caches the string value of the last column retrieved.
		/// </summary>
		private string _currentStringValue;

		/// <summary>
		/// Information about how to serialize the object.
		/// </summary>
		private ObjectReader _objectReader;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ObjectListDbDataReader class.
		/// </summary>
		/// <param name="objectReader">The objectReader to use to read the values from an object in the list.</param>
		/// <param name="list">The list of objects.</param>
		public ObjectListDbDataReader(ObjectReader objectReader, IEnumerable list)
		{
			_objectReader = objectReader;
			_schemaTable = objectReader.SchemaTable;
			_list = list.GetEnumerator();
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

				// reset the column that we are currently reading
				_currentOrdinal = -1;
				_currentValue = null;
				_currentStringOrdinal = -1;
				_currentStringValue = null;

				// not allowed to have nulls in the list
				if (_current == null && !_objectReader.IsAtomicType)
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
			// if we have switched columns, get the value
			// do this only once per ordinal, because the object may be doing calculatey things
			if (ordinal != _currentOrdinal)
			{
				_currentOrdinal = ordinal;

				// if we are reading IEnumerable<ValueType>, then there is one column and the value just needs to be converted
				if (_objectReader.IsAtomicType)
				{
					if (ordinal == 0)
						_currentValue = _current;
					else
						throw new ArgumentOutOfRangeException("ordinal");
				}
				else
				{
					// we are reading IEnumerable<ObjectType>, so look up the accessor and call it
					// if there is no accessor, that means the mapper could not find an appropriate column, so make it null
					var accessor = _objectReader.Accessors[ordinal];
					if (accessor != null)
						_currentValue = accessor(_current);
					else
						_currentValue = null;
				}
			}

			// update the current ordinal and return the value
			return _currentValue;
		}

		/// <summary>
		/// Returns the value of a column as a string.
		/// </summary>
		/// <param name="ordinal">The ordinal of the column.</param>
		/// <returns>The string value.</returns>
		public override string GetString(int ordinal)
		{
			// only convert values to strings once
			if (ordinal != _currentStringOrdinal)
			{
				_currentStringOrdinal = ordinal;

				// convert the current value to a string
				object value = GetValue(ordinal);
				_currentStringValue = value as string;

				// if it's not a string and 
				if (_currentStringValue == null && value != null)
				{
					// if the string is a value type, then convert it directly
					// otherwise we serialize the type to xml
					if (value.GetType().IsValueType)
						_currentStringValue = value.ToString();
					else
						_currentStringValue = TypeHelper.SerializeObjectToXml(value, _objectReader.MemberTypes[ordinal]);
				}
			}

			return _currentStringValue;
		}

		/// <summary>
		/// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="ordinal">The ordinal of the column to read.</param>
		/// <param name="dataOffset">The offset into the data to start reading.</param>
		/// <param name="buffer">The buffer to copy characters into.</param>
		/// <param name="bufferOffset">The offset into the buffer to copy into.</param>
		/// <param name="length">The number of characters to copy.</param>
		/// <returns>The number of characters remaining after the read.</returns>
		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			// String.CopyTo can only support Int32 precision, but we shouldn't expect an in-memory string to be larger than that.
			if (dataOffset > Int32.MaxValue)
				throw new ArgumentOutOfRangeException("dataOffset");

			// if the value is not a string and not a primitive type, then serialize it as xml
			string value = GetString(ordinal);

			// if the buffer is null, then just return the length
			if (buffer == null)
				return value.Length;

			// determine the number of characters to read
			length = Math.Min(value.Length - (int)dataOffset, length);

			value.CopyTo((int)dataOffset, buffer, bufferOffset, length);

			return length;
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
			object value = GetValue(ordinal);
			if (value is Guid)
				return (Guid)value;

			return Guid.Parse(value.ToString());
		}

		public override short GetInt16(int ordinal)
		{
			return Convert.ToInt16(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override int GetInt32(int ordinal)
		{
			return Convert.ToInt32(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override long GetInt64(int ordinal)
		{
			return Convert.ToInt64(GetValue(ordinal), CultureInfo.InvariantCulture);
		}

		public override object this[int ordinal]
		{
			get { return GetValue(ordinal); }
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
		#endregion
	}
}
