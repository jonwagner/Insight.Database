using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using Insight.Database.Reliable;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;

namespace Insight.Tests
{
	/// <summary>
	/// Test the automatic retry strategies for connections.
	/// </summary>
	[TestFixture]
	public class RetryStrategyTests : BaseTest
	{
		private MyRetryStrategy RetryStrategy { get; set; }
		private int Retries;

		class MyRetryStrategy : RetryStrategy
		{
			public bool IsTransient = true;

			public override bool IsTransientException(Exception exception)
			{
				return IsTransient;
			}
		}

		[SetUp]
		public void SetUp()
		{
			Retries = 0;

			// log messages so we can see what is going on
			RetryStrategy = new MyRetryStrategy();
			RetryStrategy.Retrying += (sender, re) => { Retries++; };

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

			Assert.Throws<AggregateException>(() => s.ExecuteWithRetryAsync<int>(null, () => { throw new Exception(); }).Wait());
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

			s.Retrying += (sender, re) => { throw new Exception(); };

			Assert.Throws<AggregateException>(() => s.ExecuteWithRetryAsync<int>(null, () => Task<int>.Factory.StartNew(() => { throw new Exception(); })).Wait());
		}
		#endregion

		#region IsTransientException Tests
		[Test]
		public void NonTransientExceptionDoesNotRetry()
		{
			// all exceptions are bad
			RetryStrategy.IsTransient = false;

			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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
			using (ReliableConnection retry = new ReliableConnection<SqlConnection>(ConnectionString, RetryStrategy))
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
			using (ReliableConnection retry = new ReliableConnection<SqlConnection>(ConnectionString, RetryStrategy))
			{
				retry.Open();

				// the ListParameterHelper.AddEnumerableClassParameters code looks specifically for SqlCommand. 
				// This test ensures that reliable connections work with this.
				var result = retry.Query<int>("ReflectInt32Table", new int[] { 1, 2, 3, 4, 5 });

				Assert.AreEqual(5, result.Count());
			}
		}
		#endregion

		#region Async Retry Tests
		[Test]
		public void AsyncBadOpenPerformsRetry()
		{
			// try a bad connection
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConnectionString);
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

        #region Transaction Tests
        [Test]
        public void ReliableShouldWorkWithTransaction()
        {
            var reliable = new ReliableConnection((DbConnection)Connection());
            using (var conn = reliable.OpenWithTransaction())
            {
                var ret = conn.QuerySql(@"SELECT 1");
            }
        }
		#endregion

		#region Retry and Reliable Tests (Issue #242)
		public class FailingStrategy : IRetryStrategy
		{
			public TResult ExecuteWithRetry<TResult>(IDbCommand commandContext, Func<TResult> func)
			{
				throw new AggregateException();
			}

			public Task<TResult> ExecuteWithRetryAsync<TResult>(IDbCommand commandContext, Func<Task<TResult>> func)
			{
				throw new AggregateException();
			}
		}
		
		public interface IParallelQuery
		{
			[Sql("SELECT x = 1")]
			Task<int> Test();
		}

		[Test]
		public void ParallelRetryShouldUseCustomRetryStrategy()
		{
			var connection = (DbConnection)Connection();
			var retryStrategy = new FailingStrategy();
			var reliableConnection = new ReliableConnection(connection, retryStrategy);

			var target = reliableConnection.AsParallel<IParallelQuery>();

			Assert.Throws<AggregateException>(() => { var x = target.Test().Result; });
		}
		#endregion
	}
}
