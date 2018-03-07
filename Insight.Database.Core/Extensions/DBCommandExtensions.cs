using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Reliable;
using Insight.Database.Structure;

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

			InsightDbProvider.For(cmd).FixupCommand(cmd);
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
			return (dynamic)command.OutputParameters<FastExpando>();
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

			InsightDbProvider.For(command).FixupOutputParameters(command);

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
            {
                DbParameterGenerator.GetOutputParameterConverter(command, result.GetType())(command, result);
            }

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
		/// <param name="outputParameters">An object to write output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<FastExpando> Query(
			this IDbCommand command,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			object outputParameters = null)
		{
			return command.Query(ListReader<FastExpando>.Default, commandBehavior, outputParameters);
		}

		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command should be initialized before calling this method.
		/// </summary>
		/// <typeparam name="T">The type of object to return in the result set.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="outputParameters">An object to write output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static IList<T> Query<T>(
			this IDbCommand command,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			object outputParameters = null)
		{
			return command.Query(ListReader<T>.Default, commandBehavior, outputParameters);
		}

		/// <summary>
		/// Execute an existing command, and translate the result set. This method supports auto-open.
		/// The Connection property of the command must be initialized before calling this method.
		/// </summary>
		/// <typeparam name="T">The type of result object to return. This must derive from Results.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="returns">The reader to use to read the object from the stream.</param>
		/// <param name="commandBehavior">The behavior of the command when executed.</param>
		/// <param name="outputParameters">An object to write output parameters onto.</param>
		/// <returns>A data reader with the results.</returns>
		public static T Query<T>(
			this IDbCommand command,
			IQueryReader<T> returns,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			object outputParameters = null)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (returns == null) throw new ArgumentNullException("returns");

			return command.Connection.ExecuteAndAutoClose(
				c => command,
				(cmd, r) =>
				{
					var results = returns.Read(cmd, r);
					cmd.OutputParameters(outputParameters);

					return results;
				},
				commandBehavior);
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// Outputs the output parameters onto two objects.
		/// </summary>
		/// <typeparam name="T">The first type of object.</typeparam>
		/// <typeparam name="T2">The second type of object.</typeparam>
		/// <param name="command">The command.</param>
		/// <param name="result">The first object.</param>
		/// <param name="result2">The second object.</param>
		internal static void OutputParameters<T, T2>(this IDbCommand command, T result, T2 result2) where T : class where T2 : class
		{
			command.OutputParameters(result);
			if (result != result2)
				command.OutputParameters(result2);
		}

		/// <summary>
		/// Will automatically close the underlying connection of the <see cref="IDbCommand"/> instance in context, if it is not currently closed.
		/// </summary>
		/// <param name="command">The command in context.</param>
		internal static void EnsureIsClosed(this IDbCommand command)
		{
			if (command == null)
			{
				return;
			}

			command.Connection.EnsureIsClosed();
		}

		/// <summary>
		/// Lets us call QueryCore into a simple delegate for dynamic calls.
		/// </summary>
		/// <typeparam name="T">The type of object returned.</typeparam>
		/// <param name="command">The command to execute.</param>
		/// <param name="returns">The definition of the return structure.</param>
		/// <param name="commandBehavior">The commandBehavior to use.</param>
		/// <param name="outputParameters">Optional output parameters.</param>
		/// <returns>The result of the query.</returns>
		private static T QueryCoreUntyped<T>(
			this IDbCommand command,
			IQueryReader returns,
			CommandBehavior commandBehavior = CommandBehavior.Default,
			object outputParameters = null)
		{
			// this method lets us convert QueryCore to a delegate for dynamic calls
			return command.Query<T>((IQueryReader<T>)returns, commandBehavior, outputParameters);
		}
		#endregion
	}
}
