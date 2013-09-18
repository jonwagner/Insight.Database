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
		public SchemaIdentity(IDataReader reader)
		{
			int fieldCount = (reader.IsClosed) ? 0 : reader.FieldCount;

			Columns = new Tuple<string, Type>[fieldCount];

			// we know that we are going to store this in a hashtable, so pre-calculate the hashcode
			unchecked
			{
				// base the hashcode on the column names and types
				_hashCode = 17;

				for (int i = 0; i < fieldCount; i++)
				{
					string name = reader.GetName(i);
					Type fieldType = reader.GetFieldType(i);

					// update the hash code for the name and type
					_hashCode *= 23;
					_hashCode += name.GetHashCode();
					_hashCode *= 23;
					_hashCode += fieldType.GetHashCode();

					Columns[i] = new Tuple<string, Type>(name, fieldType);
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the list of columns in the schema as a Tuple of string + Type.
		/// </summary>
		internal Tuple<string, Type>[] Columns { get; private set; }
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

			int columnCount = Columns.Length;
			if (columnCount != other.Columns.Length)
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
