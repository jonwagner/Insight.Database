using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Database.Mapping;
using Insight.Database.MissingExtensions;
using Insight.Database.Structure;

namespace Insight.Database.CodeGenerator
{
    /// <summary>
    /// Implements a given interface by binding to the extension methods on DbConnection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Organization is by functionality.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
    class InterfaceGenerator
    {
        #region Private Fields
        private static readonly Type[] _ifuncDbConnectionParameterTypes = new Type[] { typeof(Func<IDbConnection>) };
        private static readonly Type[] _idbConnectionParameterTypes = new Type[] { typeof(IDbConnection) };
        private static readonly Type[] _executeParameterTypes = new Type[]
        {
                typeof(IDbConnection), typeof(string), typeof(object), typeof(CommandType), typeof(bool), typeof(int?), typeof(IDbTransaction), typeof(object)
        };

        private static readonly MethodInfo _executeMethod = typeof(DBConnectionExtensions).GetMethod("Execute", _executeParameterTypes);
        private static readonly MethodInfo _executeScalarMethod = typeof(DBConnectionExtensions).GetMethod("ExecuteScalar", _executeParameterTypes);
        private static readonly MethodInfo _queryMethod = typeof(DBConnectionExtensions).GetMethods().Single(mi => mi.Name == "Query" && mi.GetParameters().Any(p => p.Name == "returns") && mi.GetParameters().Any(p => p.Name == "connection"));

        private static readonly MethodInfo _insertMethod = typeof(DBConnectionExtensions).GetMethod("Insert");
        private static readonly MethodInfo _insertListMethod = typeof(DBConnectionExtensions).GetMethod("InsertList");

        private static readonly MethodInfo _executeAsyncMethod = typeof(DBConnectionExtensions).GetMethods().Single(mi => mi.Name == "ExecuteAsync");
        private static readonly MethodInfo _executeScalarAsyncMethod = typeof(DBConnectionExtensions).GetMethods().Single(mi => mi.Name == "ExecuteScalarAsync");
        private static readonly MethodInfo _queryAsyncMethod = typeof(DBConnectionExtensions).GetMethods().Single(mi => mi.Name == "QueryAsync" && mi.GetParameters().Any(p => p.Name == "returns") && mi.GetParameters().Any(p => p.Name == "connection"));
        private static readonly MethodInfo _insertAsyncMethod = typeof(DBConnectionExtensions).GetMethod("InsertAsync");
        private static readonly MethodInfo _insertListAsyncMethod = typeof(DBConnectionExtensions).GetMethod("InsertListAsync");

        private static readonly ConcurrentDictionary<Type, Func<Func<IDbConnection>, object>> _constructors = new ConcurrentDictionary<Type, Func<Func<IDbConnection>, object>>();
        private static readonly ConcurrentDictionary<Type, Func<Func<IDbConnection>, object>> _singleThreadedConstructors = new ConcurrentDictionary<Type, Func<Func<IDbConnection>, object>>();

        private static readonly ModuleBuilder _moduleBuilder;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the InterfaceGenerator class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static InterfaceGenerator()
        {
            // make a new assembly for the generated types
            AssemblyName an = typeof(InterfaceGenerator).GetTypeInfo().Assembly.GetName();
            an.Name = "Insight.Database.DynamicAssembly";

#if NO_APP_DOMAINS
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
#else
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
#endif
            _moduleBuilder = ab.DefineDynamicModule(an.Name);
        }
        #endregion

        #region Class Implementors
        /// <summary>
        /// Gets an implementor of an interface that can handle multiple concurrent calls.
        /// </summary>
        /// <param name="type">The interface to implmement.</param>
        /// <param name="connectionProvider">A provider of a connection to wrap.</param>
        /// <param name="singleThreaded">True to get a single-threaded implementation.</param>
        /// <returns>An implmementor of the given interface.</returns>
        public static object GetImplementorOf(Type type, Func<IDbConnection> connectionProvider, bool singleThreaded)
        {
            if (!type.GetTypeInfo().IsInterface && !type.GetTypeInfo().IsAbstract)
                throw new ArgumentException("type must be an interface or abstract class", "type");

            var constructors = singleThreaded ? _singleThreadedConstructors : _constructors;

            return constructors.GetOrAdd(
                type,
                t => CreateImplementorOf(t, singleThreaded))(connectionProvider);
        }

