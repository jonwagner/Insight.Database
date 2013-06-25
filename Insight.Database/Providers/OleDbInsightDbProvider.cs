using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for OleDb connections.
	/// </summary>
	class OleDbInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(OleDbCommand);
			}
		}

		/// <summary>
		/// Gets the type for ConnectionStringBuilders supported by this provider.
		/// </summary>
		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(OleDbConnectionStringBuilder);
			}
		}

		/// <summary>
		/// Gets the type for Connections supported by this provider.
		/// </summary>
		public override Type ConnectionType
		{
			get
			{
				return typeof(OleDbConnection);
			}
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new OleDbConnection();
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			OleDbParameter p = (OleDbParameter)base.CloneParameter(command, parameter);

			OleDbParameter template = (OleDbParameter)parameter;
			p.OleDbType = template.OleDbType;

			return p;
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		protected override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OleDbCommandBuilder.DeriveParameters(command as OleDbCommand);
		}
	}
}
