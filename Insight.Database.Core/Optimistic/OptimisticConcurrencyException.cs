using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Indicates that a database call failed due to an optimistic concurrency issue.
	/// </summary>
	[Serializable]
	public class OptimisticConcurrencyException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the OptimisticConcurrencyException class.
		/// </summary>
		public OptimisticConcurrencyException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the OptimisticConcurrencyException class.
		/// </summary>
		/// <param name="innerException">The exception causing the issue.</param>
		public OptimisticConcurrencyException(Exception innerException) : base("One or more records were changed.", innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OptimisticConcurrencyException class.
		/// </summary>
		/// <param name="message">The message for the exception.</param>
		/// <param name="innerException">The exception causing the issue.</param>
		public OptimisticConcurrencyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OptimisticConcurrencyException class.
		/// </summary>
		/// <param name="message">The message for the exception.</param>
		public OptimisticConcurrencyException(string message) : base(message)
		{
		}

#if !NETSTANDARD1_5
        /// <summary>
        /// Initializes a new instance of the OptimisticConcurrencyException class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected OptimisticConcurrencyException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
#endif
    }
}
