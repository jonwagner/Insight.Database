using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
	/// <summary>
	/// An instance of IMapper can map TChild to somewhere inside TRoot.
	/// This is a one-to-many mapping, and TChild may go into sublayers of the structure.
	/// </summary>
	/// <typeparam name="TRoot">The type of the root object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TID">The type of the ID value.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	interface IChildMapper<TRoot, TChild, TID>
	{
		/// <summary>
		/// Maps a list of children into the structure.
		/// </summary>
		/// <param name="roots">The list of root objects.</param>
		/// <param name="children">The list of child objects with their parent's id.</param>
		void MapChildren(IEnumerable<TRoot> roots, IEnumerable<Guardian<TChild, TID>> children);
	}

	/// <summary>
	/// Helper methods for mapping parent-child relationships.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	class ChildMapperHelper
	{
		/// <summary>
		/// Gets the ID accessor for the given type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="name">The name of the id field or null to auto-detect.</param>
		/// <returns>The ID field for the type.</returns>
		internal static ClassPropInfo GetIDAccessor(Type type, string name = null)
		{
			var members = ClassPropInfo.GetMembersForType(type).Where(mi => mi.CanGetMember);

			// if a name was specified, use that
			if (name != null)
				return members.Single(m => m.Name == name);

			// check for a member with an id attribute
			var member = members.SingleOrDefault(m => m.MemberInfo.GetCustomAttributes(true).OfType<RecordIdAttribute>().Any());
			if (member != null)
				return member;

			// look for anything that looks like an ID
			member = members.FirstOrDefault(m => String.Compare(m.Name, "ID", StringComparison.OrdinalIgnoreCase) == 0) ??
				members.FirstOrDefault(m => String.Compare(m.Name, type.Name + "ID", StringComparison.OrdinalIgnoreCase) == 0) ??
				members.SingleOrDefault(m => m.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase));

			if (member == null)
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a way to get the ID from {0}. Please add a hint.", type));

			return member;
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

			// if a name was specified, use that
			if (name != null)
				return members.Single(m => m.Name == name);

			// find the right match with a childrecords attribute
			var member = members.SingleOrDefault(m => IsGenericListType(m.MemberType, childType) && m.MemberInfo.GetCustomAttributes(true).OfType<ChildRecordsAttribute>().Any());
			if (member != null)
				return member;

			// look for anything that looks like the right list
			member = members.SingleOrDefault(m => m.SetMethodInfo != null && IsGenericListType(m.MemberType, childType)) ??
				members.SingleOrDefault(m => m.FieldInfo != null && IsGenericListType(m.MemberType, childType));
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
	}

	/// <summary>
	/// Maps a set of children directly into a list of parent objects.
	/// This is a two-level one-to-many mapping.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TId">The type of the ID value.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	class ChildMapper<TParent, TChild, TId> : IChildMapper<TParent, TChild, TId>
	{
		#region Private Members
		/// <summary>
		/// The default function that selects IDs from the parent object.
		/// </summary>
		private static Lazy<Func<TParent, TId>> _defaultIDSelector = new Lazy<Func<TParent, TId>>(GetIDSelector);

		/// <summary>
		/// The default action that sets the children into the proper parent.
		/// </summary>
		private Lazy<Action<TParent, List<TChild>>> _defaultListSetter = new Lazy<Action<TParent, List<TChild>>>(GetListSetter);

		/// <summary>
		/// The function that selects IDs from the parent object.
		/// </summary>
		private Func<TParent, TId> _idSelector;

		/// <summary>
		/// The action that sets the children into the proper parent.
		/// </summary>
		private Action<TParent, List<TChild>> _listSetter;

		/// <summary>
		/// The type of the ID for the parent class.
		/// </summary>
		private Type _idType;
		#endregion

		/// <summary>
		/// Initializes a new instance of the ChildMapper class.
		/// </summary>
		/// <param name="idSelector">The function that selects IDs from the parent object.</param>
		/// <param name="listSetter">The action that sets the children into the proper parent.</param>
		public ChildMapper(
			Func<TParent, TId> idSelector,
			Action<TParent, List<TChild>> listSetter)
		{
			if (idSelector == null)
				_idType = ChildMapperHelper.GetIDAccessor(typeof(TParent)).MemberType;

			_idSelector = idSelector ?? _defaultIDSelector.Value;
			_listSetter = listSetter ?? _defaultListSetter.Value;
		}

		/// <inheritdoc/>
		public void MapChildren(IEnumerable<TParent> roots, IEnumerable<Guardian<TChild, TId>> children)
		{
			if (roots == null) throw new ArgumentNullException("roots");
			if (children == null) throw new ArgumentNullException("children");

			// convert the children into lists by their parent ID
			Dictionary<TId, List<TChild>> lists;
			if (_idType != null && typeof(TId) == typeof(object))
				lists = children.GroupBy(h => (TId)Convert.ChangeType(h.ParentId, _idType, CultureInfo.InvariantCulture), h => h.Object).ToDictionary(h => h.Key, h => h.ToList());
			else
				lists = children.GroupBy(h => h.ParentId, h => h.Object).ToDictionary(h => h.Key, h => h.ToList());

			// fill in each parent
			foreach (var root in roots)
			{
				// get a filled in list or create an empty list
				List<TChild> list;
				if (!lists.TryGetValue(_idSelector(root), out list))
					list = new List<TChild>();

				_listSetter(root, list);
			}
		}

		/// <summary>
		/// Gets the ID selector from the class, looking for ID, classID, and then anything with xxxID.
		/// </summary>
		/// <returns>An accessor for the ID field.</returns>
		private static Func<TParent, TId> GetIDSelector()
		{
			return ChildMapperHelper.GetIDAccessor(typeof(TParent)).CreateGetMethod<TParent, TId>();
		}

		/// <summary>
		/// Gets the list setter for the class, looking for an IList that matches the type.
		/// </summary>
		/// <returns>An accessor for the ID field.</returns>
		private static Action<TParent, List<TChild>> GetListSetter()
		{
			return ChildMapperHelper.GetListSetter(typeof(TParent), typeof(TChild)).CreateSetMethod<TParent, List<TChild>>();
		}
	}

	/// <summary>
	/// Maps a child into a hierarchy of objects.
	/// This is an n-level one-to-many mapping.
	/// </summary>
	/// <typeparam name="TRoot">The type of the root object.</typeparam>
	/// <typeparam name="TParent">The type of the parent object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TID">The type of the ID value.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	class ChildMapper<TRoot, TParent, TChild, TID> : IChildMapper<TRoot, TChild, TID>
	{
		#region Private Members
		/// <summary>
		/// The function that selects all of the eligible parents from the root objects.
		/// </summary>
		private Func<TRoot, IEnumerable<TParent>> _parentSelector;

		/// <summary>
		/// The mapper that performs the final mapping.
		/// </summary>
		private ChildMapper<TParent, TChild, TID> _mapper;
		#endregion

		/// <summary>
		/// Initializes a new instance of the ChildMapper class.
		/// </summary>
		/// <param name="parentSelector">The function that selects all of the eligible parents from the root objects.</param>
		/// <param name="idSelector">The function that selects IDs from the parent object.</param>
		/// <param name="listSetter">The action that sets the children into the proper parent.</param>
		public ChildMapper(
			Func<TRoot, IEnumerable<TParent>> parentSelector,
			Func<TParent, TID> idSelector,
			Action<TParent, List<TChild>> listSetter)
		{
			_parentSelector = parentSelector;
			_mapper = new ChildMapper<TParent, TChild, TID>(idSelector, listSetter);
		}

		/// <inheritdoc/>
		public void MapChildren(IEnumerable<TRoot> roots, IEnumerable<Guardian<TChild, TID>> children)
		{
			// we can't just use SelectMany, because the child lists may be null and SelectMany blows up on that
			_mapper.MapChildren(
				roots.Select(_parentSelector)
					.Where(list => list != null)
					.SelectMany(list => list),
					children);
		}
	}
}
