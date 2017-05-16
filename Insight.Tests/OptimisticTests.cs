using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using System.Data.Common;

namespace Insight.Tests
{
	[TestFixture]
	public class OptimisticTests : BaseTest
	{
		[Test]
		public void OptimisticConnectionCanCallProc()
		{
			var opt = new OptimisticConnection((DbConnection)Connection());
			opt.Execute("sp_who");
		}

		[Test]
		public void OptimisticExceptionsAreTranslated()
		{
			var opt = new OptimisticConnection((DbConnection)Connection());
			Assert.Throws<OptimisticConcurrencyException>(() => opt.ExecuteSql("RAISERROR('(CONCURRENCY CHECK)', 16, 1)"));
		}

		[Test]
		public void OptimisticExceptionsAreTranslatedAsync()
		{
			var opt = new OptimisticConnection((DbConnection)Connection());
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
