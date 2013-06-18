using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Reliable;

namespace Insight.Database.Providers
{
	class ReliableInsightDbProvider : WrappedInsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command is ReliableCommand;
		}

		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			ReliableCommand reliableCommand = command as ReliableCommand;
			return reliableCommand.InnerCommand;
		}
	}
}
