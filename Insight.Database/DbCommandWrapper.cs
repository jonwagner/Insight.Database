using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Wraps an IDbCommand and automatically handles retry logic for transient errors.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbCommand is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbCommand would be redundant without adding additional information.")]
	public class DbCommandWrapper : DbCommand
	{
		#region Private Members
		/// <summary>
		/// Gets the inner command to use to execute the command.
		/// </summary>
		internal DbCommand InnerCommand { get; private set; }

		internal DbConnectionWrapper InnerConnection { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the DbCommandWrapper class, and bind it to the specified ReliableConnection and innerCommand.
		/// </summary>
		/// <param name="innerConnection">The innerConnection to bind to.</param>
		/// <param name="innerCommand">The innerCommand to bind to.</param>
		public DbCommandWrapper(DbConnectionWrapper innerConnection, DbCommand innerCommand)
		{
			InnerConnection = innerConnection;
			InnerCommand = innerCommand;
		}
		#endregion

		#region Synchronous DbCommand Implementation
		public override int ExecuteNonQuery()
		{
			return InnerCommand.ExecuteNonQuery();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return InnerCommand.ExecuteReader(behavior);
		}

		public override object ExecuteScalar()
		{
			return InnerCommand.ExecuteScalar();
		}
		#endregion

		#region Async Methods
#if !NODBASYNC
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
		{
			return InnerCommand.ExecuteReaderAsync(behavior, cancellationToken);
		}

		public override Task<int> ExecuteNonQueryAsync(System.Threading.CancellationToken cancellationToken)
		{
			return InnerCommand.ExecuteNonQueryAsync(cancellationToken);
		}

		public override Task<object> ExecuteScalarAsync(System.Threading.CancellationToken cancellationToken)
		{
			return InnerCommand.ExecuteScalarAsync(cancellationToken);
		}
#endif
		#endregion

		#region Support Methods
		public override void Prepare()
		{
			InnerCommand.Prepare();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (InnerCommand != null)
						InnerCommand.Dispose();
				}
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
				return InnerConnection;
			}

			set
			{
				InnerConnection = (DbConnectionWrapper)value;
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
	}
}
