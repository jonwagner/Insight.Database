using Insight.Database.Reliable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	public class InsightDbProvider
	{
		static InsightDbProvider()
		{
			new SqlInsightDbProvider().Register();
			new ReliableInsightDbProvider().Register();
			new OdbcInsightDbProvider().Register();
			new OleDbInsightDbProvider().Register();
			new MiniProfilerInsightDbProvider().Register();
		}

		public void Register()
		{
			lock (Providers)
			{
				// we only need one provider of a given type
				if (Providers.Any(p => p.GetType() == GetType()))
					return;

				Providers.Add(this);
			}
		}

		public virtual bool SupportsCommand(IDbCommand command)
		{
			return false;
		}

		public virtual bool SupportsConnectionStringBuilder(DbConnectionStringBuilder builder)
		{
			return false;
		}

		public virtual DbConnection GetDbConnection()
		{
			throw new NotImplementedException();
		}

		public virtual List<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			throw new NotImplementedException();
		}

		public virtual IDbDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			throw new NotImplementedException();
		}

		public virtual IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			throw new NotImplementedException();
		}

		private static List<InsightDbProvider> Providers = new List<InsightDbProvider>();

		internal static InsightDbProvider For(IDbCommand command)
		{
			var provider = Providers.Where(p => p.SupportsCommand(command)).FirstOrDefault();
			if (provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of command.");

			return provider;
		}

		internal static InsightDbProvider For(DbConnectionStringBuilder builder)
		{
			var provider = Providers.Where(p => p.SupportsConnectionStringBuilder(builder)).FirstOrDefault();
			if (provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of connection string builder.");

			return provider;
		}
	}
}
