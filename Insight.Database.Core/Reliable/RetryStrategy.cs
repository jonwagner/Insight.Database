using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Providers;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Defines a strategy for detecting transient errors and retrying the operations.
	/// </summary>
	public class RetryStrategy : IRetryStrategy
	{
		#region Constructors
		/// <summary>
		/// Timespan for not repeating a timer operation.
		/// </summary>
		private static readonly TimeSpan NoRepeat = new TimeSpan(-1);

		/// <summary>
		/// The default retry strategy to use if none is specified.
		/// </summary>
		private static readonly RetryStrategy _default = new RetryStrategy();

		/// <summary>
		/// Initializes a new instance of the RetryStrategy class.
		/// </summary>
		public RetryStrategy()
		{
			FastFirstRetry = true;
			MaxRetryCount = 10;
			MinBackOff = new TimeSpan(0, 0, 0, 0, 100);
			MaxBackOff = new TimeSpan(0, 0, 0, 1, 0);
			IncrementalBackOff = new TimeSpan(0, 0, 0, 0, 100);
		}
		#endregion

		#region Public Configuration
		/// <summary>
		/// Gets the default retry strategy to use for a ReliableConnection if none is specified.
		/// </summary>
		public static RetryStrategy Default { get { return _default; } }

		/// <summary>
		/// Gets or sets an event handler that is called before an operation is retried.
		/// </summary>
		public EventHandler<RetryEventArgs> Retrying { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the first retry should be fast and without delay.
		/// </summary>
		public bool FastFirstRetry { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of retries to perform. 1 = one initial try plus one retry.
		/// </summary>
		public int MaxRetryCount { get; set; }

		/// <summary>
		/// Gets or sets the minimum amount of time to back off for a retry. Default = 100 milliseconds.
		/// </summary>
		public TimeSpan MinBackOff { get; set; }

		/// <summary>
		/// Gets or sets the maximum amount of time to back off for a retry. Default = 1 second.
		/// </summary>
		public TimeSpan MaxBackOff { get; set; }

		/// <summary>
		/// Gets or sets the amount of time to add between each retry. Default = 100 milliseconds.
		/// </summary>
		public TimeSpan IncrementalBackOff { get; set; }
		#endregion

		/// <summary>
		/// Executes a function and retries the action if a transient error is detected.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="commandContext">The IDbCommand that is expected to be executed within the function,
		/// or null if the operation is being performed directly on a connection.</param>
		/// <param name="func">The function to execute.</param>
		/// <returns>The result of the function.</returns>
		public TResult ExecuteWithRetry<TResult>(IDbCommand commandContext, Func<TResult> func)
		{
			if (func == null) throw new ArgumentNullException("func");

			int attempt = 0;
			TimeSpan delay = MinBackOff;

			while (true)
			{
				try
				{
                    return func();
				}
				catch (Exception ex)
				{
					// if it's not a transient error, then let it go
				    if (!IsTransientException(ex))
				        throw;

				    // if the number of retries has been exceeded then throw
				    if (attempt >= MaxRetryCount)
				        throw;

				    // first off the event to tell someone that we are going to retry this
				    if (OnRetrying(commandContext, ex, attempt))
				        throw;

					// the retry might take a while. we shouldn't hold open transactions that long, so abort
					// and let another retry mechanism handle it
					if (commandContext != null && commandContext.Transaction != null)
						throw;

					// since we're retrying the command, we don't want to hold open the transaction
					// so close it here and we'll reopen on retry
					if (commandContext != null)
				        commandContext.EnsureIsClosed();

					// wait before retrying the command
					// unless this is the first attempt or first retry is disabled
					if (attempt > 0 || !FastFirstRetry)
					{
						Thread.Sleep(delay);

						// update the increment
						delay += IncrementalBackOff;
						if (delay > MaxBackOff)
							delay = MaxBackOff;
					}

					// increment the attempt
					attempt++;
				}
			}
		}

		/// <summary>
		/// Asynchronously executes a function and retries the action if a transient error is detected.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="commandContext">The IDbCommand that is expected to be executed within the function,
		/// or null if the operation is being performed directly on a connection.</param>
		/// <param name="func">The function to execute.</param>
		/// <returns>The result of the function.</returns>
		public Task<TResult> ExecuteWithRetryAsync<TResult>(IDbCommand commandContext, Func<Task<TResult>> func)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

			// when that task finishes, handle the results
			CheckAsyncResult(commandContext, tcs, func, 0, MinBackOff);

			return tcs.Task;
		}

		/// <summary>
		/// Determines if an exception is a transient error.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is a transient error, false if the command should not be retried.</returns>
		public virtual bool IsTransientException(Exception exception)
		{
			InsightDbProvider provider;

			try
			{
				provider = InsightDbProvider.For(exception);
			}
			catch (NotImplementedException)
			{
				// if the provider lookup fails, then this can't be a transient exception
				return false;
			}

			return provider.IsTransientException(exception);
		}

		/// <summary>
		/// Checks the result of an async operation and continues the retry operation.
		/// </summary>
		/// <typeparam name="TResult">The type of the result returned from the operation.</typeparam>
		/// <param name="commandContext">The IDbCommand that is currently being executed.</param>
		/// <param name="tcs">The TaskCompletionSource for the completion of the operation.</param>
		/// <param name="func">The function to execute.</param>
		/// <param name="attempt">The number of the previous attempt. 0 is the first attempt.</param>
		/// <param name="delay">The current delay in between retry attempts.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void CheckAsyncResult<TResult>(IDbCommand commandContext, TaskCompletionSource<TResult> tcs, Func<Task<TResult>> func, int attempt, TimeSpan delay)
		{
			try
			{
				func().ContinueWith(t =>
				{
					try
					{
						switch (t.Status)
						{
							case TaskStatus.Canceled:
								tcs.SetCanceled();
								return;
							case TaskStatus.RanToCompletion:
								tcs.SetResult(t.Result);
								return;
						}

						// if it's not a transient error, then let it go
						var ex = t.Exception;
						if (!ex.Flatten().InnerExceptions.Any(IsTransientException))
						{
							tcs.SetException(ex);
							return;
						}

						// if the number of retries has been exceeded then throw
						if (attempt >= MaxRetryCount)
						{
							tcs.SetException(ex);
							return;
						}

						// first off the event to tell someone that we are going to retry this
						if (OnRetrying(commandContext, ex, attempt))
						{
							tcs.SetException(ex);
							return;
						}

						// if this is the first attempt and a fastretry, then just execute it
						if (attempt == 0 && FastFirstRetry)
						{
							CheckAsyncResult(commandContext, tcs, func, attempt + 1, delay);
							return;
						}

						// update the increment
						TimeSpan nextDelay = delay + IncrementalBackOff;
						if (nextDelay > MaxBackOff)
							nextDelay = MaxBackOff;

						// create a timer for the retry
						// note that we need to put the timer into a closure so we can dispose it
						// but we have to wait for the local variable to be assigned before we can start the timer
						Timer timer = null;
						timer = new Timer(_ =>
						{
							try
							{
								CheckAsyncResult(commandContext, tcs, func, attempt + 1, nextDelay);
							}
							finally
							{
								timer.Dispose();
							}
						});

						// start the timer
						timer.Change(delay, NoRepeat);
					}
					catch (Exception ex)
					{
						// Something went horribly wrong. Perhaps the OnRetrying event threw an exception.
						// Pass the exception downstream so we don't get a task hang.
						tcs.SetException(ex);
					}
				});
			}
			catch (Exception ex)
			{
				// func failed or returned a null task.
				// Pass the exception downstream so we don't get a task hang.
				tcs.SetException(ex);
			}
		}

		/// <summary>
		/// Fires the Retrying event before an operation is retried.
		/// </summary>
		/// <param name="commandContext">The IDbCommand that is being executed or null if the operation is being performed directly on a connection.</param>
		/// <param name="exception">The exception causing the retry.</param>
		/// <param name="attempt">The number of the attempt just completed. Zero is the first attempt.</param>
		/// <returns>True if the operation has been cancelled, false if the operation should be retried.</returns>
		private bool OnRetrying(IDbCommand commandContext, Exception exception, int attempt)
		{
			// log that we are retrying, then try again
			var retrying = Retrying;
			if (retrying == null)
				return false;

			// call the handler and return the value
			RetryEventArgs e = new RetryEventArgs() { Exception = exception, CommandContext = commandContext, Attempt = attempt };
			retrying(this, e);
			return e.Cancel;
		}
	}
}
