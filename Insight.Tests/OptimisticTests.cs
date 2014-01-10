using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;

namespace Insight.Tests
{
	[TestFixture]
	public class OptimisticTests : BaseDbTest
	{
		[Test, ExpectedException(typeof(OptimisticConcurrencyException))]
		public void OptimisticExceptionsAreTranslated()
		{
			var opt = new OptimisticConnection(_connection);
			opt.ExecuteSql("RAISERROR('(CONCURRENCY CHECK)', 16, 1)");
		}

		[Test]
		public void OptimisticExceptionsAreTranslatedAsync()
		{
			var opt = new OptimisticConnection(_connection);
			try
			{
				opt.ExecuteSqlAsync("RAISERROR('(CONCURRENCY CHECK)', 16, 1)").Wait();
			}
			catch (AggregateException ax)
			{
				Assert.IsNotNull(ax.Flatten().InnerExceptions.OfType<OptimisticConcurrencyException>());
			}
		}
	}
}
