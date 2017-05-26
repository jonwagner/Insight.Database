using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if NETSTANDARD1_5
using System.Runtime.Loader;
#endif

namespace Insight.Database
{
	/// <summary>
	/// Compatibility methods for Assembly loading.
	/// </summary>
	class ApplicationHelpers
	{
		/// <summary>
		/// Returns a list of paths to search for assemblies.
		/// </summary>
		/// <returns>A list of patahs.</returns>
		internal static List<string> GetAssemblySearchPaths()
		{
			var paths = new List<string>();

#if NETSTANDARD1_5
			paths.Add(AppContext.BaseDirectory);
#else
			string relativeSearchPath = System.AppDomain.CurrentDomain.RelativeSearchPath ?? String.Empty;
			paths.AddRange(relativeSearchPath.Split(';').Select(p => Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, p)));
#endif
			return paths;
		}

		/// <summary>
		/// Loads an assembly from a file path.
		/// </summary>
		/// <param name="assemblyFilePath">The path to the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		internal static Assembly LoadAssembly(string assemblyFilePath)
		{
#if NETSTANDARD1_5
			Assembly assembly = new AssemblyLoader().LoadFromAssemblyPath(assemblyFilePath);
#else
			Assembly assembly = Assembly.LoadFrom(assemblyFilePath);
#endif
			return assembly;
		}

#if NETSTANDARD1_5
		class AssemblyLoader : AssemblyLoadContext
		{
			protected override Assembly Load(AssemblyName assemblyName)
			{
				return Assembly.Load(assemblyName);
			}
		}
#endif
	}
}
