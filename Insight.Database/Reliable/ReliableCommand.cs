using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Wraps an IDbCommand and automatically handles retry logic for transient errors.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbCommand is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbCommand would be redundant without adding additional information.")]
	public sealed class ReliableCommand : IDbCommand
	{
		#region Private Members
		/// <summary>
		/// The retry strategy to use for the command.
		/// </summary>
		private IRetryStrategy _retryStrategy;

		/// <summary>
		/// Gets the inner command to use to execute the command.
		/// </summary>
		internal IDbCommand InnerCommand { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ReliableCommand class, and bind it to the specified ReliableConnection and innerCommand.
		/// </summary>
		/// <param name="retryStrategy">The retry strategy to use for the command.</param>
		/// <param name="innerCommand">The innerCommand to bind to.</param>
		public ReliableCommand(IRetryStrategy retryStrategy, IDbCommand innerCommand)
		{
			_retryStrategy = retryStrategy;
			InnerCommand = innerCommand;
		}
		#endregion

		#region Async Methods
		/// <summary>
		/// Executes the command asynchronously with retry.
		/// </summary>
		/// <param name="commandBehavior">The commandBehavior to execute with.</param>
		/// <returns>A task that provides an IDataReader of the results when complete.</returns>
		public Task<IDataReader> GetReaderAsync(CommandBehavior commandBehavior)
		{
			// start the sql command executing
			SqlCommand sqlCommand = InnerCommand as SqlCommand;
			if (sqlCommand == null)
				throw new InvalidOperationException("Cannot perform an async query on a command that is not a SqlCommand");

			return _retryStrategy.ExecuteWithRetryAsync(this, () => sqlCommand.GetReaderAsync(commandBehavior));
		}
		#endregion

		#region IDbCommand Implementation
		public void Cancel()
		{
			InnerCommand.Cancel();
		}

		public string CommandText
		{
			get
			{
				return InnerCommand.CommandText;
			}

			set
			{
				InnerCommand.CommandText = value;
			}
		}

		public int CommandTimeout
		{
			get
			{
				return InnerCommand.CommandTimeout;
			}

			set
			{
				InnerCommand.CommandTimeout = value;
			}
		}

		public CommandType CommandType
		{
			get
			{
				return InnerCommand.CommandType;
			}

			set
			{
				InnerCommand.CommandType = value;
			}
		}

		public IDbConnection Connection
		{
			get
			{
				return InnerCommand.Connection;
			}

			set
			{
				InnerCommand.Connection = value;
			}
		}

		public IDbDataParameter CreateParameter()
		{
			return InnerCommand.CreateParameter();
		}

		public int ExecuteNonQuery()
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteNonQuery());
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteReader(behavior));
		}

		public IDataReader ExecuteReader()
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteReader());
		}

		public object ExecuteScalar()
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteScalar());
		}

		public IDataParameterCollection Parameters
		{
			get { return InnerCommand.Parameters; }
		}

		public void Prepare()
		{
			ExecuteWithRetry(() => { InnerCommand.Prepare(); return true; });
		}

		public IDbTransaction Transaction
		{
			get
			{
				return InnerCommand.Transaction;
			}

			set
			{
				InnerCommand.Transaction = value;
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				return InnerCommand.UpdatedRowSource;
			}

			set
			{
				InnerCommand.UpdatedRowSource = value;
			}
		}

		public void Dispose()
		{
			InnerCommand.Dispose();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Executes a function with the retry logic specified on the ReliableConnection.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="function">The function to execute and return.</param>
		/// <returns>The return value of the function.</returns>
		private TResult ExecuteWithRetry<TResult>(Func<TResult> function)
		{
			return _retryStrategy.ExecuteWithRetry(this, function);
		}
		#endregion
	}
}
