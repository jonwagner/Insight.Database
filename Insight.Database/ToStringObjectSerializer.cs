using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Serializes an object to a string by using the ToString method.
	/// </summary>
	public static class ToStringObjectSerializer
	{
		/// <summary>
		/// Serializes an object to a string.
		/// </summary>
		/// <param name="value">The object to serialize.</param>
		/// <param name="type">The type of the object.</param>
		/// <returns>The serialized representation of the object.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "The caller expects these parameters.")]
		public static string Serialize(object value, Type type)
		{
			if (value == null)
				return null;

			return value.ToString();
		}
	}
}
