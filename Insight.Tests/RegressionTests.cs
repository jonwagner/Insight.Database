using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Insight.Database;

namespace Insight.Tests
{
	[TestFixture]
	public class RegressionTests : BaseDbTest
	{
		#region Git Issue #18
		class Beer
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public int? AlcoholPts { get; set; }
		}

		[Test]
		public void TestIssue18()
		{
			using (var connection = _connectionStringBuilder.OpenWithTransaction())
			{
				connection.ExecuteSql("CREATE TABLE Beer (id int identity, name varchar(256), alcoholpts int)");
				connection.ExecuteSql(@"
					CREATE PROC InsertBeer @id int, @name varchar(256), @alcoholpts [int] AS 
						INSERT INTO Beer (Name, AlcoholPts)
						OUTPUT Inserted.Id
						VALUES (@Name, @AlcoholPts)
				");

				Beer b = new Beer() { AlcoholPts = 11 };
				connection.ExecuteScalar<int>("InsertBeer", b);
				Assert.AreEqual(11, connection.ExecuteScalarSql<int>("SELECT AlcoholPts FROM Beer"));
			}
		}
		#endregion
	}
}
