using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Microsoft.SqlServer.Types;
using NUnit.Framework;
using Insight.Tests.Cases;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class StructureTest : BaseTest
	{
		#region Default Recordset Test
		[Recordset(typeof(HasDefaultRecordset), typeof(Beer))]
		class HasDefaultRecordset
		{
			public int ID;
			public Beer Beer;
		}

		[Test]
		public void RecordsetAttributeShouldControlDefaultOneToOneMapping()
		{
			var result = Connection().SingleSql<HasDefaultRecordset>("SELECT ID=1, ID=2");
			Assert.AreEqual(1, result.ID);
			Assert.IsNotNull(result.Beer);
			Assert.AreEqual(2, result.Beer.ID);
		}
		#endregion

		#region Default ID Test
		class HasDefaultID
		{
			[RecordId]
			public int Foo;
			public IList<Beer> List;
		}

		[Test]
		public void RecordIDAttributeShouldDefineParentID()
		{
			var result = Connection().QuerySql("SELECT Foo=1; SELECT ParentID=1, ID=2", null,
				Query.Returns(Some<HasDefaultID>.Records)
						.ThenChildren(Some<Beer>.Records))
						.First();

			Assert.AreEqual(1, result.Foo);
			Assert.AreEqual(1, result.List.Count);
			Assert.AreEqual(2, result.List[0].ID);
		}
		#endregion

		#region Default List Test
		class HasDefaultList
		{
			public int ID;

			[ChildRecords]
			public IList<Beer> List;

			// normally this would throw with two lists of beer.
			public IList<Beer> NotList;
		}

		[Test]
		public void ChildRecordsAttributeShouldDefineList()
		{
			var result = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2", null,
				Query.Returns(Some<HasDefaultList>.Records)
						.ThenChildren(Some<Beer>.Records))
						.First();

			Assert.AreEqual(1, result.ID);
			Assert.AreEqual(1, result.List.Count);
			Assert.AreEqual(2, result.List[0].ID);
		}
		#endregion

		#region Inline Column Overrides
		[Test]
		public void CanOverrideColumnsInline()
		{
			var mappedRecords = new OneToOne<HasDefaultRecordset>(
				new ColumnOverride<HasDefaultRecordset>("Foo", "ID"));

			var result = Connection().QuerySql(
				"SELECT Foo=1, ID=2",
				null,
				Query.Returns(mappedRecords)).First();

			Assert.AreEqual(1, result.ID);
			Assert.AreEqual(2, result.Beer.ID);
		}
		#endregion
	}
}