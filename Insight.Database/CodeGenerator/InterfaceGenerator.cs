﻿using System;
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
		private static readonly Type[] _idbConnectionParameterTypes = new Type[] { typeof(IDbConnection) };
		private static readonly Type[] _executeParameterTypes = new Type[]
		{
				typeof(IDbConnection), typeof(string), typeof(object), typeof(CommandType), typeof(bool), typeof(int?), typeof(IDbTransaction), typeof(object)
		};

		private static readonly Type[] _queryParameterTypes = new Type[]
		{
				typeof(IDbConnection), typeof(string), typeof(object), typeof(Type), typeof(CommandType), typeof(CommandBehavior), typeof(int?), typeof(IDbTransaction), typeof(object)
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
		private static readonly MethodInfo _insertMethod = typeof(DBConnectionExtensions).GetMethod("Insert");
		private static readonly MethodInfo _insertListMethod = typeof(DBConnectionExtensions).GetMethod("InsertList");

		private static readonly MethodInfo _executeAsyncMethod = typeof(AsyncExtensions).GetMethods().First(mi => mi.Name == "ExecuteAsync");
		private static readonly MethodInfo _executeScalarAsyncMethod = typeof(AsyncExtensions).GetMethods().First(mi => mi.Name == "ExecuteScalarAsync");
		private static readonly MethodInfo _queryAsyncMethod = typeof(AsyncExtensions).GetMethod("QueryAsync", _queryAsyncParameterTypes);
		private static readonly MethodInfo _singleAsyncMethod = typeof(AsyncExtensions).GetMethod("SingleAsync", _queryAsyncParameterTypes);
		private static readonly MethodInfo _queryResultsAsyncMethod = typeof(AsyncExtensions).GetMethods().FirstOrDefault(mi => mi.Name == "QueryResultsAsync" && mi.GetGenericArguments().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(IDbConnection));
		private static readonly MethodInfo _insertAsyncMethod = typeof(AsyncExtensions).GetMethod("InsertAsync");
		private static readonly MethodInfo _insertListAsyncMethod = typeof(AsyncExtensions).GetMethod("InsertListAsync");

		private static readonly ConcurrentDictionary<Type, Func<IDbConnection, object>> _constructors = new ConcurrentDictionary<Type, Func<IDbConnection, object>>();
		#endregion

		/// <summary>
		/// Gets the implementor of the given interface for a DbConnection.
		/// </summary>
		/// <param name="connection">The connection to wrap.</param>
		/// <param name="interfaceType">The interface to implmement.</param>
		/// <returns>An implmementor of the given interface.</returns>
		public static object GetImplementorOf(IDbConnection connection, Type interfaceType)
		{
			if (!interfaceType.IsInterface)
				throw new ArgumentException("interfaceType must be an interface", "interfaceType");

			return _constructors.GetOrAdd(interfaceType, t => CreateImplementorOf(t))(connection);
		}

		/// <summary>
		/// Creates the implementor of the given interface for a DbConnection.
		/// </summary>
		/// <param name="interfaceType">The interface to implmement.</param>
		/// <returns>An implmementor of the given interface.</returns>
		private static Func<IDbConnection, object> CreateImplementorOf(Type interfaceType)
		{
			// create a new assembly
			AssemblyName an = Assembly.GetExecutingAssembly().GetName();
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
			ModuleBuilder mb = ab.DefineDynamicModule(an.Name);

			// create a type based on DbConnectionWrapper and call the default constructor
			TypeBuilder tb = mb.DefineType(interfaceType.FullName + "_Connection", TypeAttributes.Class, typeof(DbConnectionWrapper), new Type[] { interfaceType });

			// create a constructor for the type
			ConstructorBuilder ctor0 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, _idbConnectionParameterTypes);
			ILGenerator ctor0IL = ctor0.GetILGenerator();
			ctor0IL.Emit(OpCodes.Ldarg_0);
			ctor0IL.Emit(OpCodes.Ldarg_1);
			ctor0IL.Emit(OpCodes.Call, typeof(DbConnectionWrapper).GetConstructor(_idbConnectionParameterTypes));
			ctor0IL.Emit(OpCodes.Ret);

			// for each method on the interface, try to implement it with a call to the database
			foreach (MethodInfo interfaceMethod in interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
				EmitMethodImpl(mb, tb, interfaceMethod);

			// create a static create method that we can invoke directly as a delegate
			MethodBuilder m = tb.DefineMethod("Create", MethodAttributes.Static | MethodAttributes.Public, interfaceType, _idbConnectionParameterTypes);
			ILGenerator createIL = m.GetILGenerator();
			createIL.Emit(OpCodes.Ldarg_0);
			createIL.Emit(OpCodes.Newobj, ctor0);
			createIL.Emit(OpCodes.Ret);

			// create the type
			Type t = tb.CreateType();

			// return the create method
			return (Func<IDbConnection, object>)Delegate.CreateDelegate(typeof(Func<IDbConnection, object>), t.GetMethod("Create"));
		}

		/// <summary>
		/// Emits an implementation of a given method.
		/// </summary>
		/// <param name="mb">The ModuleBuilder to emit to.</param>
		/// <param name="tb">The TypeBuilder to emit to.</param>
		/// <param name="interfaceMethod">The interface method to implement.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static void EmitMethodImpl(ModuleBuilder mb, TypeBuilder tb, MethodInfo interfaceMethod)
		{
			// look at the parameters on the interface
			var parameters = interfaceMethod.GetParameters();
			var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

			// determine the proper method to call
			MethodInfo executeMethod = GetExecuteMethod(interfaceMethod);

			// see if the interface method has inlined SQL
			var sqlAttribute = interfaceMethod.GetCustomAttributes(false).OfType<SqlAttribute>().FirstOrDefault();

            // see if the interface specifies a schema
		    var schemaAttribute = interfaceMethod.DeclaringType.GetCustomAttributes(false)
		        .OfType<SchemaAttribute>().FirstOrDefault();

			// see if the interface method has a graph defined
			var graphAttribute = interfaceMethod.GetCustomAttributes(false).OfType<DefaultGraphAttribute>().FirstOrDefault();

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

                            // prepend schema name if specified
						    if (schemaAttribute != null)
						        procName = schemaAttribute.Name.Trim() + "." + procName;

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
							Type parameterWrapperType = CreateParameterClass(mb, parameters);
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

					case "withGraph":
						if (EmitSpecialParameter(mIL, "withGraph", parameters, executeParameters))
							break;

						// if the defaultgraph attribute is on the method, use that
						if (graphAttribute != null)
							mIL.EmitLoadType(graphAttribute.GraphTypes[0]);
						else
							mIL.Emit(OpCodes.Ldnull);
						break;

					case "withGraphs":
						if (EmitSpecialParameter(mIL, "withGraphs", parameters, executeParameters))
							break;

						if (graphAttribute != null)
						{
							// if the defaultgraph attribute is on the method, use that
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
						if (EmitSpecialParameter(mIL, "commandType", parameters, executeParameters))
							break;

						IlHelper.EmitLdInt32(mIL, (sqlAttribute != null) ? (int)sqlAttribute.CommandType : (int)CommandType.StoredProcedure);
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

						var cancellationToken = mIL.DeclareLocal(typeof(CancellationToken?));
						mIL.Emit(OpCodes.Ldloca_S, cancellationToken);
						mIL.Emit(OpCodes.Initobj, typeof(CancellationToken?));
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
				if (type != null &&
					!TypeHelper.IsAtomicType(type) &&
					(method.Name.StartsWith("Insert", StringComparison.OrdinalIgnoreCase) ||
					method.Name.StartsWith("Upsert", StringComparison.OrdinalIgnoreCase)))
				{
					var enumerable = type.GetInterfaces().Union(new[] { type }).FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
					if (enumerable != null)
						return _insertListMethod.MakeGenericMethod(enumerable.GetGenericArguments()[0]);

					return _insertMethod.MakeGenericMethod(type);
				}

				// not an insert, so call execute
				return _executeMethod;
			}

			// if returntype is IList<T>, then we call Query
			if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(IList<>))
				return _queryMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0]);

			// if returntype is Results<T>, then we call QueryResults
			if (method.ReturnType.IsSubclassOf(typeof(Results)))
				return _queryResultsMethod.MakeGenericMethod(method.ReturnType);

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
			return _singleMethod.MakeGenericMethod(method.ReturnType);
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
				if (type != null &&
					!TypeHelper.IsAtomicType(type) &&
					(method.Name.StartsWith("Insert", StringComparison.OrdinalIgnoreCase) ||
					method.Name.StartsWith("Upsert", StringComparison.OrdinalIgnoreCase)))
				{
					var enumerable = type.GetInterfaces().Union(new[] { type }).FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
					if (enumerable != null)
						return _insertListAsyncMethod.MakeGenericMethod(enumerable.GetGenericArguments()[0]);

					return _insertAsyncMethod.MakeGenericMethod(type);
				}

				// just a task, call ExecuteAsync
				return _executeAsyncMethod;
			}

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

			return null;
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
			// if there are no parameters, then there is no need to create a type
			if (!parameters.Any())
				return null;

			// create a new class
			TypeBuilder tb = mb.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class);

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

				ctorIL.Emit(OpCodes.Ldarg_0);					// this
				ctorIL.Emit(OpCodes.Ldarg, p.Position + 1);		// parameter (in order)
				if (p.ParameterType.IsByRef)					// dereference the pointer if necessary
					ctorIL.Emit(OpCodes.Ldobj, parameterType);
				ctorIL.Emit(OpCodes.Stfld, fb);					// store to field
			}

			ctorIL.Emit(OpCodes.Ret);

			return tb.CreateType();
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
				case "withGraph":
				case "withGraphs":
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
	}
}
