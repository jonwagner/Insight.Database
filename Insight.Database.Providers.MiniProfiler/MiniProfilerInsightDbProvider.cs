using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Data;

namespace Insight.Database.Providers.MiniProfiler
{
	/// <summary>
	/// Implements the Insight provider for MiniProfiler connections.
	/// </summary>
	public class MiniProfilerInsightDbProvider : WrappedInsightDbProvider
	{
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
		/// Registers this provider. This is generally not needed, unless you want to force an assembly reference to this provider.
		/// </summary>
		public static void RegisterProvider()
		{
			InsightDbProvider.RegisterProvider(new MiniProfilerInsightDbProvider());
		}

		/// <inheritdoc/>
		public override IDbConnection CloneDbConnection(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// clone the inner connection
			var innerConnection = GetInnerConnection(connection);
			var innerProvider = InsightDbProvider.For(innerConnection);
			var clonedInnerConnection = (DbConnection)innerProvider.CloneDbConnection(innerConnection);

			var profiledConnection = (ProfiledDbConnection)connection;
			return new ProfiledDbConnection(clonedInnerConnection, profiledConnection.Profiler);
		}

		/// <summary>
		/// Unwraps the given connection and returns the inner connection.
		/// </summary>
		/// <param name="connection">The outer connection.</param>
		/// <returns>The inner connection.</returns>
		public override IDbConnection GetInnerConnection(IDbConnection connection)
		{
			ProfiledDbConnection profiledConnection = (ProfiledDbConnection)connection;
			return profiledConnection.WrappedConnection;
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
