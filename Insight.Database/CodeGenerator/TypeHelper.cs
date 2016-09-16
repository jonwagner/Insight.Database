﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
#if NET35 || NET40
using Insight.Database.PlatformCompatibility;
#endif

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Helper methods for dealing with system types.
	/// </summary>
	static class TypeHelper
	{
		/// <summary>
		/// Initializes static members of the TypeHelper class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		static TypeHelper()
		{
			try
			{
				var assemblyName = new AssemblyName("System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
				LinqBinaryType = Assembly.Load(assemblyName).GetType("System.Data.Linq.Binary");
			}
			catch
			{
				LinqBinaryType = typeof(LinqBinaryPlaceHolder);
			}

			LinqBinaryCtor = LinqBinaryType.GetConstructor(new Type[] { typeof(byte[]) });
			LinqBinaryToArray = LinqBinaryType.GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance);
		}

		/// <summary>
		/// Gets the type object for Linq.Binary.
		/// </summary>
		internal static Type LinqBinaryType { get; private set; }

		/// <summary>
		/// Gets the constructor for Linq.Binary.
		/// </summary>
		internal static ConstructorInfo LinqBinaryCtor { get; private set; }

		/// <summary>
		/// Gets the ToArray method for Linq.Binary.
		/// </summary>
		internal static MethodInfo LinqBinaryToArray { get; private set; }

		/// <summary>
		/// Determines whether the given type is is an atomic type that does not have members.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if this is an atomic type that does not have members.</returns>
		public static bool IsAtomicType(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			// we convert nulls to dbnull. So if the type is nullable, then we look at the underlying type.
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type == typeof(string))	return true;
			if (type.GetTypeInfo().IsArray) return true;
			if (type.GetTypeInfo().IsEnum) return true;

			// anything marked as a user-defined type is atomic for our purposes
			if (IsSqlUserDefinedType(type))
				return true;

			// value references are atomic
			if (type.GetTypeInfo().IsByRef)
				return true;

			// treat all references as non-atomic
			if (!type.GetTypeInfo().IsValueType)
				return false;

			// these are structures, but we want to treat them as atomic
			if (type == typeof(Decimal)) return true;
			if (type == typeof(DateTime)) return true;
			if (type == typeof(DateTimeOffset)) return true;
			if (type == typeof(Guid)) return true;
			if (type == typeof(TimeSpan)) return true;

			// all of the primitive types, array, etc. are atomic
			return type.GetTypeInfo().IsPrimitive;
		}

		/// <summary>
		/// Determines if a given DbType represents a string.
		/// </summary>
		/// <param name="dbType">The dbType to test.</param>
		/// <returns>True if the type is a string type.</returns>
		public static bool IsDbTypeAString(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Emits the code to load the default value of a type onto the stack.
		/// </summary>
		/// <param name="mIL">The ILGenerator to emit to.</param>
		/// <param name="type">The type of object to emit.</param>
		public static void EmitDefaultValue(ILGenerator mIL, Type type)
		{
			// if there is no type, then we don't need a value
			if (type == null || type == typeof(void))
				return;

			// for generics and values, init a local object with a blank object
			if (type.GetTypeInfo().IsGenericParameter || type.GetTypeInfo().IsValueType)
			{
				var returnValue = mIL.DeclareLocal(type);
				mIL.Emit(returnValue.LocalIndex < 256 ? OpCodes.Ldloca_S : OpCodes.Ldloca, returnValue);
				mIL.Emit(OpCodes.Initobj, type);
				mIL.Emit(OpCodes.Ldloc, returnValue);
			}
			else
				mIL.Emit(OpCodes.Ldnull);
		}

		/// <summary>
		/// Determines if a type is a sql user defined type.
		/// </summary>
		/// <param name="type">The type to examine.</param>
		/// <returns>True if it is a Sql UDT.</returns>
		public static bool IsSqlUserDefinedType(Type type)
		{
			return type.GetTypeInfo().GetCustomAttributes(true).Any(a => a.GetType().GetTypeInfo().Name == "SqlUserDefinedTypeAttribute");
		}

		#region Method Implementation Helpers
		/// <summary>
		/// Copy the generic attributes of a method.
		/// </summary>
		/// <param name="sourceMethod">The source method.</param>
		/// <param name="targetMethod">The target method.</param>
		public static void CopyGenericSignature(MethodInfo sourceMethod, MethodBuilder targetMethod)
		{
			if (sourceMethod.IsGenericMethod)
			{
				// get the interface's generic types and make our own
				var oldTypes = sourceMethod.GetGenericArguments();
				var newTypes = targetMethod.DefineGenericParameters(oldTypes.Select(t => t.GetTypeInfo().Name).ToArray());

				for (int i = 0; i < newTypes.Length; i++)
				{
					Type oldType = oldTypes[i];
					GenericTypeParameterBuilder newType = newTypes[i];

					newType.SetGenericParameterAttributes(oldType.GetTypeInfo().GenericParameterAttributes);
					newType.SetInterfaceConstraints(oldType.GetTypeInfo().GetGenericParameterConstraints());
				}
			}
		}

		/// <summary>
		/// Copies the method signature from one method to another.
		/// This includes generic parameters, constraints and parameters.
		/// </summary>
		/// <param name="sourceMethod">The source method.</param>
		/// <param name="targetMethod">The target method.</param>
		public static void CopyMethodSignature(MethodInfo sourceMethod, MethodBuilder targetMethod)
		{
			CopyGenericSignature(sourceMethod, targetMethod);

			targetMethod.SetReturnType(sourceMethod.ReturnType);

			// copy the parameters and attributes
			// it seems that we can use the source parameters directly because the target method is derived
			// from the source method
			var parameters = sourceMethod.GetParameters();
			targetMethod.SetParameters(parameters.Select(p => p.ParameterType).ToArray());

			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i];
				targetMethod.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
			}
		}
		#endregion

		#region LinqBinaryPlaceHolder
		/// <summary>
		/// Stand-in for Linq.Binary
		/// </summary>
		class LinqBinaryPlaceHolder
		{
		}
		#endregion
	}
}
