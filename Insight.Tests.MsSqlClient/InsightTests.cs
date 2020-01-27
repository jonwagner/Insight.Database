using Insight.Database;
using Insight.Tests.Cases;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;

namespace Insight.Tests.MsSqlClient
{
	public class MsSqlClientTests : BaseTest
	{
		public static IDbConnection _connection;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			TestSetup.CreateTestDatabase();
			_connection = new SqlConnection(ConnectionString);
			_connection.Open();
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			TestSetup.TeardownFixture();
		}

		[Test, Order(1)]
		public void TestQueryProcedure()
		{
			var result = _connection.Query<Beer>(Beer.SelectAllProc);

			Beer.VerifyAll(result);
		}

		[Test, Order(2)]
		public void TestInsertProcedure()
		{
			var result = _connection.Insert(Beer.InsertProc, Beer.GetSample());

			result.VerifySample();
		}
	}
}
