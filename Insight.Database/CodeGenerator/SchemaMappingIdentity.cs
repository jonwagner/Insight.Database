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
	/// An identity for the schema of a reader being mapped to a graph type.
	/// This checks all of the column names and types, as well as the type of the graph.
	/// This lets us store schemas in a dictionary and get automatic efficient storage.
	/// </summary>
	class SchemaMappingIdentity : IEquatable<SchemaMappingIdentity>
	{
		#region Fields
		/// <summary>
		/// The identity of the schema that we are mapped to.
		/// </summary>
		private SchemaIdentity _schemaIdentity;

		/// <summary>
		/// The type of mapping operation.
		/// </summary>
		private SchemaMappingType _mappingType;

		/// <summary>
		/// The id column override mapping.
		/// </summary>
		private Dictionary<Type, string> _idColumns;

		/// <summary>
		/// The hash code of this identity (precalculated).
		/// </summary>
		private int _hashCode;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SchemaMappingIdentity class.
		/// </summary>
		/// <param name="reader">The reader to construct from.</param>
		/// <param name="withGraph">The type of the object graph used in this mapping.</param>
		/// <param name="idColumns">An optional override of the columns used to determine the boundary between objects.</param>
		/// <param name="mappingType">The type of mapping operation.</param>
		public SchemaMappingIdentity(IDataReader reader, Type withGraph, Dictionary<Type, string> idColumns, SchemaMappingType mappingType)
			: this(new SchemaIdentity(reader), withGraph, idColumns, mappingType)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SchemaMappingIdentity class.
		/// </summary>
		/// <param name="schemaIdentity">The identity of the schema to map to.</param>
		/// <param name="withGraph">The type of the object graph used in this mapping.</param>
		/// <param name="idColumns">An optional override of the columns used to determine the boundary between objects.</param>
		/// <param name="mappingType">The type of mapping operation.</param>
		public SchemaMappingIdentity(SchemaIdentity schemaIdentity, Type withGraph, Dictionary<Type, string> idColumns, SchemaMappingType mappingType)
		{
			// we need the graph type to define the identity
			if (withGraph == null)
				throw new ArgumentNullException("withGraph");

			// save the values away for later
			Graph = withGraph;
			_mappingType = mappingType;
			_idColumns = idColumns;
			_schemaIdentity = schemaIdentity;

			// we know that we are going to store this in a hashtable, so pre-calculate the hashcode
			unchecked
			{
				// base the hashcode on the mapping type, target graph, and schema contents
				_hashCode = (int)_mappingType;
				if (Graph != null)
					_hashCode += Graph.GetHashCode();
				if (_idColumns != null)
					_hashCode += _idColumns.GetHashCode();

				_hashCode += schemaIdentity.GetHashCode();
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the columns in the schema as a list of Tuple of string + Type.
		/// </summary>
		internal Tuple<string, Type>[] Columns { get { return _schemaIdentity.Columns; } }

		/// <summary>
		/// Gets the type of the object graph that the schema is mapped to.
		/// </summary>
		internal Type Graph { get; private set; }
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

			if (Graph != other.Graph)
				return false;

			if (!_schemaIdentity.Equals(other._schemaIdentity))
				return false;

			// validate that the columns are the same object
			if (_idColumns != other._idColumns)
			{
				// different objects, so we have to check the contents.
				// this is a performance hit, so you should pass in the same id mapping each time!
				var otherIdColumns = other._idColumns;

				// check the count first as a short-circuit
				if (_idColumns.Count != otherIdColumns.Count)
					return false;

				// check the id mappings individually
				foreach (var pair in _idColumns)
				{
					string otherID;
					if (!otherIdColumns.TryGetValue(pair.Key, out otherID))
						return false;

					if (pair.Value != otherID)
						return false;
				}

				return false;
			}

			return true;
		}
		#endregion
	}
}
