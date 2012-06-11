using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
			int attempt = 0;
			TimeSpan delay = MinBackOff;

			for (;;)
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
			// we are only going to try to handle sql server exceptions
			SqlException sqlException = exception as SqlException;
			if (sqlException == null)
				return false;

			switch (sqlException.Number)
			{
				case 40197:		// The service has encountered an error processing your request. Please try again.
				case 40501:		// The service is currently busy. Retry the request after 10 seconds.
				case 10053:		// A transport-level error has occurred when receiving results from the server. An established connection was aborted by the software in your host machine.
				case 10054:		// A transport-level error has occurred when sending the request to the server. (provider: TCP Provider, error: 0 – An existing connection was forcibly closed by the remote host.)
				case 10060:		// A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: TCP Provider, error: 0 – A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.)
				case 40613:		// Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer support, and provide them the session tracing ID of ZZZZZ.
				case 40143:		// The service has encountered an error processing your request. Please try again.
				case 233:		// The client was unable to establish a connection because of an error during connection initialization process before login. Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. (provider: TCP Provider, error: 0 – An existing connection was forcibly closed by the remote host.)
				case 64:		// A connection was successfully established with the server, but then an error occurred during the login process. (provider: TCP Provider, error: 0 – The specified network name is no longer available.)
				case 20:		// The instance of SQL Server you attempted to connect to does not support encryption.
					return true;
			}

			return false;
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
		private void CheckAsyncResult<TResult>(IDbCommand commandContext, TaskCompletionSource<TResult> tcs, Func<Task<TResult>> func, int attempt, TimeSpan delay)
		{
			func().ContinueWith(t =>
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
				Exception ex = t.Exception;
				if (!IsTransientException(ex))
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
					CheckAsyncResult(commandContext, tcs, func, attempt + 1, nextDelay);
					timer.Dispose();
				});

				// start the timer
				timer.Change(delay, NoRepeat);
			});
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
