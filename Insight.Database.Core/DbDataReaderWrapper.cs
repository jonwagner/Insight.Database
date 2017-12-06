using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database
{
    /// <summary>
    /// Reads an object list as a data reader.
    /// Not intended to be used directly from object code.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class only implements certain members")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "This class only implements certain members")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "This class only implements certain members")]
    public abstract class DbDataReaderWrapper : DbDataReader
    {
        /// <inheritdoc/>
        public override bool GetBoolean(int i)
        {
            return Convert.ToBoolean(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override byte GetByte(int i)
        {
            return Convert.ToByte(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
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
                Array.Copy(array, (int)dataOffset, buffer, bufferOffset, length);

            return length;
        }

        /// <inheritdoc/>
        public override char GetChar(int i)
        {
            return Convert.ToChar(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override DateTime GetDateTime(int i)
        {
            return Convert.ToDateTime(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override double GetDouble(int i)
        {
            return Convert.ToDouble(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override float GetFloat(int i)
        {
            return Convert.ToSingle(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override Guid GetGuid(int i)
        {
            object value = GetValue(i);
            if (value is Guid)
                return (Guid)value;

            return Guid.Parse(value.ToString());
        }

        /// <inheritdoc/>
        public override short GetInt16(int i)
        {
            return Convert.ToInt16(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override int GetInt32(int i)
        {
            return Convert.ToInt32(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override long GetInt64(int i)
        {
            return Convert.ToInt64(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override string GetString(int i)
        {
            return Convert.ToString(GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override bool IsDBNull(int i)
        {
            return GetValue(i) == DBNull.Value;
        }

        /// <inheritdoc/>
        public override object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        /// <inheritdoc/>
        public override object this[int i]
        {
            get { return GetValue(i); }
        }

        /// <inheritdoc/>
        public override string GetDataTypeName(int ordinal)
        {
            var type = GetFieldType(ordinal);
            if (type == null)
                return "unmapped";
            else
                return type.Name;
        }
    }
}
