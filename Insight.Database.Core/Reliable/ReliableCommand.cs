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
	[System.ComponentModel.DesignerCategory("Code")]
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
                    InnerConnection.EnsureIsOpen();
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
                    InnerConnection.EnsureIsOpen();
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
                    InnerConnection.EnsureIsOpen();
					return InnerCommand.ExecuteScalar();
				});
		}
		#endregion

		#region Async Methods
        /// <inheritdoc/>
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(
				async () =>
				{
					FixupParameters();

                    await InnerConnection.EnsureIsOpenAsync(cancellationToken);
					return await InnerCommand.ExecuteReaderAsync(behavior, cancellationToken);
				});
		}

		/// <inheritdoc/>
		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			return ExecuteWithRetryAsync(
				async () =>
				{
					FixupParameters();

                    await InnerConnection.EnsureIsOpenAsync(cancellationToken);
					return await InnerCommand.ExecuteNonQueryAsync(cancellationToken);
				});
		}

		/// <inheritdoc/>
		public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
		{
		    return ExecuteWithRetryAsync(
				async () =>
				{
					await InnerConnection.EnsureIsOpenAsync(cancellationToken);
					return await InnerCommand.ExecuteScalarAsync(cancellationToken);
				});
		}
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
