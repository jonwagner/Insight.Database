using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Tests.Cases;
using NUnit.Framework;

namespace Insight.Tests
{
	[TestFixture]
	public class SyncExecuteTests : BaseTest
	{
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var recordCount = c.ExecuteSql("SELECT @p", new { p = 1 });

				Assert.AreEqual(-1, recordCount);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;
				var recordCount = c.ExecuteSql("SELECT @p", new { p = 1 }, closeConnection: true);

				Assert.AreEqual(ConnectionState.Closed, c.State);
				if (wasOpen)
					c.Open();
			});
		}

		[Test]
		public void TestOutputParameters()
		{
			var input = new InOutParameters { In = 5 };
			var output = new OutParameters();

			Connection().Execute(InOutParameters.ProcName, input, outputParameters: output);

			input.Verify(output);
		}
	}
}
