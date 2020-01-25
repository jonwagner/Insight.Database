using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Providers;

namespace Insight.Database
{
	/// <summary>
	/// Implements the Insight provider for OleDb connections.
	/// </summary>
	public class OleDbInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(OleDbConnectionStringBuilder), typeof(OleDbConnection), typeof(OleDbCommand), typeof(OleDbDataReader), typeof(OleDbException)
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

        /// <inheritdoc/>
        protected override bool HasPositionalSqlTextParameters { get { return true; } }

		/// <summary>
		/// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
		/// </summary>
		public static void RegisterProvider()
		{
			InsightDbProvider.RegisterProvider(new OleDbInsightDbProvider());
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
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public override void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			OleDbCommandBuilder.DeriveParameters(command as OleDbCommand);
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
	}
}
