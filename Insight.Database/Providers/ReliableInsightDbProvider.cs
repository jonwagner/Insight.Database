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
		public override Type CommandType
		{
			get
			{
				return typeof(ReliableCommand);
			}
		}

		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			return reliableCommand.InnerCommand;
		}
	}
}
