using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SchemaIdentity class.
        /// </summary>
        /// <param name="reader">The reader to construct from.</param>
        /// <param name="saveSchemaTable">True to save the schema table for future use.</param>
        public SchemaIdentity(IDataReader reader, bool saveSchemaTable)
        {
            // generating the schematable is expensive so only do it for bulk uploads that require it.
            if (saveSchemaTable)
            {
                SchemaTable = reader.GetSchemaTable();

                // SQL Server tells us the precision of the columns
                // but the TDS parser doesn't like the ones set on money, smallmoney and date
                // so we have to override them
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

            List<Tuple<string, Type>> columns = new List<Tuple<string, Type>>();

            // we know that we are going to store this in a hashtable, so pre-calculate the hashcode
            unchecked
            {
                // base the hashcode on the mapping type, target graph, and schema contents
                _hashCode = 0;

                if (reader != null)
                {
                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        Type fieldType = reader.GetFieldType(i);

                        // update the hash code for the name and type
                        _hashCode *= 23;
                        _hashCode += name.GetHashCode();
                        _hashCode *= 23;
                        _hashCode += fieldType.GetHashCode();

                        columns.Add(new Tuple<string, Type>(name, fieldType));
                    }
                }
            }

            Columns = new ReadOnlyCollection<Tuple<string, Type>>(columns);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the SchemaTable associated with the identity.
        /// </summary>
        internal DataTable SchemaTable { get; private set; }

        /// <summary>
        /// Gets the list of columns in the schema as a Tuple of string + Type.
        /// </summary>
        internal ReadOnlyCollection<Tuple<string, Type>> Columns { get; private set; }
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

            int columnCount = Columns.Count;
            if (columnCount != other.Columns.Count)
                return false;
            for (int i = 0; i < columnCount; i++)
            {
                var thisColumn = Columns[i];
                var thatColumn = other.Columns[i];

                if (thisColumn.Item1 != thatColumn.Item1)
                    return false;
                if (thisColumn.Item2 != thatColumn.Item2)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
