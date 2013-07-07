using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph
	{
		/// <summary>
		/// Represents a null graph type.
		/// </summary>
		public static readonly Type Null = null;

        /// <summary>
        /// Gets the first generic argument.
        /// </summary>
        /// <param name="graph">The graph type.</param>
        /// <returns>
        /// The type of the first generic argument, 
        /// or <c>null</c> if the <paramref name="graph"/> 
        /// does not inherit from <see cref="Graph"/> or 
        /// is not a generic type.
        /// </returns>
        internal static Type GetFirstGenericArgument(Type graph)
	    {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            Type[] types = GetGenericArguments(graph);
            if (types != null && types.Length != 0)
            {
                return types[0];
            }

	        return null;
	    }

	    internal static Type[] GetGenericArguments(Type graph)
	    {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            if (graph.IsSubclassOf(typeof(Graph)))
            {
                while (graph != null && !graph.IsGenericType)
                {
                    graph = graph.BaseType;
                }

                if (graph != null)
                {
                    Type[] types = graph.GetGenericArguments();
                    return types;
                }
            }
	        
            return null;
	    }
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1> : Graph
	{
	}
}
