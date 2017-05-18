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

		/// <summary>
		/// The list of columns in the schema as a Tuple of string + Type + IsNullable + IsReadOnly + IsIdentity.
		/// </summary>
		private ColumnIdentity[] _columns;
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

			int columnCount = _columns.Length;
			if (columnCount != other._columns.Length)
				return false;

			for (int i = 0; i < columnCount; i++)
				if (!_columns[i].Equals(other._columns[i]))
					return false;

			return true;
		}

		/// <summary>
		/// Represents a column identity.
		/// </summary>
		class ColumnIdentity : IEquatable<ColumnIdentity>
		{
			/// <summary>
			/// Gets or sets the name of the column.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// Gets or sets the type of the column.
			/// </summary>
			public Type Type { get; set; }
			
			/// <summary>
			/// Gets or sets a value indicating whether the column is nullable.
			/// </summary>
			public bool IsNullable { get; set; }
	
			/// <summary>
			/// Gets or sets a value indicating whether the column is the row identity.
			/// </summary>
			public bool IsIdentity { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the column is readonly.
			/// </summary>
			public bool IsReadOnly { get; set; }

			/// <summary>
			/// Determines whether two columns are equal.
			/// </summary>
			/// <param name="other">The other column.</param>
			/// <returns>True if they are equal.</returns>
			public bool Equals(ColumnIdentity other)
			{
				if (other == null)
					return false;

				if (Name != other.Name)
					return false;
				if (Type != other.Type)
					return false;
				if (IsNullable != other.IsNullable)
					return false;
				if (IsIdentity != other.IsIdentity)
					return false;
				if (IsReadOnly != other.IsReadOnly)
					return false;

				return true;
			}
		}
		#endregion

		#region Initialization
		private void ReadSchema(IDataReader reader)
		{
			int fieldCount = (reader.IsClosed) ? 0 : reader.FieldCount;

			_columns = new ColumnIdentity[fieldCount];

			// if there are no fields, then this is a simple identity
			if (fieldCount == 0)
				return;

			// we have to compare nullable, readonly and identity because it affects bulk copy
			var schemaTable = reader.GetSchemaTable();
			var isNullableColumn = schemaTable.Columns.IndexOf("AllowDbNull");
			var isReadOnlyColumn = schemaTable.Columns.IndexOf("IsReadOnly");
			var isIdentityColumn = schemaTable.Columns.IndexOf("IsIdentity");

			for (int i = 0; i < fieldCount; i++)
			{
				var row = schemaTable.Rows[i];

				var column = new ColumnIdentity()
				{
					Name = reader.GetName(i),
					Type = reader.GetFieldType(i),
					IsNullable = (isNullableColumn == -1) ? false : row.IsNull(isNullableColumn) ? false : Convert.ToBoolean(row[isNullableColumn], CultureInfo.InvariantCulture),
					IsReadOnly = (isReadOnlyColumn == -1) ? false : row.IsNull(isReadOnlyColumn) ? false : Convert.ToBoolean(row[isReadOnlyColumn], CultureInfo.InvariantCulture),
					IsIdentity = (isIdentityColumn == -1) ? false : row.IsNull(isIdentityColumn) ? false : Convert.ToBoolean(row[isIdentityColumn], CultureInfo.InvariantCulture),
				};
				_columns[i] = column;
			}
		}

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
					_hashCode += column.Name.GetHashCode();
					_hashCode *= 23;
					_hashCode += column.Type.GetHashCode();
					_hashCode *= 23;
					if (column.IsNullable)
						_hashCode++;
					_hashCode *= 23;
					if (column.IsReadOnly)
						_hashCode++;
					_hashCode *= 23;
					if (column.IsIdentity)
						_hashCode++;
				}
			}
		}
		#endregion
	}
}
