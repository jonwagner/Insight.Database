using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
	class InsertTests : BaseTest
	{
		class InsertRecord
		{
			public int Id;
			public int Id2;
			public string Text;
			public int Value;
		}

		#region Synchronous Tests
		/// <summary>
		/// Make sure that we can call a procedure with the inserted object and have it fill in identities on return.
		/// </summary>
		[Test]
		public void SingleInsertShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			i.Value = 5;

			var result = Connection().Insert("InsertIdentityReturn", i);

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(5, i.Id2);
		}

		/// <summary>
		/// Make sure that we can call a stored proc with parameters in addition to the object that we are passing in.
		/// Not a common case, but common enough that we should support it.
		/// </summary>
		[Test]
		public void SingleInsertShouldAllowParameters()
		{
			InsertRecord i = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i };

			var result = Connection().Insert("InsertIdentityReturn2", i, i.Expand(new { OtherValue = 5 }));

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(5, i.Id2);
		}

		/// <summary>
		/// Make sure that a multiple insert can return identities.
		/// Note that here we have to specify the parameters AND the list of objects that we want to treat as an insert.
		/// </summary>
		[Test]
		public void MultipleInsertShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			InsertRecord i2 = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i, i2 };

			var result = Connection().InsertList("InsertByTable", list, new { OtherValue = 5, Items = list });

			Assert.AreEqual(list, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(2, i.Id2);
			Assert.AreEqual(2, i2.Id);
			Assert.AreEqual(2, i2.Id2);
		}

		[Test]
		public void SingleInsertSqlShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i };

			// this would normally be INSERT INTO blah VALUES (@blah) SELECT @@SCOPE_IDENTITY
			var result = Connection().InsertSql<InsertRecord>("SELECT Id=1, Id2=2", i);

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(2, i.Id2);
		}

		[Test]
		public void TestIssue131()
		{
			var i = new InsertRecord();
			var c = new OptimisticConnection((DbConnection)Connection());
			Assert.Throws<OptimisticConcurrencyException>(() => c.InsertSql<InsertRecord>("THROW 51000, 'At least one record has changed or does not exist. (CONCURRENCY CHECK)', 1;", i));
		}

		[Test]
		public void TestIssue131a()
		{
			var i = new InsertRecord();
			var c = new OptimisticConnection((DbConnection)Connection());
			var result = c.InsertSql<InsertRecord>("PRINT ''", i);
		}
		#endregion

		#region Async Tests
		/// <summary>
		/// Make sure that we can call a procedure with the inserted object and have it fill in identities on return.
		/// </summary>
		[Test]
		public void AsyncSingleInsertShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			i.Value = 5;

			var result = Connection().InsertAsync("InsertIdentityReturn", i).Result;

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(5, i.Id2);
		}

		/// <summary>
		/// Make sure that we can call a stored proc with parameters in addition to the object that we are passing in.
		/// Not a common case, but common enough that we should support it.
		/// </summary>
		[Test]
		public void AsyncSingleInsertShouldAllowParameters()
		{
			InsertRecord i = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i };

			var result = Connection().InsertAsync("InsertIdentityReturn2", i, i.Expand(new { OtherValue = 5 })).Result;

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(5, i.Id2);
		}

		/// <summary>
		/// Make sure that a multiple insert can return identities.
		/// Note that here we have to specify the parameters AND the list of objects that we want to treat as an insert.
		/// </summary>
		[Test]
		public void AsyncMultipleInsertShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			InsertRecord i2 = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i, i2 };

			var result = Connection().InsertListAsync("InsertByTable", list, new { OtherValue = 5, Items = list }).Result;

			Assert.AreEqual(list, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(2, i.Id2);
			Assert.AreEqual(2, i2.Id);
			Assert.AreEqual(2, i2.Id2);
		}

		[Test]
		public void AsyncSingleInsertSqlShouldFillInIdentities()
		{
			InsertRecord i = new InsertRecord();
			List<InsertRecord> list = new List<InsertRecord>() { i };

			// this would normally be INSERT INTO blah VALUES (@blah) SELECT @@SCOPE_IDENTITY
			var result = Connection().InsertSqlAsync<InsertRecord>("SELECT Id=1, Id2=2", i).Result;

			Assert.AreEqual(i, result);
			Assert.AreEqual(1, i.Id);
			Assert.AreEqual(2, i.Id2);
		}
		#endregion
	}
}
