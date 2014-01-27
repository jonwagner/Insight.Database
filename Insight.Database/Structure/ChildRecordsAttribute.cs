using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Marks a field or property as the one that Insight should prefer to use to put lists of child records.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class ChildRecordsAttribute : Attribute
	{
	}
}
