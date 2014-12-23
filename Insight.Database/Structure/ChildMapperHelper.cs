using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Helper methods for mapping parent-child relationships.
	/// </summary>
	class ChildMapperHelper
	{

		/// <summary>
		/// Gets the ID accessor for the given type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="name">The name of the id field or null to auto-detect.</param>
		/// <returns>The ID field for the type.</returns>
		internal static IDAccessor GetIDAccessor(Type type, string name = null)
		{
			string mappingIssueDetails;

			var idMembers = FindIDProperties<RecordIdAttribute>(type, name, IDTypes.ID, out mappingIssueDetails);

			if (idMembers != null)
				return new IDAccessor(idMembers);

			else  // we have failed to find the id
			{
				if (string.IsNullOrEmpty(mappingIssueDetails))
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
															"Cannot find a way to get the ID from {0}. Please add a hint.", type));
				else
					throw new InvalidOperationException(mappingIssueDetails);
			}
		}

		/// <summary>
		/// Finds the ParentID accessor for the given type using the supplied set of names
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="names">The name of the id fields. (a name or comma separated list)</param>
		/// <returns>The ParentID field for the type</returns>
		internal static IDAccessor FindParentIDAccessor(Type type, string names)
		{
			var idMembers = FindIDProperties<ParentRecordIdAttribute>(type, names, IDTypes.ParentID);

			if (idMembers != null)
				return new IDAccessor(idMembers);

			return null;
		}

		/// <summary>
		/// Finds the ID accessor for the given type using attributes or the parents ID
		/// </summary>
		/// <param name="type">The type to analyze, e.g. InvoiceLine</param>
		/// <param name="parentType">The type's parent, e.g. Invoice</param>
		/// <returns>The ParentID field for the type</returns>
		internal static IDAccessor FindParentIDAccessor(Type type, Type parentType)
		{
			var idMembers = FindIDProperties<ParentRecordIdAttribute>(type, null, IDTypes.ParentID);

			if (idMembers != null)
				return new IDAccessor(idMembers);

			else if (parentType != null)
			{
				idMembers = InferParentIDAccessorFromParentClass(type, parentType);

				if (idMembers != null)
					return new IDAccessor(idMembers);
			}
			return null;
		}

		/// <summary>
		/// Tries to use the parent's ID as the ParentID of the child
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parentType"></param>
		/// <example>Eg, given classes Invoice and InvoiceLine, if 
		/// Invoice_ID is the ID of Invoice, we can infer that Invoice_ID is the ParentID of Invoice line (if it exists)</example>
		/// <returns></returns>
		internal static IEnumerable<ClassPropInfo> InferParentIDAccessorFromParentClass(Type type, Type parentType)
		{
			var myParentsIDProperties = FindIDProperties<RecordIdAttribute>(parentType, null, IDTypes.ID);

			if (myParentsIDProperties == null)  // We're done if its null
				return null;

			// Prevent us from saying the Invoice.ID  maps to the InvoiceLine.ID (ID can't be the FK of InvoiceLine) 
			if ((myParentsIDProperties.Count() == 1) && StringsAreTheSame(myParentsIDProperties.First().Name, IDDefaultName))
				return null;

			var parentIDPropertyNames = myParentsIDProperties.Select(p => p.Name).ToArray();

			var idProperties = FindIDPropertiesByName(type, parentIDPropertyNames);

			return idProperties;
		}

		/// <summary>
		/// Gets the list setter for the class, looking for an IList that matches the type.
		/// </summary>
		/// <param name="parentType">The type to analyze.</param>
		/// <param name="childType">The type of object in the list.</param>
		/// <param name="name">The name of the field or null to auto-detect.</param>
		/// <returns>An accessor for the ID field.</returns>
		internal static ClassPropInfo GetListSetter(Type parentType, Type childType, string name = null)
		{
			ClassPropInfo member;

			var members = ClassPropInfo.GetMembersForType(parentType).Where(mi => mi.CanSetMember);

			// if a name was specified, use that
			if (name != null)
			{
				member = members.SingleOrDefault(m => String.Compare(m.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (member == null)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Set Member {0} not found on type {1}", name, parentType.FullName));
				
				return member;
			}

			// find the right match with a childrecords attribute
			member = members.SingleOrDefault(m => IsGenericListType(m.MemberType, childType) && m.MemberInfo.GetCustomAttributes(true).OfType<ChildRecordsAttribute>().Any());
			if (member != null)
				return member;

			// look for anything that looks like the right list
			member = members.SingleOrDefault(m => m.SetMethodInfo != null && IsGenericListType(m.MemberType, childType)) ??
				members.SingleOrDefault(m => m.FieldInfo != null && IsGenericListType(m.MemberType, childType));
			if (member != null)
				return member;

			// look for a single record of the given type
			member = members.SingleOrDefault(m => m.SetMethodInfo != null && m.MemberType == childType) ??
				members.SingleOrDefault(m => m.FieldInfo != null && m.MemberType == childType);

			if (member == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a way to set a list of {0} into {1}. Please add a hint.", childType, parentType));

			return member;
		}

		/// <summary>
		/// Returns true if the type represents a generic of a list.
		/// Used to determine if the type represents a result set.
		/// </summary>
		/// <param name="type">The type to inspect.</param>
		/// <param name="childType">The type of object in the list.</param>
		/// <returns>True if it is a list type.</returns>
		private static bool IsGenericListType(Type type, Type childType)
		{
			if (!type.IsGenericType)
				return false;

			var genericParameters = type.GetGenericArguments();
			if (genericParameters.Length != 1 || genericParameters[0] != childType)
				return false;

			type = type.GetGenericTypeDefinition();

			return type == typeof(IList<>) ||
					type == typeof(List<>) ||
					type == typeof(IEnumerable<>) ||
					type == typeof(ICollection<>);
		}

		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <typeparam name="TAttribute">The type of IRecordIDAttribute to use to look for the fields.</typeparam>
		/// <param name="type">The type to search.</param>
		/// <param name="names">Optional nameArray of fields to use (comma-separated).</param>
		/// <param name="idType">The type of ID to look for: ID (Primary ID) or ParentID.</param>
		/// <returns>The list of property(s) in the ID or null if they were not found.</returns>
		private static IEnumerable<ClassPropInfo> FindIDProperties<TAttribute>(Type type, string names, IDTypes idType)
																	where TAttribute : IRecordIdAttribute
		{
			string mappingIssueDetails;

			return FindIDProperties<TAttribute>(type, names, idType, out mappingIssueDetails);
		}

		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <typeparam name="TAttribute">The type of IRecordIDAttribute to use to look for the fields.</typeparam>
		/// <param name="type">The type to search.</param>
		/// <param name="names">Optional nameArray of fields to use (comma-separated).</param>
		/// <param name="idType">The type of ID to look for: ID (Primary ID) or ParentID.</param>
		/// <param name="mappingIssueDetails_out">Issues with the mapping that might explain a Null result.</param>
		/// 
		/// <returns>The list of property(s) in the ID or null if they were not found.</returns>
		private static IEnumerable<ClassPropInfo> FindIDProperties<TAttribute>(Type type, string names, IDTypes idType, out string mappingIssueDetails_out)
												where TAttribute : IRecordIdAttribute
		{
			IEnumerable<ClassPropInfo> idMembers;
			mappingIssueDetails_out = null;

			var allMembers = ClassPropInfo.GetMembersForType(type).Where(mi => mi.CanGetMember);

			// if names were specified, split on commas and use that only
			if (string.IsNullOrEmpty(names) == false)
			{
				var nameArray = CommaSeparatedStringToArray(names);
				idMembers = FindIDPropertiesByName(type, nameArray, out mappingIssueDetails_out);
				return idMembers;
			}

			idMembers = FindIDPropertiesByAttribute<TAttribute>(allMembers);

			if (idMembers != null)
				return idMembers;

			var idMember = FindIDPropertiesByConvention(type, idType, allMembers);

			if (idMember != null) // Convert to a list so we can return it
				idMembers = new List<ClassPropInfo>(1) { idMember };

			return idMembers;
		}

		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <param name="type">The type to search.</param>
		/// <param name="names">Array of the names to find.</param>
		/// <returns>A list of the matching properties or NULL when the lookup fails to fin the list</returns>
		/// 
		private static List<ClassPropInfo> FindIDPropertiesByName(Type type, string[] names)
		{
			string mappingIssueDetails;

			return FindIDPropertiesByName(type, names, out mappingIssueDetails);
		}

		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <param name="type">The type to search.</param>
		/// <param name="names">Array of the names to find.</param>
		/// <param name="mappingIssueDetails_out"></param>
		/// <returns>A list of the matching properties or NULL when the lookup fails to fin the list</returns>
		/// 
		private static List<ClassPropInfo> FindIDPropertiesByName(Type type, string[] names, out string mappingIssueDetails_out)
		{
			//Find the member in 'names', preserving the order.  Unmateched names have a null entry in the list:
			List<ClassPropInfo> matches = names.Select(n => ClassPropInfo.GetMemberByName(type, n.Trim())).ToList();

			mappingIssueDetails_out = null;

			for (int i = 0; i < names.Length; i++)
			{
				if (matches[i] == null)
				{
					mappingIssueDetails_out = String.Format(CultureInfo.InvariantCulture
														, "Get Member '{0}' not found on type '{1}'", names[i], type.FullName);
					return null;
				}
				if (!matches[i].CanGetMember)
				{
					mappingIssueDetails_out = String.Format(CultureInfo.InvariantCulture
														, "Get Member '{0}' is inaccessible on type '{1}'", names[i], type.FullName);
					return null;
				}
			}
			return matches;
		}

		private static IEnumerable<ClassPropInfo> FindIDPropertiesByAttribute<TAttribute>(IEnumerable<ClassPropInfo> members
																		) where TAttribute : IRecordIdAttribute
		{
			var taggedMembers = members.Where(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Any()).ToList();

			if (taggedMembers.Count >= 1)
				return taggedMembers.OrderBy(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Single().Order);

			return null;
		}

		private static ClassPropInfo FindIDPropertiesByConvention(Type type, IDTypes idType, IEnumerable<ClassPropInfo> allMembers)
		{
			ClassPropInfo idMember = null;

			if (idType == IDTypes.ID)
				idMember = FindPrimaryIDPropertiesByConvention(type, allMembers);
			else if (idType == IDTypes.ParentID)
				idMember = FindParentIDPropertiesByConvention(type, allMembers);

			return idMember;
		}

		private static ClassPropInfo FindPrimaryIDPropertiesByConvention(Type type, IEnumerable<ClassPropInfo> members)
		{
			ClassPropInfo member = null;

			member = members.FirstOrDefault(m => String.Compare(m.Name, IDDefaultName, StringComparison.OrdinalIgnoreCase) == 0) ??
					members.FirstOrDefault(m => String.Compare(m.Name, type.Name + "_" + IDDefaultName, StringComparison.OrdinalIgnoreCase) == 0) ??
					members.FirstOrDefault(m => String.Compare(m.Name, type.Name + IDDefaultName, StringComparison.OrdinalIgnoreCase) == 0) ??
					members.SingleOrDefault(m => m.Name.EndsWith("_" + IDDefaultName, StringComparison.OrdinalIgnoreCase)) ??
					members.SingleOrDefault(m => m.Name.EndsWith(IDDefaultName, StringComparison.OrdinalIgnoreCase));

			return member;
		}

		private static ClassPropInfo FindParentIDPropertiesByConvention(Type type, IEnumerable<ClassPropInfo> members)
		{
			ClassPropInfo member = null;

			// Prevent us from saying that Parent.ParentID is the class' 'ParentID' (its the ID), a special case 
			if (StringsAreTheSame(type.Name + IDDefaultName, ParentIDDefaultName) == false)
			{
				member = members.FirstOrDefault(m => String.Compare(m.Name, ParentIDDefaultName, StringComparison.OrdinalIgnoreCase) == 0);

				if (member != null)
					return member;
			}

			member = members.FirstOrDefault(m => String.Compare(m.Name, type.Name + "_" + ParentIDDefaultName, StringComparison.OrdinalIgnoreCase) == 0) ??
					members.FirstOrDefault(m => String.Compare(m.Name, type.Name + ParentIDDefaultName, StringComparison.OrdinalIgnoreCase) == 0) ??
					members.FirstOrDefault(m => m.Name.EndsWith("_" + ParentIDDefaultName, StringComparison.OrdinalIgnoreCase)) ??
					members.FirstOrDefault(m => m.Name.EndsWith(ParentIDDefaultName, StringComparison.OrdinalIgnoreCase));

			return member;
		}

		/// <summary> Performs a case insensitive string comparision</summary>
		private static bool StringsAreTheSame(string string1, string string2)
		{
			return String.Compare(string1, string2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>Converts a comma separated list into an array</summary>
		private static string[] CommaSeparatedStringToArray(string commaSeparatedString)
		{
			var array = commaSeparatedString.Split(',');

			for (int i = 0; i < array.Length - 1; i++)
				array[i] = array[i].Trim();

			return array;
		}

		/// <summary>The type of ID to look for: ID (Primary ID) or ParentID</summary>
		enum IDTypes { ID = 1, ParentID = 2 }

		private const string IDDefaultName = "ID";
		private const string ParentIDDefaultName = "ParentID";
	}

#if DEBUG

	/// <summary>
	/// A shim to help unit tests.  Insight.Schema is not signed, so we can't can't sign Insight.Tests and use InternalVisibleTo
	/// </summary>
	public class ChildMapperHelperTests
	{
		public static IEnumerable<String> GetIDAccessor(Type type, string name = null)
		{
			var accessor = ChildMapperHelper.GetIDAccessor(type, name);

			if (accessor != null) 
				return accessor.GetIdFields();

			return new List<string>();
		}

		public static IEnumerable<String> FindParentIDAccessor(Type type, Type parentType)
		{
			var accessor = ChildMapperHelper.FindParentIDAccessor(type, parentType);

			if (accessor != null) 
				return accessor.GetIdFields();

			return new List<string>();
		}
	}

#endif

}
