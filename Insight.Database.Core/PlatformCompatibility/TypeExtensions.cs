using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Insight.Database
{
    /// <summary>
    /// Provides stub methods for reflection in some framework versions.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "This class is an implementation wrapper.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
    static class TypeExtensions
    {
#if NETSTANDARD1_5
		public static byte[] GetBuffer(this MemoryStream stream) { return stream.ToArray(); }

		public static ConstructorInfo GetConstructor(this Type type, Type[] parameters) { return type.GetTypeInfo().GetConstructor(parameters); }
		public static ConstructorInfo[] GetConstructors(this Type type) { return type.GetTypeInfo().GetConstructors(); }
		public static ConstructorInfo[] GetConstructors(this Type type, BindingFlags flags) { return type.GetTypeInfo().GetConstructors(flags); }
		public static object[] GetCustomAttributes(this Type type, bool inherit) { return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray(); }
		public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit) { return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray(); }
		public static object[] GetCustomAttributes(this MemberInfo memberInfo, bool inherit) { return ((ICustomAttributeProvider)memberInfo).GetCustomAttributes(inherit); }
		public static FieldInfo GetField(this Type type, string name) { return type.GetTypeInfo().GetField(name); }
		public static FieldInfo GetField(this Type type, string name, BindingFlags flags) { return type.GetTypeInfo().GetField(name, flags); }
		public static FieldInfo[] GetFields(this Type type) { return type.GetTypeInfo().GetFields(); }
		public static FieldInfo[] GetFields(this Type type, BindingFlags flags) { return type.GetTypeInfo().GetFields(flags); }
		public static Type[] GetGenericArguments(this Type type) { return type.GetTypeInfo().GetGenericArguments(); }
		public static Type[] GetInterfaces(this Type type) { return type.GetTypeInfo().GetInterfaces(); }
		public static MethodInfo GetMethod(this Type type, string name) { return type.GetTypeInfo().GetMethod(name); }
		public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags) { return type.GetTypeInfo().GetMethod(name, flags); }
		public static MethodInfo GetMethod(this Type type, string name, Type[] parameters) { return type.GetTypeInfo().GetMethod(name, parameters); }
		public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags, Type[] parameters) { return type.GetMethod(name, flags, null, parameters, null); }
		public static PropertyInfo GetProperty(this Type type, string name) { return type.GetTypeInfo().GetProperty(name); }
		public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags) { return type.GetTypeInfo().GetProperties(flags); }
		public static MethodInfo[] GetMethods(this Type type) { return type.GetTypeInfo().GetMethods(); }
		public static MethodInfo[] GetMethods(this Type type, BindingFlags flags) { return type.GetTypeInfo().GetMethods(flags); }
		public static bool IsAssignableFrom(this Type type, Type otherType) { return type.GetTypeInfo().IsAssignableFrom(otherType); }
		public static bool IsSubclassOf(this Type type, Type otherType) { return type.GetTypeInfo().IsSubclassOf(otherType); }

		public static ConstructorInfo GetConstructor(this Type type, BindingFlags flags, object binder, Type[] parameterTypes, object parameterModifiers)
		{
			if (binder != null) throw new ArgumentException("binder");
			if (parameterModifiers != null) throw new ArgumentException("parameterModifiers");

			foreach (var constructor in type.GetConstructors(flags))
			{
				if (SignaturesMatch(parameterTypes, constructor.GetParameters()))
					return constructor;
			}

			return null;
		}

		public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags, object binder, Type[] parameterTypes, object parameterModifiers)
		{
			if (binder != null) throw new ArgumentException("binder");
			if (parameterModifiers != null) throw new ArgumentException("parameterModifiers");

			foreach (var method in type.GetMethods(flags))
			{
				if (method.Name != name)
					continue;

				if (SignaturesMatch(parameterTypes, method.GetParameters()))
					return method;
			}

			throw new InvalidOperationException("Cannot find the desired constructor");
		}

        private static bool SignaturesMatch(Type[] passedTypes, ParameterInfo[] methodParameters)
        {
			if (passedTypes.Length != methodParameters.Length)
				return false;

			for (int i = 0; i < passedTypes.Length; i++)
			{
				if (!CanConvert(passedTypes[i], methodParameters[i].ParameterType))
					return false;
			}

			return true;
        }

		private static bool CanConvert(Type fromType, Type toType)
		{
			if (toType.IsAssignableFrom(fromType))
				return true;

			var converter = System.ComponentModel.TypeDescriptor.GetConverter(fromType);

			return converter.CanConvertTo(toType);
		}
#endif
    }
}
