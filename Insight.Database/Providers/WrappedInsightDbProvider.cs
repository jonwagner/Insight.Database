using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for wrapped connections.
	/// </summary>
	/// <remarks>
	/// For connections that wrap commands in other commands, 
	/// this class unwraps the command when Insight needs to access advanced features.
	/// </remarks>
	public abstract class WrappedInsightDbProvider : InsightDbProvider
	{
		public abstract IDbCommand GetInnerCommand(IDbCommand command);

		public override IList<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).DeriveParameters(command);
		}

		public override IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).CreateTableValuedParameter(command, parameterName, tableTypeName);
		}

		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).GetTableTypeSchema(command, tableTypeName);
		}
	}
}
