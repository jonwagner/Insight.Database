using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChildMappingTests =Insight.Tests.StructureTest_ChildMappingHelperUnitTests;

namespace Insight.Tests
{
	public class GroupByTests
	{
		#region AutoParentAutoChild Tests
		[TestFixture]
		public class AutoParentAutoChild : BaseTest
		{
			public class Parent
			{
				public int ID;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public class Parent2
			{
				public int ID;
				public List<Child2> Children;
			}

			public class Child2
			{
				public int Value;
				public int ParentID;
			}

			public class Parent3
			{
				public int Parent3_id;
				public List<Child3> Children;
			}

			public class Child3
			{
				public int Value;
				public int Parent3_id;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				// This is the fallback case, where the child's parentID is being set by postion 
				// (Child's ParentID is is not present in the class, so class based mapping hints are not possible)
				var parent = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records))
						.First();
				Verify(parent);
			}

			[Test]
			public void List_MappingOnParentID_InClass()
			{

				// Existing Functionality?? (confirm), note that we are now using child with the parentID property present,
				// so class based mapping hints works
				var parent = Connection().QuerySql(
								"SELECT ID=1; SELECT ID=66, ParentID=1, Value=2 UNION SELECT ID=66, ParentID=1, Value=3", null,
								Query.Returns(Some<Parent2>.Records)
								.ThenChildren(Some<Child2>.Records))
								.First();

				Verify(parent);
			}

			/// <summary>
			/// Detecting Child's parent id by looking at Parent's ID (Parent3_id), auto everything
			/// New Functionality (see issue 169 in root repo) 
			/// </summary>
			[Test]
			public void List_MappingOnParentID_ByParentPK()
			{
				var parent = Connection().QuerySql(
								"SELECT Parent3_id=1; SELECT ID=66, Parent3_id=1, Value=2 UNION SELECT ID=66, Parent3_id=1, Value=3", null,
								Query.Returns(Some<Parent3>.Records)
								.ThenChildren(Some<Child3>.Records))
								.First();

				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{

				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}

			private static void Verify(Parent2 parent)
			{
				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
				Assert.AreEqual(1, parent.Children[0].ParentID);
				Assert.AreEqual(1, parent.Children[1].ParentID);
			}

			private static void Verify(Parent3 parent)
			{
				Assert.AreEqual(1, parent.Parent3_id);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
				Assert.AreEqual(1, parent.Children[0].Parent3_id);
				Assert.AreEqual(1, parent.Children[1].Parent3_id);
			}

		}
		#endregion

		#region AutoParentManualChild Tests
		[TestFixture]
		public class AutoParentManualChild : BaseTest
		{
			public class Parent
			{
				public int ID;
				public List<Child> Children;
			}

