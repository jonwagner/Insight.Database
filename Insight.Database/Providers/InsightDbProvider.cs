﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

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
		private static Regex _parameterRegex = new Regex("[?@:]([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
			new DbConnectionWrapperInsightDbProvider().Register();
			new ReliableInsightDbProvider().Register();
			new OdbcInsightDbProvider().Register();
			new OleDbInsightDbProvider().Register();
		}
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
		/// Gets the set of bulk copy options supported by this provider.
		/// </summary>
		public virtual InsightBulkCopyOptions SupportedBulkCopyOptions { get { return 0; } }
		#endregion

		/// <summary>
		/// Registers this provider.
		/// </summary>
		public void Register()
		{
			var supportedTypes = SupportedTypes;
			if (supportedTypes == null)
				return;

			lock (_providerMap)
			{
				foreach (var type in supportedTypes)
					_providerMap[type] = this;
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

			// call the server to get the parameters
			command.Connection.ExecuteAndAutoClose(
				_ => 
				{
					if (command.CommandType == System.Data.CommandType.Text)
						DeriveParametersFromSqlText(command);
					else if (command.CommandType == System.Data.CommandType.StoredProcedure)
						DeriveParametersFromStoredProcedure(command);
					else
						throw new InvalidOperationException("Cannot derive parameters from this command");

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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Derives the parameter list from a sql text command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		public virtual void DeriveParametersFromSqlText(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			foreach (var p in _parameterRegex.Matches(command.CommandText)
				.Cast<Match>()
				.Select(m => m.Groups[1].Value)
				.Distinct()
				.Select(p =>
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

			return p;
		}

		/// <summary>
		/// When building the parameter template, this allows the provider to fix properties.
		/// </summary>
		/// <param name="command">The command being prepared.</param>
		/// <param name="parameter">The parameter to fix up.</param>
		/// <param name="dbType">The best guess at the DbType of the parameter.</param>
		/// <param name="type">The type of the object that will be passed in the parameter.</param>
		public virtual void FixupParameter(IDbCommand command, IDataParameter parameter, DbType dbType, Type type)
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns SQL that queries a table for the schema only, no rows.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table to query.</param>
		/// <returns>SQL that queries a table for the schema only, no rows.</returns>
		public virtual string GetTableSchemaSql(IDbConnection connection, string tableName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines if the given column in the schema table is an XML column.
		/// </summary>
		/// <param name="command">The command associated with the reader.</param>
		/// <param name="schemaTable">The schema table to analyze.</param>
		/// <param name="index">The index of the column.</param>
		/// <returns>True if the column is an XML column.</returns>
		public virtual bool IsXmlColumn(IDbCommand command, DataTable schemaTable, int index)
		{
			throw new NotImplementedException();
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
		/// <returns>Number of rows copied.</returns>
		public virtual int BulkCopy(IDbConnection connection, string tableName, IDataReader reader, Action<InsightBulkCopy> configure, InsightBulkCopyOptions options, IDbTransaction transaction)
		{
			throw new NotImplementedException();
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
		/// Gets the provider that supports the given object.
		/// </summary>
		/// <param name="o">The object to inspect.</param>
		/// <returns>The provider for the object.</returns>
		internal static InsightDbProvider For(object o)
		{
			InsightDbProvider provider;
			
			if (!_providerMap.TryGetValue(o.GetType(), out provider) || provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of object.");

			return provider;
		}

		/// <summary>
		/// Copies a single parameter by index and adds it to the command.
		/// </summary>
		/// <param name="command">The command to copy into.</param>
		/// <param name="parameters">The parameter template to copy from.</param>
		/// <param name="index">The index of the parameter to copy.</param>
		/// <returns>The new parameter.</returns>
		internal IDataParameter CopyParameter(IDbCommand command, IList<IDataParameter> parameters, int index)
		{
			var p = CloneParameter(command, parameters[index]);
			command.Parameters.Add(p);
			return p;
		}
	}
}
