using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Wraps a DbConnection and translates OptimisticConcurrencyExceptions.
	/// </summary>
	[System.ComponentModel.DesignerCategory("Code")]
	public class OptimisticConnection : DbConnectionWrapper
	{
		/// <summary>
		/// Initializes a new instance of the OptimisticConnection class.
		/// </summary>
		/// <param name="innerConnection">The inner connection.</param>
		public OptimisticConnection(DbConnection innerConnection)
			: base(innerConnection)
		{
		}

		/// <summary>
		/// Returns true if the exception is an optimistic concurrency exception.
		/// This method may be overridden.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>Whether the exception is a concurrency exception.</returns>
		public virtual bool IsConcurrencyException(Exception exception)
		{
			if (!(exception is DbException))
				return false;

			return exception.Message.Contains("CONCURRENCY CHECK");
		}

		/// <inheritdoc/>
		protected override DbCommand CreateDbCommand()
		{
			return new OptimisticCommand(this, InnerConnection.CreateCommand());
		}
	}

	/// <summary>
	/// Wraps a DbConnection and translates OptimisticConcurrencyExceptions.
	/// </summary>
	/// <typeparam name="TConnection">The type of connection to create.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class OptimisticConnection<TConnection> : OptimisticConnection where TConnection : DbConnection, new()
	{
		/// <summary>
		/// Initializes a new instance of the OptimisticConnection class.
		/// </summary>
		public OptimisticConnection() : base(new TConnection())
		{
		}

		/// <summary>
		/// Initializes a new instance of the OptimisticConnection class.
		/// </summary>
		/// <param name="connectionString">The connection string to the database.</param>
		public OptimisticConnection(string connectionString)
			: base(new TConnection())
		{
			InnerConnection.ConnectionString = connectionString;
		}
	}
}
