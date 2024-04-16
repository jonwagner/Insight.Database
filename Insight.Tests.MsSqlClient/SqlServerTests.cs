using Insight.Database;
using Insight.Tests.MsSqlClient.Cases;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Data;
using System.Linq;

namespace Insight.Tests.MsSqlClient
{
	[TestFixture]
	public partial class SqlServerTests : MsSqlClientBaseTest
	{
		public class SprocMetadata
		{
			public string Name;
		}

		[Test]
		public void TestSysProcs()
		{
			// Issue #498
			var results = Connection().QuerySql<SprocMetadata>("SELECT * FROM sys.objects");
			ClassicAssert.Greater(results.Count, 0);
			ClassicAssert.IsNotNull(results[0].Name);
		}
	}
}
