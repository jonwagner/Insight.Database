using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Wraps an IDbCommand and automatically handles retry logic for transient errors.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbCommand is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbCommand would be redundant without adding additional information.")]
	public sealed class ReliableCommand : DbCommand
	{
		#region Private Members
		/// <summary>
		/// The retry strategy to use for the command.
		/// </summary>
		private IRetryStrategy _retryStrategy;

		/// <summary>
		/// Gets the inner command to use to execute the command.
		/// </summary>
		internal DbCommand InnerCommand { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ReliableCommand class, and bind it to the specified ReliableConnection and innerCommand.
		/// </summary>
		/// <param name="retryStrategy">The retry strategy to use for the command.</param>
		/// <param name="innerCommand">The innerCommand to bind to.</param>
		public ReliableCommand(IRetryStrategy retryStrategy, DbCommand innerCommand)
		{
			_retryStrategy = retryStrategy;
			InnerCommand = innerCommand;
		}
		#endregion

		#region Synchronous DbCommand Implementation
		public override int ExecuteNonQuery()
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteNonQuery());
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteReader(behavior));
		}

		public override object ExecuteScalar()
		{
			return ExecuteWithRetry(() => InnerCommand.ExecuteScalar());
		}
		#endregion

		#region Async Methods
#if NODBASYNC
		/// <summary>
		/// Executes the command asynchronously with retry.
		/// </summary>
		/// <param name="commandBehavior">The commandBehavior to execute with.</param>
		/// <param name="cancellationToken">The cancellationToken to use for the operation.</param>
		/// <returns>A task that provides an IDataReader of the results when complete.</returns>
		internal Task<IDataReader> GetReaderAsync(CommandBehavior commandBehavior, CancellationToken cancellationToken)
		{
			// fallback to calling execute reader in a blocking task
			return ExecuteWithRetryAsync(() =>
			{
				// start the sql command executing
				var sqlCommand = InnerCommand as System.Data.SqlClient.SqlCommand;
				if (sqlCommand != null)
					return Task<IDataReader>.Factory.FromAsync(sqlCommand.BeginExecuteReader(commandBehavior), ar => sqlCommand.EndExecuteReader(ar));

				return Task<IDataReader>.Factory.StartNew(
				() =>
				{
					cancellationToken.ThrowIfCancellationRequested();

					return InnerCommand.ExecuteReader(commandBehavior);
				});
			});
		}
#endif

#if !NODBASYNC
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(() => InnerCommand.ExecuteReaderAsync(behavior, cancellationToken));
		}

		public override Task<int> ExecuteNonQueryAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(() => InnerCommand.ExecuteNonQueryAsync(cancellationToken));
		}

		public override Task<object> ExecuteScalarAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(() => InnerCommand.ExecuteScalarAsync(cancellationToken));
		}
#endif
		#endregion

		#region Support Methods
		public override void Prepare()
		{
			ExecuteWithRetry(() => { InnerCommand.Prepare(); return true; });
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				InnerCommand.Dispose();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		#endregion

		#region IDbCommand Implementation
		public override void Cancel()
		{
			InnerCommand.Cancel();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
		public override string CommandText
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

		public override int CommandTimeout
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

		public override CommandType CommandType
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

		protected override DbConnection DbConnection
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

		protected override DbParameter CreateDbParameter()
		{
			return InnerCommand.CreateParameter();
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get { return InnerCommand.Parameters; }
		}

		protected override DbTransaction DbTransaction
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

		public override UpdateRowSource UpdatedRowSource
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

		public override bool DesignTimeVisible
		{
			get
			{
				return InnerCommand.DesignTimeVisible;
			}

			set
			{
				InnerCommand.DesignTimeVisible = value;
			}
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

		/// <summary>
		/// Executes a function with the retry logic specified on the ReliableConnection.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the function.</typeparam>
		/// <param name="function">The function to execute and return.</param>
		/// <returns>The return value of the function.</returns>
		private Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> function)
		{
			return _retryStrategy.ExecuteWithRetryAsync(this, function);
		}
		#endregion
	}
}
