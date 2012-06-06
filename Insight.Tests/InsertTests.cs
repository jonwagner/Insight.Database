using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;

#pragma warning disable 0649

namespace Insight.Tests
{
	/// <summary>
	/// Test the ability to return identity values from the database and zipping them into inserted objects.
	/// </summary>
	[TestFixture]
	class InsertTests : BaseDbTest
	{
		class InsertRecord
		{
			public int Id;
			public int Id2;
			public string Text;
			public int Value;
		}

		/// <summary>
		/// Make sure that we can call a procedure with the inserted object and have it fill in identities on return.
		/// </summary>
		[Test]
		public void SingleInsertShouldFillInIdentities()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Value int) AS SELECT Id=1, Id2=@Value", transaction: t);

				InsertRecord i = new InsertRecord();
				i.Value = 5;

				var result = _connection.Insert<InsertRecord>("InsightTestProc", i, transaction: t);

				Assert.AreEqual(i, result);
				Assert.AreEqual(1, i.Id);
				Assert.AreEqual(5, i.Id2);
			}
		}

		/// <summary>
		/// Make sure that we can call a stored proc with parameters in addition to the object that we are passing in.
		/// Not a common case, but common enough that we should support it.
		/// </summary>
		[Test]
		public void SingleInsertShouldAllowParameters()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProcSingleInsert (@OtherValue int) AS SELECT Id=1, Id2=@OtherValue", transaction: t);

				InsertRecord i = new InsertRecord();
				List<InsertRecord> list = new List<InsertRecord>() { i };

				var result = _connection.Insert<InsertRecord>("InsightTestProcSingleInsert", i, i.Expand(new { OtherValue = 5 }), transaction: t);

				Assert.AreEqual(i, result);
				Assert.AreEqual(1, i.Id);
				Assert.AreEqual(5, i.Id2);
			}
		}

		/// <summary>
		/// Make sure that a multiple insert can return identities.
		/// Note that here we have to specify the parameters AND the list of objects that we want to treat as an insert.
		/// </summary>
		[Test]
		public void MultipleInsertShouldFillInIdentities()
		{
			try
			{
				_connection.ExecuteSql(@"CREATE TABLE InsightTestTableMultiInsert (ID [int] IDENTITY, ID2 [int] DEFAULT (2), Text [varchar](128), Value int)");
				_connection.ExecuteSql(@"CREATE TYPE InsightTestTableTypeMultiInsert AS TABLE (Text [varchar](128), Value int)");
				_connection.ExecuteSql(@"
						CREATE PROC InsightTestProcMultiInsert (@OtherValue int, @Items [InsightTestTableTypeMultiInsert] READONLY) AS 
							INSERT INTO InsightTestTableMultiInsert (Text, Value)
								OUTPUT inserted.ID, inserted.ID2
								SELECT Text, @OtherValue FROM @Items
						");

				InsertRecord i = new InsertRecord();
				InsertRecord i2 = new InsertRecord();
				List<InsertRecord> list = new List<InsertRecord>() { i, i2 };

				var result = _connection.InsertList("InsightTestProcMultiInsert", list, new { OtherValue = 5, Items = list });

				Assert.AreEqual(list, result);
				Assert.AreEqual(1, i.Id);
				Assert.AreEqual(2, i.Id2);
				Assert.AreEqual(2, i2.Id);
				Assert.AreEqual(2, i2.Id2);
			}
			finally
			{
				Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestProcMultiInsert') DROP PROCEDURE [InsightTestProcMultiInsert]");
				Cleanup("IF EXISTS (SELECT * FROM sys.types WHERE name = 'InsightTestTableTypeMultiInsert') DROP TYPE [InsightTestTableTypeMultiInsert]");
				Cleanup("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'InsightTestTableMultiInsert') DROP TABLE [InsightTestTableMultiInsert]");
			}
		}

		[Test]
		public void SingleInsertSqlShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i };

			// this would normally be INSERT INTO blah VALUES (@blah) SELECT @@SCOPE_IDENTITY
			var result = _connection.InsertSql<InsertRecord>("SELECT Id=1, Id2=2", i);

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(2, i.Id2);
		}
	}
}
