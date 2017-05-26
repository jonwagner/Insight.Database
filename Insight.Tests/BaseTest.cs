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

namespace Insight.Tests
{
	public class BaseTest
	{
		/// <summary>
		/// The connection string for our database.
		/// </summary>
		public static readonly string ConnectionString = "Data Source = .; Initial Catalog = InsightDbTests; Integrated Security = true";

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
#if !NO_SQL_TYPES
			SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..");
#endif

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
					foreach (var script in reader.ReadToEnd().Split(new String[] { "\r\nGO\r\n" }, StringSplitOptions.None))
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