        /// <summary>
        /// Creates an implementation of the given type.
        /// If the type is an interface, the implementation is derived from DbConnectionWrapper.
        /// If the type is an abstract class, the implementation is derived from the class and the abstract methods are auto-implemented.
        /// </summary>
        /// <param name="type">The type to implement.</param>
        /// <param name="singleThreaded">True to create a single-threaded implementation.</param>
        /// <returns>A construction function that takes a connection provider and returns the implementation.</returns>
        private static Func<Func<IDbConnection>, object> CreateImplementorOf(Type type, bool singleThreaded)
        {
            // create the type
            string typeName = type.FullName + Guid.NewGuid().ToString();
            TypeBuilder tb;
            if (type.GetTypeInfo().IsInterface)
            {
                tb = _moduleBuilder.DefineType(typeName, TypeAttributes.Class, typeof(DbConnectionWrapper), new Type[] { type });
            }
            else
            {
                tb = _moduleBuilder.DefineType(typeName, TypeAttributes.Class, type, null);
            }

            // create a field for the connection provider
            FieldBuilder connectionField = tb.DefineField("_connection", singleThreaded ? typeof(IDbConnection) : typeof(Func<IDbConnection>), FieldAttributes.Private);

            // implement GetConnection
            var getConnection = EmitGetConnection(tb, type, connectionField);

            // define the constructor
            ConstructorBuilder ctor0 = EmitConstructor(tb, connectionField, singleThreaded);

            // for each method on the interface, try to implement it with a call to the database
            foreach (MethodInfo interfaceMethod in DiscoverMethods(type).Where(method => method != getConnection))
                EmitMethodImpl(tb, interfaceMethod, connectionField);

            // create a static create method that we can invoke directly as a delegate
            MethodBuilder m = tb.DefineMethod("Create", MethodAttributes.Static | MethodAttributes.Public, type, _ifuncDbConnectionParameterTypes);
            ILGenerator createIL = m.GetILGenerator();
            createIL.Emit(OpCodes.Ldarg_0);
            createIL.Emit(OpCodes.Newobj, ctor0);
            createIL.Emit(OpCodes.Ret);

            // create the type
            try
            {
                Type t = tb.CreateType();

                // return the create method
                var createMethod = t.GetMethod("Create", _ifuncDbConnectionParameterTypes);
                var delegateType = typeof(Func<Func<IDbConnection>, object>);

                return (Func<Func<IDbConnection>, object>)createMethod.CreateDelegate(delegateType);
            }
            catch (TypeLoadException e)
            {
                // inaccessible interface
                if (e.HResult == -2146233054)
                {
                    var template = "{1} {0} is inaccessible to Insight.Database. Make sure that the interface is public, or add " +
                        "[assembly:InternalsVisibleTo(\"Insight.Database.DynamicAssembly\")] " +
                        "to your assembly (System.Runtime.CompilerServices).  If the interface is nested, then it must be public to the world, " +
                        "or public to the assembly while using the InternalsVisibleTo attribute. " +
                        "Interfaces intended to be used AsParallel should not derive from IDbConnection or IDbTransaction.";
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, template, type.FullName, e.Message));
                }

