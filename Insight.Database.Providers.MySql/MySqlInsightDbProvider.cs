using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Insight.Database.Providers.MySql
{
    /// <summary>
    /// Implements the Insight provider for MySql connections.
    /// </summary>
    public class MySqlInsightDbProvider : InsightDbProvider
    {
        /// <summary>
        /// The prefix used on parameter names.
        /// </summary>
        private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// The list of types supported by this provider.
        /// </summary>
        private static Type[] _supportedTypes = new Type[]
        {
            typeof(MySqlConnectionStringBuilder), typeof(MySqlConnection), typeof(MySqlCommand), typeof(MySqlParameter), typeof(MySqlDataReader), typeof(MySqlException)
        };

        /// <summary>
        /// Gets the types of objects that this provider supports.
        /// Include connectionstrings, connections, commands, and readers.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _supportedTypes;
            }
        }

        /// <summary>
        /// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
        /// </summary>
        public static void RegisterProvider()
        {
            InsightDbProvider.RegisterProvider(new MySqlInsightDbProvider());
        }

        /// <summary>
        /// Creates a new DbConnection supported by this provider.
        /// </summary>
        /// <returns>A new DbConnection.</returns>
        public override DbConnection CreateDbConnection()
        {
            return new MySqlConnection();
        }

        /// <summary>
        /// Derives the parameter list from a stored procedure command.
        /// </summary>
        /// <param name="command">The command to derive.</param>
        public override void DeriveParametersFromStoredProcedure(IDbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            global::MySql.Data.MySqlClient.MySqlCommandBuilder.DeriveParameters(command as MySqlCommand);

            // remove the @ from any parameters
            foreach (var p in command.Parameters.OfType<MySqlParameter>())
                p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty);
        }

        /// <summary>
        /// Clones a parameter so that it can be used with another command.
        /// </summary>
        /// <param name="command">The command to use.</param>
        /// <param name="parameter">The parameter to clone.</param>
        /// <returns>The clone.</returns>
        public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
        {
            MySqlParameter p = (MySqlParameter)parameter;

            MySqlParameter clone = new MySqlParameter(
                p.ParameterName,
                p.MySqlDbType,
                p.Size,
                p.SourceColumn);

            clone.Direction = p.Direction;
            clone.IsNullable = p.IsNullable;
            clone.Precision = p.Precision;
            clone.Scale = p.Scale;
            clone.Value = p.Value;

            return clone;
        }

        /// <summary>
        /// Returns SQL that queries a table for the schema only, no rows.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="tableName">The name of the table to query.</param>
        /// <returns>SQL that queries a table for the schema only, no rows.</returns>
        public override string GetTableSchemaSql(IDbConnection connection, string tableName)
        {
            return String.Format(CultureInfo.InvariantCulture, "SELECT TOP 0 * FROM {0}", tableName);
        }

        /// <summary>
        /// Determines if a database exception is a transient exception and if the operation could be retried.
        /// </summary>
        /// <param name="exception">The exception to test.</param>
        /// <returns>True if the exception is transient.</returns>
        public override bool IsTransientException(Exception exception)
        {
            MySqlException mex = (MySqlException)exception;
            switch (mex.Number)
            {
                case 1042:      // ER_BAD_HOST_ERROR
                case 2002:      // CR_CONNECTION_ERROR
                case 2003:      // CR_CONN_HOST_ERROR
                case 2006:      // CR_SERVER_GONE_ERROR
                case 2009:      // CR_WRONG_HOST_INFO
                case 2013:      // CR_SERVER_LOST
                    return true;
            }

            return false;
        }
    }
}
