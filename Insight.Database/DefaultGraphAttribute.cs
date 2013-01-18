using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Specified the default object graph to use when deserializing the class from a database.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultGraphAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the DefaultGraphAttribute class.
		/// </summary>
		/// <param name="graphType">The graph type to use.</param>
		public DefaultGraphAttribute(Type graphType)
		{
			GraphTypes = new Type[] { graphType };
		}

		/// <summary>
		/// Initializes a new instance of the DefaultGraphAttribute class.
		/// </summary>
		/// <param name="graphTypes">An array of object graphs to use.</param>
		public DefaultGraphAttribute(params Type[] graphTypes)
		{
			GraphTypes = graphTypes;
		}

		/// <summary>
		/// Gets the object graph to use when deserializing a set of results.
		/// </summary>
		internal Type[] GraphTypes { get; private set; }

		/// <summary>
		/// Gets the object graph to use when deserializing a set of results.
		/// </summary>
		/// <returns>The array of object graphs used for deserializing a set of results.</returns>
		public Type[] GetGraphTypes() { return (Type[])GraphTypes.Clone(); }
	}
}