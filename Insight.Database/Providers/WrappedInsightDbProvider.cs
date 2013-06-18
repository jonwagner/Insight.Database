using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	public abstract class WrappedInsightDbProvider : InsightDbProvider
	{
		public abstract IDbCommand GetInnerCommand(IDbCommand command);

		public override List<IDbDataParameter> DeriveParameters(IDbCommand command)
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
