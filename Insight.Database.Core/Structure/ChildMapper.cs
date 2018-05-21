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
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ChildMapper class.
		/// </summary>
		/// <param name="idSelector">The function that selects IDs from the parent object.</param>
		/// <param name="listSetter">The action that sets the children into the proper parent.</param>
		public ChildMapper(
			Func<TParent, TId> idSelector,
			Action<TParent, List<TChild>> listSetter)
		{
			_idSelector = idSelector ?? _defaultIDSelector.Value;
			_listSetter = listSetter ?? _defaultListSetter.Value;
		}
		#endregion

		#region Mapping Methods
		/// <inheritdoc/>
		public void MapChildren(IEnumerable<TParent> roots, IEnumerable<IGrouping<TId, TChild>> children)
		{
			if (roots == null) throw new ArgumentNullException("roots");
			if (children == null) throw new ArgumentNullException("children");

			// convert the children into lists by their parent ID
			Dictionary<TId, List<TChild>> lists = children.ToDictionary(g => g.Key, g => g.ToList());

			// fill in each parent
			foreach (var root in roots)
			{
				// In certain instances the elements inside are null, dont do any mappings then.
                if (root == null) break;

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
			return ChildMapperHelper.GetListAccessor(typeof(TParent), typeof(TChild)).CreateSetMethod<TParent, List<TChild>>();
		}
		#endregion
	}

	/// <summary>
	/// Maps a child into a hierarchy of objects.
	/// This is an n-level one-to-many mapping.
	/// </summary>
	/// <typeparam name="TRoot">The type of the root object.</typeparam>
	/// <typeparam name="TParent">The type of the parent object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TId">The type of the ID value.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	class ChildMapper<TRoot, TParent, TChild, TId> : IChildMapper<TRoot, TChild, TId>
	{
		#region Private Members
		/// <summary>
		/// The function that selects all of the eligible parents from the root objects.
		/// </summary>
		private Func<TRoot, IEnumerable<TParent>> _parentSelector;

		/// <summary>
		/// The mapper that performs the final mapping.
		/// </summary>
		private ChildMapper<TParent, TChild, TId> _mapper;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ChildMapper class.
		/// </summary>
		/// <param name="parentSelector">The function that selects all of the eligible parents from the root objects.</param>
		/// <param name="idSelector">The function that selects IDs from the parent object.</param>
		/// <param name="listSetter">The action that sets the children into the proper parent.</param>
		public ChildMapper(
			Func<TRoot, IEnumerable<TParent>> parentSelector,
			Func<TParent, TId> idSelector,
			Action<TParent, List<TChild>> listSetter)
		{
			_parentSelector = parentSelector;
			_mapper = new ChildMapper<TParent, TChild, TId>(idSelector, listSetter);
		}
		#endregion

		#region Mapping Methods
		/// <inheritdoc/>
		public void MapChildren(IEnumerable<TRoot> roots, IEnumerable<IGrouping<TId, TChild>> children)
		{
			// we can't just use SelectMany, because the child lists may be null and SelectMany blows up on that
			_mapper.MapChildren(
				roots.Select(_parentSelector)
					.Where(list => list != null)
					.SelectMany(list => list),
					children);
		}
		#endregion
	}
}
