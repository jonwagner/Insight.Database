using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Insight.Database.CodeGenerator
{
    /// <summary>
    /// An identity for the schema of a reader.
    /// </summary>
    class SchemaIdentity : IEquatable<SchemaIdentity>
    {
        #region Fields
        /// <summary>
        /// The pre-calculated hashcode.
        /// </summary>
        private int _hashCode;

        /// <summary>
        /// The list of columns in the schema as a Tuple of string + Type + IsNullable + IsReadOnly + IsIdentity.
        /// </summary>
        private List<ColumnInfo> _columns;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SchemaIdentity class.
        /// </summary>
        /// <param name="reader">The reader to construct from.</param>
        public SchemaIdentity(IDataReader reader)
        {
            ReadSchema(reader);
            CalculateHashCode();
        }
        #endregion

        #region Equality Members
        /// <summary>
        /// Returns the hash code for the identity.
        /// </summary>
        /// <returns>The hash code for the identity.</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        /// Determines if this is equal to another object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SchemaIdentity);
        }

        /// <summary>
        /// Determines if this is equal to another object.
        /// </summary>
        /// <param name="other">The object to test against.</param>
        /// <returns>True if the objects are equal.</returns>
        public bool Equals(SchemaIdentity other)
        {
            if (other == null)
                return false;

            int columnCount = _columns.Count;
            if (columnCount != other._columns.Count)
                return false;

            for (int i = 0; i < columnCount; i++)
            {
                if (!_columns[i].Equals(other._columns[i]))
                    return false;
            }

            return true;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Reads the schema from a data reader.
        /// </summary>
        /// <param name="reader">The reader to process.</param>
        private void ReadSchema(IDataReader reader)
        {
            _columns = ColumnInfo.FromDataReader(reader);
        }

        /// <summary>
        /// Calculates the hash code for the identity.
        /// </summary>
        private void CalculateHashCode()
        {
            // we know that we are going to store this in a hashtable, so pre-calculate the hashcode
            unchecked
            {
                // base the hashcode on the column names and types
                _hashCode = 17;

                foreach (var column in _columns)
                {
                    // update the hash code for the name and type
                    _hashCode *= 23;
                    _hashCode += column.GetHashCode();
                }
            }
        }
        #endregion
    }
}
