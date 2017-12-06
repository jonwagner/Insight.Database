using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Internal helpers.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "The reader is owned by other code.")]
    public static class Helpers
	{
        /// <summary>
        /// Represents a completed false task.
        /// </summary>
        internal static readonly Task<bool> FalseTask = Task.FromResult(false);

		/// <summary>
		/// Returns a completed task from the given result.
		/// </summary>
		/// <typeparam name="T">The type of the result.</typeparam>
		/// <param name="result">The result.</param>
		/// <returns>A completed task.</returns>
		internal static Task<T> FromResult<T>(T result)
		{
			return Task.FromResult(result);
		}

        /// <summary>
        /// Determines whether two strings are case-insensitive equal.
        /// </summary>
        /// <param name="s1">The first string.</param>
        /// <param name="s2">The second string.</param>
        /// <returns>True if they are equal.</returns>
        internal static bool IsIEqualTo(this string s1, string s2)
        {
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns the only object matching the predicate, or default/null otherwise.
        /// </summary>
        /// <typeparam name="T">The type of object in the enumeration.</typeparam>
        /// <param name="enumerable">The enumberable to scan.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns>The only object matching the predicate, or default/null.</returns>
        internal static T OnlyOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            bool found = false;
            var result = default(T);

            foreach (var t in enumerable.Where(predicate))
            {
                if (found)
                {
                    return default(T);
                }
                else
                {
                    found = true;
                    result = t;
                }
            }

            return result;
        }
	}
}
