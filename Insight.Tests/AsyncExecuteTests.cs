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
	public class AsyncExecuteTests : BaseTest
	{
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var recordCount = c.ExecuteSqlAsync("SELECT @p", new { p = 1 }).Result;

				Assert.AreEqual(-1, recordCount);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;
				var recordCount = c.ExecuteSqlAsync("SELECT @p", new { p = 1 }, closeConnection: true).Result;

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

			Connection().ExecuteAsync(InOutParameters.ProcName, input, outputParameters: output).Wait();

			input.Verify(output);
		}
	}
}
