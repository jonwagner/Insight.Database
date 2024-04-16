using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdoNetCore.AseClient;

namespace Insight.Database.Providers.SybaseAse
{
	/// <summary>
	/// An InsightDbProvider for Sybase ASE.
	/// </summary>
    public class SybaseAseInsightDbProvider : InsightDbProvider
    {
		/// <summary>
		/// The prefix used on parameter names.
		/// </summary>
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(AseConnectionStringBuilder), typeof(AseConnection), typeof(AseCommand), typeof(AseDataReader), typeof(AseException), typeof(AseParameter)
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
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new AseConnection();
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			AseCommandBuilder.DeriveParameters(command as AseCommand);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<AseParameter>())
				p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty);
		}

		/// <inheritdoc/>
		public override void FixupCommand(IDbCommand command)
		{
			base.FixupCommand(command);

			foreach (var p in command.Parameters.OfType<AseParameter>())
				p.ParameterName = "@" + p.ParameterName;
		}

		/// <summary>
		/// Called before reading output parameters from the command.
		/// </summary>
		/// <param name="command">The command to fix up.</param>
		public override void FixupOutputParameters(IDbCommand command)
		{
			base.FixupOutputParameters(command);

			// remove the @ from any parameters
			foreach (var p in command.Parameters.OfType<AseParameter>())
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
			// thank you, sybase
			AseParameter p = (AseParameter)parameter;
			return (IDataParameter)p.Clone();
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
			AseException aseException = (AseException)exception;

			return aseException.Errors.OfType<AseError>().Any(e => e.MessageNumber == 30012);
		}
	}
}
