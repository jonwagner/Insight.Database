using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods specifically for SQL Server
    /// </summary>
    public static class SqlExtensions
    {
        /// <summary>
        /// Executes a FOR XML command and returns the results as a single XmlDocument.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The XmlDocument.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public static XmlDocument ExecuteXml(this SqlCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            using (var reader = command.ExecuteXmlReader())
            {
                var doc = new XmlDocument();
                doc.Load(reader);
                return doc;
            }
        }

        /// <summary>
        /// Executes a FOR XML query and returns the result as a single XmlDocument.
        /// </summary>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>An XmlDocument with the results.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public static XmlDocument QueryXml(
            this SqlConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.ExecuteAndAutoClose(
                c =>
                {
                    using (var cmd = (SqlCommand)c.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
                    {
                        cmd.OutputParameters(outputParameters);
                        return cmd.ExecuteXml();
                    }
                },
                commandBehavior.HasFlag(CommandBehavior.CloseConnection));
        }

        /// <summary>
        /// Executes a FOR XML query and returns the result as a single XmlDocument.
        /// </summary>
        /// <param name="connection">The connection to execute on.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandBehavior">The behavior of the command.</param>
        /// <param name="commandTimeout">An optional timeout for the command.</param>
        /// <param name="transaction">An optional transaction to participate in.</param>
        /// <param name="outputParameters">An optional object to send the output parameters to. This may be the same as parameters.</param>
        /// <returns>An XmlDocument with the results.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public static XmlDocument QueryXmlSql(
            this SqlConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            object outputParameters = null)
        {
            return connection.QueryXml(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction, outputParameters);
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction with the specified transaction name
        /// that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <param name="transactionName">Name for the transaction.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this SqlConnection connection, string transactionName)
        {
            var wrapper = new DbConnectionWrapper(connection);
            wrapper.Open();
            var transaction = connection.BeginTransaction(transactionName);
            wrapper.UsingTransaction(transaction);
            return wrapper;
        }

        /// <summary>
        /// Opens a database connection and begins a new transaction with the specified transaction name
        /// and isolationLevel that is disposed when the returned object is disposed.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <param name="transactionName">Name for the transaction.</param>
        /// <returns>A wrapper for the database connection.</returns>
        public static DbConnectionWrapper OpenWithTransaction(this SqlConnection connection, IsolationLevel isolationLevel, string transactionName)
        {
            var wrapper = new DbConnectionWrapper(connection);
            wrapper.Open();
            var transaction = connection.BeginTransaction(isolationLevel, transactionName);
            wrapper.UsingTransaction(transaction);
            return wrapper;
        }
    }
}
