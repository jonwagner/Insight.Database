using Sybase.Data.AseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers.Sybase
{
    public class SybaseInsightDbProvider : InsightDbProvider
    {
		/// <summary>
		/// The list of types supported by this provider.
		/// </summary>
		private static Type[] _supportedTypes = new Type[]
		{
			typeof(AseConnectionStringBuilder), typeof(AseConnection), typeof(AseCommand), typeof(AseDataReader), typeof(AseException)
		};

    }
}
