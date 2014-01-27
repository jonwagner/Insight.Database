using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace Insight.Database.Structure
{
	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="TId">The type of the ID value.</typeparam>
	public class Guardian<TChild, TId>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*")]
		public TId ParentId { get; set; }

		/// <summary>
		/// Gets or sets the child object.
		/// </summary>
		public TChild Object { get; set; }
	}
}
