using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for Odbc connections.
	/// </summary>
	public class OdbcInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(OdbcConnectionStringBuilder), typeof(OdbcConnection), typeof(OdbcCommand), typeof(OdbcDataReader), typeof(OdbcException)
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
			return new OdbcConnection();
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OdbcCommandBuilder.DeriveParameters(command as OdbcCommand);
		}
	
		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			OdbcParameter p = (OdbcParameter)base.CloneParameter(command, parameter);

			OdbcParameter template = (OdbcParameter)parameter;
			p.OdbcType = template.OdbcType;

			return p;
		}
	}
}
