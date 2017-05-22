using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Defines a strategy for handling retries for transient exceptions in database connections such as SQL Azure.
	/// </summary>
	public interface IRetryStrategy
	{
		/// <summary>
		/// Executes a function and retries the action if a transient error is detected.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="commandContext">The IDbCommand that is expected to be executed within the function, 
		/// or null if the operation is being performed directly on a connection.</param>
		/// <param name="func">The function to execute.</param>
		/// <returns>The result of the function.</returns>
		TResult ExecuteWithRetry<TResult>(IDbCommand commandContext, Func<TResult> func);

		/// <summary>
		/// Asynchronously executes a function and retries the action if a transient error is detected.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="commandContext">The IDbCommand that is expected to be executed within the function, 
		/// or null if the operation is being performed directly on a connection.</param>
		/// <param name="func">The function to execute.</param>
		/// <returns>The result of the function.</returns>
		Task<TResult> ExecuteWithRetryAsync<TResult>(IDbCommand commandContext, Func<Task<TResult>> func);
	}
}
