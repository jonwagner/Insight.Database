using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// This class is not intended to be used by user code.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DesignRules", "CA1020:AvoidNamespacesWithFewTypes", Justification = "This is an internal class not intended to be used by user code.")]
	public static class InterfaceGeneratorHelper
	{
		/// <summary>
		/// A cache of runtime handles to graph types.
		/// </summary>
		private static ConcurrentDictionary<RuntimeMethodHandle, Type[]> _types = new ConcurrentDictionary<RuntimeMethodHandle, Type[]>();

		/// <summary>
		/// Returns the graph types for a method specified by a method handle.
		/// </summary>
		/// <param name="handle">The handle to the method.</param>
		/// <returns>The graph types for the method.</returns>
		public static Type[] GetGraphTypesFromMethodHandle(RuntimeMethodHandle handle)
		{
			return _types.GetOrAdd(
				handle,
				h =>
				{
					MethodInfo method = (MethodInfo)MethodInfo.GetMethodFromHandle(h);
					var graphAttribute = method.GetCustomAttributes(false).OfType<DefaultGraphAttribute>().FirstOrDefault();

					return graphAttribute.GraphTypes;
				});
		}
	}
}
