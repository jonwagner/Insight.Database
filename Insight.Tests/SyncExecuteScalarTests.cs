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
	public class SyncExecuteScalarTests : BaseTest
	{
		[Test]
		public void TestAutoClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				var parameters = new { p = 1 };
				var result = c.ExecuteScalarSql<int>("SELECT @p", parameters);
				Assert.AreEqual(parameters.p, result);
			});
		}

		[Test]
		public void TestForceClose()
		{
			ConnectionStateCase.ForEach(c =>
			{
				bool wasOpen = c.State == ConnectionState.Open;
				var recordCount = c.ExecuteScalarSql<int>("SELECT @p", new { p = 1 }, closeConnection: true);
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

			var result = Connection().ExecuteScalar<int>(InOutParameters.ProcName, input, outputParameters: output);

			Assert.AreEqual(input.In, result);
			input.Verify(output);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void NonNullableReturnShouldThrowWhenNoRowsAreReturned()
		{
			Connection().ExecuteScalarSql<int>("PRINT 1");
		}

		[Test]
		public void TestNullableReturn()
		{
			var result = Connection().ExecuteScalarSql<int?>("SELECT CAST(NULL as INT)");

			Assert.AreEqual(null, result);
		}

        [Test]
        public void TestNullableStringReturnWhenNoRowsAreReturned()
        {
            var result = Connection().ExecuteScalarSql<string>("PRINT 1");

            Assert.AreEqual(null, result);
        }

		[Test]
		public void Given_a_null_result_When_querying_for_a_scalar_int_Then_the_result_is_not_silently_converted()
		{
			using (var connection = ConnectionWithTransaction())
			{
				TestDelegate act = () => connection.ExecuteScalarSql<int>("SELECT NULL");

				Assert.That(act, Throws.Exception);
			}
		}

		[Test]
		public void Given_a_null_result_When_expecting_an_int_Then_the_result_is_not_silently_converted()
		{
			using (var connection = ConnectionWithTransaction())
			{
				TestDelegate act = () => connection.QuerySql<int>("SELECT NULL").Single();

				Assert.That(act, Throws.Exception);
			}
		}
	}
}
