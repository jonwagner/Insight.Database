using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Database;

namespace Insight.Tests
{
	[TestFixture]
	public class RegressionTests : BaseTest
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
			using (var connection = ConnectionWithTransaction())
			{
				connection.ExecuteSql("CREATE TABLE Beer18 (id int identity, name varchar(256), alcoholpts int)");
				connection.ExecuteSql(@"
					CREATE PROC InsertBeer18 @id int, @name varchar(256), @alcoholpts [int] AS 
						INSERT INTO Beer18 (Name, AlcoholPts)
						OUTPUT Inserted.Id
						VALUES (@Name, @AlcoholPts)
				");

				Beer b = new Beer() { AlcoholPts = 11 };
				connection.ExecuteScalar<int>("InsertBeer18", b);
				ClassicAssert.AreEqual(11, connection.ExecuteScalarSql<int>("SELECT AlcoholPts FROM Beer18"));
			}
		}
		#endregion
	}
}
