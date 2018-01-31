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

        #region Read-Only List Test
        class HasReadOnlyList
        {
            public int ID;

            public IList<Beer> List { get { return _list; } }
            private readonly List<Beer> _list = new List<Beer>();
        }

        [Test]
        public void ReadOnlyListShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2", null,
                    Query.Returns(Some<HasReadOnlyList>.Records)
                            .ThenChildren(Some<Beer>.Records))
                            .First());
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

        #region Multi-Class Reader Tests
        class MyClass
        {
        }

        class MyClassA : MyClass
        {
            public int A;
        }

        class MyClassB : MyClass
        {
            public int B;
        }

        [Test]
        public void MultiReaderCanDeserializeDifferentClasses()
        {
            var mr = new MultiReader<MyClass>(
                reader =>
                {
                    switch ((string)reader["Type"])
                    {
                        default:
                        case "a":
                            return OneToOne<MyClassA>.Records;
                        case "b":
                            return OneToOne<MyClassB>.Records;
                    }
                });

            var results = Connection().QuerySql("SELECT [Type]='a', A=1, B=NULL UNION SELECT [Type]='b', A=NULL, B=2", Parameters.Empty, Query.Returns(mr));

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results[0] is MyClassA);
            Assert.AreEqual(1, ((MyClassA)results[0]).A);
            Assert.IsTrue(results[1] is MyClassB);
            Assert.AreEqual(2, ((MyClassB)results[1]).B);
        }

        [Test]
        public void PostProcessCanReadFieldsInAnyOrder()
        {
            var pr = new PostProcessRecordReader<MyClassA>(
                (reader, a) =>
                {
                    if (reader["Type"].ToString() == "a")
                        a.A = 9;
                    return a;
                });

            var results = Connection().QuerySql("SELECT [Type]='a', A=1, B=NULL", Parameters.Empty, Query.Returns(pr));

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0] is MyClassA);
            Assert.AreEqual(9, ((MyClassA)results[0]).A);
        }
        #endregion

        #region Issue 109 Tests
        public class ParentFor109
        {
            public int ID;
            public List<ChildFor109> Children;
        }

        public class ChildFor109
        {
            public int Foo;
        }

        [Test]
        public void ColumnMappingShouldWorkForChildRecordsets()
        {
            var childReader = new OneToOne<ChildFor109>(new ColumnOverride("Bar", "Foo"));

            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Bar=3", Parameters.Empty,
                            Query.Returns(Some<ParentFor109>.Records)
                                .ThenChildren(childReader)).First();

            Assert.AreEqual(1, results.Children.Count);
            Assert.AreEqual(3, results.Children[0].Foo);
        }
        #endregion

        #region Issue 199 Tests
        public abstract class BaseRecord
        {
            public abstract int? Id { get; set; }
        }

        public class ChildRecord : BaseRecord
        {
            [Column("ChildRecordID")]
            public override int? Id { get; set; }

            internal GrandChildRecord GrandChild { get; set; }
        }

        public class GrandChildRecord : BaseRecord
        {
            [Column("GrandChildRecordId")]
            public override int? Id { get; set; }
        }

        [Test]
        public void TestIssue199()
        {
            // overriding Id two different ways was causing a conflict in some of the internal reflection structures
            var result = Connection().SingleSql<ChildRecord, GrandChildRecord>("SELECT ChildRecordID=12345, GrandChildRecordId=99999");
            Assert.AreEqual(12345, result.Id);
            Assert.AreEqual(99999, result.GrandChild.Id);
        }
        #endregion
    }

    #region Single Child Tests
    [TestFixture]
    public class SingleChildTest : BaseTest
    {
        public class Parent
        {
            public int ID;
            public Child Child;
        }

        public class Child
        {
            public int ID;
            public string Name;
        }

        public interface IHaveSingleChild
        {
            [Sql("SELECT ID=1; SELECT ParentID=1, ID=2, Name='Alice'")]
            [Recordset(1, typeof(Child), Into = "Child", IsChild = true)]
            Parent GetParent();
        }

        [Test]
        public void CanHaveSingleChild()
        {
            var result = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2, Name='Alice'", null,
                Query.Returns(Some<Parent>.Records).ThenChildren(Some<Child>.Records, into: (p, l) => p.Child = l.First())).First();

            Assert.AreEqual("Alice", result.Child.Name);
        }

        [Test]
        public void CanHaveSingleChildAutoDetect()
        {
            var result = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2, Name='Alice'", null,
                Query.Returns(Some<Parent>.Records).ThenChildren(Some<Child>.Records)).First();

            Assert.AreEqual("Alice", result.Child.Name);
        }

        [Test]
        public void InterfaceCanHaveSingleChild()
        {
            var result = Connection().As<IHaveSingleChild>().GetParent();

            Assert.AreEqual("Alice", result.Child.Name);
        }

    }
    #endregion

    #region ChildMappingHelper Unit Tests
    [TestFixture]
    public class StructureTest_ChildMappingHelperUnitTests
    {
        [Test]
        public void GetIDAccessor_PlainID()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ID", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ClassNamePlusUnderscoreID_Simple()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.Invoice));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Invoice_id", result.ElementAt(0));
        }

        /// <summary>
        /// This is the case where there are multiple fields ending with '_'
        /// It should first try to match to [ClassName]_ID
        /// New Functionality (see issue 169 in root repo)
        /// </summary>
        [Test]
        public void GetIDAccessor_ClassNamePlusUnderscoreID_MultipleOptions()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.InvoiceLine));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("InvoiceLine_id", result.ElementAt(0));
        }

        /// <summary>
        /// This is the case where there are multiple fields ending with '_'
        /// New Functionality (see issue 169 in root repo)
        /// </summary>
        [Test]
        public void GetParentIDAccessor_UseParentsIDToGetParentID()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.InvoiceLine), null, typeof(TestClasses.Invoice));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Invoice_id", result.ElementAt(0));
        }

        /// <summary>
        /// Issue 238: Support myClass.BeerId ->Beer.ID
        /// </summary>
        [Test]
        public void GetParentIDAccessor_WhenPKIsJustId()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.Glass238a), null, typeof(TestClasses.Beer238));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Beer238ID", result.ElementAt(0));
        }

        /// <summary>
        /// Issue 238: Support myClass.BeerId ->Beer.ID
        /// </summary>
        [Test]
        public void GetParentIDAccessor_WhenPKIsJustId_Underscored()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.Glass238b), null, typeof(TestClasses.Beer238));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Beer238_ID", result.ElementAt(0));
        }


        // throws System.InvalidOperationException 'Sequence contains more than one matching element'
        // This was barfing on   public int Paid as it end with ID			 
        /// It should first try to match to [ClassName]ID aka  public int ClassWithSuffixedId;  
        /// New Functionality (see issue 169 in root repo) 
        [Test]
        public void GetIDAccessor_ClassNamePlusId_WithOtherIdFields()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixed));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ClassWithSuffixedId", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ClassNamePlusId()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixed2));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ClassWithSuffixed2Id", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ClassNamePlusUnderscoreID()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixedUnderscore));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ClassWithSuffixedUnderscore_ID", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ByName()
        {
            //-----------------test single fields-----------------
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID), "ID");
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ID", result.ElementAt(0));

            result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID), "AnotherID");
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("AnotherID", result.ElementAt(0));

            //-----------------test multiple fields-----------------
            result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID), "ID, AnotherID");
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("ID", result.ElementAt(0));
            Assert.AreEqual("AnotherID", result.ElementAt(1));

            // take out the spaces
            result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID), "ID,AnotherID");
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("ID", result.ElementAt(0));
            Assert.AreEqual("AnotherID", result.ElementAt(1));

            // reverse the order
            result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithPlainID), "AnotherID, ID");
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("AnotherID", result.ElementAt(0));
            Assert.AreEqual("ID", result.ElementAt(1));
        }

        /// <summary> Verifies bad requests throw the expected exceptions</summary>
        [Test]
        public void GetIDAccessor_ByName_BadName()
        {
            var ex = Assert.Throws<InvalidOperationException>(()
                    => ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixedUnderscore), "foo123"));
            Assert.That(ex.Message, Does.Contain("foo123"));

            ex = Assert.Throws<InvalidOperationException>(()
                    => ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixedUnderscore), "ClassWithSuffixedUnderscore_ID, foo123"));
            Assert.That(ex.Message, Does.Contain("foo123"));

            ex = Assert.Throws<InvalidOperationException>(()
                    => ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixedUnderscore), "foo123, ClassWithSuffixedUnderscore_ID"));
            Assert.That(ex.Message, Does.Contain("foo123"));

            // we could clean up trailing commas, but this test verifies current behavior
            ex = Assert.Throws<InvalidOperationException>(()
                    => ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithSuffixedUnderscore), "ID, "));
        }


        /// <summary> Tests the base case, a field named ParentID</summary>
        [Test]
        public void FindParentIDAccessor_ChildWithGenericNames()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.ChildWithGenericNames), null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ParentID", result.ElementAt(0));
        }

        [Test]
        public void FindParentIDAccessor_ChildEndingWithUnderscoreParentID()
        {
            // This is case in the rules, a field ending in _ParentID
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.ChildEndingWithUnderscoreParentID), null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("My_ParentID", result.ElementAt(0));
        }

        [Test]
        public void FindParentIDAccessor_ChildEndingWithParentID()
        {
            // This is case in the rules, a field ending in ParentID
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.ChildEndingWithParentID), null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("MyParentID", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ByAttribute()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.InvoiceLineWithAttributes));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("InvoiceLine_id", result.ElementAt(0));
        }

        [Test]
        public void GetParentIDAccessor_ByAttribute()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.InvoiceLineWithAttributes), null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Invoice_id", result.ElementAt(0));
        }

        [Test]
        public void GetIDAccessor_ByMultiAttribute()
        {
            var result = ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.InvoiceLineWithMultiAttributes));

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("InvoiceLine_id", result.ElementAt(0));
            Assert.AreEqual("Company_id", result.ElementAt(1));
        }

        [Test]
        public void GetParentIDAccessor_ByMultiAttribute()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.InvoiceLineWithMultiAttributes), null, null);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Company_id", result.ElementAt(0));
            Assert.AreEqual("Invoice_id", result.ElementAt(1));
        }

        [Test]
        public void GetIDAccessor_ClassWithNoDetectableID()
        {
            var ex = Assert.Throws<InvalidOperationException>(()
                            => ChildMapperTestingHelper.GetIDAccessor(typeof(TestClasses.ClassWithInsufficientData)));

            Assert.That(ex.Message, Does.StartWith("Cannot find a way to get the ID from"));
        }

        [Test]
        public void GetParentIDAccessor_ClassWithNoDetectableParentID()
        {
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.ClassWithInsufficientData), null, null);

            // FindParentIDAccessor does not throw an exception like FindIDAccessor does:
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void FindParentIDAccessor_RespectReservedWords()
        {
            // This is case in the rules, a field ending in _ParentID
            var result = ChildMapperTestingHelper.FindParentIDAccessor(typeof(TestClasses.Parent), null, typeof(TestClasses.Grandparent));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("GrandparentID", result.ElementAt(0));
        }

        //*************************** Test Classes ***************************
        public class TestClasses
        {

            public class ClassWithPlainID
            {
                public int AnotherID;
                public int ID;   //should match this first
                public ChildWithGenericNames ChildWithGenericNames;
                public string Name;
            }

            public class ClassWithSuffixed
            {
                public int ClassWithSuffixedId;  //should match this first
                public ChildWithGenericNames ChildWithGenericNames;
                public int Paid;
                public string Name;
            }

            public class ClassWithSuffixed2
            {
                public int ClassWithSuffixed2Id;  //should match this first
                public ChildWithGenericNames ChildWithGenericNames;
                //public int Paid;
                public string Name;
            }

            public class ClassWithSuffixedUnderscore
            {
                public int AnotherID;
                public int ClassWithSuffixedUnderscoreID;
                public int ClassWithSuffixedUnderscore_ID;      //should match this first
                public ChildWithGenericNames ChildWithGenericNames;
                public string Name;
            }

            public class ChildWithGenericNames
            {
                public int ID;
                public int ParentID;
                public string Name;
            }

            public class ChildEndingWithUnderscoreParentID
            {
                public int ID;
                public int My_ParentID;
                public string Name;
            }

            public class ChildEndingWithParentID
            {
                public int ID;
                public int MyParentID;
                public string Name;
            }

            public class Invoice
            {
                public int InvoiceDisruptor_id;  // baiting the for a mismatch
                public int Invoice_id;  //ID
                public string Name;
            }

            public class InvoiceLine
            {
                public int InvoiceLineDisruptor_id;  // baiting the for a mismatch
                public int InvoiceLine_id;  //ID
                public int Invoice_id;      //ParentId
                public string Name;
            }

            public class InvoiceLineWithAttributes
            {
                [RecordId]
                public int InvoiceLine_id;  //ID
                [ParentRecordId]
                public int Invoice_id;      //ParentId
                public string Name;
            }

            public class InvoiceLineWithMultiAttributes
            {
                [RecordId]
                public int InvoiceLine_id;  //ID

                [RecordId]
                [ParentRecordId]
                public int Company_id;  //ID, ParentId

                [ParentRecordId]
                public int Invoice_id;      //ParentId

                public string Name;
            }

            public class ClassWithInsufficientData
            {
                public int NotMe;
                public int Nope;
                public ChildWithGenericNames ChildWithGenericNames;
                public string Name;
            }

            public class Grandparent
            {
                public int GrandparentID;
                public int ParentId;
                public ChildWithGenericNames ChildWithGenericNames;
                public string Name;
            }

            public class Parent
            {
                public int GrandparentID;
                public int ParentId;
                public ChildWithGenericNames ChildWithGenericNames;
                public string Name;
            }

            public class Beer238
            {
                public int ID;
                public IList<Glass238a> GlassesA;
                public IList<Glass238b> GlassesB;
            }

            public class Glass238a
            {
                public int ID;
                public int Beer238ID;
            }

            public class Glass238b
            {
                public int ID;
                public int Beer238_ID;
                public int Value;
            }

        }
    }
    #endregion

    #region Issue 130 Tests
    [TestFixture]
    public class Issue130 : BaseTest
    {
        public class Test
        {
            [RecordId]
            public int Id { get; set; }
            public string Name { get; set; }
            public Nullable<System.DateTimeOffset> Created { get; set; }
            [ChildRecords]
            public virtual ICollection<Test2> Test2 { get; set; }
        }

        public class Test2
        {
            public int Id { get; set; }
            public int TestId { get; set; }
            public string Name { get; set; }
            public Nullable<System.DateTimeOffset> Created { get; set; }
        }

        /// <summary>
        /// Virtual ID getter or list accessor was failing.
        /// </summary>
        [Test]
        public void Test130()
        {
            var results = Connection().QuerySql("SELECT id=1, name='parent'; SELECT Id=1, testid=2, name='sub'",
                Parameters.Empty,
                Query.Returns(Some<Test>.Records).ThenChildren(Some<Test2>.Records));
        }
    }

    [TestFixture]
    public class Issue129 : BaseTest
    {
        public class TestParent { public int ParentA; public int ParentB; public TestChild Child; public int ParentC; }
        public class TestChild { public int ChildA; public int ChildB; }

        [Test]
        public void Test129()
        {
            // child is not null, so child should not be null
            var results = Connection().QuerySql<TestParent, TestChild>("SELECT ParentA=1, ParentB=2, ChildA=3, ChildB=4, ParentC=5");
            var result = results.First();
            Assert.AreEqual(1, result.ParentA);
            Assert.AreEqual(2, result.ParentB);
            Assert.AreEqual(3, result.Child.ChildA);
            Assert.AreEqual(4, result.Child.ChildB);
            Assert.AreEqual(0, result.ParentC);

            // all of child is null, so the child should be null
            results = Connection().QuerySql<TestParent, TestChild>("SELECT ParentA=1, ParentB=2, ChildA=NULL, ChildB=NULL, ParentC=5");
            result = results.First();
            Assert.AreEqual(1, result.ParentA);
            Assert.AreEqual(2, result.ParentB);
            Assert.AreEqual(null, result.Child);
            Assert.AreEqual(0, result.ParentC);
        }
    }

    [TestFixture]
    public class Issue141 : BaseTest
    {
        public class TestParent
        {
            public int ID;
            public List<TestChild> Children;
        }

        public class TestChild
        {
            public int ChildA;
            public int ChildB;
            public TestChild(int a, int b)
            {
                ChildA = a;
                ChildB = b;
            }
        }

        [Test]
        public void Test141()
        {
            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ChildA=2, ChildB=3", null,
                Query.Returns(Some<TestParent>.Records)
                    .ThenChildren(CustomRecordReader<TestChild>.Read(r => new TestChild((int)r["ChildA"], (int)r["ChildB"]))));

            var result = results.First();
            Assert.AreEqual(1, result.ID);
            Assert.AreEqual(1, result.Children.Count);
        }
    }
    #endregion

    #region Issue 142 Tests
    [TestFixture]
    public class Issue142 : BaseTest
    {
        public class Parent
        {
            public int ID;
            public List<string> Children;
        }

        [Test]
        public void AtomicChildRecords()
        {
            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, Name='Child1' UNION SELECT ParentID=1, Name='Child2'", null,
                Query.Returns(Some<Parent>.Records)
                    .ThenChildren(Some<string>.Records));

            var result = results.First();
            Assert.AreEqual(1, result.ID);
            Assert.AreEqual(2, result.Children.Count);
            Assert.AreEqual("Child1", result.Children[0]);
            Assert.IsTrue(result.Children.Contains("Child1"));
            Assert.IsTrue(result.Children.Contains("Child2"));
        }
    }
    #endregion

    #region Issue 146 Tests
    [TestFixture]
    public class Issue146 : BaseTest
    {
        public class LiquorStore
        {
            public int ID;
            public List<Liquor> Beer;
        }

        public class Liquor
        {
            public int ID;
        }

        public interface IHaveStructure3
        {
            [Sql("SELECT TOP(0) ID=1; SELECT TOP(0) ParentID=1, ID=2;")]
            [Recordset(1, typeof(Liquor), IsChild = true)]
            LiquorStore GetNoLiquorStore();
        }

        [Test]
        public void ReturnsNullForEmptyParentChildSets()
        {
            var i = Connection().As<IHaveStructure3>();
            var result = i.GetNoLiquorStore();

            Assert.IsNull(result);
        }
    }
    #endregion

    #region Issue 160 Tests
    [TestFixture]
    public class Issue160 : BaseTest
    {
        public class TestParent
        {
            public int ID;
            public List<TestChild> Children;
        }

        public class TestChild
        {
            public int ChildA;
            public int ChildB;
            public TestChild() { }
            public TestChild(int a, int b)
            {
                ChildA = a;
                ChildB = b;
            }
        }

        [Test]
        public void CustomRecordReaderCanDropNulls()
        {
            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ChildA=2, ChildB=3", null,
                        Query.Returns(Some<TestParent>.Records)
                            .ThenChildren(CustomRecordReader<TestChild>.Read(r =>
                            {
                                var childValue = (int)r["ChildA"];
                                if (childValue != 2)
                                    return new TestChild(childValue, (int)r["ChildB"]);
                                return null;
                            }),
                            into: (p, children) => p.Children = children.Where(c => c != null).ToList()));

            var result = results.First();
            Assert.AreEqual(1, result.ID);
            Assert.AreEqual(0, result.Children.Count);  // FAIL! I would expect that the child wouldn't be added, but a null is added
        }
    }
    #endregion

    [TestFixture]
    public class Issue188 : BaseTest
    {
        class Parent
        {
            public int ID;
            public List<Child> Children;
        }

        class Child
        {
            public int ParentID;
            public int ID;
        }

        [Test]
        public void ResultsCanBeContinuedWithThenChildren()
        {
            var returns =
                Query.Returns(Some<Parent>.Records)
                    .ThenChildren(Some<Child>.Records)
                    .Then(Some<Parent>.Records)
                    .ThenChildren(Some<Child>.Records);

            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2; SELECT ID=2; SELECT ParentID=2, ID=3", null, returns);
            Assert.AreEqual(2, results.Set2[0].ID);
            Assert.AreEqual(3, results.Set2[0].Children[0].ID);
        }

        [Test]
        public void ResultsCanBeContinuedWithThenChildren2()
        {
            var returns =
                Query.Returns(Some<Parent>.Records)
                    .ThenChildren(Some<Child>.Records)
                    .Then(Some<Parent>.Records)
                    .ThenChildren<Parent, Parent, Child, int>(Some<Child>.Records.GroupBy(c => c.ParentID));

            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2; SELECT ID=2; SELECT ParentID=2, ID=3", null, returns);
            Assert.AreEqual(2, results.Set2[0].ID);
            Assert.AreEqual(3, results.Set2[0].Children[0].ID);
        }
    }

    [TestFixture]
    public class Issue265 : BaseTest
    {
        class Parent
        {
            public int? ID;
            public List<int> Children;
        }

        [Test]
        public void ChildMapsToParentWithNullableId()
        {
            var returns = Query.Returns(Some<Parent>.Records)
                .ThenChildren(Some<int>.Records, x => x.ID, (p, r) => p.Children = r.Select(x => x).ToList());
            var results = Connection().QuerySql("SELECT ID=1; SELECT ParentID=1, ID=2;", null, returns);
            Assert.AreEqual(1, results[0].ID);
            Assert.AreEqual(2, results[0].Children[0]);
        }
    }

    #region Issue 343
    [TestFixture]
    public class Issue343Test : BaseTest
    {
        public class Parent
        {
            public int ID;
            public List<Child> children;
        }

        public class Child
        {
            public string Name;
            public bool WasPostProcessed;
        }

        public static PostProcessRecordReader<Child> PostProcess = new PostProcessRecordReader<Child>(
            (reader, child) =>
            {
                if (reader == null)
                {
                    throw new InvalidOperationException("Need to have a reader during postprocessing");
                }
                if (child != null)
                {
                    child.Name += " Postprocessed";
                    child.WasPostProcessed = true;
                }

                return child;
            });

        [Test]
        public void PostProcessRecordReaderCanProcessChildrenFromSingleReader()
        {
            var parents = Connection().QuerySql("SELECT ID=1; SELECT ID=1, Name='Child'",
                    null,
                    Query.ReturnsSingle(Some<Parent>.Records)
                        .ThenChildren(PostProcess));

            Child child = parents.children.First();
            Assert.IsTrue(child.WasPostProcessed);
            Assert.AreEqual("Child Postprocessed", child.Name);
        }

        [Test]
        public void PostProcessRecordReaderCanProcessChildrenFromMultiReaderWithAutoID()
        {
            var parents = Connection().QuerySql("SELECT ID=1; SELECT ID=1, Name='Child'",
                    null,
                    Query.Returns(Some<Parent>.Records)
                        .ThenChildren(PostProcess));

            Child child = parents.First().children.First();
            Assert.IsTrue(child.WasPostProcessed);
            Assert.AreEqual("Child Postprocessed", child.Name);
        }

        [Test]
        public void PostProcessRecordReaderCanProcessChildrenFromMultiReaderWithManualID()
        {
            var parents = Connection().QuerySql("SELECT ID=1; SELECT ID=1, Name='Child'",
                    null,
                    Query.Returns(Some<Parent>.Records)
                        .ThenChildren(PostProcess));

            Child child = parents.First().children.First();
            Assert.IsTrue(child.WasPostProcessed);
            Assert.AreEqual("Child Postprocessed", child.Name);
        }
    }
    #endregion

    #region Issue 345
    [TestFixture]
    public class SelfReferencingTests : BaseTest
    {
        public class SelfReferencing
        {
            public int ID;
            public int ParentID;
            public SelfReferencing Parent;
        }

        [Test]
        public void SelfReferencingClassShouldAssignWithManualFunctions()
        {
            var results = Connection().QuerySql("SELECT ID=2, ParentID=1 UNION SELECT ID=1, ParentID=NULL",
                    null,
                    Query.Returns(Some<SelfReferencing>.Records)
                         .SelfReferencing(id: r => r.ID, parentId: r => r.ParentID, into: (c, p) => c.Parent = p)
                    );

            SelfReferencing parent = results.First(s => s.ID == 1);
            SelfReferencing child = results.First(s => s.ID == 2);

            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void SelfReferencingClassShouldAssignWithAutomaticFunctions()
        {
            var results = Connection().QuerySql("SELECT ID=2, ParentID=1 UNION SELECT ID=1, ParentID=NULL",
                    null,
                    Query.Returns(Some<SelfReferencing>.Records)
                         .SelfReferencing()
                    );

            SelfReferencing parent = results.First(s => s.ID == 1);
            SelfReferencing child = results.First(s => s.ID == 2);

            Assert.AreEqual(parent, child.Parent);
        }
    }
    #endregion
}
