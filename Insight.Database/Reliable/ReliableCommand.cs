using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Wraps an IDbCommand and automatically handles retry logic for transient errors.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbCommand is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbCommand would be redundant without adding additional information.")]
	public sealed class ReliableCommand : DbCommandWrapper
	{
		#region Private Members
		/// <summary>
		/// The retry strategy to use for the command.
		/// </summary>
		private IRetryStrategy _retryStrategy;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ReliableCommand class, and bind it to the specified ReliableConnection and innerCommand.
		/// </summary>
		/// <param name="retryStrategy">The retry strategy to use for the command.</param>
		/// <param name="innerConnection">The innerConnection to bind to.</param>
		/// <param name="innerCommand">The innerCommand to bind to.</param>
		public ReliableCommand(IRetryStrategy retryStrategy, ReliableConnection innerConnection, DbCommand innerCommand) : base(innerConnection, innerCommand)
		{
			_retryStrategy = retryStrategy;
		}
		#endregion

		#region Synchronous DbCommand Implementation
		/// <inheritdoc/>
		public override int ExecuteNonQuery()
		{
			return ExecuteWithRetry(
				() =>
				{
					FixupParameters();
                    InnerConnection.AutoOpen();
					return InnerCommand.ExecuteNonQuery();
				});
		}

		/// <inheritdoc/>	
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteWithRetry(
				() =>
				{
					FixupParameters();
                    InnerConnection.AutoOpen();
					return InnerCommand.ExecuteReader(behavior);
				});
		}

		/// <inheritdoc/>
		public override object ExecuteScalar()
		{
			return ExecuteWithRetry(
				() =>
				{
					FixupParameters();
                    InnerConnection.AutoOpen();
					return InnerCommand.ExecuteScalar();
				});
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
				FixupParameters();
                InnerConnection.AutoOpen();

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
        /// <inheritdoc/>
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(
				() =>
				{
					FixupParameters();
                    InnerConnection.AutoOpen();
					return InnerCommand.ExecuteReaderAsync(behavior, cancellationToken);
				});
		}

		/// <inheritdoc/>
		public override Task<int> ExecuteNonQueryAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(
				() =>
				{
					FixupParameters();
                    InnerConnection.AutoOpen();
					return InnerCommand.ExecuteNonQueryAsync(cancellationToken);
				});
		}

		/// <inheritdoc/>
		public override Task<object> ExecuteScalarAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(() =>
			{
                InnerConnection.AutoOpen();
			    return InnerCommand.ExecuteScalarAsync(cancellationToken);
			});
		}
#endif
		#endregion

		#region Support Methods
		/// <inheritdoc/>
		public override void Prepare()
		{
			ExecuteWithRetry(() => { InnerCommand.Prepare(); return true; });
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

        private void FixupParameters()
        {
            foreach (var reader in Parameters.OfType<DbParameter>().Select(p => p.Value).OfType<ObjectListDbDataReader>())
                reader.Reset();
        }
		#endregion
	}
}
