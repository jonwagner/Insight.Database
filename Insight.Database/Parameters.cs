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
		/// An empty parameter array.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly", Justification = "The array is immutable and the object is immutable")]
		public static readonly object[] EmptyArray = new object[0];

		/// <summary>
		/// An empty list.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The array is immutable and the object is immutable")]
		public static readonly IEnumerable<object> EmptyList = new object[0];

		/// <summary>
		/// Returns an empty list of a given type.
		/// </summary>
		/// <typeparam name="T">The type contained in the list.</typeparam>
		/// <returns>An empty enumerator of the given type.</returns>
		public static IEnumerable<T> EmptyListOf<T>()
		{
			yield break;
		}
	}
}
