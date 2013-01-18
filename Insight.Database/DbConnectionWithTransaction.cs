using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Encapsulates a database connection that has an open transaction.
	/// Insight will automatically propagate the IDbTransaction to any database calls to the connection.
	/// </summary>
	public class DbConnectionWithTransaction : DbConnectionWrapper, IDbTransaction
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the DbConnectionWithTransaction class.
		/// </summary>
		/// <param name="connection">The connection to wrap. The connection must be open.</param>
		public DbConnectionWithTransaction(DbConnection connection) : this(connection, IsolationLevel.Unspecified)
		{
			// everything is handled in the other constructor
		}

		/// <summary>
		/// Initializes a new instance of the DbConnectionWithTransaction class.
		/// </summary>
		/// <param name="connection">The connection to wrap. The connection must be open.</param>
		/// <param name="isolationLevel">The isolation level for the transaction.</param>
		public DbConnectionWithTransaction(DbConnection connection, IsolationLevel isolationLevel)
			: base(connection)
		{
			InnerTransaction = InnerConnection.BeginTransaction(isolationLevel);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the inner transaction for the connection.
		/// </summary>
		public DbTransaction InnerTransaction { get; private set; }
		#endregion

		#region IDbTransaction Members
		/// <summary>
		/// Gets the connection associated with this transaction.
		/// </summary>
		public IDbConnection Connection
		{
			get { return this; }
		}

		/// <summary>
		/// Gets the isolation level for the transaction.
		/// </summary>
		public IsolationLevel IsolationLevel
		{
			get { return InnerTransaction.IsolationLevel; }
		}

		/// <summary>
		/// Commits the open transaction.
		/// </summary>
		public void Commit()
		{
			InnerTransaction.Commit();
		}

		/// <summary>
		/// Rolls back the open transaction.
		/// </summary>
		public void Rollback()
		{
			InnerTransaction.Rollback();
		}
		#endregion

		/// <summary>
		/// Creates a command tied to this connection and transaction.
		/// </summary>
		/// <returns>A new command for this connection and transaction.</returns>
		protected override DbCommand CreateDbCommand()
		{
			DbCommand command = base.CreateDbCommand();
			command.Transaction = InnerTransaction;
			return command;
		}

		/// <summary>
		/// Releases the unmanaged resources held by the connection.
		/// </summary>
		/// <param name="disposing">True to dispose the inner transaction.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (InnerTransaction != null)
				{
					InnerTransaction.Dispose();
					InnerTransaction = null;
				}
			}

			base.Dispose(disposing);
		}
	}
}