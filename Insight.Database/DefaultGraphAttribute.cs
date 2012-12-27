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
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultGraphAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the DefaultGraphAttribute class.
		/// </summary>
		/// <param name="graphType">The type of object graph to use.</param>
		public DefaultGraphAttribute(Type graphType)
		{
			if (typeof(Graph<>).IsSubclassOf(graphType))
				throw new ArgumentException("graphType must be of type Graph<T>", "graphType");

			GraphType = graphType;
		}

		/// <summary>
		/// Gets the object graph to use when deserializing the class.
		/// </summary>
		public Type GraphType { get; private set; }
	}
}