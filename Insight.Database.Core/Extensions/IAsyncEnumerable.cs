using System;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Allows record streams to be read asynchronously.
	/// </summary>
	/// <typeparam name="T">The type of object returned by the enumerator.</typeparam>
	public interface IAsyncEnumerable<T> : IDisposable
	{
		/// <summary>
		/// Gets the current object in the stream.
		/// </summary>
		T Current { get; }

		/// <summary>
		/// Moves to the next record asynchronously and returns a task representing whether a record was retrieved.
		/// </summary>
		/// <returns>A task representing whether a record was retrieved.</returns>
		Task<bool> MoveNextAsync();

		/// <summary>
		/// Advances the reader to the next result.
		/// </summary>
		/// <returns>A task representing whether there is another result set.</returns>
		Task<bool> NextResultAsync();
	}
}
