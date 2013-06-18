using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	class MiniProfilerInsightDbProvider : WrappedInsightDbProvider
	{
		public override bool SupportsCommand(IDbCommand command)
		{
			return command.GetType().Name == "ProfiledDbCommand";
		}

		public override IDbCommand GetInnerCommand(IDbCommand command)
		{
			dynamic dynamicCommand = command;
			return dynamicCommand.InternalCommand;
		}
	}
}
