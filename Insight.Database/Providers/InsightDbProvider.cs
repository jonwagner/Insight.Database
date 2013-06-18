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
	/// <summary>
	/// Provides the infrastructure to access advanced features of database connections so Insight can do its magic.
	/// </summary>
	public class InsightDbProvider
	{
		private static List<InsightDbProvider> _providers = new List<InsightDbProvider>();
		private static Dictionary<Type, InsightDbProvider> _providerMap = new Dictionary<Type, InsightDbProvider>();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static InsightDbProvider()
		{
			new SqlInsightDbProvider().Register();
			new ReliableInsightDbProvider().Register();
			new OdbcInsightDbProvider().Register();
			new OleDbInsightDbProvider().Register();
		}

		public void Register()
		{
			lock (_providerMap)
			{
				if (CommandType != null)
					_providerMap[CommandType] = this;

				if (ConnectionStringBuilderType != null)
					_providerMap[ConnectionStringBuilderType] = this;
			}

			lock (_providers)
			{
				// we only need one provider of a given type
				if (_providers.Any(p => p.GetType() == GetType()))
					return;

				_providers.Add(this);
			}
		}

		public virtual Type CommandType
		{
			get
			{
				return null;
			}
		}

		public virtual Type ConnectionStringBuilderType
		{
			get
			{
				return null;
			}
		}

		public virtual DbConnection CreateDbConnection()
		{
			throw new NotImplementedException();
		}

		public virtual IList<IDbDataParameter> DeriveParameters(IDbCommand command)
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

		internal static InsightDbProvider For(object o)
		{
			InsightDbProvider provider;
			
			if (!_providerMap.TryGetValue(o.GetType(), out provider) || provider == null)
				throw new NotImplementedException("No Insight.Database provider supports the given type of command.");

			return provider;
		}
	}
}
