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
	static class Helpers
	{
#if NODBASYNC
		/// <summary>
		/// Represents a completed false task.
		/// </summary>
		internal static readonly Task<bool> FalseTask = Task<bool>.Factory.StartNew(() => false);

		/// <summary>
		/// Represents a completed true task.
		/// </summary>
		internal static readonly Task<bool> TrueTask = Task<bool>.Factory.StartNew(() => true);

		/// <summary>
		/// Represents a completed IDataReader task.
		/// </summary>
		internal static readonly Task<IDataReader> DataReaderTask = Task<IDataReader>.Factory.StartNew(() => (IDataReader)null);

		/// <summary>
		/// Returns a completed task from the given result.
		/// </summary>
		/// <typeparam name="T">The type of the result.</typeparam>
		/// <param name="result">The result.</param>
		/// <returns>A completed task.</returns>
		internal static Task<T> FromResult<T>(T result)
		{
			return Task.Factory.StartNew(() => result);
		}
#else
		/// <summary>
		/// Represents a completed false task.
		/// </summary>
		internal static readonly Task<bool> FalseTask = Task.FromResult(false);

		/// <summary>
		/// Represents a completed true task.
		/// </summary>
		internal static readonly Task<bool> TrueTask = Task.FromResult(true);

		/// <summary>
		/// Represents a completed IDataReader task.
		/// </summary>
		internal static readonly Task<IDataReader> DataReaderTask = Task.FromResult((IDataReader)null);

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
#endif
	}
}
