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
using NUnit.Framework.Legacy;
namespace Insight.Tests
{
	[TestFixture]
	public class AsyncQueryReaderTests : BaseTest
	{
		#region Base Tests
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var result = c.QuerySqlAsync("SELECT @p", new { p = 1 }, reader => 1).Result;

				ClassicAssert.AreEqual(1, result);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;

				var result = c.QuerySqlAsync("SELECT @p", new { p = 1 }, reader => 1, commandBehavior: CommandBehavior.CloseConnection).Result;

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

			// notice here how we don't actually read the recordset, but the framework goes all the way to the end for us
			Connection().QueryAsync(InOutParameters.ProcName, input, reader => 1, outputParameters: output).Wait();

			input.Verify(output);
		}
		#endregion

		[Test]
		public void TestReadWithoutResults()
		{
			IList<Beer> results = null;
			Connection().QueryAsync(Beer.SelectAllProc, null, reader => { results = reader.ToList<Beer>(); }).Wait();

			Beer.VerifyAll(results);
		}

		[Test]
		public void TestReadList()
		{
			var results = Connection().QueryAsync(Beer.SelectAllProc, null, reader => reader.ToList<Beer>()).Result;

			Beer.VerifyAll(results);
		}
	}
}