using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Insight.Database.Reliable
{
	/// <summary>
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbConnection is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbConnection would be redundant without adding additional information.")]
	public class ReliableConnection : IDbConnection
	{
		#region Private Members
		/// <summary>
		/// The inner connection to use to execute the database commands.
		/// </summary>
		private IDbConnection _innerConnection;

		/// <summary>
		/// Gets the retry strategy to use to handle exceptions.
		/// </summary>
		public IRetryStrategy RetryStrategy { get; private set; }
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
			_innerConnection = innerConnection;
		}

		/// <summary>
		/// Initializes a new instance of the ReliableConnection class.
		/// </summary>
		/// <param name="innerConnection">The inner connection to wrap.</param>
		/// <param name="retryStrategy">The retry strategy to use.</param>
		public ReliableConnection(IDbConnection innerConnection, IRetryStrategy retryStrategy)
		{
			RetryStrategy = retryStrategy;
			_innerConnection = innerConnection;
		}
		#endregion

		#region IDbConnection Implementation
		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return _innerConnection.BeginTransaction(il);
		}

		public IDbTransaction BeginTransaction()
		{
			return _innerConnection.BeginTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			_innerConnection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			_innerConnection.Close();
		}

		public string ConnectionString
		{
			get
			{
				return _innerConnection.ConnectionString;
			}

			set
			{
				_innerConnection.ConnectionString = value;
			}
		}

		public int ConnectionTimeout
		{
			get { return _innerConnection.ConnectionTimeout; }
		}

		public IDbCommand CreateCommand()
		{
			return new ReliableCommand(RetryStrategy, _innerConnection.CreateCommand());
		}

		public string Database
		{
			get { return _innerConnection.Database; }
		}

		public void Open()
		{
			RetryStrategy.ExecuteWithRetry(null, () => { _innerConnection.Open(); return true; });
		}

		public ConnectionState State
		{
			get { return _innerConnection.State; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			_innerConnection.Dispose();
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
