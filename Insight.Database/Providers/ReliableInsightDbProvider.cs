using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Reliable;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for Reliable connections.
	/// </summary>
	class ReliableInsightDbProvider : WrappedInsightDbProvider
	{
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(ReliableCommand);
			}
		}

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			return reliableCommand.InnerCommand;
		}
	}
}
