using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// An identity for the schema of a reader. This checks all of the column names and types.
	/// This lets us store schemas in a dictionary and get automatic efficient storage.
	/// </summary>
	class SchemaIdentity : IEquatable<SchemaIdentity>
	{
		#region Fields
		/// <summary>
		/// Information about the columns in the schema.
		/// </summary>
		private List<Tuple<string, Type>> _columns = new List<Tuple<string, Type>>();

		/// <summary>
		/// The hash code of this identity (precalculated).
		/// </summary>
		private int _hashCode;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SchemaIdentity class.
		/// </summary>
		/// <param name="reader">The reader to construct from.</param>
		public SchemaIdentity(IDataReader reader)
			: this(reader.GetSchemaTable())
		{
		}

		/// <summary>
		/// Initializes a new instance of the SchemaIdentity class.
		/// </summary>
		/// <param name="schemaTable">The schema table to analyze.</param>
		public SchemaIdentity(DataTable schemaTable)
		{
			if (schemaTable == null)
				throw new ArgumentNullException("schemaTable", "schemaTable cannot be null. Generally this indicates that the IDataReader did not return any data.");

			SchemaTable = schemaTable;

			unchecked
			{
				_hashCode = 17;

				int length = schemaTable.Rows.Count;
				for (int i = 0; i < length; i++)
				{
					// get the name and type
					string name = (string)schemaTable.Rows[i]["ColumnName"];
					Type fieldType = (Type)schemaTable.Rows[i]["DataType"];

					// update the hash code for the name and type
					_hashCode *= 23;
					_hashCode += name.GetHashCode();
					_hashCode *= 23;
					_hashCode += fieldType.GetHashCode();

					// add the column information to the list
					_columns.Add(new Tuple<string, Type>(name, fieldType));
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		///  Gets the underlying schema table for the identity.
		/// </summary>
		public DataTable SchemaTable { get; private set; }
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

			if (_columns.Count != other._columns.Count)
				return false;

			int length = _columns.Count;
			for (int i = 0; i < length; i++)
			{
				var thisColumn = _columns[i];
				var thatColumn = other._columns[i];

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
