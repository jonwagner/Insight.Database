using System;
using System.Collections.Generic;
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
#else
		/// <summary>
		/// Represents a completed false task.
		/// </summary>
		internal static readonly Task<bool> FalseTask = Task.FromResult(false);

		/// <summary>
		/// Represents a completed true task.
		/// </summary>
		internal static readonly Task<bool> TrueTask = Task.FromResult(true);
#endif
	}
}
