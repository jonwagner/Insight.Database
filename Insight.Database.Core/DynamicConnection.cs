using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Structure;

namespace Insight.Database
{
    /// <summary>
    /// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Separating the IDBConnection interface improves readability.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Separating the IDBConnection interface improves readability.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
    public class DynamicConnection : DynamicObject, IDbConnection
    {
        #region Fields
        /// <summary>
        /// Caches for MethodInfo for query methods.
        /// </summary>
        private static ConcurrentDictionary<Type, Func<IDbCommand, IQueryReader, CommandBehavior, CancellationToken, object, object>> _queryAsyncMethods = new ConcurrentDictionary<Type, Func<IDbCommand, IQueryReader, CommandBehavior, CancellationToken, object, object>>();
        private static ConcurrentDictionary<Type, Func<IDbCommand, IQueryReader, CommandBehavior, object, object>> _queryMethods = new ConcurrentDictionary<Type, Func<IDbCommand, IQueryReader, CommandBehavior, object, object>>();

        /// <summary>
        /// The internal cache for parameters to stored procedures.
        /// </summary>
        private ConcurrentDictionary<string, List<IDataParameter>> _parameters = new ConcurrentDictionary<string, List<IDataParameter>>();

        /// <summary>
        /// The SQL schema to use when calling the procedure.
        /// </summary>
        private string _schema = null;

        /// <summary>
        /// The connection to use to connect to the database.
        /// </summary>
        private IDbConnection _connection;

