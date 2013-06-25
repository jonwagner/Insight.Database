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
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(ReliableConnection), typeof(ReliableCommand) };
			}
		}

		/// <summary>
		/// Unwraps the given connection and returns the inner command.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public override IDbConnection GetInnerConnection(IDbConnection connection)
		{
			ReliableConnection reliableConnection = (ReliableConnection)connection;
			return reliableConnection.InnerConnection;
		}

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			ReliableCommand reliableCommand = (ReliableCommand)command;
			return reliableCommand.InnerCommand;
		}
	}
}
