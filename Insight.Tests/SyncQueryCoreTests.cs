using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Tests.Cases;
using NUnit.Framework;

namespace Insight.Tests
{
    [TestFixture]
    public partial class SyncQueryCoreTests : BaseTest
    {
        #region Basic Tests
        [Test]
        public void TestAutoClose()
        {
            ConnectionStateCase.ForEach(c =>
            {
                var result = c.Query<Beer>(Beer.SelectAllProc);
                Beer.VerifyAll(result);
            });
        }

        [Test]
        public void TestForceClose()
        {
            ConnectionStateCase.ForEach(c =>
            {
                bool wasOpen = c.State == ConnectionState.Open;

                c.Query<Beer>(Beer.SelectAllProc, commandBehavior: CommandBehavior.CloseConnection);

                Assert.AreEqual(ConnectionState.Closed, c.State);
                if (wasOpen)
                    c.Open();
            });
        }

        [Test]
        public void TestOutputParameters()
        {
            var input = new InOutParameters { In = 5 };
            var output = new OutParameters();

            Connection().Query<Beer>(InOutParameters.ProcName, input, outputParameters: output);

            input.Verify(output);
        }
        #endregion

        [Test]
        public void TestFastExpandoResult()
        {
            var result = Connection().QuerySql("SELECT Field123=123, FieldAbc='abC'");

            Assert.AreEqual(1, result.Count());

            dynamic row = result.First();
            Assert.IsNotNull(row);

            Assert.AreEqual(row.Field123, 123);
            Assert.AreEqual(row.FieldAbc, "abC");
        }

        [Test]
        public void TestChildrenWithAutoMapping()
        {
            // child records that include a one-to-one mapping

            var result = Connection().QuerySql(
                Beer.GetSelectNestedChildren(1, 2),
                null,
                Query.Returns(Some<InfiniteBeerList>.Records)
                    .ThenChildren(OneToOne<InfiniteBeerList, InfiniteBeer>.Records));

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, result[0].List.Count());
            Assert.IsNotNull(result[0].List[0].More);
            result[0].List[0].More.Verify();
        }

        [Test]
        public void TestChildrenWithAutoMappingAndWrongIDType()
        {
            // child records that include a one-to-one mapping
            // but the object has parentid=int, and the sql has parentid=string

            var result = Connection().QuerySql(
                "SELECT ID=1; SELECT ParentID='1', ID=2",
                null,
                Query.Returns(Some<InfiniteBeerList>.Records)
                    .ThenChildren(OneToOne<InfiniteBeerList, InfiniteBeer>.Records));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result[0].List.Count());
        }

        [Test]
        public void TestChildrenWithAutoMappingFields()
        {
            // child records that include a one-to-one mapping

            var result = Connection().QuerySql(
                Beer.GetSelectNestedChildren(1, 2),
                null,
                Query.Returns(Some<InfiniteBeerListWithFields>.Records)
                    .ThenChildren(Some<InfiniteBeerListWithFields>.Records));
        }

        [Test]
        public void TestChildrenWithOneToOne()
        {
            // child records that include a one-to-one mapping

            var result = Connection().QuerySql(
                Beer.GetSelectNestedChildren(1, 2),
                null,
                Query.Returns(Some<InfiniteBeerList>.Records)
                    .ThenChildren(OneToOne<InfiniteBeerList, InfiniteBeer>.Records));

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, result[0].List.Count());
            Assert.IsNotNull(result[0].List[0].More);
            result[0].List[0].More.Verify();
        }

        [Test]
        public void TestIssue426()
        {
            // allow NOBIND to inform insight to not bind the parameter
            var result = Connection().ExecuteScalarSql<int>("DECLARE /*NOBIND*/ @i int = 5, @j int; SELECT @i", new { i = 10 });
            Assert.AreEqual(result, 5);
        }
    }
}
