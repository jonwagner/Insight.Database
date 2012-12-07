using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// An identity for the schema of a reader being mapped to a graph type.
    /// This checks all of the column names and types, as well as the type of the graph.
	/// This lets us store schemas in a dictionary and get automatic efficient storage.
	/// </summary>
	class SchemaMappingIdentity : IEquatable<SchemaMappingIdentity>
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

        /// <summary>
        /// The type of the object graph that the schema is mapped to.
        /// </summary>
        private Type _graph;

        /// <summary>
        /// The type of mapping operation.
        /// </summary>
        private SchemaMappingType _mappingType;

        /// <summary>
        /// The id column override mapping.
        /// </summary>
        private Dictionary<Type, string> _idColumns;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SchemaMappingIdentity class.
		/// </summary>
		/// <param name="reader">The reader to construct from.</param>
        /// <param name="withGraph">The type of the object graph used in this mapping.</param>
        /// <param name="mappingType">The type of mapping operation.</param>
		public SchemaMappingIdentity(IDataReader reader, Type withGraph, Dictionary<Type, string> idColumns, SchemaMappingType mappingType)
			: this(reader.GetSchemaTable(), withGraph, idColumns, mappingType)
		{
		}

		/// <summary>
        /// Initializes a new instance of the SchemaMappingIdentity class.
		/// </summary>
		/// <param name="schemaTable">The schema table to analyze.</param>
        /// <param name="withGraph">The type of the object graph used in this mapping.</param>
        /// <param name="mappingType">The type of mapping operation.</param>
        public SchemaMappingIdentity(DataTable schemaTable, Type withGraph, Dictionary<Type, string> idColumns, SchemaMappingType mappingType)
		{
            // we need the graph type to define the identity
            if (withGraph == null)
                throw new ArgumentNullException("withGraph");

			// if there is no schema table, then we got an empty recordset. There are no columns to map, so create an empty schema.
			if (schemaTable == null)
			{
				schemaTable = new DataTable();
				schemaTable.Locale = CultureInfo.InvariantCulture;
				schemaTable.Columns.Add("ColumnName");
				schemaTable.Columns.Add("DataType");
			}

            // save the values away for later
			SchemaTable = schemaTable;
            _graph = withGraph;
            _mappingType = mappingType;
            _idColumns = idColumns;

            // we know that we are going to store this in a hashtable, so pre-calculate the hashcode
			unchecked
			{
                // base the hashcode on the mapping type, target graph, and schema contents
                _hashCode = (int)_mappingType;
                if (_graph != null)
                    _hashCode += _graph.GetHashCode();
                if (_idColumns != null)
                    _hashCode += _idColumns.GetHashCode();

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
			return Equals(obj as SchemaMappingIdentity);
		}

		/// <summary>
		/// Determines if this is equal to another object.
		/// </summary>
		/// <param name="other">The object to test against.</param>
		/// <returns>True if the objects are equal.</returns>
		public bool Equals(SchemaMappingIdentity other)
		{
			if (other == null)
				return false;

            if (_mappingType != other._mappingType)
                return false;

            if (_graph != other._graph)
                return false;

            if (_idColumns != other._idColumns)
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
