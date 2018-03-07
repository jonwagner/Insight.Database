using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Reliable;

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
		private static Lazy<Dictionary<Type, InsightDbProvider>> _providerMap = new Lazy<Dictionary<Type, InsightDbProvider>>(LoadProviders);

		/// <summary>
		/// Regex to detect parameters in sql text.
		/// </summary>
		private static Regex _parameterRegex = new Regex("[?@:]([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		/// <summary>
		/// The default provider to use if we don't understand a given type.
		/// </summary>
		private static InsightDbProvider _defaultProvider = new InsightDbProvider();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public virtual IEnumerable<Type> SupportedTypes
		{
			get
			{
				return null;
			}
		}

        /// <summary>
        /// Gets a value indicating whether SQL Text queries use positional parameters.
        /// </summary>
        protected virtual bool HasPositionalSqlTextParameters { get { return false; } }
        #endregion

		#region Static Members
		/// <summary>
		/// Manually registers a provider.
		/// </summary>
		/// <param name="provider">The provider to register.</param>
		public static void RegisterProvider(InsightDbProvider provider)
		{
			RegisterProvider(_providerMap.Value, provider);
		}

		/// <summary>
		/// Gets the provider that supports the given object.
		/// </summary>
		/// <param name="databaseObject">The object to inspect.</param>
		/// <returns>The provider for the object.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object")]
		public static InsightDbProvider For(object databaseObject)
		{
			if (databaseObject == null) throw new ArgumentNullException("databaseObject");

			InsightDbProvider provider;

			// walk the base classes to see if we know anything about what class it is derived from
			for (Type type = databaseObject.GetType(); type != null; type = type.GetTypeInfo().BaseType)
			{
				if (_providerMap.Value.TryGetValue(type, out provider) && provider != null)
					return provider;
			}

			return _defaultProvider;
		}
		#endregion

		#region Overrideables
		/// <summary>
		/// Gets the set of bulk copy options supported by this provider.
		/// </summary>
		/// <param name="connection">The connection to inspect for bulk copy options.</param>
		/// <returns>The supported bulk copy options.</returns>
		public virtual InsightBulkCopyOptions GetSupportedBulkCopyOptions(IDbConnection connection)
		{
			return 0;
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public virtual DbConnection CreateDbConnection()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Clones a new DbConnection supported by this provider.
		/// </summary>
		/// <param name="connection">The connection to clone.</param>
		/// <returns>A new DbConnection.</returns>
		public virtual IDbConnection CloneDbConnection(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			var newConnection = CreateDbConnection();
			newConnection.ConnectionString = connection.ConnectionString;
			return newConnection;
		}

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public virtual IList<IDataParameter> DeriveParameters(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			// call the server to get the parameters
			command.Connection.ExecuteAndAutoClose(
				_ =>
				{
					if (command.CommandType == System.Data.CommandType.Text)
						DeriveParametersFromSqlText(command);
					else if (command.CommandType == System.Data.CommandType.StoredProcedure)
						DeriveParametersFromStoredProcedure(command);
					else
						throw new InvalidOperationException("Cannot derive parameters from this command. Have you loaded the provider for your database?");

					return null;
				},
				(_, __) => false,
				CommandBehavior.Default);

			// remove the parameters from the command so they are unbound from it
			var parameters = command.Parameters.Cast<IDataParameter>().ToList();
			command.Parameters.Clear();
			return parameters;
		}

		/// <summary>
		/// Derives the parameter list from a stored procedure command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public virtual void DeriveParametersFromStoredProcedure(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");
			throw CreateNotRegisteredException(command, String.Format(CultureInfo.InvariantCulture, "Cannot derive parameters for the stored procedure {0}", command.CommandText));
		}

		/// <summary>
		/// Derives the parameter list from a sql text command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public virtual void DeriveParametersFromSqlText(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

            var parameters = _parameterRegex.Matches(command.CommandText)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value);

            if (!HasPositionalSqlTextParameters)
                parameters = parameters.Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var p in parameters.Select(
                p =>
                {
                    var dbParameter = (IDataParameter)command.CreateParameter();
                    dbParameter.ParameterName = p;
                    return dbParameter;
                }))
                command.Parameters.Add(p);
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

