using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using Insight.Database;

namespace Insight.Tests
{
	/// <summary>
	/// Tests asynchronous SQL features. Requires a local database with a trusted connection.
	/// </summary>
	[TestFixture]
	public class AsyncExtensionTests : BaseDbTest
	{
		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("CREATE PROCEDURE [InsightAsyncTestProc] @p [int] AS SELECT @p");
			_connection.ExecuteSql("CREATE PROCEDURE [InsightAsyncTestProc2] @p [int] AS SELECT @p");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			CleanupObjects();

			base.TearDownFixture();
		}

		private void CleanupObjects()
		{
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightAsyncTestProc') DROP PROCEDURE [InsightAsyncTestProc]");
			Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightAsyncTestProc2') DROP PROCEDURE [InsightAsyncTestProc2]");
		}
		#endregion

		#region Tests
		/// <summary>
		/// Asynchronously run a command that is not a query
		/// </summary>
		[Test]
		public void TestNonQuery()
		{
			var task = _connection.ExecuteSqlAsync("DECLARE @i int", Parameters.Empty, closeConnection: true);
			task.Wait();

			Assert.IsTrue(_connection.State == ConnectionState.Closed);
		}

        /// <summary>
        /// Asynchronously run a command that is not a query
        /// </summary>
        [Test]
        public void TestScalar()
        {
            var task = _connection.ExecuteScalarSqlAsync<int>("SELECT 5", Parameters.Empty, closeConnection: true);
            task.Wait();

            Assert.IsTrue(_connection.State == ConnectionState.Closed);
            Assert.AreEqual(5, task.Result);
        }

        /// <summary>
		/// Asynchronously run a stored procedure with parameter detection
		/// </summary>
		[Test]
		public void TestAsyncQueryStoredProcedure()
		{
			// make sure the connection is closed so we can test parameter detection with a closed async connection
			_connection.Close();

			var task = _connection.QueryAsync<int>("InsightAsyncTestProc", new { p = 5 });
			var results = task.Result;

			Assert.AreEqual(5, results.First());
		}

		/// <summary>
		/// Asynchronously run a stored procedure with parameter detection
		/// </summary>
		[Test]
		public void TestAsyncExecuteStoredProcedure()
		{
			// make sure the connection is closed so we can test parameter detection with a closed async connection
			_connection.Close();

			var task = _connection.ExecuteAsync("InsightAsyncTestProc2", new { p = 5 });
			task.Wait();

			_connection.Close();
		}

		/// <summary>
		/// Asynchronously run a command that returns a data reader
		/// </summary>
		[Test]
		public void TestAsyncReader()
		{
			var task = _connection.QuerySqlAsync(
				"SELECT 1", 
				Parameters.Empty, 
				reader =>
				{
					using (reader)
					{
						reader.Read ();
						Assert.AreEqual (1, reader.GetInt32 (0));
					}
				},
				commandBehavior: CommandBehavior.CloseConnection);

			task.Wait();
			
			Assert.IsTrue (_connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Make sure that we can chain sql tasks
		/// </summary>
		[Test]
		public void TestAsyncReaderContinuation()
		{
			int result = 0;

			var task = _connection.QuerySqlAsync (
				"SELECT 1", 
				Parameters.Empty, 
				reader =>
				{
					using (reader)
					{
						reader.Read ();
						result = reader.GetInt32 (0);
					}					
				},
				CommandBehavior.CloseConnection);

			task.Wait ();

			Assert.AreEqual (1, result);
			Assert.IsTrue (_connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Make sure that we can chain sql tasks
		/// </summary>
		[Test]
		public void TestSqlError ()
		{
			int result = 0;

			var task = _connection.QuerySqlAsync(
				"THIS IS INVALID SQL",
				Parameters.Empty,
				reader =>
				{
					using (reader)
					{
						reader.Read();
						result = reader.GetInt32(0);
					}
				},
				commandBehavior: CommandBehavior.CloseConnection);

			Assert.Throws<AggregateException> (() => task.Wait ());
			Assert.IsTrue (_connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Asynchronously run a command that is not a query
		/// </summary>
		[Test]
		public void TestNonQuerySqlError ()
		{
			var task = _connection.ExecuteSqlAsync ("THIS IS INVALID SQL", Parameters.Empty, closeConnection: true);

			Assert.Throws<AggregateException> (() => task.Wait ());
			Assert.IsTrue (_connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Run a multiple recordset expando query asynchronously.
		/// </summary>
		[Test]
		public void TestAsyncReaderMultiRecordsetWithExpandos()
		{
			dynamic foo = null;
			dynamic goo = null;

			var task = _connection.QuerySqlAsync(
				"SELECT Foo=1 SELECT Goo='Text'",
				Parameters.Empty,
				reader =>
				{
					// note that ToList willo automatically advance to the next recordset
					foo = reader.ToList();
					goo = reader.ToList();
				},
				commandBehavior: CommandBehavior.CloseConnection);

			task.Wait();

			Assert.IsTrue(_connection.State == ConnectionState.Closed);
			Assert.IsNotNull(foo);
			Assert.AreEqual(1, foo[0].Foo);
			Assert.IsNotNull(goo);
			Assert.AreEqual("Text", goo[0].Goo);
		}
		#endregion
    }
}
