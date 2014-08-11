using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Marks a method as an Insert/Update/Upsert method so Insight will merge the outputs onto the input objects.
	/// </summary>
	/// <remarks>
	/// Normally when Insight auto-implements an interface, it only merges the results of methods whose names start with Insert or Update.
	/// If you want a method of a different name to automatically merge the results, add the MergeOutput attribute to the method.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MergeOutputAttribute : Attribute
	{
		public MergeOutputAttribute()
		{
			MergeOutputs = true;
		}

		public MergeOutputAttribute(bool merge = true)
		{
			MergeOutputs = merge;
		}

		public bool MergeOutputs { get; private set; }
	}
}
