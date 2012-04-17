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
	public class AsyncExtensionTests
	{
		#region Setup/TearDown
		/// <summary>
		/// We open the connection for each of these because we want to test that it gets closed properly.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			// set up the connection and allow it to run asynchronously
			SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder ();
			sb.IntegratedSecurity = true;
			sb.AsynchronousProcessing = true;

			_connection = new SqlConnection (sb.ConnectionString);
			_connection.Open ();
		}

		[TearDown]
		public void TearDown()
		{
			_connection.Close ();
		}
		#endregion

		/// <summary>
		/// The connection for the test
		/// </summary>
		private SqlConnection _connection;

		#region Tests
		/// <summary>
		/// Asynchronously run a command that is not a query
		/// </summary>
		[Test]
		public void TestNonQuery ()
		{
			var task = _connection.AsyncExecuteSql ("DECLARE @i int", Parameters.Empty, closeConnection: true);
			task.Wait ();

			Assert.IsTrue (_connection.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Asynchronously run a command that returns a data reader
		/// </summary>
		[Test]
		public void TestAsyncReader()
		{
			var task = _connection.AsyncQuerySql(
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

			var task = _connection.AsyncQuerySql (
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

			var task = _connection.AsyncQuerySql(
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
			var task = _connection.AsyncExecuteSql ("THIS IS INVALID SQL", Parameters.Empty, closeConnection: true);

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

			var task = _connection.AsyncQuerySql(
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
