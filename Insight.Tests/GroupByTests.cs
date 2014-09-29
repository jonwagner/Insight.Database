using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			public interface IGroupBy
			{
				[Sql("SELECT ID=1; SELECT ParentID=1, Value=2 UNION SELECT ParentID=1, Value=3")]
				[Recordset(0, typeof(Parent))]
				[Recordset(1, typeof(Child), IsChild=true)]
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
				Assert.AreEqual(2, parent.Children[0].Value);
				Assert.AreEqual(3, parent.Children[1].Value);
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
				[Recordset(1, typeof(Child), IsChild = true, GroupBy="OtherID")]
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
				[Recordset(1, typeof(Child), IsChild = true, GroupBy="ParentID1,ParentID2")]
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