                throw;
            }
        }

        /// <summary>
        /// Emits the constructor for the type.
        /// </summary>
        /// <param name="tb">The type we are implementing.</param>
        /// <param name="connectionField">The field containing the connection to use.</param>
        /// <param name="singleThreaded">True for a single-threaded implementation.</param>
        /// <returns>The constructor.</returns>
        private static ConstructorBuilder EmitConstructor(TypeBuilder tb, FieldBuilder connectionField, bool singleThreaded)
        {
            // create a constructor for the type - just store the connection provider
            var ctor0 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, _ifuncDbConnectionParameterTypes);
            var ctor0IL = ctor0.GetILGenerator();

            // for single-threaded mode, create the connection once at construction
            var localConnection = ctor0IL.DeclareLocal(singleThreaded ? typeof(IDbConnection) : typeof(Func<IDbConnection>));
            ctor0IL.Emit(OpCodes.Ldarg_1);
            if (singleThreaded)
                ctor0IL.Emit(OpCodes.Call, typeof(Func<IDbConnection>).GetMethod("Invoke"));
            ctor0IL.Emit(OpCodes.Stloc, localConnection);

            // call the base constructor first
            var baseConstructorParameters = singleThreaded ? _idbConnectionParameterTypes : _ifuncDbConnectionParameterTypes;
            var baseConstructor = tb.BaseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, baseConstructorParameters, null) ??
                                  tb.BaseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (baseConstructor == null)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "{0} cannot be implemented Insight.Database. Make sure that the class has a default constructor, or another constructor that Insight can call.", tb.BaseType.FullName));
            var hasParameters = (baseConstructor.GetParameters().Length == 1);
            ctor0IL.Emit(OpCodes.Ldarg_0);
            if (hasParameters)
                ctor0IL.Emit(OpCodes.Ldloc, localConnection);
            ctor0IL.Emit(OpCodes.Call, baseConstructor);

            // store the connection in our connection field for later
            ctor0IL.Emit(OpCodes.Ldarg_0);
            if (hasParameters)
                ctor0IL.Emit(OpCodes.Ldarg_0);
            else
                ctor0IL.Emit(OpCodes.Ldloc, localConnection);
            ctor0IL.Emit(OpCodes.Stfld, connectionField);

            ctor0IL.Emit(OpCodes.Ret);

            return ctor0;
        }

        /// <summary>
        /// Emits a GetConnection method if the class needs one.
        /// </summary>
        /// <param name="tb">The TypeBuilder.</param>
        /// <param name="baseType">The type being implemented.</param>
        /// <param name="connectionField">The field containing the connection provider.</param>
        /// <returns>The GetConnection MethodInfo from the base class.</returns>
        private static MethodInfo EmitGetConnection(TypeBuilder tb, Type baseType, FieldBuilder connectionField)
        {
            const string GetConnectionName = "GetConnection";

            var getConnection = baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => String.Compare(m.Name, GetConnectionName, StringComparison.OrdinalIgnoreCase) == 0 && m.ReturnType == typeof(IDbConnection) && m.GetParameters().Length == 0);
            if (getConnection != null && getConnection.IsAbstract)
            {
                var gc = tb.DefineMethod(GetConnectionName, MethodAttributes.Public | MethodAttributes.Virtual);
                TypeHelper.CopyMethodSignature(getConnection, gc);
                var mIL = gc.GetILGenerator();

                mIL.Emit(OpCodes.Ldarg_0);
                mIL.Emit(OpCodes.Ldfld, connectionField);
                if (connectionField.FieldType == typeof(Func<IDbConnection>))
                    mIL.Emit(OpCodes.Call, connectionField.FieldType.GetMethod("Invoke"));
                mIL.Emit(OpCodes.Ret);
            }

            return getConnection;
        }
        #endregion

        #region Internal Members
        /// <summary>
        /// Finds all of the methods on a given interface.
        /// </summary>
        /// <param name="type">The type to explore.</param>
        /// <returns>All of the methods defined on the interface.</returns>
        private static IEnumerable<MethodInfo> DiscoverMethods(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod)
                    .Union(type.GetInterfaces().Where(i => !i.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)).SelectMany(i => DiscoverMethods(i)));
            }
            else
            {
                return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod)
                    .Where(m => m.IsAbstract);
            }
        }

        /// <summary>
        /// Emits an implementation of a given method.
        /// </summary>
        /// <param name="tb">The TypeBuilder to emit to.</param>
        /// <param name="interfaceMethod">The interface method to implement.</param>
        /// <param name="connectionField">The fields storing the connection to the database.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static void EmitMethodImpl(TypeBuilder tb, MethodInfo interfaceMethod, FieldBuilder connectionField)
        {
            // look at the parameters on the interface
            var parameters = interfaceMethod.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

            // determine the proper method to call
            MethodInfo executeMethod = GetExecuteMethod(interfaceMethod);

            // get the sql attributes from the  method and class/interface
            var sqlAttribute = interfaceMethod.GetCustomAttributes(false).OfType<SqlAttribute>().FirstOrDefault() ?? new SqlAttribute();
            var typeSqlAttribute = interfaceMethod.DeclaringType.GetCustomAttributes(false).OfType<SqlAttribute>().FirstOrDefault() ?? new SqlAttribute();

            // calculate the query parameters
            var schema = sqlAttribute.Schema ?? typeSqlAttribute.Schema;
            var procName = (executeMethod.DeclaringType == typeof(DBConnectionExtensions)) ? Regex.Replace(interfaceMethod.Name, "Async$", String.Empty, RegexOptions.IgnoreCase) : interfaceMethod.Name;
            var sql = sqlAttribute.Sql ?? typeSqlAttribute.Sql ?? procName;
            var commandType = sqlAttribute.CommandType ?? typeSqlAttribute.CommandType ?? (sql.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure);
            if (commandType == CommandType.StoredProcedure && !schema.IsNullOrWhiteSpace() && !sql.Contains('.'))
                sql = schema.Trim() + "." + sql;

            // start a new method
            MethodBuilder m = tb.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual);
            TypeHelper.CopyMethodSignature(interfaceMethod, m);
            ILGenerator mIL = m.GetILGenerator();

            LocalBuilder parameterWrapper = null;

            var executeParameters = executeMethod.GetParameters();
            for (int i = 0; i < executeParameters.Length; i++)
            {
                switch (executeParameters[i].Name)
                {
                    case "connection":
                        // get the connection from the getConnection method
                        mIL.Emit(OpCodes.Ldarg_0);
                        mIL.Emit(OpCodes.Ldfld, connectionField);
                        if (connectionField.FieldType == typeof(Func<IDbConnection>))
                            mIL.Emit(OpCodes.Call, connectionField.FieldType.GetMethod("Invoke"));
                        break;

                    case "sql":
                        // if the sql attribute is on the method, use that
                        mIL.Emit(OpCodes.Ldstr, sql);
                        break;

                    case "parameters":
                        // load all of the parameters and convert it to a parameters object
                        if (parameters.Length == 0)
                        {
                            // no parameters, just pass null
                            mIL.Emit(OpCodes.Ldnull);
                        }
                        else if (parameters.Length == 1 && !TypeHelper.IsAtomicType(parameters[0].ParameterType))
                        {
                            // one parameter that is a non-atomic object, just pass it and let the insight framework handle it
                            mIL.Emit(OpCodes.Ldarg_1);
                            if (parameters[0].ParameterType.GetTypeInfo().IsValueType)
                                mIL.Emit(OpCodes.Box, parameters[0].ParameterType);
                        }
                        else
                        {
                            // create a class for the parameters and stick them in there
                            Type parameterWrapperType = CreateParameterClass(interfaceMethod, parameters);
                            for (int pi = 0; pi < parameters.Length; pi++)
                                mIL.Emit(OpCodes.Ldarg, pi + 1);
                            mIL.Emit(OpCodes.Newobj, parameterWrapperType.GetConstructors()[0]);

                            // store the parameters in a local so we can unwrap output parameters
                            parameterWrapper = mIL.DeclareLocal(parameterWrapperType);
                            mIL.Emit(OpCodes.Dup);
                            mIL.Emit(OpCodes.Stloc, parameterWrapper);
                        }

                        break;

                    case "outputParameters":
                        // fill in the output parameters object with the temporary parameters object
                        if (parameterWrapper != null)
                            mIL.Emit(OpCodes.Ldloc, parameterWrapper.LocalIndex);
                        else
                            mIL.Emit(OpCodes.Ldnull);
                        break;

                    case "inserted":
                        // always pass argument 1 in
                        mIL.Emit(OpCodes.Ldarg_1);
                        break;

                    case "returns":
                        if (!EmitSpecialParameter(mIL, "returns", parameters, executeParameters))
                            GenerateReturnsStructure(interfaceMethod, mIL);
                        break;

                    case "commandType":
                        if (EmitSpecialParameter(mIL, "commandType", parameters, executeParameters))
                            break;

                        IlHelper.EmitLdInt32(mIL, (int)commandType);
                        break;

                    case "commandBehavior":
                        if (EmitSpecialParameter(mIL, "commandBehavior", parameters, executeParameters))
                            break;

                        IlHelper.EmitLdInt32(mIL, (int)CommandBehavior.Default);
                        break;

                    case "closeConnection":
                        if (EmitSpecialParameter(mIL, "closeConnection", parameters, executeParameters))
                            break;

                        IlHelper.EmitLdInt32(mIL, (int)0);
                        break;

                    case "commandTimeout":
                        if (EmitSpecialParameter(mIL, "commandTimeout", parameters, executeParameters))
                            break;

                        var commandTimeout = mIL.DeclareLocal(typeof(int?));
                        mIL.Emit(OpCodes.Ldloca_S, commandTimeout);
                        mIL.Emit(OpCodes.Initobj, typeof(int?));
                        mIL.Emit(OpCodes.Ldloc, commandTimeout);
                        break;

                    case "transaction":
                        if (EmitSpecialParameter(mIL, "transaction", parameters, executeParameters))
                            break;

                        mIL.Emit(OpCodes.Ldnull);
                        break;

                    case "cancellationToken":
                        if (EmitSpecialParameter(mIL, "cancellationToken", parameters, executeParameters))
                            break;

                        var cancellationToken = mIL.DeclareLocal(typeof(CancellationToken));
                        mIL.Emit(OpCodes.Ldloca_S, cancellationToken);
                        mIL.Emit(OpCodes.Initobj, typeof(CancellationToken));
                        mIL.Emit(OpCodes.Ldloc, cancellationToken);
                        break;

                    default:
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot determine how to generate parameter {1} for method {0}", executeMethod.Name, executeParameters[i].Name));
                }
            }

            // call the execute method
            mIL.Emit(OpCodes.Call, executeMethod);

            // if the method returns void, throw away the return value
            if (interfaceMethod.ReturnType == typeof(void))
                mIL.Emit(OpCodes.Pop);

            // copy the output parameters from our parameter structure back to the output parameters
            EmitOutputParameters(parameters, parameterWrapper, mIL);

            mIL.Emit(OpCodes.Ret);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        private static void GenerateReturnsStructure(MethodInfo interfaceMethod, ILGenerator mIL)
        {
            // if we are returning a task, unwrap the task result
            var returnType = interfaceMethod.ReturnType;
            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                returnType = returnType.GetGenericArguments()[0];

            // if there are returns attributes specified, then build the structure for the coder
            var returnsAttributes = interfaceMethod.GetCustomAttributes(true).OfType<RecordsetAttribute>().ToDictionary(r => r.Index);

            if (returnType.IsSubclassOf(typeof(Results)) && !returnType.Name.StartsWith("Results`", StringComparison.OrdinalIgnoreCase))
            {
                // we have a return that is derived from results
                mIL.Emit(OpCodes.Ldsfld, typeof(DerivedResultsReader<>).MakeGenericType(returnType).GetField("Default"));
            }
            else
            {
                bool isSingle = !returnType.IsSubclassOf(typeof(Results)) && !IsGenericListType(returnType);

                // we are returning results<T...>, or IList<T...>
                var returnTypeArgs = isSingle ? new Type[] { returnType } : returnType.GetGenericArguments();
                Type currentType = null;

                // go through all of the type arguments or recordsets
                int returnIndex = 0;
                for (int i = 0; i < Math.Max(returnTypeArgs.Length, returnsAttributes.Keys.MaxOrDefault(k => k + 1)); i++)
                {
                    RecordsetAttribute r;
                    returnsAttributes.TryGetValue(i, out r);
                    var types = (r != null) ? r.Types : new Type[] { returnTypeArgs[i] };

                    // if the return type is a named tuple, emit column mappings
                    var elementNames = interfaceMethod.ReturnTypeCustomAttributes.GetCustomAttributes(true).OfType<System.Runtime.CompilerServices.TupleElementNamesAttribute>().FirstOrDefault();
                    if (elementNames != null)
                    {
                        var overrides = elementNames.TransformNames.Select((n, index) => new ColumnOverride(n, String.Format("Item{0}", index + 1)));
                        var oneToOne = System.Activator.CreateInstance(Query.GetOneToOneType(types), new object[] { null, overrides, null });
                        StaticFieldStorage.EmitLoad(mIL, oneToOne);
                    }
                    else
                    {
                        // grab the records field for the appropriate OneToOne mapping
                        mIL.Emit(OpCodes.Ldsfld, Query.GetOneToOneType(types).GetField("Records", BindingFlags.Public | BindingFlags.Static));
                    }

                    // keep track of the type that we are returning
                    if (r == null || !r.IsChild)
                        returnIndex++;

                    if (i == 0)
                    {
                        // start the chain of calls
                        if (isSingle)
                        {
                            var recordReader = typeof(IRecordReader<>).MakeGenericType(returnType);
                            var readerType = typeof(SingleReader<>).MakeGenericType(returnType);
                            var constructor = readerType.GetConstructor(new Type[] { recordReader });
                            mIL.Emit(OpCodes.Newobj, constructor);
                            currentType = readerType;
                        }
                        else if (returnType.IsSubclassOf(typeof(Results)))
                        {
                            var method = typeof(Query).GetMethod("ReturnsResults", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(types[0]);
                            currentType = method.ReturnType;
                            mIL.Emit(OpCodes.Call, method);
                        }
                        else
                        {
                            // IList<T> or List<T>, etc...
                            var oneToOneBaseType = typeof(OneToOne<>).MakeGenericType(types[0]);

                            currentType = typeof(ListReader<>).MakeGenericType(types[0]);
                            Type readerType = currentType;

                            // if we're not returning an IList, then we need to insert the reader adapter class
                            if (returnType.GetGenericTypeDefinition() != typeof(IList<>))
                                readerType = typeof(ListReaderAdapter<,>).MakeGenericType(returnType, types[0]);

                            mIL.Emit(OpCodes.Newobj, readerType.GetConstructor(new Type[] { oneToOneBaseType }));
                        }
                    }
                    else if (r != null && r.IsChild)
                    {
                        var parentType = currentType.GetGenericArguments()[0];
                        var childType = types[0];

                        MethodInfo method;

                        // the parent is single and we haven't overridden the id field
                        if (r.Id == null && TypeIsSingleReader(currentType))
                        {
	                        var list = ChildMapperHelper.GetListAccessor(parentType, childType, r.Into);
    	                    var listMethod = typeof(ClassPropInfo).GetMethod("CreateSetMethod").MakeGenericMethod(parentType, typeof(List<>).MakeGenericType(childType)).Invoke(list, Parameters.EmptyArray);

                            // previous and recordReader are on the stack, add the id and list method
                            StaticFieldStorage.EmitLoad(mIL, listMethod);

                            method = typeof(Query).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                .Single(
                                    mi => mi.Name == "ThenChildren" &&
                                        mi.GetGenericArguments().Length == 2 &&
                                        currentType.GetGenericTypeDefinition().Name == mi.GetParameters()[0].ParameterType.Name)
                                .MakeGenericMethod(
                                    new Type[]
                                    {
                                        parentType,		// TParent
										childType,		// TChild
									});
                        }
                        else
                        {
							var rootType = returnTypeArgs[0];
                            var recordReaderType = typeof(RecordReader<>).MakeGenericType(childType);

							// if the Parents attribute is set, then we're dealing with a multi-level hierarchy
							ClassPropInfo parents = null;
							if (r.Parents != null)
							{
								parents = ChildMapperHelper.GetListAccessor(rootType, null, r.Parents, setter: false);
								parentType = parents.MemberType.GenericTypeArguments[0];
							}

							var list = ChildMapperHelper.GetListAccessor(parentType, childType, r.Into);
							var listMethod = typeof(ClassPropInfo).GetMethod("CreateSetMethod").MakeGenericMethod(parentType, typeof(List<>).MakeGenericType(childType)).Invoke(list, Parameters.EmptyArray);

                            var parentid = ChildMapperHelper.GetIDAccessor(parentType, r.Id);
                            var parentIdType = parentid.MemberType;
                            var parentIdMethod = parentid.GetType().GetMethod("CreateGetMethod").MakeGenericMethod(parentType, parentIdType).Invoke(parentid, Parameters.EmptyArray);

                            // if groupby is specified, then convert the RecordReader to an IChildRecordReader by groupby
                            if (r.GroupBy != null)
                            {
                                var childid = ChildMapperHelper.FindParentIDAccessor(childType, r.GroupBy, parentType);
                                if (childid == null)
                                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find GroupBy {0} on {1}", r.GroupBy, childType.FullName));

                                var getMethod = childid.GetType().GetMethod("CreateGetMethod").MakeGenericMethod(childType, parentIdType).Invoke(childid, Parameters.EmptyArray);
                                StaticFieldStorage.EmitLoad(mIL, getMethod);

                                var groupByMethod = recordReaderType.GetMethod("GroupBy").MakeGenericMethod(parentIdType);
                                mIL.Emit(OpCodes.Call, groupByMethod);

                                recordReaderType = groupByMethod.ReturnType;
                            }

                            // previous and recordReader are on the stack, add the id and list methods

							if (r.Parents != null)
							{
								var parentsMethod = typeof(ClassPropInfo).GetMethod("CreateGetMethod").MakeGenericMethod(rootType, parents.MemberType).Invoke(parents, Parameters.EmptyArray);

	                            StaticFieldStorage.EmitLoad(mIL, parentsMethod); // parents
	                            StaticFieldStorage.EmitLoad(mIL, parentIdMethod); // id
	                            StaticFieldStorage.EmitLoad(mIL, listMethod); // into

								method = typeof(Query).GetMethods(BindingFlags.Public | BindingFlags.Static)
									.Single(
										mi => mi.Name == "ThenChildren" &&
											mi.GetGenericArguments().Length == 4 &&
											currentType.GetGenericTypeDefinition().Name == mi.GetParameters()[0].ParameterType.Name &&
											mi.GetParameters()[1].ParameterType.Name == recordReaderType.Name &&
											mi.GetParameters().Any(p => String.Compare(p.Name, "parents", StringComparison.OrdinalIgnoreCase) == 0) &&
											mi.GetParameters().Any(p => String.Compare(p.Name, "id", StringComparison.OrdinalIgnoreCase) == 0) &&
											mi.GetParameters().Any(p => String.Compare(p.Name, "into", StringComparison.OrdinalIgnoreCase) == 0))
									.MakeGenericMethod(
										new Type[]
										{
											rootType,		// TRoot
											parentType,		// TParent
											childType,		// TChild
											parentIdType	// TId
										});
							}
							else
							{
	                            StaticFieldStorage.EmitLoad(mIL, parentIdMethod); // id
	                            StaticFieldStorage.EmitLoad(mIL, listMethod); // into

								method = typeof(Query).GetMethods(BindingFlags.Public | BindingFlags.Static)
									.Single(
										mi => mi.Name == "ThenChildren" &&
											mi.GetGenericArguments().Length == 3 &&
											currentType.GetGenericTypeDefinition().Name == mi.GetParameters()[0].ParameterType.Name &&
											mi.GetParameters()[1].ParameterType.Name == recordReaderType.Name &&
											mi.GetParameters().Any(p => String.Compare(p.Name, "id", StringComparison.OrdinalIgnoreCase) == 0) &&
											mi.GetParameters().Any(p => String.Compare(p.Name, "into", StringComparison.OrdinalIgnoreCase) == 0))
									.MakeGenericMethod(
										new Type[]
										{
											parentType,		// TParent
											childType,		// TChild
											parentIdType	// TId
										});
							}
                        }

                        mIL.Emit(OpCodes.Call, method);
                        currentType = method.ReturnType;
                    }
                    else
                    {
                        var method = typeof(Query).GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Single(mi => mi.Name == "Then" && mi.GetGenericArguments().Length == returnIndex && mi.GetParameters()[0].ParameterType.Name.StartsWith("IQueryReader", StringComparison.OrdinalIgnoreCase))
                            .MakeGenericMethod(returnTypeArgs.Take(returnIndex).ToArray());
                        mIL.Emit(OpCodes.Call, method);
                        currentType = method.ReturnType;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the given type is a single reader.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>True if the type is a single reader.</returns>
        private static bool TypeIsSingleReader(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType)
                return false;

            var generic = type.GetGenericTypeDefinition();
            return generic == typeof(SingleReader<>) || generic.IsSubclassOf(typeof(SingleReader<>));
        }

        /// <summary>
        /// Copies fields from the temporary output parameters structure to the output parameters object specified by the caller.
        /// </summary>
        /// <param name="parameters">The list of parameters.</param>
        /// <param name="parameterWrapper">The local variable for the temporary object.</param>
        /// <param name="mIL">The ILGenerator to use.</param>
        private static void EmitOutputParameters(ParameterInfo[] parameters, LocalBuilder parameterWrapper, ILGenerator mIL)
        {
            foreach (var outputParameter in parameters.Where(p => p.IsOut))
            {
                mIL.Emit(OpCodes.Ldarg, outputParameter.Position + 1);
                mIL.Emit(OpCodes.Ldloc, parameterWrapper.LocalIndex);
                var fields = parameterWrapper.LocalType.GetFields();
                mIL.Emit(OpCodes.Ldfld, fields[outputParameter.Position]);
                mIL.Emit(OpCodes.Stobj, outputParameter.ParameterType.GetElementType());
            }
        }

        /// <summary>
        /// Determines the proper connection extension method to call to implement the interface method.
        /// </summary>
        /// <param name="method">The interface method to analyze.</param>
        /// <returns>The extension method that can implement the given method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static MethodInfo GetExecuteMethod(MethodInfo method)
        {
            // if returntype is null
            if (method.ReturnType == typeof(void))
            {
                // if the first parameter is enumerable, then we assume we are doing an insert/update multiple
                var parameter = method.GetParameters().FirstOrDefault();
                Type type = (parameter == null) ? null : parameter.ParameterType;

                // if the object is not atomic, then attempt to merge the results into the first object
                // methods that start with insert/upsert map to insert
                if (type != null && !TypeHelper.IsAtomicType(type) && IsMethodAnUpsert(method))
                {
                    var enumerable = type.GetInterfaces().Union(new[] { type }).FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (enumerable != null)
                        return _insertListMethod.MakeGenericMethod(enumerable.GetGenericArguments()[0]);

                    return _insertMethod.MakeGenericMethod(type);
                }

                // not an insert, so call execute
                return _executeMethod;
            }

            // check for async signatures
            var asyncMethod = GetExecuteAsyncMethod(method);
            if (asyncMethod != null)
            {
                // there is no way for us to return the output parameters before the results are retrieved
                if (method.GetParameters().Any(p => p.ParameterType.IsByRef))
                    throw new InvalidOperationException("Ref and Out parameters are not permitted on asynchronous methods");

                return asyncMethod;
            }

            // if returntype is an atomic object, then we call ExecuteScalar
            if (TypeHelper.IsAtomicType(method.ReturnType))
                return _executeScalarMethod.MakeGenericMethod(method.ReturnType);

            // if returntype is not an atomic object, then we call Single
            return _queryMethod.MakeGenericMethod(method.ReturnType);
        }

        /// <summary>
        /// Determines the proper connection extension method to call to implement the interface method when the return type is a task.
        /// </summary>
        /// <param name="method">The interface method to analyze.</param>
        /// <returns>The extension method that can implement the given method.</returns>
        private static MethodInfo GetExecuteAsyncMethod(MethodInfo method)
        {
            // if returntype is Task
            if (method.ReturnType == typeof(Task))
            {
                // if the first parameter is enumerable, then we assume we are doing an insert/update multiple
                var parameter = method.GetParameters().FirstOrDefault();
                Type type = (parameter == null) ? null : parameter.ParameterType;

                // if the object is not atomic, then attempt to merge the results into the first object
                // methods that start with insert/upsert map to insert
                if (type != null && !TypeHelper.IsAtomicType(type) && IsMethodAnUpsert(method))
                {
                    var enumerable = type.GetInterfaces().Union(new[] { type }).FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (enumerable != null)
                        return _insertListAsyncMethod.MakeGenericMethod(enumerable.GetGenericArguments()[0]);

                    return _insertAsyncMethod.MakeGenericMethod(type);
                }

                // just a task, call ExecuteAsync
                return _executeAsyncMethod;
            }

            // if the returntype is Task<T>, look a little deeper
            if (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // get the inside of the task
                var taskResultType = method.ReturnType.GetGenericArguments()[0];

                // if returntype is Task<T>, and T is an atomic object, then we call ExecuteScalarAsync
                if (TypeHelper.IsAtomicType(taskResultType))
                    return _executeScalarAsyncMethod.MakeGenericMethod(taskResultType);

                // if returntype is Task<IList<T>>, or Task<Results<T>>, then we call QueryAsync
                return _queryAsyncMethod.MakeGenericMethod(taskResultType);
            }

            return null;
        }

        /// <summary>
        /// Determines whether a method is an insert/upsert and should have its outputs reflected back onto the inputs.
        /// </summary>
        /// <param name="method">The method to test.</param>
        /// <returns>True if the method is an insert or upsert.</returns>
        private static bool IsMethodAnUpsert(MethodInfo method)
        {
            // if the method is marked with a mergeoutputattribute, check that before the default logic
            var mergeAttributes = (MergeOutputAttribute[])method.GetCustomAttributes(typeof(MergeOutputAttribute), true);
            if (mergeAttributes.Any(m => m.MergeOutputs))
                return true;

            return (method.Name.StartsWith("Insert", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.StartsWith("Update", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.StartsWith("Upsert", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates an anonymous class to hold the parameters to the execute call.
        /// This allows us to pass in the exact same objects as in our dynamic methods.
        /// </summary>
        /// <param name="method">The method being emitted.</param>
        /// <param name="parameters">The list of parameters to capture.</param>
        /// <returns>An anonymous wrapper class for the parameters.</returns>
        private static Type CreateParameterClass(MethodInfo method, IEnumerable<ParameterInfo> parameters)
        {
            // if there are no parameters, then there is no need to create a type
            if (!parameters.Any())
                return null;

            // create a new class
            TypeBuilder tb = _moduleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);

            // enable deep binding on the parameter class
            var bindAttribute = (BindChildrenAttribute)method.GetCustomAttributes(typeof(BindChildrenAttribute), true).FirstOrDefault() ?? new BindChildrenAttribute();
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(BindChildrenAttribute).GetConstructor(new Type[] { typeof(BindChildrenFor) }), new object[] { bindAttribute.For }));

            // create a constructor for the class that takes all of the parameters
            ConstructorBuilder ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameters.Select(p => p.ParameterType).ToArray());
            ILGenerator ctorIL = ctor.GetILGenerator();

            // the constructor just copies the parameters into the internal fields
            foreach (ParameterInfo p in parameters)
            {
                // if the parameter is one of our special parameters, strip it out
                if (IsSpecialParameter(p))
                    continue;

                // if we have a ref parameter, create a field of the base element type so we have a place to put values
                var parameterType = p.ParameterType;
                if (parameterType.IsByRef)
                    parameterType = parameterType.GetElementType();

                FieldBuilder fb = tb.DefineField(p.Name, parameterType, FieldAttributes.Public);

                // if there is a column attribute, copy it to the field
                var columnAttribute = (ColumnAttribute)p.GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault();
                if (columnAttribute != null)
                    fb.SetCustomAttribute(columnAttribute.GetCustomAttributeBuilder());

                ctorIL.Emit(OpCodes.Ldarg_0);                   // this
                ctorIL.Emit(OpCodes.Ldarg, p.Position + 1);     // parameter (in order)
                if (p.ParameterType.IsByRef)                    // dereference the pointer if necessary
                    ctorIL.Emit(OpCodes.Ldobj, parameterType);
                ctorIL.Emit(OpCodes.Stfld, fb);                 // store to field
            }

            ctorIL.Emit(OpCodes.Ret);

            return tb.CreateType();
        }

        /// <summary>
        /// Returns true if the type represents a generic of a list.
        /// Used to determine if the type represents a result set.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>True if it is a list type.</returns>
        private static bool IsGenericListType(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType)
                return false;

            var generic = type.GetGenericTypeDefinition();

            return generic == typeof(IList<>) ||
                    generic == typeof(List<>) ||
                    generic == typeof(IEnumerable<>) ||
                    generic == typeof(ICollection<>);
        }

        /// <summary>
        /// Returns true if the parameter is a parameter that should be handled specially.
        /// </summary>
        /// <param name="parameterInfo">Information about the parameter.</param>
        /// <returns>True if the parameter is a special parameter, false otherwise.</returns>
        private static bool IsSpecialParameter(ParameterInfo parameterInfo)
        {
            switch (parameterInfo.Name)
            {
                case "returns":
                case "commandBehavior":
                case "closeConnection":
                case "commandTimeout":
                case "transaction":
                case "cancellationToken":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to emit the code to push a special parameter by looking it up in the interface parameters.
        /// If the parameter is defined, this emits a Ldarg operation.
        /// </summary>
        /// <param name="il">The ILGenerator to use.</param>
        /// <param name="parameterName">The name of the special parameter.</param>
        /// <param name="interfaceParameters">The parameters on the interface method.</param>
        /// <param name="executeParameters">The parameters on the execute method.</param>
        /// <returns>True if a parameter was emitted.</returns>
        private static bool EmitSpecialParameter(ILGenerator il, string parameterName, ParameterInfo[] interfaceParameters, ParameterInfo[] executeParameters)
        {
            // attempt to find the parameter on the interface method
            var interfaceParameter = interfaceParameters.FirstOrDefault(p => String.Compare(p.Name, parameterName, StringComparison.OrdinalIgnoreCase) == 0);
            if (interfaceParameter == null)
                return false;

            // attempt to find the parameter on the execute method
            var executeParameter = executeParameters.FirstOrDefault(p => String.Compare(p.Name, parameterName, StringComparison.OrdinalIgnoreCase) == 0);
            if (executeParameter == null)
                return false;

            // the types must match
            if (interfaceParameter.ParameterType != executeParameter.ParameterType)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Special Parameter {0} must have type {1}", parameterName, executeParameter.ParameterType.FullName));

            // the parameter list is 0-based, but 0 is the this pointer, so we add one
            il.Emit(OpCodes.Ldarg, (int)interfaceParameter.Position + 1);
            return true;
        }
        #endregion
    }
}
