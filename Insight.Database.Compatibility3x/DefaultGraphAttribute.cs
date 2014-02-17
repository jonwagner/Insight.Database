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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class DefaultGraphAttribute : RecordsetAttribute
	{
		/// <summary>
		/// Initializes a new instance of the DefaultGraphAttribute class.
		/// </summary>
		/// <param name="graphType">The graph type to use.</param>
		public DefaultGraphAttribute(Type graphType) : base(graphType.GetGenericArguments())
		{
		}

		/// <summary>
		/// Initializes a new instance of the DefaultGraphAttribute class.
		/// </summary>
		/// <param name="graphTypes">An array of object graphs to use.</param>
		public DefaultGraphAttribute(params Type[] graphTypes) : base(graphTypes)
		{
			if (graphTypes.Length > 1)
				throw new InvalidOperationException("DefaultGraph with more than one type is no longer supported. Use RecordsetAttribute.");
		}
	}
}