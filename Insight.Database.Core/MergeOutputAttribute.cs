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
		/// <summary>
		/// Initializes a new instance of the MergeOutputAttribute class.
		/// </summary>
		public MergeOutputAttribute()
		{
			MergeOutputs = true;
		}

		/// <summary>
		/// Initializes a new instance of the MergeOutputAttribute class.
		/// </summary>
		/// <param name="mergeOutputs">True to merge the outputs, false to skip merging.</param>
		/// <remarks>Use MergeOutput(false) to disable merging for methods named InsertXXX, UpdateXXX, or UpsertXXX.</remarks>
		public MergeOutputAttribute(bool mergeOutputs = true)
		{
			MergeOutputs = mergeOutputs;
		}

		/// <summary>
		/// Gets a value indicating whether the outputs will be merged.
		/// </summary>
		public bool MergeOutputs { get; private set; }
	}
}
