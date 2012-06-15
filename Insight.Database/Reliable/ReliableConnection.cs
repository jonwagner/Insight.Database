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
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbConnection is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbConnection would be redundant without adding additional information.")]
	public class ReliableConnection : IDbConnection
	{
		#region Private Members
		/// <summary>
		/// Gets the retry strategy to use to handle exceptions.
		/// </summary>
		public IRetryStrategy RetryStrategy { get; private set; }

		/// <summary>
		/// Gets the inner connection to use to execute the database commands.
		/// </summary>
		internal IDbConnection InnerConnection { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ReliableConnection class.
		/// A default retry strategy is used.
		/// </summary>
		/// <param name="innerConnection">The inner connection to wrap.</param>
		public ReliableConnection(IDbConnection innerConnection)
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
		public ReliableConnection(IDbConnection innerConnection, IRetryStrategy retryStrategy)
		{
			RetryStrategy = retryStrategy;
			InnerConnection = innerConnection;
		}
		#endregion

		/// <summary>
		/// Asynchronously opens a ReliableConnection.
		/// </summary>
		/// <returns>A task representing the completion of the open operation.</returns>
		public Task OpenAsync()
		{
			return RetryStrategy.ExecuteWithRetryAsync(
				null, 
				() => 
				{ 
#if !NODBASYNC
					// if we have an inner SqlConnection, we can open that asynchronously
					SqlConnection sqlConnection = InnerConnection.UnwrapSqlConnection();
					if (sqlConnection != null)
						return sqlConnection.OpenAsync().ContinueWith((t => { t.Wait(); return true; }));
#endif

					// fallback strategy. We don't have a database connection that can open asynchronously, so create a task that does a blocking open.	
					return Task<bool>.Factory.StartNew(() => { InnerConnection.Open(); return true; });
				});
		}

		#region IDbConnection Implementation
		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return InnerConnection.BeginTransaction(il);
		}

		public IDbTransaction BeginTransaction()
		{
			return InnerConnection.BeginTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			InnerConnection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			InnerConnection.Close();
		}

		public string ConnectionString
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

		public int ConnectionTimeout
		{
			get { return InnerConnection.ConnectionTimeout; }
		}

		public IDbCommand CreateCommand()
		{
			return new ReliableCommand(RetryStrategy, InnerConnection.CreateCommand());
		}

		public string Database
		{
			get { return InnerConnection.Database; }
		}

		public void Open()
		{
			RetryStrategy.ExecuteWithRetry(null, () => { InnerConnection.Open(); return true; });
		}

		public ConnectionState State
		{
			get { return InnerConnection.State; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			InnerConnection.Dispose();
		}
		#endregion
	}

	/// <summary>
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	/// <typeparam name="TConnection">The type of database connection to use to connect to the database.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class ReliableConnection<TConnection> : ReliableConnection where TConnection : IDbConnection, new()
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
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class with the given retry strategy.
		/// </summary>
		/// <param name="connectionString">The connection string to use for the connection.</param>
		/// <param name="retryStrategy">The retry strategy to use for the connection.</param>
		public ReliableConnection(string connectionString, IRetryStrategy retryStrategy)
			: base(new TConnection(), retryStrategy)
		{
			ConnectionString = connectionString;
		}
	}
}
