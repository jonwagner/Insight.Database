using System;
using System.Reflection;

namespace Insight.Database.PlatformCompatibility
{

#if NET35 || NET40

	/// <summary>
	/// Provides Type.GetType() to .Net 3.5 / 4.0
	/// </summary>
	internal static class TypeExtensionsLegacy
	{
		internal static Type GetTypeInfo(this Type type)
		{
			return type;
		}

	}
#endif

#if NETCORE
	//#else

	/// <summary>
	/// For .Net 4.5+, ideally use the .Net Extension method that returns a TypeInfo.
	/// These extension of Type keeps us from having to make a bunch up updates to Type.GetTypeInfo()
	/// (this approach does not work for props that were moved, because Extension Props are not a thing
	/// </summary>
	internal static class TypeExtensionsNetCore
	{

		internal static MethodInfo GetMethod(this Type type, string name, System.Reflection.BindingFlags bindingAttr)
		{
			return type.GetTypeInfo().GetMethod(name, bindingAttr);
		}

		internal static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return type.GetTypeInfo().GetConstructor(bindingAttr, binder, types, modifiers);
		}

		internal static ConstructorInfo GetConstructor(this Type type, System.Type[] types)
		{
			return type.GetTypeInfo().GetConstructor(types);
		}

		internal static PropertyInfo GetProperty(this Type type, string name)
		{
			return type.GetTypeInfo().GetProperty(name);
		}

	}

#endif

}