#if !NO_DBPARAMETER
			// NOTE: some builds of .NET Core have a bug where IDbDataParameter is implemented incorrectly.
			// Use DbParameter first to get more direct access to the properties.
			DbParameter dbp = p as DbParameter;
			if (dbp != null)
			{
				DbParameter dbParameter = parameter as DbParameter;
				dbp.Scale = dbParameter.Scale;
				dbp.Precision = dbParameter.Precision;

				if (dbParameter.Direction != ParameterDirection.Input && TypeHelper.IsDbTypeAString(dbParameter.DbType))
					dbp.Size = -1;
				else
					dbp.Size = dbParameter.Size;
			}
			else
#endif
			{
				IDbDataParameter dp = p as IDbDataParameter;
				if (dp != null)
				{
					IDbDataParameter dbParameter = parameter as IDbDataParameter;
					dp.Scale = dbParameter.Scale;
					dp.Precision = dbParameter.Precision;

					if (dbParameter.Direction != ParameterDirection.Input && TypeHelper.IsDbTypeAString(dbParameter.DbType))
						dp.Size = -1;
					else
						dp.Size = dbParameter.Size;
				}
			}

			return p;
		}

		/// <summary>
		/// Called after parameters are added to a command.
		/// </summary>
		/// <param name="command">The command to fix up.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public virtual void FixupCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

            if (HasPositionalSqlTextParameters)
                command.CommandText = _parameterRegex.Replace(command.CommandText, "?");
        }

		/// <summary>
		/// Called before reading output parameters from the command.
		/// </summary>
		/// <param name="command">The command to fix up.</param>
		public virtual void FixupOutputParameters(IDbCommand command)
		{
		}

		/// <summary>
		/// When building the parameter template, this allows the provider to fix properties.
		/// </summary>
		/// <param name="command">The command being prepared.</param>
		/// <param name="parameter">The parameter to fix up.</param>
		/// <param name="dbType">The best guess at the DbType of the parameter.</param>
		/// <param name="type">The type of the object that will be passed in the parameter.</param>
		/// <param name="serializationMode">The serialization mode used to encode the parameter.</param>
		public virtual void FixupParameter(IDbCommand command, IDataParameter parameter, DbType dbType, Type type, SerializationMode serializationMode)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (parameter == null) throw new ArgumentNullException("parameter");

			// if we are executing a text procedure, fill in the dbtype with our best guess at the type
			if (command.CommandType != CommandType.StoredProcedure && dbType != DbParameterGenerator.DbTypeEnumerable)
				parameter.DbType = dbType;
		}

		/// <summary>
		/// Determines if a parameter is an XML type parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is an XML parameter.</returns>
		public virtual bool IsXmlParameter(IDbCommand command, IDataParameter parameter)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");

			return parameter.DbType == DbType.Xml;
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
		/// Set up a table-valued parameter to a procedure.
		/// </summary>
		/// <param name="command">The command to operate on.</param>
		/// <param name="parameter">The parameter to set up.</param>
		/// <param name="list">The list of records.</param>
		/// <param name="listType">The type of object in the list.</param>
		public virtual void SetupTableValuedParameter(IDbCommand command, IDataParameter parameter, IEnumerable list, Type listType)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");
			throw CreateNotRegisteredException(command, String.Format(CultureInfo.InvariantCulture, "Cannot set up the table valued parameter for parameter {0}.", parameter.ParameterName));
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public virtual string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			throw CreateNotRegisteredException(connection, String.Format(CultureInfo.InvariantCulture, "Cannot get the schema for table {0}.", tableName));
		}

		/// <summary>
		/// Determines if the given column in the schema table is an XML column.
		/// </summary>
		/// <param name="reader">The data reader to analyze.</param>
		/// <param name="index">The index of the column.</param>
		/// <returns>True if the column is an XML column.</returns>
		public virtual bool IsXmlColumn(IDataReader reader, int index)
		{
			return false;
		}

		/// <summary>
		/// Bulk copies a set of objects to the server.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="reader">The reader to read objects from.</param>
		/// <param name="configure">A callback method to configure the bulk copy object.</param>
		/// <param name="options">Options for initializing the bulk copy object.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		public virtual void BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			throw CreateNotRegisteredException(connection, String.Format(CultureInfo.InvariantCulture, "Cannot bulk copy into table {0}", tableName));
		}

		/// <summary>
		/// Bulk copies a set of objects to the server.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="reader">The reader to read objects from.</param>
		/// <param name="configure">A callback method to configure the bulk copy object.</param>
		/// <param name="options">Options for initializing the bulk copy object.</param>
		/// <param name="transaction">An optional transaction to participate in.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task representing the completion of the bulk copy.</returns>
		public virtual Task BulkCopyAsync(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction, CancellationToken cancellationToken)
		{
			// default - punt to the sync version
			return Task.Factory.StartNew(() => BulkCopy(connection, tableName, reader, configure, options, transaction));
		}

		/// <summary>
		/// Determines if a database exception is a transient exception and if the operation could be retried.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception is transient.</returns>
		public virtual bool IsTransientException(Exception exception)
		{
			return false;
		}
