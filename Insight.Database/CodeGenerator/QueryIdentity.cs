using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// An identity for a query.
	/// This lets us store queries in a dictionary and get automatic efficient storage.
	/// </summary>
	/// <remarks>This currently assumes that a query is unique across databases. If that is not true, then the identity needs to take the connection string.</remarks>
	class QueryIdentity : IEquatable<QueryIdentity>
	{
		#region Private Members
		/// <summary>
		/// The command text that we are bound to.
		/// </summary>
		private string _commandText;

		/// <summary>
		/// The type that we are bound to.
		/// </summary>
		private Type _type;

		/// <summary>
		/// The hash code of this identity (precalculated).
		/// </summary>
		private int _hashCode;
		#endregion

		/// <summary>
		/// Initializes a new instance of the QueryIdentity class.
		/// </summary>
		/// <param name="command">The command to represent.</param>
		/// <param name="type">The type of the parameters for the command.</param>
		public QueryIdentity(IDbCommand command, Type type)
		{
			_commandText = command.CommandText;
			_type = type;

			// precalculate the hash code
			unchecked
			{
				_hashCode = 17 + type.GetHashCode();
				_hashCode *= 23;
				_hashCode += _commandText.GetHashCode();
			}
		}

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
			return Equals(obj as QueryIdentity);
		}

		/// <summary>
		/// Determines if this is equal to another object.
		/// </summary>
		/// <param name="other">The object to test against.</param>
		/// <returns>True if the objects are equal.</returns>
		public bool Equals(QueryIdentity other)
		{
			if (other == null)
				return false;

			if (_type != other._type)
				return false;

			if (_commandText != other._commandText)
				return false;

			return true;
		}
		#endregion
	}
}
