﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Tests.Cases;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
				ClassicAssert.AreEqual(parameters.p, result);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;
				var recordCount = c.ExecuteScalarSqlAsync<int>("SELECT @p", new { p = 1 }, closeConnection: true).Result;
				ClassicAssert.AreEqual(ConnectionState.Closed, c.State);
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

			ClassicAssert.AreEqual(input.In, result);
			input.Verify(output);
		}

		[Test]
		public void TestNullableReturn()
		{
			var result = Connection().ExecuteScalarSqlAsync<int?>("SELECT CAST(NULL as INT)").Result;

			ClassicAssert.AreEqual(null, result);
		}
	}
}
