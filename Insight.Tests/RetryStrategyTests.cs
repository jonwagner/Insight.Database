using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using Insight.Database.Reliable;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data;
using Moq;

namespace Insight.Tests
{
	/// <summary>
	/// Test the automatic retry strategies for connections.
	/// </summary>
	[TestFixture]
	public class RetryStrategyTests : BaseDbTest
	{
		private Mock<RetryStrategy> _mockRetryStrategy;
		private RetryStrategy RetryStrategy { get { return _mockRetryStrategy.Object; } }
		private int Retries;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			Retries = 0;

			// set up all exceptions as transient, since it is hard to reproduce them
			_mockRetryStrategy = new Mock<RetryStrategy>();
			_mockRetryStrategy.Setup(r => r.IsTransientException(It.IsAny<Exception>())).Returns(true);

			// log messages so we can see what is going on
			RetryStrategy.Retrying += (sender, re) => { Console.WriteLine("Retrying. Attempt {0}", re.Attempt); Retries++; };

			// by default only retry once or tests will take long
			RetryStrategy.MaxRetryCount = 1;
		}

		#region IsTransientException Tests
		[Test]
		public void NonTransientExceptionDoesNotRetry()
		{
			// all exceptions are bad
			_mockRetryStrategy.Setup(r => r.IsTransientException(It.IsAny<Exception>())).Returns(false);

			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			b.InitialCatalog = "bad";
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			try
			{
				retry.Query("SELECT 1");
			}
			catch
			{
			}

			Assert.IsFalse(Retries > 0);
		}

		[Test]
		public void BadOpenPerformsRetry()
		{
			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			b.InitialCatalog = "bad";
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			try
			{
				retry.Query("SELECT 1");
			}
			catch
			{
			}

			Assert.IsTrue(Retries > 0);
		}
		#endregion

		#region Parameter Tests
		[Test]
		public void MaxRetryCountCapsNumberOfRetries()
		{
			int retries = 0;
			RetryStrategy.Retrying += (sender, re) => { retries++; };
			RetryStrategy.MaxRetryCount = 5;

			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			b.InitialCatalog = "bad";
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			try
			{
				retry.Query("SELECT 1");
			}
			catch
			{
			}

			Assert.AreEqual(5, retries);
		}
		#endregion

		#region Tests for Successful Queries
		[Test]
		public void StoredProcedureParametersDetectedWithReliableSqlConnection()
		{
			ReliableConnection retry = new ReliableConnection(_connection, RetryStrategy);

			// the SqlCommand.DeriveParameters code looks specifically for SqlCommand. 
			// This test ensures that reliable connections work with this.

			using (IDbTransaction t = retry.BeginTransaction())
			{
				retry.ExecuteSql("CREATE PROC InsightTestProc (@Value int = 5) AS SELECT Value=@Value", transaction: t);

				int result = retry.Query<int>("InsightTestProc", new { Value = 1 }, transaction: t).First();

				Assert.AreEqual(1, result);
			}
		}
		#endregion

		// TODO: async retries
		// TODO: test cancelling retries
	}
}
