using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Insight.Database.CodeGenerator;

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
	}
}
