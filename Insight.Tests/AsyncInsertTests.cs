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
	public class AsyncInsertTests : BaseTest
	{
		#region Base Cases
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var input = new InOutParameters { In = 5 };
				var result = c.InsertSqlAsync("SELECT @In", input).Result;
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;

				var input = new InOutParameters { In = 5 };
				var recordCount = c.InsertSqlAsync("SELECT @In", input, commandBehavior: CommandBehavior.CloseConnection).Result;

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

			var result = Connection().InsertAsync(InOutParameters.ProcName, input, outputParameters: output).Result;

			input.Verify(output);
		}

		[Test]
		public void TestOutputParametersInList()
		{
			var list = new List<int>();
			var input = new InOutParameters { In = 5 };
			var output = new OutParameters();

			var result = Connection().InsertListAsync(InOutParameters.ProcName, list, input, outputParameters: output).Result;

			input.Verify(output);
		}
		#endregion

		[Test]
		public void TestInsertWithIDReturn()
		{
			using (var c = Connection().OpenWithTransaction())
			{
				var beer = Beer.GetSample();
				c.InsertAsync(Beer.InsertProc, beer).Wait();
				beer.VerifySample();
			}
		}

		[Test]
		public void TestInsertListWithIDReturn()
		{
			using (var c = Connection().OpenWithTransaction())
			{
				var list = new List<Beer>();
				list.Add(Beer.GetSample());
				list.Add(Beer.GetSample());

				c.InsertListAsync(Beer.InsertManyProc, list).Wait();

				foreach (var beer in list)
					beer.VerifySample();
			}
		}
	}
}