        /// <summary>
        /// Gets the inner connection.
        /// </summary>
        protected IDbConnection InnerConnection { get { return _connection; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DynamicConnection class.
        /// </summary>
        /// <param name="connection">The connection to use as the inner database connection.</param>
        public DynamicConnection(IDbConnection connection) : this(connection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DynamicConnection class.
        /// </summary>
        /// <param name="connection">The connection to use as the inner database connection.</param>
        /// <param name="schema">The SQL schema to use when calling procedures.</param>
        protected DynamicConnection(IDbConnection connection, string schema)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _connection = connection;
            _schema = schema;
        }
        #endregion

        #region DynamicObject Members
        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the DynamicObject class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = DoInvokeMember(binder, args, null);

            return true;
        }

        /// <summary>
        /// Implements the procedure invocation process.
        /// </summary>
        /// <param name="binder">The binder used on the method.</param>
        /// <param name="args">The arguments to the method.</param>
        /// <param name="returnType">The default type of object to return if no type parameter is specified.</param>
        /// <returns>The results of the stored procedure call.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
        protected internal object DoInvokeMember(InvokeMemberBinder binder, object[] args, Type returnType)
        {
            if (binder == null) throw new ArgumentNullException("binder");
            if (args == null) throw new ArgumentNullException("args");

            bool doAsync = false;
            IDbCommand cmd = null;
            int specialParameters = 0;
            int? timeout = null;
            IDbTransaction transaction = null;
            IQueryReader returns = null;
            object outputParameters = null;
            CancellationToken cancellationToken = default(CancellationToken);

			CallInfo callInfo = binder.CallInfo;
            int unnamedParameterCount = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;

            // check the proc name - if it ends with Async, then call it asynchronously and return the results
            string procName = binder.Name;
            if (procName.EndsWith("async", StringComparison.OrdinalIgnoreCase))
            {
                procName = procName.Substring(0, procName.Length - 5);
                doAsync = true;
            }

            // if there is a schema, use it
            if (!String.IsNullOrWhiteSpace(_schema))
                procName = _schema + "." + procName;

            // go through the arguments and look for our special arguments
            // NOTE: this is intentionally case-sensitive so that you can use other cases if you need to pass a parameter by the same name.
            var argumentNames = callInfo.ArgumentNames;
            for (int i = 0; i < argumentNames.Count; i++)
            {
                switch (argumentNames[i])
                {
                    case "cancellationToken":
                        cancellationToken = (CancellationToken)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "transaction":
                        transaction = (IDbTransaction)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "commandTimeout":
                        timeout = (int)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "returnType":
                        returnType = (Type)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "returns":
                        returns = (IQueryReader)args[i + unnamedParameterCount];
                        specialParameters++;
                        break;

                    case "outputParameters":
                        outputParameters = args[i + unnamedParameterCount];
                        specialParameters++;
                        break;
                }
            }

            try
            {
                // if there is exactly one unnamed parameter, and the named parameters are all special parameters, and it's a reference type (and not a string)
                // then we will attempt to use the object's fields as the parameter values
                // this is so you can send an entire object to an insert method
                if (unnamedParameterCount == 1 &&
                    (callInfo.ArgumentNames.Count == specialParameters) &&
                    !args[0].GetType().GetTypeInfo().IsValueType && args[0].GetType() != typeof(String))
                {
                    cmd = _connection.CreateCommand(procName, args[0], CommandType.StoredProcedure, timeout, transaction);
                }
                else
                {
                    // this isn't a single-object parameter, so we are going to map the parameters by position and by name

                    // create a command
                    cmd = _connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;

                    if (timeout.HasValue)
                        cmd.CommandTimeout = timeout.Value;

                    // unwrap the transaction because the transaction has to match the command and connection
                    if (transaction != null)
                        cmd.Transaction = DBConnectionExtensions.UnwrapDbTransaction(transaction);

                    // fill in the parameters for the command object
                    // we will do the values next
                    DeriveParameters(cmd);

                    // look at the unnamed parameters first. we will add them by position.
                    var inputParameters = cmd.Parameters.OfType<IDataParameter>().Where(p => p.Direction.HasFlag(ParameterDirection.Input)).ToList();
                    for (int i = 0; i < unnamedParameterCount; i++)
                        inputParameters[i].Value = args[i];

                    // go through all of the named arguments next. Note that they may overwrite indexed parameters.
                    for (int i = unnamedParameterCount; i < callInfo.ArgumentNames.Count; i++)
                    {
                        string argumentName = callInfo.ArgumentNames[i];

                        // ignore our special parameters
                        if (argumentName == "cancellationToken" ||
                            argumentName == "transaction" ||
                            argumentName == "commandTimeout" ||
                            argumentName == "returnType" ||
                            argumentName == "returns" ||
                            argumentName == "outputParameters")
                            continue;

                        IDataParameter p = cmd.Parameters.OfType<IDataParameter>().First(parameter => String.Equals(parameter.ParameterName, argumentName, StringComparison.OrdinalIgnoreCase));
                        p.Value = args[i];
                    }

					// special handling for table parameters - replace them with list parameters
					// note that we may be modifying the parameters collection, so we copy the list here
					var provider = InsightDbProvider.For(cmd);
                    foreach (var p in cmd.Parameters.OfType<IDataParameter>().Where(p => provider.IsTableValuedParameter(cmd, p)).ToList())
                    {
                        // if any parameters are missing table parameters, then alert the developer
                        if (p.Value == null)
                            throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Table parameter {0} must be specified", p.ParameterName));

                        // convert the value to an objectreader
                        DbParameterGenerator.ListParameterHelper.ConvertListParameter(p, p.Value, cmd);
                    }
                }

                // if we don't have a type, use FastExpando
                if (returnType == null)
                    returnType = typeof(FastExpando);

                // if there was no named returns definition, check for an unnamed IQueryParameter
                if (returns == null)
                    returns = args.OfType<IQueryReader>().FirstOrDefault();

                // if there is no returns definition supplied, get one from the return type
                if (returns == null)
                {
                    if (returnType.IsSubclassOf(typeof(Results)))
                        returns = (IQueryReader)returnType.GetMethod("GetReader", BindingFlags.Public | BindingFlags.Static).Invoke(null, Parameters.EmptyArray);
                    else
                        returns = (IQueryReader)typeof(ListReader<>).MakeGenericType(returnType).GetField("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }

				InsightDbProvider.For(cmd).FixupCommand(cmd);

				// get the proper query method to call based on whether we are doing this async and whether there is a single or multiple result set
				// the nice thing is that the generic expansion will automatically create the proper return type like IList<T> or Results<T>.
				if (doAsync)
                    return CallQueryAsync(cmd, returns, cancellationToken);
                else
                    return CallQuery(cmd, returns, outputParameters);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            result = new DynamicConnection(_connection, binder.Name);
            return true;
        }

        #region Delegate Invocation Methods
        /// <summary>
        /// Invokes a cached delegate of Query.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="queryReader">The reader to use to read the records.</param>
        /// <param name="outputParameters">An object to output parameters onto.</param>
        /// <returns>The result of the invocation.</returns>
        private static object CallQuery(IDbCommand command, IQueryReader queryReader, object outputParameters)
        {
            var method = _queryMethods.GetOrAdd(
                queryReader.ReturnType,
                t =>
                {
                    var delegateType = typeof(Func<IDbCommand, IQueryReader, CommandBehavior, object, object>);
                    var implementationMethod = typeof(DBCommandExtensions).GetMethod("QueryCoreUntyped", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(t);
                    return (Func<IDbCommand, IQueryReader, CommandBehavior, object, object>)implementationMethod.CreateDelegate(delegateType);
                });
            return method(command, queryReader, CommandBehavior.Default, outputParameters);
        }

        /// <summary>
        /// Invokes a cached delegate of QueryAsync.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="queryReader">The reader to use to read the records.</param>
        /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
        /// <returns>The result of the invocation.</returns>
        private static object CallQueryAsync(IDbCommand command, IQueryReader queryReader, CancellationToken cancellationToken)
        {
            var method = _queryAsyncMethods.GetOrAdd(
                queryReader.ReturnType,
                t =>
                {
                    var delegateType = typeof(Func<IDbCommand, IQueryReader, CommandBehavior, CancellationToken, object, object>);
                    var implementationMethod = typeof(DBConnectionExtensions).GetMethod("QueryCoreAsyncUntyped", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(t);
                    return (Func<IDbCommand, IQueryReader, CommandBehavior, CancellationToken, object, object>)implementationMethod.CreateDelegate(delegateType);
                });
            return method(command, queryReader, CommandBehavior.Default, cancellationToken, null);
        }
        #endregion

        /// <summary>
        /// Derive the parameters that are needed to execute a given command.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        private void DeriveParameters(IDbCommand cmd)
        {
            var provider = InsightDbProvider.For(cmd);

            // look in the concurrent dictionary to find the parameters.
            // if not found, call the Server to get them.
            var parameterList = _parameters.GetOrAdd(
                cmd.CommandText,
                name => provider.DeriveParameters(cmd).ToList());

            // copy the parameter list
            foreach (IDataParameter parameter in parameterList)
                cmd.Parameters.Add(provider.CloneParameter(cmd, parameter));
        }
        #endregion

        #region IDbConnection Members
        /// <inheritdoc/>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _connection.BeginTransaction(il);
        }

        /// <inheritdoc/>
        public IDbTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        /// <inheritdoc/>
        public void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        /// <inheritdoc/>
        public void Close()
        {
            _connection.Close();
        }

        /// <inheritdoc/>
        public string ConnectionString
        {
            get
            {
                return _connection.ConnectionString;
            }

            set
            {
                _connection.ConnectionString = value;
            }
        }

        /// <inheritdoc/>
        public int ConnectionTimeout
        {
            get { return _connection.ConnectionTimeout; }
        }

        /// <inheritdoc/>
        public IDbCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        /// <inheritdoc/>
        public string Database
        {
            get { return _connection.Database; }
        }

        /// <inheritdoc/>
        public void Open()
        {
            _connection.Open();
        }

        /// <inheritdoc/>
        public ConnectionState State
        {
            get { return _connection.State; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
        #endregion
    }

    #region DynamicConnection Generic Class
    /// <summary>
    /// A DbConnection that supports invoking stored procedures using a nice .ProcName syntax.
    /// You should usually use the non-generic version of DynamicConnection so you can return different types of objects.
    /// e.g.
    /// var dynamicConnection = _connection.Dynamic()
    /// IList&lt;MyData&gt; results = dynamicConnection.MyProc(10, returnType: typeof(MyData));
    /// This class generates methods that always return objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to return from all queries from this connection.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public sealed class DynamicConnection<T> : DynamicConnection
    {
        /// <summary>
        /// Initializes a new instance of the DynamicConnection class.
        /// </summary>
        /// <param name="connection">The connection to use as the inner database connection.</param>
        public DynamicConnection(IDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DynamicConnection class.
        /// </summary>
        /// <param name="connection">The connection to use as the inner database connection.</param>
        /// <param name="schema">The SQL schema to use when calling the procedure.</param>
        private DynamicConnection(IDbConnection connection, string schema)
            : base(connection, schema)
        {
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the DynamicObject class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = DoInvokeMember(binder, args, typeof(T));

            return true;
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            result = new DynamicConnection<T>(InnerConnection, binder.Name);
            return true;
        }
    }
    #endregion
}
