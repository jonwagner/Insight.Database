#if !NET35
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
using System.Threading.Tasks;

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
			RetryStrategy.MaxBackOff = new TimeSpan(0, 0, 0, 0, 10);
		}

		#region Pure RetryStrategy Tests
		[Test]
		public void RetryStrategyShouldCompleteWhenFuncReturnsNullTask()
		{
			RetryStrategy s = new RetryStrategy();

			Assert.Throws<AggregateException>(() => s.ExecuteWithRetryAsync<int>(null, () => null).Wait());
		}

		[Test]
		public void RetryStrategyShouldCompleteWhenFuncThrows()
		{
			RetryStrategy s = new RetryStrategy();

			Assert.Throws<AggregateException>(() => s.ExecuteWithRetryAsync<int>(null, () => { throw new ApplicationException(); }).Wait());
		}

		class TestRetryStrategy : RetryStrategy
		{
			public override bool IsTransientException(Exception exception)
			{
				return true;
			}
		}

		[Test]
		public void RetryStrategyShouldCompleteWhenHandlerThrows()
		{
			RetryStrategy s = new RetryStrategy();

			s.Retrying += (sender, re) => { throw new ApplicationException(); };

			Assert.Throws<AggregateException>(() => s.ExecuteWithRetryAsync<int>(null, () => Task<int>.Factory.StartNew(() => { throw new ApplicationException(); })).Wait());
		}
		#endregion

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
			using (ReliableConnection retry = new ReliableConnection<SqlConnection>(_connection.ConnectionString, RetryStrategy))
			{
				retry.Open();

				// the SqlCommand.DeriveParameters code looks specifically for SqlCommand. 
				// This test ensures that reliable connections work with this.

				using (IDbTransaction t = retry.BeginTransaction())
				{
					retry.ExecuteSql("CREATE PROC ReliableInsightTestProc (@Value int = 5) AS SELECT Value=@Value", transaction: t);

					int result = retry.Query<int>("ReliableInsightTestProc", new { Value = 1 }, transaction: t).First();

					Assert.AreEqual(1, result);
				}
			}
		}

		[Test]
		public void ExecuteStoredProcWithTVPThroughReliableConnection()
		{
			using (ReliableConnection retry = new ReliableConnection<SqlConnection>(_connection.ConnectionString, RetryStrategy))
			{
				retry.Open();

				try
				{
					retry.ExecuteSql("CREATE TYPE [Int32Table] AS TABLE ([Value] [int])");
					retry.ExecuteSql("CREATE PROC InsightTestProc (@Value Int32Table READONLY) AS SELECT * FROM @Value");

					// the ListParameterHelper.AddEnumerableClassParameters code looks specifically for SqlCommand. 
					// This test ensures that reliable connections work with this.
					var result = retry.Query<int>("InsightTestProc", new int[] { 1, 2, 3, 4, 5 });

					Assert.AreEqual(5, result.Count());
				}
				finally
				{
					Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestProc') DROP PROCEDURE [InsightTestProc]");
					Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'Int32Table') DROP TYPE [Int32Table]");
				}
			}
		}
		#endregion

		#region Async Retry Tests
		[Test]
		public void AsyncBadOpenPerformsRetry()
		{
			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			b.InitialCatalog = "bad";
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			RetryStrategy.MaxRetryCount = 5;
			try
			{
				retry.QueryAsync("SELECT 1").Wait();
			}
			catch
			{
			}

			Assert.AreEqual(5, Retries);
		}

		[Test]
		public void AsyncBadQueryPerformsRetry()
		{
			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			RetryStrategy.MaxRetryCount = 5;
			try
			{
				retry.QuerySqlAsync("INVALID SQL").Wait();
			}
			catch
			{
			}

			Assert.AreEqual(5, Retries);
		}

		[Test]
		public void AsyncWorksWithReliableConnection()
		{
			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(_connection.ConnectionString);
			ReliableConnection<SqlConnection> retry = new ReliableConnection<SqlConnection>(b.ConnectionString, RetryStrategy);

			int result = retry.QuerySqlAsync<int>("SELECT 10").Result.First();

			Assert.AreEqual(10, result);
		}
		#endregion

		#region Tests for Bad Exceptions
		[Test]
		public void InvalidExceptionShouldNotBeTransient()
		{
			// this was throwing an exception during provider lookup

			RetryStrategy retry = new RetryStrategy();
			Assert.IsFalse(retry.IsTransientException(new InvalidOperationException()));
		}
		#endregion
	}
}
#endif