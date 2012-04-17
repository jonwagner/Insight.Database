using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Helper class to make expando objects easier to use.
	/// </summary>
	public static class ExpandoExtensions
	{
		/// <summary>
		/// Converts an object to a FastExpando.
		/// </summary>
		/// <typeparam name="T">The type of object to convert.</typeparam>
		/// <param name="obj">The object to convert.</param>
		/// <returns>A fast expando containing the public properties of the object.</returns>
		public static FastExpando Expand<T>(this T obj)
		{
			return FastExpando.FromObject(obj);
		}

		/// <summary>
		/// Converts an object to a FastExpando.
		/// </summary>
		/// <typeparam name="T1">The type of object to merge into.</typeparam>
		/// <typeparam name="T2">The type of object to merge.</typeparam>
		/// <param name="obj1">The object to convert.</param>
		/// <param name="obj2">The other object to merge in.</param>
		/// <returns>A fast expando containing the public properties of the object.</returns>
		public static FastExpando Expand<T1, T2>(this T1 obj1, T2 obj2)
		{
			return FastExpando.FromObject(obj1).Expand(obj2);
		}

		/// <summary>
		/// Mutates a list of FastExpandos.
		/// </summary>
		/// <param name="list">The list to mutate.</param>
		/// <param name="map">The mapping of input to output field names.</param>
		/// <returns>The same list, but mutated.</returns>
		public static IEnumerable<FastExpando> Mutate(this IEnumerable<FastExpando> list, Dictionary<string, string> map)
		{
			foreach (FastExpando obj in list)
			{
				if (obj == null)
					yield return null;
				else
				{
					obj.Mutate(map);
					yield return obj;
				}
			}
		}

		/// <summary>
		/// Transforms a list of FastExpandos.
		/// </summary>
		/// <param name="list">The list to transform.</param>
		/// <param name="map">The mapping of input to output field names.</param>
		/// <returns>A new list of transformed expandos.</returns>
		public static IEnumerable<FastExpando> Transform(this IEnumerable<FastExpando> list, Dictionary<string, string> map)
		{
			foreach (FastExpando obj in list)
			{
				if (obj == null)
					yield return null;
				else
					yield return obj.Transform(map);
			}
		}
	}
}
