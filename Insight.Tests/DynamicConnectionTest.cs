using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace Insight.Tests
{
    public class DynamicConnectionTest : BaseDbTest
    {
		class Data
		{
			public int Value;
		}

        /// <summary>
        /// Make sure that using dynamic on an unopened connection properly auto-opens the connection when getting the procedure.
        /// </summary>
        [Test]
        public void TestUnopenedConnection()
        {
            // make sure the connection is closed first
            _connection.Close();
            Assert.AreEqual(ConnectionState.Closed, _connection.State);

            // call a proc that we know exists and make sure we get data back
            var result = _connection.Dynamic().sp_Who();
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(ConnectionState.Closed, _connection.State);

            // call a proc with no results
            var result2 = _connection.Dynamic().sp_validname("foo");
            Assert.AreEqual(ConnectionState.Closed, _connection.State);

            // call a proc that we know exists and make sure we get data back
            var result3 = _connection.Dynamic().sp_WhoAsync().Result;
            Assert.IsTrue(result3.Count > 0);
            Assert.AreEqual(ConnectionState.Closed, _connection.State);

            // call a proc async with no results
            var result4 = _connection.Dynamic().sp_validnameAsync("foo").Result;
            Assert.AreEqual(ConnectionState.Closed, _connection.State);
        }

		[Test]
		public void Test()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value", transaction: t);

				List<Data> result = _connection.Dynamic<Data>().InsightTestProc(transaction: t, value: 5);
			}
		}

		[Test]
		public void TestUnnamedParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value", transaction: t);

				List<Data> result = _connection.Dynamic<Data>().InsightTestProc(5, transaction: t);
			}
		}

		[Test]
		public void TestExecute()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS PRINT 'foo'", transaction: t);

				_connection.Dynamic().InsightTestProc(transaction: t, value: 5);
			}
		}

		[Test]
		public void TestObjectParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value", transaction: t);

				Data d = new Data() { Value = 10 };
				IList<Data> list = _connection.Dynamic<Data>().InsightTestProc(d, transaction: t);
				Data result = list.First();

				Assert.AreEqual(d.Value, result.Value);
			}
		}

		[Test]
		public void TestSingleStringParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value varchar(128)) AS SELECT Value=@Value", transaction: t);

				string value = "foo";
				IList<string> list = _connection.Dynamic<string>().InsightTestProc(value, transaction: t);
				string result = list.First();

				Assert.AreEqual(value, result);
			}
		}
	}
}
