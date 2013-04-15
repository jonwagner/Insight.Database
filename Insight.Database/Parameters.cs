using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Convenience class for empty parameter list.
	/// </summary>
	public static class Parameters
	{
		/// <summary>
		/// An empty parameter.
		/// </summary>
		public static readonly object Empty = new object();

		/// <summary>
		/// An empty list.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The array is immutable and the object is immutable")]
		public static readonly IEnumerable<object> EmptyList = new object[0];
	}
}
