using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Insight.Tests
{
	public class BaseTest
	{
		/// <summary>
		/// The connection string for our database.
		/// </summary>
		public static readonly string ConnectionString = "Data Source = .; Initial Catalog = InsightDbTests; Integrated Security = true";
		public static readonly string TestHost;
		public static readonly string Password;

		static BaseTest()
		{
			var testHost = Environment.GetEnvironmentVariable("INSIGHT_TEST_HOST");
			if (testHost != null)
				TestHost = Regex.Match(testHost, @"\d+\.\d+\.\d+\.\d+").Value;
			if (string.IsNullOrEmpty(TestHost)  && !string.IsNullOrEmpty(testHost))
				TestHost = testHost;

			Password = Environment.GetEnvironmentVariable("INSIGHT_TEST_PASSWORD");

			ConnectionString = String.Format("Data Source = {0}; Initial Catalog = InsightDbTests; Integrated Security = {1}; {2}",
				TestHost ?? ".",
				(Password != null) ? "false" : "true",
				(Password != null) ? String.Format("User ID=sa; Password={0}", Password) : "");
		}

		public IDbConnection Connection()
		{
			return new SqlConnection(ConnectionString);
		}

		public IDbConnection ConnectionWithTransaction()
		{
			return new SqlConnection(ConnectionString).OpenWithTransaction();
		}
	}

	[SetUpFixture]
	public class TestSetup
	{
		[OneTimeSetUp]
		public static void SetUpFixture()
		{
			CreateTestDatabase();
		}

		[OneTimeTearDown]
		public static void TeardownFixture()
		{
			DropTestDatabase();
		}

		public static void CreateTestDatabase()
		{
			DropTestDatabase();

			using (var connection = OpenMasterDatabase())
			{
				connection.ExecuteSql(String.Format("IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name = '{0}') CREATE DATABASE [{0}]", TestDatabaseName()));
			}

			using (var connection = new SqlConnection(BaseTest.ConnectionString).OpenConnection())
			{
				using (var stream = typeof(TestSetup).GetTypeInfo().Assembly.GetManifestResourceStream("Insight.Tests.InsightDbTest.sql"))
				using (var reader = new StreamReader(stream))
				{
					foreach (var script in reader.ReadToEnd().Split(new String[] { Environment.NewLine + "GO" + Environment.NewLine }, StringSplitOptions.None))
					{
						script.Trim();

						if (script.Length > 0)
							connection.ExecuteSql(script);
					}
				}
			}
		}

		private static void DropTestDatabase()
		{
			using (var connection = OpenMasterDatabase())
			{
				connection.ExecuteSql(String.Format("IF EXISTS(SELECT * FROM master.sys.databases WHERE name = '{0}') BEGIN ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE [{0}] END", TestDatabaseName()));
			}
		}

		private static SqlConnection OpenMasterDatabase()
		{
			SqlConnectionStringBuilder master = new SqlConnectionStringBuilder(BaseTest.ConnectionString);
			master.InitialCatalog = "master";

			return new SqlConnection(master.ConnectionString).OpenConnection();
		}

		private static string TestDatabaseName()
		{
			return new SqlConnectionStringBuilder(BaseTest.ConnectionString).InitialCatalog;
		}
	}
}
