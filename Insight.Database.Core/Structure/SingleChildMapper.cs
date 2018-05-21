using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Maps a child into a single parent.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	class SingleChildMapper<TParent, TChild>
	{
		/// <summary>
		/// The default action that sets the children into the proper parent.
		/// </summary>
		private Lazy<Action<TParent, List<TChild>>> _defaultListSetter = new Lazy<Action<TParent, List<TChild>>>(GetListSetter);

		/// <summary>
		/// The action that sets the children into the proper parent.
		/// </summary>
		private Action<TParent, List<TChild>> _listSetter;

		/// <summary>
		/// Initializes a new instance of the SingleChildMapper class.
		/// </summary>
		/// <param name="listSetter">The function that can be used to set the list into the parent.</param>
		public SingleChildMapper(Action<TParent, List<TChild>> listSetter)
		{
			_listSetter = listSetter ?? _defaultListSetter.Value;
		}

		/// <summary>
		/// Maps the children into the parent object.
		/// </summary>
		/// <param name="roots">The list of parents. This must have a single item.</param>
		/// <param name="children">The list of children.</param>
		public void MapChildren(IEnumerable<TParent> roots, IEnumerable<TChild> children)
		{
			var single = roots.SingleOrDefault();

			if (single == null)
			{
				if (children.Any())
					throw new InvalidOperationException("Child records were returned, but there was no parent record.");

				return;
			}

			_listSetter(single, children.ToList());
		}

		/// <summary>
		/// Gets the list setter for the class, looking for an IList that matches the type.
		/// </summary>
		/// <returns>An accessor for the ID field.</returns>
		private static Action<TParent, List<TChild>> GetListSetter()
		{
			return ChildMapperHelper.GetListAccessor(typeof(TParent), typeof(TChild)).CreateSetMethod<TParent, List<TChild>>();
		}
	}
}