			public class Child
			{
				public int OtherID;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT OtherID=1, Value=2 UNION SELECT OtherID=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true, GroupBy = "OtherID")]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT OtherID=1, Value=2 UNION SELECT OtherID=1, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupBy(c => c.OtherID)))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT OtherID=1, Value=2 UNION SELECT OtherID=1, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupBy(c => c.OtherID)));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{

				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID, parent.Children[0].OtherID);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID, parent.Children[1].OtherID);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AutoParentColumnChild Tests
		[TestFixture]
		public class AutoParentColumnChild : BaseTest
		{
			public class Parent
			{
				public int ID;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT x=1, Value=2 UNION SELECT x=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT x=1, Value=2 UNION SELECT x=1, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupByColumn<int>()))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT x=1, Value=2 UNION SELECT x=1, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupByColumn<int>()));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{

				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AttributeParentAutoChildComposite Tests

		[TestFixture]
		public class AttributeParentAutoChild : BaseTest  //Attributes are on the parent, but auto detect the child setup
		{

			public class Parent
			{
				[RecordId]
				public int Int1;
				public int Int2;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
				public int Int3 { get; set; }
				public int ParentId { get; set; }
			}

			public class Parent2
			{
				[RecordId]
				public int ParentKey;
				public int Int2;
				public List<Child2> Children;
			}

			public class Child2
			{
				public int Value;
				public int ChildKey { get; set; }
				public int ParentKey { get; set; }

			}

			/// <summary>
			/// Detecting Child's parent id by its name, "ParentID"
			/// Parent class' ID is set by attribute
			/// </summary>
			[Test]
			public void List_MappingOfParentID_ByConvention()
			{
				var parent = Connection().QuerySql(
								"SELECT Int1=1, Int2=10; " +
								"SELECT ParentId=1, Int3=30, Value=2" + " UNION " +
								"SELECT ParentId=1, Int3=31, Value=3"
								, null,
								Query.Returns(Some<Parent>.Records)
								.ThenChildren(Some<Child>.Records))
								.First();

				Assert.AreEqual(1, parent.Int1 = 1);
				Assert.AreEqual(10, parent.Int2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
				Assert.AreEqual(1, parent.Children[0].ParentId);
				Assert.AreEqual(30, parent.Children[0].Int3);
			}

			/// <summary>
			/// Detecting Child's parent id by looking at Parent's ID
			/// New Functionality (see issue 169 in root repo) 
			/// </summary>
			[Test]
			public void List_MappingOfParentID_ByParentPK()
			{
				var parent = Connection().QuerySql(
								"SELECT ParentKey=1, Int2=10; " +
								"SELECT ChildKey=30, ParentKey=1, Value=2" + " UNION " +
								"SELECT ChildKey=31, ParentKey=1, Value=3"
								, null,
								Query.Returns(Some<Parent2>.Records)
								.ThenChildren(Some<Child2>.Records))
								.First();

				Assert.AreEqual(1, parent.ParentKey = 1);
				Assert.AreEqual(10, parent.Int2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
				Assert.AreEqual(1, parent.Children[0].ParentKey);
				Assert.AreEqual(30, parent.Children[0].ChildKey);
			}

			/// <summary>
			/// Detecting Child's parent id by looking at Parent's ID
			/// Handle InvoiceLine.Invoice_ID => Invoice.ID
			/// aka Glass.BeerId ->Beer.Id
			/// New Functionality (see issue 238 in root repo) 
			/// </summary>
			[Test]
			public void List_MappingOfParentID_ByParentPK_NakedId()
			{
				var beer = Connection().QuerySql(
								"SELECT ID=1, Int2=10; " +
								"SELECT ID=30, Beer238_ID=1, Value=40" + " UNION " +
								"SELECT ID=31, Beer238_ID=1, Value=41"
								, null,
								Query.Returns(Some<ChildMappingTests.TestClasses.Beer238>.Records)
								.ThenChildren(Some<ChildMappingTests.TestClasses.Glass238b>.Records))
								.First();

				Assert.AreEqual(1, beer.ID = 1);
				Assert.AreEqual(2, beer.GlassesB.Count);
				Assert.IsNull(beer.GlassesA);
				Assert.AreEqual(2, beer.GlassesB.Count);
				Assert.AreEqual(1, beer.GlassesB[0].Beer238_ID);
				Assert.AreEqual(1, beer.GlassesB[1].Beer238_ID);
				Assert.AreEqual(40, beer.GlassesB[0].Value);
				Assert.AreEqual(41, beer.GlassesB[1].Value);
			}

		}
		#endregion

		#region AttributeParentAutoChildComposite Tests
		[TestFixture]
		public class AttributeParentAutoChildComposite : BaseTest
		{
			public class Parent
			{
				[RecordId(0)]
				public int ID1;
				[RecordId(1)]
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{

				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AttributeParentColumnChildComposite Tests
		[TestFixture]
		public class AttributeParentColumnChildComposite : BaseTest
		{
			public class Parent
			{
				[RecordId(0)]
				public int ID1;
				[RecordId(1)]
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public class Parent4
			{
				[RecordId(0)]
				public int Int1;
				[RecordId(1)]
				public int Int2;
				public List<Child4> Children;
			}

			public class Child4
			{
				public int Value;
				public int Int1 { get; set; }
				public int Int2 { get; set; }
				public int Int3 { get; set; }
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records.GroupByColumns<int, int>()))
						.First();
				Verify(parent);
			}

			/// <summary>
			/// Detecting Child's parent id by looking at Parent's ID (CompositeKey)
			/// New Functionality (see issue 169 in root repo) 
			[Test]
			public void List_MappingOnParentID_ByParentPK_MultiID()
			{
				// New Functionality, detecting Child's parent id by looking at Parent's ID (multiple properties in ID)
				var parent = Connection().QuerySql(
								"SELECT Int1=1, Int2=10; " +
								"SELECT Int1=1, Int2=10, Int3=30, Value=2" + " UNION " +
								"SELECT Int1=1, Int2=10, Int3=31, Value=3"
								, null,
								Query.Returns(Some<Parent4>.Records)
								.ThenChildren(Some<Child4>.Records))
								.First();

				Assert.AreEqual(1, parent.Int1 = 1);
				Assert.AreEqual(10, parent.Int2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
				Assert.AreEqual(1, parent.Children[0].Int1);
				Assert.AreEqual(10, parent.Children[0].Int2);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupByColumns<int, int>()));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{

				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region ManualParentManualChildComposite Tests
		[TestFixture]
		public class ManualParentManualChildComposite : BaseTest
		{
			public class Parent
			{
				public int ID1;
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int ParentID1;
				public int ParentID2;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true, Id = "ID1,ID2", GroupBy = "ParentID1,ParentID2")]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records.GroupBy(c => Tuple.Create(c.ParentID1, c.ParentID2)), id: p => Tuple.Create(p.ID1, p.ID2)))
						.First();

				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupBy(c => Tuple.Create(c.ParentID1, c.ParentID2)), id: p => Tuple.Create(p.ID1, p.ID2)));

				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID1, parent.Children[0].ParentID1);
				Assert.AreEqual(parent.ID2, parent.Children[0].ParentID2);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID1, parent.Children[1].ParentID1);
				Assert.AreEqual(parent.ID2, parent.Children[1].ParentID2);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region ManualParentAutoChildComposite Tests
		[TestFixture]
		public class ManualParentAutoChildComposite : BaseTest
		{
			public class Parent
			{
				public int ID1;
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true, Id = "ID1,ID2")]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				Func<Parent, Tuple<int, int>> id = (Parent p) => Tuple.Create(p.ID1, p.ID2);

				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records, id: p => Tuple.Create(p.ID1, p.ID2)))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records, id: p => Tuple.Create(p.ID1, p.ID2)));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region ManualParentColumnChildComposite Tests
		[TestFixture]
		public class ManualParentColumnChildComposite : BaseTest
		{
			public class Parent
			{
				public int ID1;
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true, Id = "ID1,ID2")]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records.GroupByColumns<int, int>(), id: p => Tuple.Create(p.ID1, p.ID2)))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records.GroupByColumns<int, int>(), id: p => Tuple.Create(p.ID1, p.ID2)));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AttributeParentManualChildComposite Tests
		[TestFixture]
		public class AttributeParentManualChildComposite : BaseTest
		{
			public class Parent
			{
				[RecordId(0)]
				public int ID1;
				[RecordId(1)]
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				public int ParentID1;
				public int ParentID2;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true, GroupBy = "ParentID1,ParentID2")]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records.GroupBy(c => Tuple.Create(c.ParentID1, c.ParentID2))))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT ParentID1=1, ParentID2=2, Value=2 UNION SELECT ParentID1=1, ParentID2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
					.ThenChildren(Some<Child>.Records.GroupBy(c => Tuple.Create(c.ParentID1, c.ParentID2))));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID1, parent.Children[0].ParentID1);
				Assert.AreEqual(parent.ID2, parent.Children[0].ParentID2);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID1, parent.Children[1].ParentID1);
				Assert.AreEqual(parent.ID2, parent.Children[1].ParentID2);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AutoParentNamedChild Tests
		[TestFixture]
		public class AutoParentNamedChild : BaseTest
		{
			public class Parent
			{
				public int ID;
				public List<Child> Children;
			}

			public class Child
			{
				public int ParentID;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID, parent.Children[0].ParentID);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID, parent.Children[1].ParentID);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AutoParentAttributeChild Tests
		[TestFixture]
		public class AutoParentAttributeChild : BaseTest
		{
			public class Parent
			{
				public int ID;
				public List<Child> Children;
			}

			public class Child
			{
				[ParentRecordId]
				public int Foo;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT Foo=1, Value=2 UNION SELECT Foo=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT Foo=1, Value=2 UNION SELECT Foo=1, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID=1; SELECT Foo=1, Value=2 UNION SELECT ParentID=1, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID, parent.Children[0].Foo);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID, parent.Children[1].Foo);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion

		#region AttributeParentAttributeChildComposite Tests
		[TestFixture]
		public class AttributeParentAttributeChildComposite : BaseTest
		{
			public class Parent
			{
				[RecordId(0)]
				public int ID1;
				[RecordId(1)]
				public int ID2;
				public List<Child> Children;
			}

			public class Child
			{
				[ParentRecordId(0)]
				public int Foo1;
				[ParentRecordId(1)]
				public int Foo2;
				public int Value;
			}

			public interface IGroupBy
			{
				[Sql("SELECT ID1=1, ID2=2; SELECT Foo1=1, Foo2=2, Value=2 UNION SELECT Foo1=1, Foo2=2, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild = true)]
				IList<Parent> Get();
			}

			[Test]
			public void List()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT Foo1=1, Foo2=2, Value=2 UNION SELECT Foo1=1, Foo2=2, Value=3", null,
					Query.Returns(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records))
						.First();
				Verify(parent);
			}

			[Test]
			public void Single()
			{
				var parent = Connection().QuerySql("SELECT ID1=1, ID2=2; SELECT Foo1=1, Foo2=2, Value=2 UNION SELECT Foo1=1, Foo2=2, Value=3", null,
					Query.ReturnsSingle(Some<Parent>.Records)
						.ThenChildren(Some<Child>.Records));
				Verify(parent);
			}

			[Test]
			public void Interface()
			{
				var parent = Connection().As<IGroupBy>().Get().First();
				Verify(parent);
			}

			private static void Verify(Parent parent)
			{
				Assert.AreEqual(1, parent.ID1);
				Assert.AreEqual(2, parent.ID2);
				Assert.AreEqual(2, parent.Children.Count);
				Assert.AreEqual(parent.ID1, parent.Children[0].Foo1);
				Assert.AreEqual(parent.ID2, parent.Children[0].Foo2);
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(parent.ID1, parent.Children[1].Foo1);
				Assert.AreEqual(parent.ID2, parent.Children[1].Foo2);
				Assert.AreEqual(3, parent.Children[1].Value);
			}
		}
		#endregion
	}
}
