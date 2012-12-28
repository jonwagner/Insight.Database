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
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph
	{
		/// <summary>
		/// Represents a null graph type.
		/// </summary>
		public static readonly Type Null = null;
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

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="TSub4">The type of the fourth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3, TSub4> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="TSub4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="TSub5">The type of the fifth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="TSub4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="TSub5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="TSub6">The type of the sixth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5, TSub6> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="TSub4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="TSub5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="TSub6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="TSub7">The type of the seventh subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5, TSub6, TSub7> : Graph
	{
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	/// <typeparam name="TSub1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="TSub2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="TSub3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="TSub4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="TSub5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="TSub6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="TSub7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="TSub8">The type of the eighth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5, TSub6, TSub7, TSub8> : Graph
	{
	}
}
