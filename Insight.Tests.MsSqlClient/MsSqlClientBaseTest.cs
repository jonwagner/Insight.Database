using Insight.Database;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;

namespace Insight.Tests.MsSqlClient
{
	public class MsSqlClientBaseTest 
	{
		public static IDbConnection _connection;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			TestSetup.CreateTestDatabase();
			_connection = new SqlConnection(BaseTest.ConnectionString);
			_connection.Open();
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			TestSetup.TeardownFixture();
		}

		public IDbConnection Connection()
		{
			return new SqlConnection(BaseTest.ConnectionString);
		}

		public IDbConnection ConnectionWithTransaction()
		{
			return new SqlConnection(BaseTest.ConnectionString).OpenWithTransaction();
		}
	}
}
