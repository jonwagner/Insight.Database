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

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Implements a given interface by binding to the extension methods on DbConnection.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	class InterfaceGenerator
	{
		#region Private Fields
		private static readonly MethodInfo _typeGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
		private static readonly Type[] _dbConnectionParameterTypes = new Type[] { typeof(DbConnection) };
		private static readonly Type[] _executeParameterTypes = new Type[]
		{
				typeof(IDbConnection), typeof(string), typeof(object), typeof(CommandType), typeof(bool), typeof(int?), typeof(IDbTransaction)
		};

		private static readonly Type[] _queryParameterTypes = new Type[]
		{
				typeof(IDbConnection), typeof(string), typeof(object), typeof(Type), typeof(CommandType), typeof(CommandBehavior), typeof(int?), typeof(IDbTransaction)
		};

		private static readonly Type[] _queryAsyncParameterTypes = new Type[]
		{
				typeof(IDbConnection), typeof(string), typeof(object), typeof(Type), typeof(CommandType), typeof(CommandBehavior), typeof(int?), typeof(IDbTransaction), typeof(CancellationToken?)
		};

		private static readonly MethodInfo _executeMethod = typeof(DBConnectionExtensions).GetMethod("Execute", _executeParameterTypes);
		private static readonly MethodInfo _executeScalarMethod = typeof(DBConnectionExtensions).GetMethod("ExecuteScalar", _executeParameterTypes);
		private static readonly MethodInfo _queryMethod = typeof(DBConnectionExtensions).GetMethod("Query", _queryParameterTypes);
		private static readonly MethodInfo _queryResultsMethod = typeof(DBConnectionExtensions).GetMethods().FirstOrDefault(mi => mi.Name == "QueryResults" && mi.GetGenericArguments().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(IDbConnection));
		private static readonly MethodInfo _singleMethod = typeof(DBConnectionExtensions).GetMethod("Single", _queryParameterTypes);

		private static readonly MethodInfo _executeAsyncMethod = typeof(AsyncExtensions).GetMethods().First(mi => mi.Name == "ExecuteAsync");
		private static readonly MethodInfo _executeScalarAsyncMethod = typeof(AsyncExtensions).GetMethods().First(mi => mi.Name == "ExecuteScalarAsync");
		private static readonly MethodInfo _queryAsyncMethod = typeof(AsyncExtensions).GetMethod("QueryAsync", _queryAsyncParameterTypes);
		private static readonly MethodInfo _singleAsyncMethod = typeof(AsyncExtensions).GetMethod("SingleAsync", _queryAsyncParameterTypes);
		private static readonly MethodInfo _queryResultsAsyncMethod = typeof(AsyncExtensions).GetMethods().FirstOrDefault(mi => mi.Name == "QueryResultsAsync" && mi.GetGenericArguments().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(IDbConnection));

		private static readonly ConcurrentDictionary<Type, Func<DbConnection, object>> _constructors = new ConcurrentDictionary<Type, Func<DbConnection, object>>();
		#endregion

		/// <summary>
		/// Gets the implementor of the given interface for a DbConnection.
		/// </summary>
		/// <param name="connection">The connection to wrap.</param>
		/// <param name="interfaceType">The interface to implmement.</param>
		/// <returns>An implmementor of the given interface.</returns>
		public static object GetImplementorOf(DbConnection connection, Type interfaceType)
		{
			return _constructors.GetOrAdd(interfaceType, t => CreateImplementorOf(t))(connection);
		}

		/// <summary>
		/// Creates the implementor of the given interface for a DbConnection.
		/// </summary>
		/// <param name="interfaceType">The interface to implmement.</param>
		/// <returns>An implmementor of the given interface.</returns>
		private static Func<DbConnection, object> CreateImplementorOf(Type interfaceType)
		{
			// create a new assembly
			AssemblyName an = Assembly.GetExecutingAssembly().GetName();
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
			ModuleBuilder mb = ab.DefineDynamicModule(an.Name);

			// create a type based on DbConnectionWrapper and call the default constructor
			TypeBuilder tb = mb.DefineType(interfaceType.FullName + "_Connection", TypeAttributes.Class, typeof(DbConnectionWrapper), new Type[] { interfaceType });

			// create a constructor for the type
			ConstructorBuilder ctor0 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, _dbConnectionParameterTypes);
			ILGenerator ctor0IL = ctor0.GetILGenerator();
			ctor0IL.Emit(OpCodes.Ldarg_0);
			ctor0IL.Emit(OpCodes.Ldarg_1);
			ctor0IL.Emit(OpCodes.Call, typeof(DbConnectionWrapper).GetConstructor(_dbConnectionParameterTypes));
			ctor0IL.Emit(OpCodes.Ret);

			// for each method on the interface, try to implement it with a call to the database
			foreach (MethodInfo interfaceMethod in interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
				EmitMethodImpl(mb, tb, interfaceMethod);

			// create a static create method that we can invoke directly as a delegate
			MethodBuilder m = tb.DefineMethod("Create", MethodAttributes.Static | MethodAttributes.Public, interfaceType, _dbConnectionParameterTypes);
			ILGenerator createIL = m.GetILGenerator();
			createIL.Emit(OpCodes.Ldarg_0);
			createIL.Emit(OpCodes.Newobj, ctor0);
			createIL.Emit(OpCodes.Ret);

			// create the type
			Type t = tb.CreateType();

			// return the create method
			return (Func<DbConnection, object>)Delegate.CreateDelegate(typeof(Func<DbConnection, object>), t.GetMethod("Create"));
		}

		/// <summary>
		/// Emits an implementation of a given method.
		/// </summary>
		/// <param name="mb">The ModuleBuilder to emit to.</param>
		/// <param name="tb">The TypeBuilder to emit to.</param>
		/// <param name="interfaceMethod">The interface method to implement.</param>
		private static void EmitMethodImpl(ModuleBuilder mb, TypeBuilder tb, MethodInfo interfaceMethod)
		{
			// TODO: function name/parameter mapping
			// TODO: handle special parameters
			// TODO: handle graph specifications

			// look at the parameters on the interface
			var parameters = interfaceMethod.GetParameters();
			var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

			// determine the proper method to call
			MethodInfo executeMethod = GetExecuteMethod(interfaceMethod);

			// see if the interface method has inlined SQL
			var sqlAttribute = interfaceMethod.GetCustomAttributes(false).OfType<SqlAttribute>().FirstOrDefault();

			// see if the interface method has a graph defined
			var graphAttribute = interfaceMethod.GetCustomAttributes(false).OfType<DefaultGraphAttribute>().FirstOrDefault();

			// start a new method
			MethodBuilder m = tb.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, interfaceMethod.ReturnType, parameterTypes);
			ILGenerator mIL = m.GetILGenerator();
			mIL.DeclareLocal(typeof(int?)); // loc.0
			mIL.DeclareLocal(typeof(CancellationToken?)); // loc.1

			var executeParameters = executeMethod.GetParameters();
			for (int i = 0; i < executeParameters.Length; i++)
			{
				switch (executeParameters[i].Name)
				{
					case "connection":
						// 'this' is the connection
						mIL.Emit(OpCodes.Ldarg_0);
						break;

					case "sql":
						// if the sql attribute is on the method, use that
						if (sqlAttribute != null)
						{
							mIL.Emit(OpCodes.Ldstr, sqlAttribute.Sql);
						}
						else
						{
							// if this is an async method, remove async from the end of the proc name
							var procName = (executeMethod.DeclaringType == typeof(AsyncExtensions)) ? Regex.Replace(interfaceMethod.Name, "Async$", String.Empty, RegexOptions.IgnoreCase) : interfaceMethod.Name;
							mIL.Emit(OpCodes.Ldstr, procName);
						}

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
							if (parameters[0].ParameterType.IsValueType)
								mIL.Emit(OpCodes.Box, parameters[0].ParameterType);
						}
						else
						{
							// create a class for the parameters and stick them in there
							Type parameterWrapper = CreateParameterClass(mb, parameters);
							for (int pi = 0; pi < parameters.Length; pi++)
								mIL.Emit(OpCodes.Ldarg, pi + 1);
							mIL.Emit(OpCodes.Newobj, parameterWrapper.GetConstructors()[0]);
						}

						break;

					case "withGraph":
						// if the defaultgraph attribute is on the method, use that
						if (graphAttribute != null)
						{
							mIL.Emit(OpCodes.Ldtoken, graphAttribute.GraphTypes[0]);
							mIL.Emit(OpCodes.Call, _typeGetTypeFromHandle);
						}
						else
							mIL.Emit(OpCodes.Ldnull);
						break;

					case "withGraphs":
						// if the defaultgraph attribute is on the method, use that
						if (graphAttribute != null)
						{
							// need to pass in the GraphTypes array
							// this way copies the graph attribute onto our new method and then pulls it out at runtime
							// i haven't found a good way to embed the types into the dynamic assembly
							// type references seem to get lost if you convert to a handle via ldtoken and then back with type.gettype
							// this method serializes the types into attributes, and then they pop out on the other side
							CustomAttributeBuilder cab = new CustomAttributeBuilder(typeof(DefaultGraphAttribute).GetConstructor(new Type[] { typeof(Type[]) }), new object[] { graphAttribute.GraphTypes });
							m.SetCustomAttribute(cab);
							mIL.Emit(OpCodes.Ldtoken, m);
							mIL.Emit(OpCodes.Call, typeof(InterfaceGeneratorHelper).GetMethod("GetGraphTypesFromMethodHandle", BindingFlags.Static | BindingFlags.Public));
						}
						else
							mIL.Emit(OpCodes.Ldnull);
						break;

					case "commandType":
						IlHelper.EmitLdInt32(mIL, (sqlAttribute != null) ? (int)sqlAttribute.CommandType : (int)CommandType.StoredProcedure);
						break;

					case "commandBehavior":
						IlHelper.EmitLdInt32(mIL, (int)CommandBehavior.Default);
						break;

					case "closeConnection":
						IlHelper.EmitLdInt32(mIL, (int)0);
						break;

					case "commandTimeout":
						mIL.Emit(OpCodes.Ldloca_S, (int)0);
						mIL.Emit(OpCodes.Initobj, typeof(int?));
						mIL.Emit(OpCodes.Ldloc_0);
						break;

					case "transaction":
						mIL.Emit(OpCodes.Ldnull);
						break;

					case "cancellationToken":
						mIL.Emit(OpCodes.Ldloca_S, (int)1);
						mIL.Emit(OpCodes.Initobj, typeof(CancellationToken?));
						mIL.Emit(OpCodes.Ldloc_1);
						break;

					default:
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot determine how to generate parameters for interfaceMethod {0}", executeMethod.Name));
				}
			}

			// call the execute method
			mIL.Emit(OpCodes.Call, executeMethod);

			// if the method returns void, throw away the return value
			if (interfaceMethod.ReturnType == typeof(void))
				mIL.Emit(OpCodes.Pop);

			mIL.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Determines the proper connection extension method to call to implement the interface method.
		/// </summary>
		/// <param name="method">The interface method to analyze.</param>
		/// <returns>The extension method that can implement the given method.</returns>
		private static MethodInfo GetExecuteMethod(MethodInfo method)
		{
			// if returntype is null, then we call Execute
			if (method.ReturnType == typeof(void))
				return _executeMethod;

			// if returntype is IList<T>, then we call Query
			if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(IList<>))
				return _queryMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0]);

			// if returntype is Results<T>, then we call QueryResults
			if (method.ReturnType.IsSubclassOf(typeof(Results)))
				return _queryResultsMethod.MakeGenericMethod(method.ReturnType);

			// if returntype is Task, then we call ExecuteAsync
			if (method.ReturnType == typeof(Task))
				return _executeAsyncMethod;

			// if the returntype is Task<T>, look a little deeper
			if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
			{
				// get the inside of the task
				var taskResultType = method.ReturnType.GetGenericArguments()[0];

				// if returntype is Task<IList<T>>, then we call QueryAsync
				if (taskResultType.IsGenericType && taskResultType.GetGenericTypeDefinition() == typeof(IList<>))
					return _queryAsyncMethod.MakeGenericMethod(taskResultType.GetGenericArguments()[0]);

				// if returntype is Task<Results<T>>, then then we call QueryResultsAsync
				if (taskResultType.IsSubclassOf(typeof(Results)))
					return _queryResultsAsyncMethod.MakeGenericMethod(taskResultType);

				// if returntype is Task<T>, and T is an atomic object, then we call ExecuteScalarAsync
				if (TypeHelper.IsAtomicType(taskResultType))
					return _executeScalarAsyncMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0]);

				// if returntype if Task<T> and T is not an atomic object, then we call SingleAsync
				return _singleAsyncMethod.MakeGenericMethod(taskResultType);
			}

			// if returntype is an atomic object, then we call ExecuteScalar
			if (TypeHelper.IsAtomicType(method.ReturnType))
				return _executeScalarMethod.MakeGenericMethod(method.ReturnType);

			// if returntype is not an atomic object, then we call Single
			return _singleMethod.MakeGenericMethod(method.ReturnType);
		}

		/// <summary>
		/// Creates an anonymous class to hold the parameters to the execute call.
		/// This allows us to pass in the exact same objects as in our dynamic methods.
		/// </summary>
		/// <param name="mb">The ModuleBuilder to append to.</param>
		/// <param name="parameters">The list of parameters to capture.</param>
		/// <returns>An anonymous wrapper class for the parameters.</returns>
		private static Type CreateParameterClass(ModuleBuilder mb, IEnumerable<ParameterInfo> parameters)
		{
			// TODO: handle/filter-out special parameters

			// if there are no parameters, then there is no need to create a type
			if (!parameters.Any())
				return null;

			// create a new class
			TypeBuilder tb = mb.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);

			// create a constructor for the class that takes all of the parameters
			ConstructorBuilder ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameters.Select(p => p.ParameterType).ToArray());
			ILGenerator ctorIL = ctor.GetILGenerator();

			// the constructor just copies the parameters into the internal fields
			int i = 1;
			foreach (ParameterInfo p in parameters)
			{
				FieldBuilder fb = tb.DefineField(p.Name, p.ParameterType, FieldAttributes.Private);

				ctorIL.Emit(OpCodes.Ldarg_0);			// this
				ctorIL.Emit(OpCodes.Ldarg, (int)i);		// parameter (in order)
				ctorIL.Emit(OpCodes.Stfld, fb);			// store to field

				i++;
			}

			ctorIL.Emit(OpCodes.Ret);

			return tb.CreateType();
		}
	}
}
