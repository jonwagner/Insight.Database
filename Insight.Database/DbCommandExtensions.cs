using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

			DbParameterGenerator.GetParameterGenerator(cmd, parameters.GetType())(cmd, parameters);
		}

		/// <summary>
		/// Unwraps an IDbCommand to determine its inner SqlCommand to use with advanced features.
		/// </summary>
		/// <param name="command">The command to unwrap.</param>
		/// <returns>The inner SqlCommand.</returns>
		private static SqlCommand UnwrapSqlCommand(this IDbCommand command)
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
