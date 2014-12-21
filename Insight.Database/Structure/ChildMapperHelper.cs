﻿using System;
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
			var idFields = FindIdFields<RecordIdAttribute>(type, name, "ID");

			if (idFields != null)
				return new IDAccessor(idFields);

			throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a way to get the ID from {0}. Please add a hint.", type));
		}

		/// <summary>
		/// Finds the ID accessor for the given type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="name">The name of the id field or null to auto-detect.</param>
		/// <returns>The ID field for the type, or null if it's not found.</returns>
		internal static IDAccessor FindParentIDAccessor(Type type, string name = null)
		{
			var idFields = FindIdFields<ParentRecordIdAttribute>(type, name, "ParentID");

			if (idFields != null)
				return new IDAccessor(idFields);

			return null;
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
			var members = ClassPropInfo.GetMembersForType(parentType).Where(mi => mi.CanSetMember);

			ClassPropInfo member;

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
		/// <param name="name">Optional names of fields to use (comma-separated).</param>
		/// <param name="defaultFieldName"></param>The name of the default field to use.</param>
		/// <returns>The list of properties in the ID or null.  It will throw an exception if you search by name and its not found 
		/// (Legacy behavior we'll move up one or two levels when we improve the parent id code) </returns>
		private static IEnumerable<ClassPropInfo> FindIdFields<TAttribute>(Type type, string name, string defaultFieldName)
			where TAttribute : IRecordIdAttribute
		{
			IEnumerable<ClassPropInfo> idFields;

			var allFields = ClassPropInfo.GetMembersForType(type).Where(mi => mi.CanGetMember);

			// if a name was specified, split on commas and use that
			if (name != null)
			{
				idFields = FindIDPropertiesByName(type, name);  //TODO this throws an exception if the name is not found, throw from here?
			}
			else
			{
				idFields = FindIDPropertiesByAttribute<TAttribute>(allFields);

				if ((idFields == null) || (idFields.Any() == false))
					idFields = FindIDPropertiesByConvention(defaultFieldName, allFields);
			}
			return idFields;
		}

		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <param name="type">The type to search.</param>
		/// <param name="name">Optional names of fields to use (comma-separated).</param>
		/// <returns></returns>
		private static List<ClassPropInfo> FindIDPropertiesByName(Type type, string name)
		{
			var names = name.Split(',');

			return FindIDPropertiesByName(type, names);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="names"></param>
		/// <returns></returns>
		/// 
		/// <summary>
		/// Finds the property(s) that make up the ID
		/// </summary>
		/// <param name="type">The type to search.</param>
		/// <param name="name">Optional array of the names of the fields to use.</param>
		/// <returns>A list of the matching properties, with NULLs when something can't be </returns>
		private static List<ClassPropInfo> FindIDPropertiesByName(Type type, string[] names)
		{
			List<ClassPropInfo> matches = names.Select(n => ClassPropInfo.GetMemberByName(type, n.Trim())).ToList();

			// it assumes fields will come back in the same order (even when one is missing)
			for (int i = 0; i < names.Length; i++)
				if (matches[i] == null || !matches[i].CanGetMember)
					throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
						"Get Member '{0}' not found on type '{1}'", names[i], type.FullName));

			return matches;
		}

		private static IEnumerable<ClassPropInfo> FindIDPropertiesByAttribute<TAttribute>(IEnumerable<ClassPropInfo> members
																		) where TAttribute : IRecordIdAttribute
		{
			IList<ClassPropInfo> taggedMembers;

			// check for a member with an id attribute
			taggedMembers = members.Where(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Any()).ToList();

			if (taggedMembers.Count >= 1)
			{
				var orderedIdFields = taggedMembers.OrderBy(m => m.MemberInfo.GetCustomAttributes(true).OfType<TAttribute>().Single().Order);
				return orderedIdFields;
			}
			return null;
		}

		private static List<ClassPropInfo> FindIDPropertiesByConvention(string defaultFieldName, IEnumerable<ClassPropInfo> members)
		{
			ClassPropInfo idField;
			List<ClassPropInfo> idFields = null;

			// look for anything that looks like an ID
			idField =
				members.FirstOrDefault(m => String.Compare(m.Name, defaultFieldName, StringComparison.OrdinalIgnoreCase) == 0) ?? /* Ex*/
				members.SingleOrDefault(m => m.Name.EndsWith("_" + defaultFieldName, StringComparison.OrdinalIgnoreCase)) ??
				members.SingleOrDefault(m => m.Name.EndsWith(defaultFieldName, StringComparison.OrdinalIgnoreCase));

			if (idField != null)
			{
				idFields = new List<ClassPropInfo>(1);
				idFields.Add(idField);
			}

			return idFields;
		}

	}

#if DEBUG

	public class ChildMapperHelperTests
	{
		public static IEnumerable<String> GetIDAccessor(Type type, string name = null)
		{
			var accessor = ChildMapperHelper.GetIDAccessor(type, name);

			if (accessor == null)
			{
				return null;
			}

			return accessor.GetIdFields();
		}

		public static IEnumerable<String> FindParentIDAccessor(Type type, string name = null)
		{
			var accessor = ChildMapperHelper.FindParentIDAccessor(type, name);

			if (accessor == null)
				return null;

			return accessor.GetIdFields();
		}

	}

#endif

}
