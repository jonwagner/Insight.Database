using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph
	{
		#region Properties
		/// <summary>
		/// Represents a null graph type.
		/// </summary>
		public static readonly Type Null = null;

		/// <summary>
		/// Represents a null array of graphs.
		/// </summary>
		public static readonly Type[] Nulls = null;
		#endregion

		#region Methods
		/// <summary>
		/// Converts a single graph into an array of graphs.
		/// </summary>
		/// <param name="withGraph">The graph to convert.</param>
		/// <returns>An array containing the graph.</returns>
		internal static Type[] ToArray(Type withGraph)
		{
			if (withGraph == null)
				return null;

			return new Type[] { withGraph };
		}

		/// <summary>
		/// Gets a onetoone mapping from a graph.
		/// </summary>
		/// <typeparam name="T">The type of object being read.</typeparam>
		/// <param name="withGraph">The graph definition.</param>
		/// <param name="callback">An optional assembly callback.</param>
		/// <param name="idColumns">An optional ID column mapping.</param>
		/// <returns>A RecordReader that can read objects of the give graph.</returns>
		internal static OneToOne<T> GetOneToOne<T>(Type withGraph, Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{

			// if there is no graph, then just call the base
			if (withGraph == null)
			{
				Action<T> handler = null;
				if (callback != null)
					handler = (T t1) => callback(new object[] { t1 });
				return new OneToOne<T>(handler, null, idColumns);
			}

			// we have a graph, so instantiate an instance of it, and tell it to convert to a onetoone mapping
			var graph = (Graph<T>)System.Activator.CreateInstance(withGraph);
			return graph.GetOneToOneMapping(callback, idColumns);
		}

		/// <summary>
		/// Gets a queryreader from a graph array.
		/// </summary>
		/// <param name="resultsType">The expected return type.</param>
		/// <param name="withGraphs">An optional set of graphs.</param>
		/// <returns>A query reader for the graph definitions.</returns>
		internal static IQueryReader GetDefinitionFromGraphArray(Type resultsType, Type[] withGraphs)
		{
			// we get Results<T1, ...>, we need an array of [T1, ...]
			var types = resultsType.GetGenericArguments();

			// if there are no graphs, then we can just return Results[T1...]
			if (withGraphs == null)
				return (IQueryReader)GetResultsType(types).GetField("Default").GetValue(null);

			// we have graphs. let's make sure the graphs array is the same size as our types
			if (withGraphs.Length < types.Length)
			{
				var temp = new Type[types.Length];
				Array.Copy(withGraphs, temp, withGraphs.Length);
				withGraphs = temp;
			}

			// now go through all of the types and build the results reader
			object def = null;

			for (int i = 0; i < types.Length; i++)
			{
				// get a onetoone for the graph or just for the type
				var graphType = (withGraphs[i] != null) ? withGraphs[i].GetGenericArguments() : new Type[1] { types[i] };
				var oneToOne = GetOneToOneType(graphType).GetField("Records").GetValue(null);

				if (def == null)
				{
					var returns = typeof(Query)
						.GetMethods(BindingFlags.Public | BindingFlags.Static)
						.First(m => m.Name == "ReturnsResults")
						.MakeGenericMethod(types.Take(i + 1).ToArray());
					def = returns.Invoke(null, new object[] { oneToOne });
				}
				else
				{
					var then = typeof(Query)
						.GetMethods(BindingFlags.Public | BindingFlags.Static)
						.First(m => m.Name == "Then" && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType.Name.StartsWith("IQueryReader") && m.GetGenericArguments().Length == i + 1)
						.MakeGenericMethod(types.Take(i + 1).ToArray());
					def = (IQueryReader)then.Invoke(null, new object[] { def, oneToOne });
				}
			}

			return (IQueryReader)def;
		}
		#endregion

		#region OneToOne Methods
		/// <summary>
		/// Gets the onetoone mapping type for a list of types.
		/// This method is not intended to be used by user code.
		/// </summary>
		/// <param name="types">The list of types to convert.</param>
		/// <returns>The OneToOne mapping.</returns>
		[SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine")]
		private static Type GetOneToOneType(Type[] types)
		{
			if (types == null) throw new ArgumentNullException("types");

			Type oneToOne = null;
			switch (types.Length)
			{
				case 1: oneToOne = typeof(OneToOne<>); break;
				case 2: oneToOne = typeof(OneToOne<,>); break;
				case 3: oneToOne = typeof(OneToOne<,,>); break;
				case 4: oneToOne = typeof(OneToOne<,,,>); break;
				case 5: oneToOne = typeof(OneToOne<,,,,>); break;
				case 6: oneToOne = typeof(OneToOne<,,,,,>); break;
				case 7: oneToOne = typeof(OneToOne<,,,,,,>); break;
				case 8: oneToOne = typeof(OneToOne<,,,,,,,>); break;
				case 9: oneToOne = typeof(OneToOne<,,,,,,,,>); break;
				case 10: oneToOne = typeof(OneToOne<,,,,,,,,,>); break;
				case 11: oneToOne = typeof(OneToOne<,,,,,,,,,,>); break;
				case 12: oneToOne = typeof(OneToOne<,,,,,,,,,,,>); break;
				case 13: oneToOne = typeof(OneToOne<,,,,,,,,,,,,>); break;
				case 14: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,>); break;
				case 15: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,,>); break;
				case 16: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,,,>); break;
			}

			return oneToOne.MakeGenericType(types);
		}

		/// <summary>
		/// Gets the set of types to the appropriate results class.
		/// This method is not intended to be used by user code.
		/// </summary>
		/// <param name="types">The list of types to convert.</param>
		/// <returns>The Results class.</returns>
		[SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine")]
		private static Type GetResultsType(Type[] types)
		{
			if (types == null) throw new ArgumentNullException("types");

			Type results = null;
			switch (types.Length)
			{
				case 1: results = typeof(Results<>); break;
				case 2: results = typeof(Results<,>); break;
				case 3: results = typeof(Results<,,>); break;
				case 4: results = typeof(Results<,,,>); break;
				case 5: results = typeof(Results<,,,,>); break;
				case 6: results = typeof(Results<,,,,,>); break;
				case 7: results = typeof(Results<,,,,,,>); break;
				case 8: results = typeof(Results<,,,,,,,>); break;
				case 9: results = typeof(Results<,,,,,,,,>); break;
				case 10: results = typeof(Results<,,,,,,,,,>); break;
				case 11: results = typeof(Results<,,,,,,,,,,>); break;
				case 12: results = typeof(Results<,,,,,,,,,,,>); break;
				case 13: results = typeof(Results<,,,,,,,,,,,,>); break;
				case 14: results = typeof(Results<,,,,,,,,,,,,,>); break;
				case 15: results = typeof(Results<,,,,,,,,,,,,,,>); break;
				case 16: results = typeof(Results<,,,,,,,,,,,,,,,>); break;
			}

			return results.MakeGenericType(types);
		}
		#endregion
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T">The type of the root-level object in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T> : Graph
	{
		internal virtual OneToOne<T> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T> handler = null;
			if (callback != null)
				handler = (T t1) => callback(new object[] { t1 });

			return new OneToOne<T>(handler, null, idColumns);
		}

		/// <summary>
		/// Gets a list reader for the graph type.
		/// </summary>
		/// <returns>A list reader for the graph type.</returns>
		public ListReader<T> GetListReader()
		{
			return new ListReader<T>(GetOneToOneMapping());
		}
	}
}
