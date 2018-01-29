using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Insight.Database.Providers.MySqlConnector
{
	/// <summary>
	/// Implements the Insight provider for MySQL connections using MySqlConnector.
	/// </summary>
	public class MySqlConnectorInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// The prefix used on parameter names.
		/// </summary>
		private static readonly Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static readonly Type[] _supportedTypes =
		{
			typeof(MySqlConnectionStringBuilder), typeof(MySqlConnection), typeof(MySqlCommand), typeof(MySqlParameter), typeof(MySqlDataReader), typeof(MySqlException)
		};

		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes => _supportedTypes;

		/// <summary>
		/// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
		/// </summary>
		public static void RegisterProvider() => InsightDbProvider.RegisterProvider(new MySqlConnectorInsightDbProvider());

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection() => new MySqlConnection();

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));

			MySqlCommandBuilder.DeriveParameters((MySqlCommand)command);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.Cast<MySqlParameter>())
				p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, "");
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

#if NET45
			byte precision = 0;
			byte scale = 0;
#else
			byte precision = p.Precision;
			byte scale = p.Scale;
#endif

			return new MySqlParameter(
				p.ParameterName,
				p.MySqlDbType,
				p.Size,
				p.Direction,
				p.IsNullable,
				precision,
				scale,
				p.SourceColumn,
				p.SourceVersion,
				p.Value
			);
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public override string GetTableSchemaSql(IDbConnection connection, string tableName) => String.Format(CultureInfo.InvariantCulture, "SELECT TOP 0 * FROM {0}", tableName);

		/// <summary>
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public override bool IsTransientException(Exception exception)
		{
			switch (((MySqlException)exception).Number)
			{
				case 1042:		// ER_BAD_HOST_ERROR
				case 2002:		// CR_CONNECTION_ERROR
				case 2003:		// CR_CONN_HOST_ERROR
				case 2006:		// CR_SERVER_GONE_ERROR
				case 2009:		// CR_WRONG_HOST_INFO
				case 2013:		// CR_SERVER_LOST
					return true;
			}

			return false;
		}
	}
}
