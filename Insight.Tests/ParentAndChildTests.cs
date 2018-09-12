using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using Insight.Tests.Cases;
using Insight.Database.Structure;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class ParentAndChildrenTests : BaseTest
	{
		private const string query = @"
				SELECT GrandParentID=0, ParentID=1, ParentName='Parent', ChildID=11, ChildName='ChildA' UNION
				SELECT GrandParentID=0, ParentID=1, ParentName='Parent', ChildID=12, ChildName='ChildB'
			";

		#region Query Structure Tests
		[Test]
		public void CanReadParentAndChild()
		{
            var results = Connection().QuerySql(query, null,
				Query.Returns(Together<Parent, Child>.Records));

			Validate(results);
		}

		[Test]
		public void CanReadParentAndChildAsync()
		{
            var results = Connection().QuerySqlAsync(query, null,
				Query.Returns(Together<Parent, Child>.Records))
				.Result;

			Validate(results);
		}

		[Test]
		public void CanChainReaderWithMultipleResults()
		{
            var results = Connection().QuerySql(query + ";" + query, null,
				Query.Returns(Together<Parent, Child>.Records)
					.Then(Together<Parent, Child>.Records));

			Validate(results.Set1);
			Validate(results.Set2);
		}	

		[Test]
		public void CanChainReaderAsChildrenWithGroupBy()
		{
           var results = Connection().QuerySql("SELECT GrandParentID=0;" + query, null,
				Query.Returns(Some<Grandparent>.Records)
					.ThenChildren(Together<Parent, Child>.Records.GroupBy(p => p.GrandParentID), 
									id: g => g.GrandParentID,
									into: (g, list) => g.Parents = list));

			Validate(results[0].Parents);
		}	

		[Test]
		public void CanChainReaderAsChildrenWithGroupByUnusedColumn()
		{
           var results = Connection().QuerySql("SELECT GrandParentID=0;" + query, null,
				Query.Returns(Some<Grandparent>.Records)
					.ThenChildren(Together<Parent, Child>.Records.GroupByColumn<int>(), 
									id: g => g.GrandParentID,
									into: (g, list) => g.Parents = list));

			Validate(results[0].Parents);
		}	
		#endregion

		#region Interface Tests
		public interface IReadParentAndChildren
		{
			[Sql(query)]
			[Recordset(0, typeof(Parent), typeof(Child), IsParentAndChild = true)]
			IList<Parent> CanReturnParentAndChild();

			[Sql(query)]
			[Recordset(0, typeof(Parent), typeof(Child), IsParentAndChild = true)]
			Task<IList<Parent>> CanReturnParentAndChildAsync();

			[Sql(query + ";" + query)]
			[Recordset(0, typeof(Parent), typeof(Child), IsParentAndChild = true)]
			[Recordset(1, typeof(Parent), typeof(Child), IsParentAndChild = true)]
			Results<Parent, Parent> CanReturnParentAndChildWithMultipleResults();			
		}

		[Test]
		public void InterfaceCanReturnParentAndChild()
		{
			var results = Connection().As<IReadParentAndChildren>().CanReturnParentAndChild();

			Validate(results);
		}

		[Test]
		public void InterfaceCanReturnParentAndChildAsync()
		{
			var results = Connection().As<IReadParentAndChildren>().CanReturnParentAndChildAsync().Result;

			Validate(results);
		}	

		[Test]
		public void InterfaceCanChainReaderWithMultipleResults()
		{
			var results = Connection().As<IReadParentAndChildren>().CanReturnParentAndChildWithMultipleResults();

			Validate(results.Set1);
			Validate(results.Set2);
		}				
		#endregion

		#region Validation Methods
		private void Validate(IList<Parent> results)
		{
			Assert.AreEqual(1, results.Count);

			Validate(results[0]);
		}

		private void Validate(Parent parent)
		{
			Assert.AreEqual(1, parent.ParentID);
			Assert.AreEqual("Parent", parent.ParentName);
			
			var children = parent.Children;
			Assert.AreEqual(2, children.Count);
			Assert.AreEqual(11, children[0].ChildID);
			Assert.AreEqual("ChildA", children[0].ChildName);
			Assert.AreEqual(12, children[1].ChildID);
			Assert.AreEqual("ChildB", children[1].ChildName);
		}
		#endregion

		#region Test Classes
		public class Grandparent
		{
			public int GrandParentID;
			public List<Parent> Parents;
		}

		public class Parent
		{
			public int GrandParentID;
			public int ParentID;
			public string ParentName;
			public List<Child> Children;
		}

		public class Child
		{
			public int ChildID;
			public string ChildName;
		}
		#endregion
	}
}
