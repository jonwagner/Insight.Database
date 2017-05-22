using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// An instance of IMapper can map TChild to somewhere inside TRoot.
	/// This is a one-to-many mapping, and TChild may go into sublayers of the structure.
	/// </summary>
	/// <typeparam name="TRoot">The type of the root object.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TId">The type of the ID value.</typeparam>
	interface IChildMapper<TRoot, TChild, TId>
	{
		/// <summary>
		/// Maps a list of children into the structure.
		/// </summary>
		/// <param name="roots">The list of root objects.</param>
		/// <param name="children">The list of child objects with their parent's id.</param>
		void MapChildren(IEnumerable<TRoot> roots, IEnumerable<IGrouping<TId, TChild>> children);
	}
}
