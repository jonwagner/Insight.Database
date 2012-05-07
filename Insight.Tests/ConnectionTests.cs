using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;

namespace Insight.Tests
{
	/// <summary>
	/// Tests that connections are opened and closed properly
	/// </summary>
	[TestFixture]
	public class ConnectionTests : BaseDbTest
	{
		#region Synchronous Queries
		[Test]
		public void ExecuteSqlShouldAutoClose ()
		{
			_connection.Close ();

			int i = _connection.ExecuteSql ("SELECT 1");
			Assert.AreEqual (-1, i);

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void ExecuteScalarSqlShouldAutoClose ()
		{
			_connection.Close ();

			int i = _connection.ExecuteScalarSql<int> ("SELECT 1");
			Assert.AreEqual (1, i);

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void QuerySqlShouldAutoClose()
		{
			_connection.Close ();

			int i = _connection.QuerySql<int> ("SELECT 1").FirstOrDefault ();
			Assert.AreEqual (1, i);

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void QueryProcShouldAutoClose ()
		{
			_connection.Close ();

			_connection.QuerySql ("sp_who").FirstOrDefault ();

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void FailedEnumerationShouldAutoClose()
		{
			_connection.Close();

			try
			{
				string sql = "SELECT p=1 SELECT p=2";
				using (var reader = _connection.GetReaderSql(sql, Parameters.Empty))
				{
					foreach (var i in reader.AsEnumerable<int>())
						throw new ApplicationException();
				}
			}
			catch { }

			Assert.AreEqual(ConnectionState.Closed, _connection.State);
		}
		#endregion

		#region Asynchronous Queries
		[Test]
		public void ExecuteSqlAsyncShouldAutoClose ()
		{
			_connection.Close ();

			int i = _connection.ExecuteSqlAsync ("SELECT 1").Result;
			Assert.AreEqual (-1, i);

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void QuerySqlAsyncShouldAutoClose ()
		{
			_connection.Close ();

			int i = _connection.QuerySqlAsync<int> ("SELECT 1").Result.FirstOrDefault ();
			Assert.AreEqual (1, i);

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}

		[Test]
		public void QueryAsyncProcShouldAutoClose ()
		{
			_connection.Close ();

			_connection.QuerySqlAsync ("sp_who").Result.FirstOrDefault ();

			Assert.AreEqual (ConnectionState.Closed, _connection.State);
		}
		#endregion
	}
}
