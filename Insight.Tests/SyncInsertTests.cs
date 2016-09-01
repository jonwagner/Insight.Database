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
	public class SyncInsertTests : BaseTest
	{
		#region Base Cases
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var input = new InOutParameters { In = 5 };
				var result = c.InsertSql("SELECT @In", input);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;

				var input = new InOutParameters { In = 5 };
				var recordCount = c.InsertSql("SELECT @In", input, commandBehavior: CommandBehavior.CloseConnection);

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

			var result = Connection().Insert(InOutParameters.ProcName, input, outputParameters: output);

			input.Verify(output);
		}

		[Test]
		public void TestOutputParametersInList()
		{
			var list = new List<int>();
			var input = new InOutParameters { In = 5 };
			var output = new OutParameters();

			var result = Connection().InsertList(InOutParameters.ProcName, list, input, outputParameters: output);

			input.Verify(output);
		}
		#endregion

		[Test]
		public void TestInsertWithIDReturn()
		{
			using (var c = Connection().OpenWithTransaction())
			{
				var beer = Beer.GetSample();
				c.Insert(Beer.InsertProc, beer);
				beer.VerifySample();
			}
		}

		[Test]
		public void TestInsertWithIDReturn_ScopeIdentity()
		{
			using (var c = Connection().OpenWithTransaction())
			{
				var beer = Beer.GetSample();
				c.Insert(Beer.InsertProcScopeIdentity, beer);
				beer.VerifySample();
			}
		}


		[Test]
		public void TestInsertWithIDReturn_ScopeIdentity_SQL()
		{
			using (DbConnectionWrapper c = Connection().OpenWithTransaction())
			{
				var beer = Beer.GetSample();
				c.InsertSql("INSERT INTO[dbo].[Beer] ([Name], [Style]) VALUES(@Name , @Style ); SELECT SCOPE_IDENTITY() AS [Id]", beer);
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

				c.InsertList(Beer.InsertManyProc, list);

				foreach (var beer in list)
					beer.VerifySample();
			}
		}
	}
}
