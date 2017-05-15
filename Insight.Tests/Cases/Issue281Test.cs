using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Insight.Tests.Cases
{
	[TestFixture]
	public class InsightTest : BaseTest
	{
		public class Parent
		{
			public int ParentId { get; set; }
			public string Name { get; set; }
			public List<Children> Children { get; set; }
		}

		public class Children
		{
			public int ChildrenId { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void TestMethodFound()
		{
			var conn = Connection();

			try
			{
				conn.ExecuteSql(@"CREATE TABLE Parent (ParentId INT IDENTITY(1, 1) NOT NULL, name nvarchar(20) null);
                                CREATE TABLE Child (ChildId INT IDENTITY(1, 1) NOT NULL, ParentId int NOT NULL, Name nvarchar(50) null)
                                INSERT INTO Parent VALUES ('Parent')
                                INSERT INTO Child (ParentId, Name) SELECT ParentId, 'Child' FROM Child");

				var response = conn.QuerySql(@" SET NOCOUNT ON;
                            --Parent
                            SELECT ParentId, Name FROM Parent WHERE @ParentId = ParentId
                            --Child
                            SELECT ParentId, ChildId, Name FROM Child WHERE @ParentId = ParentId", new { ParentId = 1 },
					Query.ReturnsSingle(Some<Parent>.Records)
					.ThenChildren(OneToOne<Children>.Records, id: (d) => d.ParentId, into: (p, c) => p.Children = c));

				Assert.AreEqual(response.ParentId, 1);
				Assert.AreEqual(response.Name, "Parent");

			}
			catch (Exception e)
			{
				Assert.Fail(e.Message);
			}
			finally
			{
				conn.ExecuteSql(@"DROP TABLE Parent; Drop Table Child");
			}
		}

		[Test]
		public void TestMethodNotFound()
		{
			var conn = Connection();

			try
			{
				conn.ExecuteSql(@"CREATE TABLE Parent (ParentId INT IDENTITY(1, 1) NOT NULL, name nvarchar(20) null);
                                CREATE TABLE Child (ChildId INT IDENTITY(1, 1) NOT NULL, ParentId int NOT NULL, Name nvarchar(50) null)
                                INSERT INTO Parent VALUES ('Parent')
                                INSERT INTO Child (ParentId, Name) SELECT ParentId, 'Child' FROM Child");

				var response = conn.QuerySql(@" SET NOCOUNT ON;
                            --Parent
                            SELECT ParentId, Name FROM Parent WHERE @ParentId = ParentId
                            --Child
                            SELECT ParentId, ChildId, Name FROM Child WHERE @ParentId = ParentId", new { ParentId = 2 },
					Query.ReturnsSingle(Some<Parent>.Records)
					.ThenChildren(Some<Children>.Records, id: (d) => d.ParentId, into: (p, c) => p.Children = c));

				Assert.AreEqual(response, null);

			}
			finally
			{
				conn.ExecuteSql(@"DROP TABLE Parent; Drop Table Child");
			}
		}

		[Test]
		public void TestMethodNotFoundOneToOne()
		{
			var conn = Connection();

			try
			{
				conn.ExecuteSql(@"CREATE TABLE Parent (ParentId INT IDENTITY(1, 1) NOT NULL, name nvarchar(20) null);
                                CREATE TABLE Child (ChildId INT IDENTITY(1, 1) NOT NULL, ParentId int NOT NULL, Name nvarchar(50) null)
                                INSERT INTO Parent VALUES ('Parent')
                                INSERT INTO Child (ParentId, Name) SELECT ParentId, 'Child' FROM Child");

				var response = conn.QuerySql(@" SET NOCOUNT ON;
                            --Parent
                            SELECT ParentId, Name FROM Parent WHERE @ParentId = ParentId
                            --Child
                            SELECT ParentId, ChildId, Name FROM Child WHERE @ParentId = ParentId", new { ParentId = 2 },
					Query.ReturnsSingle(Some<Parent>.Records)
					.ThenChildren(OneToOne<Children>.Records, id: (d) => d.ParentId, into: (p, c) => p.Children = c));

				Assert.AreEqual(response, null);

			}
			finally
			{
				conn.ExecuteSql(@"DROP TABLE Parent; Drop Table Child");
			}
		}
	}
}
