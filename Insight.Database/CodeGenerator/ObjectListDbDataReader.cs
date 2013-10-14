using System;
using System.Collections;
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
		private IEnumerable _enumerable;

		/// <summary>
		/// The current position in the list of objects.
		/// </summary>
		private IEnumerator _enumerator;

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

        /// <summary>
        /// Number of rows read.
        /// </summary>
	    private int _readRowCount;
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
			_enumerable = list;
			_enumerator = list.GetEnumerator();
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
			bool hasItems = _enumerator.MoveNext();
			if (hasItems)
			{
				_current = _enumerator.Current;

				// reset the column that we are currently reading
				_currentOrdinal = -1;
				_currentValue = null;
				_currentStringOrdinal = -1;
				_currentStringValue = null;
			    _readRowCount++;

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

				// we are reading IEnumerable<ObjectType>, so look up the accessor and call it
				// if there is no accessor, that means the mapper could not find an appropriate column, so make it null
				// note that the accessor has code in it that converts to the target schema type (see objectreader)
				// this is much better than letting sql do it later
				var accessor = _objectReader.GetAccessor(ordinal);
				if (accessor != null)
					_currentValue = accessor(_current);
				else
					_currentValue = null;
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
				object value = GetValue(ordinal);

				_currentStringValue = value as string;

				if (_currentStringValue == null && value != null)
					_currentStringValue = value.ToString();
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

			// make sure that the value we retrieve is a string value
			string value = GetString(ordinal);
			if (value == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Invalid attempt to convert column {0} - {1} to a string", ordinal, GetName(ordinal)));

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

#if NET35
			return new Guid(value.ToString());
#else
			return Guid.Parse(value.ToString());
#endif
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
			return _objectReader.GetName(ordinal);
		}

		public override int GetOrdinal(string name)
		{
			return _objectReader.GetOrdinal(name);
		}

		public override void Close()
		{
		}

		public override int Depth
		{
			get { return 0; }
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			// arrays can only have 32-bit indexing
			if (dataOffset >= Int32.MaxValue)
				throw new ArgumentException("dataOffset must be less than Int32.MaxValue", "dataOffset");
			if (length >= Int32.MaxValue)
				throw new ArgumentException("length must be less than Int32.MaxValue", "length");

			// if this is called for a null value, don't copy any bytes
			object value = GetValue(ordinal);
			if (value == null)
				return 0;

			// if this is not a byte array, we will have to add additional implementations
			byte[] array = value as byte[];
			if (array == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Invalid attempt to convert column {0} - {1} to a byte array", ordinal, GetName(ordinal)));

			// if the buffer is null, then just return the length
			if (buffer == null)
				return array.Length;

			// finally copy the data
			length = Math.Min((int)length, array.Length - (int)dataOffset);
			if (length > 0)
				Array.Copy(array, dataOffset, buffer, bufferOffset, length);

			return length;
		}

		public override string GetDataTypeName(int ordinal)
		{
			var type = GetFieldType(ordinal);
			if (type == null)
				return "unmapped";
			else
				return type.Name;
		}

		public override IEnumerator GetEnumerator()
		{
			return _enumerable.GetEnumerator();
		}

		public override Type GetFieldType(int ordinal)
		{
			return _objectReader.GetType(ordinal);
		}

		public override int GetValues(object[] values)
		{
			if (values == null) throw new ArgumentNullException("values");

			int length = Math.Min(values.Length, FieldCount);

			for (int i = 0; i < length; i++)
				values[i] = GetValue(i);

			return length;
		}

		public override bool HasRows
		{
			get { return _enumerable.GetEnumerator().MoveNext(); }
		}

		public override bool IsClosed
		{
			get { return false; }
		}

        /// <summary>
        /// Obtains the number of rows read by the current DbDataReader.
        /// </summary>
		public override int RecordsAffected
		{
            get { return _readRowCount; }
		}

		public override object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}
		#endregion
	}
}
