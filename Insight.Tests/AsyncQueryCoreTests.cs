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
	public partial class AsyncQueryCoreTests : BaseTest
	{
		#region Basic Tests
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var result = c.QueryAsync<Beer>(Beer.SelectAllProc).Result;
				Beer.VerifyAll(result);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;

				c.QueryAsync<Beer>(Beer.SelectAllProc, commandBehavior: CommandBehavior.CloseConnection).Wait();

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

			Connection().QueryAsync<Beer>(InOutParameters.ProcName, input, outputParameters: output).Wait();

			input.Verify(output);
		}
		#endregion

		[Test]
		public void TestChildrenWithOneToOne()
		{
			// child records that include a one-to-one mapping

			var result = Connection().QuerySqlAsync(
				Beer.GetSelectNestedChildren(1, 2),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(OneToOne<InfiniteBeerList, InfiniteBeer>.Records, b => b.ID, (b, list) => b.List = list)
					).Result;

			Assert.AreEqual(3, result.Count());
			Assert.AreEqual(1, result[0].List.Count());
			Assert.IsNotNull(result[0].List[0].More);
			result[0].List[0].More.Verify();
		}
	}
}
