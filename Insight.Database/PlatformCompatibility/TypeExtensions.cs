using System;

namespace Insight.Database.PlatformCompatibility
{

#if NET35 || NET40

	/// <summary>
	/// Provides Type.GetType() to .Net 3.5 / 4.0
	/// </summary>
	internal static class TypeExtensions
	{
		internal static Type GetTypeInfo(this Type type)
		{
			return type;
		}
	}

#else
	// For .Net 4.5+, use the existing .Net Extension method that returns a TypeInfo
#endif

}


