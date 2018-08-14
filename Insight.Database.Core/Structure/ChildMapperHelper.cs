using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
    /// <summary>
    /// Used for internal testing.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "This is a class for testing.")]
    public static class ChildMapperTestingHelper
    {
        /// <summary>
        /// Used for internal testing.
        /// </summary>
        /// <param name="type">The parameter is not used.</param>
        /// <param name="name">The parameter is not used.</param>
        /// <returns>The parameter is not used.</returns>
        public static IList<string> GetIDAccessor(Type type, string name = null)
        {
            return ChildMapperHelper.GetIDAccessor(type, name).MemberNames.ToList();
        }

        /// <summary>
        /// Used for internal testing.
        /// </summary>
        /// <param name="type">The parameter is not used.</param>
        /// <param name="name">The parameter is not used.</param>
        /// <param name="parentType">The parameter is not used.</param>
        /// <returns>The parameter is not used.</returns>
        public static IList<string> FindParentIDAccessor(Type type, string name, Type parentType)
        {
            var accessor = ChildMapperHelper.FindParentIDAccessor(type, name, parentType);

            if (accessor == null)
                return new List<string>();
            else
                return accessor.MemberNames.ToList();
        }
    }

	/// <summary>
	/// Helper methods for mapping parent-child relationships.
	/// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "This is a class for testing.")]
    class ChildMapperHelper
	{
        /// <summary>
        /// The default field name for ID fields.
        /// </summary>
        private const string DefaultIDField = "ID";

        /// <summary>
        /// The default field name for Parent ID fields.
        /// </summary>
        private const string DefaultParentIDField = "ParentID";

		/// <summary>
		/// Gets the ID accessor for the given type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="name">The name of the id field or null to auto-detect.</param>
		/// <returns>The ID field for the type.</returns>
		internal static IDAccessor GetIDAccessor(Type type, string name = null)
		{
            var names = (name != null) ? name.Split(',') : null;
            var accessor = FindIDAccessor<RecordIdAttribute>(type, names, DefaultIDField);

			if (accessor == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a way to get the ID from {0}. Please add a hint.", type));

			return accessor;
		}

		/// <summary>
		/// Finds the ID accessor for the given type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="name">The name of the id field or null to auto-detect.</param>
        /// <param name="parentType">The type containing the current type.</param>
        /// <returns>The ID field for the type, or null if it's not found.</returns>
		internal static IDAccessor FindParentIDAccessor(Type type, string name, Type parentType)
		{
            var names = (name != null) ? name.Split(',') : null;
            return FindIDAccessor<ParentRecordIdAttribute>(type, names, DefaultParentIDField) ??
                InferParentIDAccessorFromParentClass(type, parentType);
		}

		/// <summary>
		/// Gets the list getter or setter for the class, looking for an IList that matches the type.
		/// </summary>
		/// <param name="parentType">The type to analyze.</param>
		/// <param name="childType">The type of object in the list.</param>
		/// <param name="name">The name of the field or null to auto-detect.</param>
		/// <param name="setter">True to return the setter, false to return the getter.</param>
		/// <returns>An accessor for the ID field.</returns>
		internal static ClassPropInfo GetListAccessor(Type parentType, Type childType, string name = null, bool setter = true)
		{
			var members = ClassPropInfo.GetMembersForType(parentType).Where(mi => setter ? mi.CanSetMember : mi.CanGetMember);

			ClassPropInfo member;

			// if a name was specified, use that
			if (name != null)
			{
				member = members.SingleOrDefault(m => String.Compare(m.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (member == null)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Set Member {0} not found on type {1}", name, parentType.FullName));
				return member;
			}

            var listMembers = members.Where(m => IsGenericListType(m.MemberType, childType));

			// find the right match with a childrecords attribute
            member = listMembers.SingleOrDefault(m => m.MemberInfo.GetCustomAttributes(true).OfType<ChildRecordsAttribute>().Any());
			if (member != null)
				return member;

			// look for anything that looks like the right list
            member = listMembers.SingleOrDefault(m => (setter ? m.SetMethodInfo : m.GetMethodInfo) != null) ?? listMembers.SingleOrDefault(m => m.FieldInfo != null);
            if (member != null)
                return member;

            // look for a single record of the given type
            var childTypeMembers = members.Where(m => m.MemberType == childType);
            member = childTypeMembers.SingleOrDefault(m => m.SetMethodInfo != null) ?? childTypeMembers.SingleOrDefault(m => m.FieldInfo != null);

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
			if (!type.GetTypeInfo().IsGenericType)
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
		/// Finds an IDAccessor to get IDs out of an object.
		/// </summary>
		/// <typeparam name="TAttribute">The type of IRecordIDAttribute to use to look for the fields.</typeparam>
		/// <param name="type">The type to search.</param>
		/// <param name="names">Optional names of fields to use.</param>
		/// <param name="defaultFieldName">The name of the default field to use.</param>
		/// <returns>The IDAccessor or null.</returns>
		private static IDAccessor FindIDAccessor<TAttribute>(Type type, IList<string> names, string defaultFieldName) where TAttribute : IRecordIdAttribute
		{
            if (names != null && !names.Any())
                throw new ArgumentException("ID field names cannot be empty", "names");

            var members = ClassPropInfo.GetMembersForType(type).Where(mi => mi.CanGetMember);
			ClassPropInfo member;

			// if a name was specified, split on commas and use that
			if (names != null)
                return FindIDAccessorByNameList(type, names, true);

			// check for a member with an id attribute
			var taggedMembers = members.Where(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Any()).ToList();
			if (taggedMembers.Count == 1)
			{
				return new IDAccessor(taggedMembers.First());
			}
			else if (taggedMembers.Count > 1)
			{
				var ordered = taggedMembers.OrderBy(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Single().Order);
				return new IDAccessor(ordered);
			}

            // handle the special case where the class Parent has a ParentID
            var parentIDFieldIsReserved = defaultFieldName.IsIEqualTo(type.Name + DefaultIDField);

			// look for anything that looks like an ID
            member = members.SingleOrDefault(m => m.Name.IsIEqualTo(defaultFieldName) && !parentIDFieldIsReserved) ??
                members.SingleOrDefault(m => m.Name.IsIEqualTo(type.Name + "_" + defaultFieldName)) ??
                members.SingleOrDefault(m => m.Name.IsIEqualTo(type.Name + defaultFieldName)) ??
				members.OnlyOrDefault(m => m.Name.EndsWith("_" + defaultFieldName, StringComparison.OrdinalIgnoreCase)) ??
				members.OnlyOrDefault(m => m.Name.EndsWith(defaultFieldName, StringComparison.OrdinalIgnoreCase));

			if (member != null)
				return new IDAccessor(member);

			return null;
		}

        /// <summary>
        /// Finds an IDAccessor that matches the list of names exactly.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <param name="names">The list of fields to find.</param>
        /// <param name="required">If required, and the names are not all found, an exception is thrown.</param>
        /// <returns>The ID accessor matching the list of names.</returns>
        private static IDAccessor FindIDAccessorByNameList(Type type, IList<string> names, bool required)
        {
            var members = new List<ClassPropInfo>();

            foreach (string name in names)
            {
                var member = ClassPropInfo.GetMemberByName(type, name.Trim());
                if (member == null || !member.CanGetMember)
                {
                    if (required)
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "A public get Member {0} not found on type {1}", name, type.FullName));
                    else
                        return null;
                }

                members.Add(member);
            }

            return new IDAccessor(members);
        }

        /// <summary>
        /// Attempts to infer the parent ID accessor by finding a set of fields that exactly match the parent type's ID accessor.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <param name="parentType">The parent type to analyze.</param>
        /// <returns>A matching IDAccessor or null.</returns>
        private static IDAccessor InferParentIDAccessorFromParentClass(Type type, Type parentType)
        {
            IDAccessor idAccessor = null;

            if (parentType == null)
                return null;

            // Determine the PK of the parent so we can try to translate it to the child
            var parentIDAccessor = FindIDAccessor<RecordIdAttribute>(parentType, null, DefaultIDField);
            if (parentIDAccessor == null)
                return null;

            var parentIDNames = parentIDAccessor.MemberNames.ToList();

            // If the parent's PK is just ID, we can't say
            // InvoiceLine.ID => Invoice.ID, but InvoiceLine.Invoice_ID => Invoice.ID makes sense
            if (parentIDNames.Count() == 1 && parentIDNames.First().IsIEqualTo(DefaultIDField))
            {
                parentIDNames[0] = parentType.Name + '_' + DefaultIDField;
                idAccessor = FindIDAccessorByNameList(type, parentIDNames, false);

                if (idAccessor == null)
                {
                    // Now try InvoiceLine.InvoiceID => Invoice.ID
                    parentIDNames[0] = parentType.Name + DefaultIDField;
                    idAccessor = FindIDAccessorByNameList(type, parentIDNames, false);
                }
            }
            else
            {
				idAccessor = FindIDAccessorByNameList(type, parentIDNames, false);
			}

            return idAccessor;
        }
    }
}
