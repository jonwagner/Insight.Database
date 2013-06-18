using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Data;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for MiniProfiler connections.
	/// </summary>
	public class MiniProfilerInsightDbProvider : WrappedInsightDbProvider
	{
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(ProfiledDbCommand);
			}
		}

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			ProfiledDbCommand profiledCommand = command as ProfiledDbCommand;
			return profiledCommand.InternalCommand;
		}
	}
}
