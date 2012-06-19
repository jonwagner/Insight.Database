using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbConnection is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbConnection would be redundant without adding additional information.")]
	public class ReliableConnection : DbConnection
	{
		#region Private Members
		/// <summary>
		/// Gets the retry strategy to use to handle exceptions.
		/// </summary>
		public IRetryStrategy RetryStrategy { get; private set; }

		/// <summary>
		/// Gets the inner connection to use to execute the database commands.
		/// </summary>
		internal DbConnection InnerConnection { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ReliableConnection class.
		/// A default retry strategy is used.
		/// </summary>
		/// <param name="innerConnection">The inner connection to wrap.</param>
		public ReliableConnection(DbConnection innerConnection)
		{
			// use the default retry strategy by default
			RetryStrategy = Insight.Database.Reliable.RetryStrategy.Default;
			InnerConnection = innerConnection;
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class.
		/// </summary>
		/// <param name="innerConnection">The inner connection to wrap.</param>
		/// <param name="retryStrategy">The retry strategy to use.</param>
		public ReliableConnection(DbConnection innerConnection, IRetryStrategy retryStrategy)
		{
			RetryStrategy = retryStrategy;
			InnerConnection = innerConnection;
		}
		#endregion

		#region Core Implementation Methods
		/// <summary>
		/// Opens the database connection with retry.
		/// </summary>
		public override void Open()
		{
			RetryStrategy.ExecuteWithRetry(null, () => { InnerConnection.Open(); return true; });
		}

		/// <summary>
		/// Creates a DbCommand for calls to the database.
		/// </summary>
		/// <returns>A ReliableCommand.</returns>
		protected override DbCommand CreateDbCommand()
		{
			return new ReliableCommand(RetryStrategy, InnerConnection.CreateCommand());
		}

#if !NODBASYNC
		/// <summary>
		/// Asynchronously opens a ReliableConnection.
		/// </summary>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task representing the completion of the open operation.</returns>
		public override Task OpenAsync(CancellationToken cancellationToken)
		{
			return RetryStrategy.ExecuteWithRetryAsync(null, () => InnerConnection.OpenAsync().ContinueWith(t => { t.Wait(); return true; }));
		}
#endif
		#endregion

		#region IDbConnection Implementation
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return InnerConnection.BeginTransaction(isolationLevel);
		}

		protected override bool CanRaiseEvents
		{
			get
			{
				return false;
			}
		}

		public override void ChangeDatabase(string databaseName)
		{
			InnerConnection.ChangeDatabase(databaseName);
		}

		public override void Close()
		{
			InnerConnection.Close();
		}

		public override string ConnectionString
		{
			get
			{
				return InnerConnection.ConnectionString;
			}

			set
			{
				InnerConnection.ConnectionString = value;
			}
		}

		public override string ServerVersion
		{
			get { return InnerConnection.ServerVersion; }
		}

		public override string DataSource
		{
			get { return InnerConnection.DataSource; }
		}

		public override int ConnectionTimeout
		{
			get { return InnerConnection.ConnectionTimeout; }
		}

		public override string Database
		{
			get { return InnerConnection.Database; }
		}

		public override ConnectionState State
		{
			get { return InnerConnection.State; }
		}

		public override DataTable GetSchema()
		{
			return InnerConnection.GetSchema();
		}

		public override DataTable GetSchema(string collectionName)
		{
			return InnerConnection.GetSchema(collectionName);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return InnerConnection.GetSchema(collectionName, restrictionValues);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				InnerConnection.Dispose();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		#endregion
	}

	/// <summary>
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	/// <typeparam name="TConnection">The type of database connection to use to connect to the database.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class ReliableConnection<TConnection> : ReliableConnection where TConnection : DbConnection, new()
	{
		/// <summary>
		/// Initializes a new instance of the ReliableConnection class.
		/// </summary>
		public ReliableConnection() : base(new TConnection())
		{
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class with the given retry strategy.
		/// </summary>
		/// <param name="retryStrategy">The retry strategy to use for the connection.</param>
		public ReliableConnection(IRetryStrategy retryStrategy)
			: base(new TConnection(), retryStrategy)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class with the given retry strategy.
		/// A default retry strategy is used.
		/// </summary>
		/// <param name="connectionString">The connection string to use for the connection.</param>
		public ReliableConnection(string connectionString)
			: base(new TConnection())
		{
			InnerConnection.ConnectionString = connectionString;
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class with the given retry strategy.
		/// </summary>
		/// <param name="connectionString">The connection string to use for the connection.</param>
		/// <param name="retryStrategy">The retry strategy to use for the connection.</param>
		public ReliableConnection(string connectionString, IRetryStrategy retryStrategy)
			: base(new TConnection(), retryStrategy)
		{
			InnerConnection.ConnectionString = connectionString;
		}
	}
}
