using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Mapping
{
	/// <summary>
	/// Represents the mapping from a database object to a class member.
	/// </summary>
	class FieldMapping
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the FieldMapping class.
		/// </summary>
		/// <param name="pathToMember">The path to the member.</param>
		/// <param name="member">The member that is bound.</param>
		/// <param name="serializer">The serializer for the mapping.</param>
		public FieldMapping(string pathToMember, ClassPropInfo member, IDbObjectSerializer serializer)
		{
			PathToMember = pathToMember;
			Member = member;
			Serializer = serializer;
			Prefix = ClassPropInfo.GetMemberPrefix(pathToMember);
			IsDeep = (Prefix != null);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the path to get to the member, in dotted notation (a.b.c.d).
		/// </summary>
		public string PathToMember { get; private set; }

		/// <summary>
		/// Gets the member this is bound to.
		/// </summary>
		public ClassPropInfo Member { get; private set; }

		/// <summary>
		/// Gets the serializer for this mapping.
		/// </summary>
		public IDbObjectSerializer Serializer { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this is a mapping into a subobject.
		/// </summary>
		public bool IsDeep { get; private set; }

		/// <summary>
		/// Gets the prefix part of the PathToMember.
		/// </summary>
		public string Prefix { get; private set; }
		#endregion
	}
}
