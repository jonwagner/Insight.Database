using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Knows how to read a record containing an object and its parent id.
	/// </summary>
	/// <typeparam name="T">The type of object to read.</typeparam>
	public interface IChildRecordReader<T>
	{
		/// <summary>
		/// Returns a reader that can read the guardian and the object.
		/// </summary>
		/// <typeparam name="TId">The type of the parent ID field.</typeparam>
		/// <returns>A record reader for the guardian.</returns>
		IRecordReader<Guardian<T, TId>> GetGuardianReader<TId>();
	}
}
