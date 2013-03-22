using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using Insight.Database.CodeGenerator;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for executing database commands.
	/// </summary>
	public static class DBCommandExtensions
	{
		#region Input Parameter Methods
		/// <summary>
		/// Add parameters to a given command.
		/// </summary>
		/// <param name="cmd">The command to add parameters to.</param>
		/// <param name="parameters">The object containing parameters to add.</param>
		public static void AddParameters(this IDbCommand cmd, object parameters = null)
		{
			// fill in a null parameter with empty parameter
			if (parameters == null)
				parameters = Parameters.Empty;

			DbParameterGenerator.GetInputParameterGenerator(cmd, parameters.GetType())(cmd, parameters);
		}
		#endregion

		#region Output Parameter Methods
		/// <summary>
		/// Takes the output parameters of a result set and inserts them into the result object.
		/// </summary>
		/// <param name="command">The command to evaluate.</param>
		/// <returns>A dynamic object containing the output parameters.</returns>
		public static dynamic OutputParameters(this IDbCommand command)
		{
			return command.OutputParameters<FastExpando>();
		}

		/// <summary>
		/// Takes the output parameters of a result set and inserts them into a new object of type T.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to evaluate.</param>
		/// <returns>An object of type T containing the output parameters.</returns>
		public static T OutputParameters<T>(this IDbCommand command) where T : new()
		{
			return command.OutputParameters(new T());
		}

		/// <summary>
		/// Takes the output parameters of a result set and inserts them into the result object.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to evaluate.</param>
		/// <param name="result">The result to insert into.</param>
		/// <returns>The object that was filled in.</returns>
		public static T OutputParameters<T>(this IDbCommand command, T result)
		{
			if (command == null) throw new ArgumentNullException("command");

			// if there is no output object, don't attempt to fill it in
			if (result == null)
				return result;

			if (result is DynamicObject)
			{
				// handle dynamic objects by assigning their properties right into the dictionary
				IDictionary<string, object> dictionary = result as IDictionary<string, object>;
				if (dictionary == null)
					throw new InvalidOperationException("Dynamic object must support IDictionary<string, object>.");

				foreach (IDataParameter p in command.Parameters)
				{
					if (p.Direction.HasFlag(ParameterDirection.Output))
						dictionary[p.ParameterName] = p.Value;
				}
			}
			else
				DbParameterGenerator.GetOutputParameterConverter(command, result.GetType())(command, result);

			return result;
		}
		#endregion

		#region Query Methods
		/// <summary>
		/// Execute an existing command, and translate the result set into a FastExpando. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<FastExpando> Query(
			this IDbCommand command,
			CommandBehavior commandBehavior = CommandBehavior.Default)
		{
			return command.Query<FastExpando>(Graph.Null, commandBehavior);
		}

		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraph">The object graph to use to deserialize the object or null to use the default graph.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <typeparam name="TResult">The type of object to return in the result set.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static IList<TResult> Query<TResult>(
			this IDbCommand command,
			Type withGraph = null,
			CommandBehavior commandBehavior = CommandBehavior.Default)
		{
			if (command == null) throw new ArgumentNullException("command");

			return command.Connection.ExecuteAndAutoClose(
				c => command,
				(cmd, r) => r.ToList<TResult>(withGraph),
				commandBehavior);
		}
		#endregion

		#region QueryResults Methods
		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command must be initialized before calling this method.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <typeparam name="T">The type of result object to return. This must derive from Results.</typeparam>
		/// <returns>A data reader with the results.</returns>
		public static T QueryResults<T>(
			this IDbCommand command,
			Type[] withGraphs = null,
			CommandBehavior commandBehavior = CommandBehavior.Default) where T : Results, new()
		{
			if (command == null) throw new ArgumentNullException("command");

			return command.Connection.ExecuteAndAutoClose(
				c => command,
				(cmd, r) =>
				{
					T results = new T();
					results.Read(cmd, r, withGraphs);

					return results;
				},
				commandBehavior);
		}
		#endregion

		/// <summary>
		/// Unwraps an IDbCommand to determine its inner SqlCommand to use with advanced features.
		/// </summary>
		/// <param name="command">The command to unwrap.</param>
		/// <returns>The inner SqlCommand.</returns>
		internal static SqlCommand UnwrapSqlCommand(this IDbCommand command)
		{
			// if we have a SqlCommand, use it
			SqlCommand sqlCommand = command as SqlCommand;
			if (sqlCommand != null)
				return sqlCommand;

			// if we have a reliable command, break it down
			ReliableCommand reliable = command as ReliableCommand;
			if (reliable != null)
				return reliable.InnerCommand.UnwrapSqlCommand();

			// if the command is not a SqlCommand, then maybe it is wrapped by something like MiniProfiler
			if (command.GetType().Name == "ProfiledDbCommand")
			{
				dynamic dynamicCommand = command;
				return UnwrapSqlCommand(dynamicCommand.InternalCommand);
			}

			return null;
		}
	}
}
