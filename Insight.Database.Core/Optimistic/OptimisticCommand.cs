using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Translates database exceptions to OptimisticConcurrencyExceptions.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[System.ComponentModel.DesignerCategory("Code")]
	class OptimisticCommand : DbCommandWrapper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OptimisticCommand class, and bind it to the specified OptimisticConnection and innerCommand.
		/// </summary>
		/// <param name="innerConnection">The innerConnection to bind to.</param>
		/// <param name="innerCommand">The innerCommand to bind to.</param>
		public OptimisticCommand(OptimisticConnection innerConnection, DbCommand innerCommand) : base(innerConnection, innerCommand)
		{
		}
		#endregion

		#region Synchronous DbCommand Implementation
		/// <inheritdoc/>
		public override int ExecuteNonQuery()
		{
			return ExecuteAndTranslateExceptions(() => InnerCommand.ExecuteNonQuery());
		}

		/// <inheritdoc/>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteAndTranslateExceptions(() => InnerCommand.ExecuteReader(behavior));
		}

		/// <inheritdoc/>
		public override object ExecuteScalar()
		{
			return ExecuteAndTranslateExceptions(() => InnerCommand.ExecuteScalar());
		}
		#endregion

		/// <inheritdoc/>
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteAndTranslateExceptionsAsync(() => InnerCommand.ExecuteReaderAsync(behavior, cancellationToken));
		}

		/// <inheritdoc/>
		public override Task<int> ExecuteNonQueryAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteAndTranslateExceptionsAsync(() => InnerCommand.ExecuteNonQueryAsync(cancellationToken));
		}

		/// <inheritdoc/>
		public override Task<object> ExecuteScalarAsync(System.Threading.CancellationToken cancellationToken)
		{
			return ExecuteAndTranslateExceptionsAsync(() => InnerCommand.ExecuteScalarAsync(cancellationToken));
		}

		/// <summary>
		/// Returns true if the exception is an optimistic concurrency exception.
		/// This method may be overridden.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>Whether the exception is a concurrency exception.</returns>
		protected bool IsConcurrencyException(Exception exception)
		{
			return ((OptimisticConnection)Connection).IsConcurrencyException(exception);
		}

		/// <summary>
		/// Perform an action and translate exceptions.
		/// </summary>
		/// <typeparam name="T">The type of the result of the action.</typeparam>
		/// <param name="action">The action to perform.</param>
		/// <returns>The result of the action.</returns>
		private T ExecuteAndTranslateExceptions<T>(Func<T> action)
		{
			try
			{
				return action();
			}
			catch (DbException exception)
			{
				if (IsConcurrencyException(exception))
					throw new OptimisticConcurrencyException(exception);
				else
					throw;
			}
		}

		/// <summary>
		/// Asynchronously performs an action and translate exceptions.
		/// </summary>
		/// <typeparam name="T">The type of the result of the action.</typeparam>
		/// <param name="action">The action to perform.</param>
		/// <returns>A task representing the result of the action.</returns>
		private async Task<T> ExecuteAndTranslateExceptionsAsync<T>(Func<Task<T>> action)
		{
			try
			{
				return await action();
			}
			catch (AggregateException e)
			{
				if (e.Flatten().InnerExceptions.Any(x => IsConcurrencyException(x)))
					throw new OptimisticConcurrencyException(e);

				throw;
			}
			catch (Exception e)
			{
				if (IsConcurrencyException(e))
					throw new OptimisticConcurrencyException(e);

				throw;
			}
		}
	}
}
