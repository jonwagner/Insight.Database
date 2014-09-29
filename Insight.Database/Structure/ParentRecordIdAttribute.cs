using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Marks a field or property as the one Insight should consider as the ID field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class ParentRecordIdAttribute : Attribute, IRecordIdAttribute
	{
		/// <summary>
		/// Initializes a new instance of the ParentRecordIdAttribute class.
		/// </summary>
		public ParentRecordIdAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ParentRecordIdAttribute class.
		/// </summary>
		/// <param name="order">An index representing the order the Ids should be evaluated.</param>
		public ParentRecordIdAttribute(int order)
		{
			Order = order;
		}

		/// <summary>
		/// Gets the order of the ID field.
		/// </summary>
		public int Order { get; private set; }
	}
}
