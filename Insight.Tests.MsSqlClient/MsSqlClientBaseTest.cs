﻿using Insight.Database;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;

namespace Insight.Tests.MsSqlClient
{
	public class MsSqlClientBaseTest 
	{
		public IDbConnection Connection()
		{
			return new SqlConnection(BaseTest.ConnectionString);
		}

		public IDbConnection ConnectionWithTransaction()
		{
			return new SqlConnection(BaseTest.ConnectionString).OpenWithTransaction();
		}
	}

	[SetUpFixture]
	public class MsSqlTestSetup
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			TestSetup.CreateTestDatabase();
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			TestSetup.TeardownFixture();
		}
	}
}
