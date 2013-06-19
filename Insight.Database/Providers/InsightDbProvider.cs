using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Provides the infrastructure to access advanced features of database connections so Insight can do its magic.
	/// </summary>
	public class InsightDbProvider
	{
		#region Fields
		/// <summary>
		/// The map from object types to providers. This includes DbCommand and DbConnectionString types.
		/// </summary>
		private static Dictionary<Type, InsightDbProvider> _providerMap = new Dictionary<Type, InsightDbProvider>();

		/// <summary>
		/// Regex to detect parameters in sql text.
		/// </summary>
		private static Regex _parameterRegex = new Regex("[?@]([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes static members of the InsightDbProvider class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static InsightDbProvider()
		{
			// only automatically initialize providers that are built into the framework
			new SqlInsightDbProvider().Register();
			new ReliableInsightDbProvider().Register();
			new OdbcInsightDbProvider().Register();
			new OleDbInsightDbProvider().Register();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public virtual Type CommandType
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the type for ConnectionStringBuilders supported by this provider.
		/// </summary>
		public virtual Type ConnectionStringBuilderType
		{
			get
			{
				return null;
			}
		}
		#endregion

		/// <summary>
		/// Registers this provider.
		/// </summary>
		public void Register()
		{
			lock (_providerMap)
			{
				if (CommandType != null)
					_providerMap[CommandType] = this;

				if (ConnectionStringBuilderType != null)
					_providerMap[ConnectionStringBuilderType] = this;
			}
		}

		#region Overrideables
		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public virtual DbConnection CreateDbConnection()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public virtual IList<IDataParameter> DeriveParameters(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			// we only support pure text
			if (command.CommandType != System.Data.CommandType.Text)
				throw new NotImplementedException();

			return _parameterRegex.Matches(command.CommandText)
				.Cast<Match>()
				.Select(m => m.Groups[1].Value.ToUpperInvariant())
				.Distinct()
				.Select(p =>
				{
					var dbParameter = (IDataParameter)command.CreateParameter();
					dbParameter.ParameterName = p;
					return dbParameter;
				})
				.ToList();
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public virtual IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (parameter == null) throw new ArgumentNullException("parameter");

			IDataParameter p = command.CreateParameter();
			p.ParameterName = parameter.ParameterName;
			p.DbType = parameter.DbType;
			p.Direction = parameter.Direction;

			IDbDataParameter dp = p as IDbDataParameter;
			if (dp != null)
			{
				IDbDataParameter dbParameter = parameter as IDbDataParameter;
				dp.Size = dbParameter.Size;
				dp.Scale = dbParameter.Scale;
				dp.Precision = dbParameter.Precision;
			}

			return p;
		}

		/// <summary>
		/// Determines if a parameter is a Table-valued parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is a table-valued parameter.</returns>
		public virtual bool IsTableValuedParameter(IDbCommand command, IDataParameter parameter)
		{
			return false;
		}

		/// <summary>
		/// Calculates the table type name for a table parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <param name="listType">The type of object being stored in the table.</param>
		/// <returns>The name of the table parameter.</returns>
		public virtual string GetTableParameterTypeName(IDbCommand command, IDataParameter parameter, Type listType)
		{
			return null;
		}

		/// <summary>
		/// Gets the schema for a given user-defined table type.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to use.</param>
		/// <returns>An open reader with the schema.</returns>
		/// <remarks>The caller is responsible for closing the reader and the connection.</remarks>
		public virtual IDataReader GetTableTypeSchema(IDbCommand command, IDataParameter parameter)
		{
			throw new NotImplementedException();
		}
		#endregion

		/// <summary>
		/// Gets the provider that supports the given object.
		/// </summary>
		/// <param name="o">The object to inspect.</param>
		/// <returns>The provider for the object.</returns>
		internal static InsightDbProvider For(object o)
		{
			InsightDbProvider provider;
			
			if (!_providerMap.TryGetValue(o.GetType(), out provider) || provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of command.");

			return provider;
		}
	}
}
