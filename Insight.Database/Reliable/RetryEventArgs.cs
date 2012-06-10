using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Represents an event that occurs when a database connection is about to be retried.
	/// </summary>
	public sealed class RetryEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the IDbCommand that is executing.
		/// </summary>
		public IDbCommand CommandContext { get; set; }

		/// <summary>
		/// Gets or sets the exception that was caught and retried.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Gets or sets the number of the attempt just completed. Zero is the first attempt.
		/// </summary>
		public int Attempt { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the RetryStrategy should cancel the retry operation.
		/// </summary>
		public bool Cancel { get; set; }
	}
}
