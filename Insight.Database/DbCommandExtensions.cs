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
			IDictionary<string, object> dictionary = new FastExpando();

			foreach (IDataParameter p in command.Parameters)
			{
				if (p.Direction.HasFlag(ParameterDirection.Output))
					dictionary[p.ParameterName] = p.Value;
			}

			return dictionary;
		}

		/// <summary>
		/// Takes the output parameters of a result set and inserts them into the result object.
		/// </summary>
		/// <param name="command">The command to evaluate.</param>
		/// <param name="result">The result to insert into.</param>
		public static void OutputParameters(this IDbCommand command, object result)
		{
			// if there is no output object, don't attempt to fill it in
			if (result == null)
				return;

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
