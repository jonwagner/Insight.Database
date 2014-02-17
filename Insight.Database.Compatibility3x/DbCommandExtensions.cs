using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Extends the DbCommand class with handy extensions.
	/// </summary>
	public static class DbCommandExtensions
	{
		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <typeparam name="T">The type of object to return in the result set.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static IList<T> Query<T>(
			this IDbCommand command,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default)
		{
			var queryReader = new ListReader<T>(Graph.GetOneToOne<T>(withGraph));
			return command.Query(queryReader, commandBehavior);
		}

		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command must be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="outputParameters">An object to write output parameters onto.</param>
		/// <typeparam name="T">The type of result object to return. This must derive from Results.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static T QueryResults<T>(
			this IDbCommand command,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			object outputParameters = null) where T : Results, new()
		{
			return command.Query<T>((IQueryReader<T>)Graph.GetDefinitionFromGraphArray(typeof(T), withGraphs), commandBehavior, outputParameters);
		}
	}
}
