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
		/// Prevents a default instance of the <see cref="MiniProfilerInsightDbProvider"/> class from being created.
		/// </summary>
		private MiniProfilerInsightDbProvider()
		{
		}

		/// <summary>
		/// Gets the types of objects that this provider supports.
		/// Include connectionstrings, connections, commands, and readers.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(ProfiledDbConnection), typeof(ProfiledDbCommand) };
			}
		}

		/// <summary>
		/// Registers the Oracle Provider
		/// </summary>
		public static void RegisterProvider()
		{
			new MiniProfilerInsightDbProvider().Register();
		}

		/// <summary>
		/// Unwraps the given connection and returns the inner connection.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public override IDbConnection GetInnerConnection(IDbConnection connection)
		{
			ProfiledDbConnection profiledConnection = (ProfiledDbConnection)connection;
			return profiledConnection.InnerConnection;
		}

		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			ProfiledDbCommand profiledCommand = (ProfiledDbCommand)command;
			return profiledCommand.InternalCommand;
		}
	}
}
