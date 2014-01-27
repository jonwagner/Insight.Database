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
	public class AsyncExecuteScalarTests : BaseTest
	{
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var parameters = new { p = 1 };
				var result = c.ExecuteScalarSqlAsync<int>("SELECT @p", parameters).Result;
				Assert.AreEqual(parameters.p, result);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;
				var recordCount = c.ExecuteScalarSqlAsync<int>("SELECT @p", new { p = 1 }, closeConnection: true).Result;
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

			var result = Connection().ExecuteScalarAsync<int>(InOutParameters.ProcName, input, outputParameters: output).Result;

			Assert.AreEqual(input.In, result);
			input.Verify(output);
		}
	}
}
