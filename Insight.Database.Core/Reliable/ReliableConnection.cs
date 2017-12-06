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
    /// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbConnection is generated code")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbConnection would be redundant without adding additional information.")]
    [System.ComponentModel.DesignerCategory("Code")]
    public class ReliableConnection : DbConnectionWrapper
    {
        #region Private Members
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
        public ReliableConnection(DbConnection innerConnection) : base(innerConnection)
        {
            // use the default retry strategy by default
            RetryStrategy = Insight.Database.Reliable.RetryStrategy.Default;
        }

        /// <summary>
        /// Initializes a new instance of the ReliableConnection class.
        /// </summary>
        /// <param name="innerConnection">The inner connection to wrap.</param>
        /// <param name="retryStrategy">The retry strategy to use.</param>
        public ReliableConnection(DbConnection innerConnection, IRetryStrategy retryStrategy) : base(innerConnection)
        {
            RetryStrategy = retryStrategy;
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
            var command = new ReliableCommand(RetryStrategy, this, InnerConnection.CreateCommand());
            try
            {
                if (InnerTransaction != null)
                    command.Transaction = InnerTransaction;
                return command;
            }
            catch
            {
                command.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously opens a ReliableConnection.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>A task representing the completion of the open operation.</returns>
        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return RetryStrategy.ExecuteWithRetryAsync(null, async () => { await InnerConnection.OpenAsync(); return true; });
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
        public ReliableConnection()
            : base(new TConnection())
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