#endregion

		/// <summary>
		/// Copies the list of parameters into the given command.
		/// </summary>
		/// <param name="command">The command to copy into.</param>
		/// <param name="parameters">The parameter template to copy from.</param>
		/// <returns>The list of parameters.</returns>
		internal IDataParameter[] CopyParameters(IDbCommand command, IList<IDataParameter> parameters)
		{
			var array = new IDataParameter[parameters.Count];

			for (int i = 0; i < array.Length; i++)
			{
				var template = parameters[i];
				if (template == null)
					continue;

				var p = CloneParameter(command, template);
				command.Parameters.Add(p);
				array[i] = p;
			}

			return array;
		}

		/// <summary>
		/// Creates a NotImplementedException when we can't handle particular type of database object.
		/// </summary>
		/// <param name="databaseObject">The object we're working with.</param>
		/// <param name="message">The error message.</param>
		/// <returns>An exception that can be thrown.</returns>
		private static Exception CreateNotRegisteredException(object databaseObject, string message)
		{
			return new NotImplementedException(String.Format(CultureInfo.InvariantCulture, "{0}. Have you loaded the provider that supports {1}?", message, databaseObject.GetType().Name));
		}

#region Registration
		/// <summary>
		/// Initializes static members of the InsightDbProvider class.
		/// </summary>
		/// <returns>The set of providers.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		private static Dictionary<Type, InsightDbProvider> LoadProviders()
		{
			var providerMap = new Dictionary<Type, InsightDbProvider>();

			// load the internal providers
			RegisterProvider(providerMap, new ReliableConnectionInsightDbProvider());
			RegisterProvider(providerMap, new DbConnectionWrapperInsightDbProvider());

			// look for any provider assemblies in the search path and load them automatically
			var paths = new List<string>();

            foreach (string assemblyFile in ApplicationHelpers.GetAssemblySearchPaths().Distinct()
                .SelectMany(path => Directory.GetFiles(path, "Insight.Database.Providers.*.dll").Distinct()))
			{
                var assembly = ApplicationHelpers.LoadAssembly(assemblyFile);
				foreach (var type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(InsightDbProvider))))
					RegisterProvider(providerMap, (InsightDbProvider)System.Activator.CreateInstance(type));
			}

			return providerMap;
		}

		/// <summary>
		/// Registers a provider.
		/// </summary>
		/// <param name="providerMap">The provider map.</param>
		/// <param name="provider">The provider to register.</param>
		private static void RegisterProvider(Dictionary<Type, InsightDbProvider> providerMap, InsightDbProvider provider)
		{
			var supportedTypes = provider.SupportedTypes;
			if (supportedTypes == null)
				return;

			lock (providerMap)
			{
				foreach (var type in supportedTypes)
					providerMap[type] = provider;
			}
		}
#endregion
	}
}
