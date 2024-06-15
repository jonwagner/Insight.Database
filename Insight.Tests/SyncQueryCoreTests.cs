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
using NUnit.Framework.Legacy;

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

                ClassicAssert.AreEqual(ConnectionState.Closed, c.State);
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

            ClassicAssert.AreEqual(1, result.Count());

            dynamic row = result.First();
            ClassicAssert.IsNotNull(row);

            ClassicAssert.AreEqual(row.Field123, 123);
            ClassicAssert.AreEqual(row.FieldAbc, "abC");
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

            ClassicAssert.AreEqual(3, result.Count());
            ClassicAssert.AreEqual(1, result[0].List.Count());
            ClassicAssert.IsNotNull(result[0].List[0].More);
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

            ClassicAssert.AreEqual(1, result.Count());
            ClassicAssert.AreEqual(1, result[0].List.Count());
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

            ClassicAssert.AreEqual(3, result.Count());
            ClassicAssert.AreEqual(1, result[0].List.Count());
            ClassicAssert.IsNotNull(result[0].List[0].More);
            result[0].List[0].More.Verify();
        }

        [Test]
        public void TestIssue426()
        {
            // allow NOBIND to inform insight to not bind the parameter
            var result = Connection().ExecuteScalarSql<int>("DECLARE /*NOBIND*/ @inky int = 5, @j int; SELECT @inky", new { inky = 10 });
            ClassicAssert.AreEqual(result, 5);
        }

        #region Issue512
        public interface ITestIssue512
        {
            [Sql("SELECT Name FROM Beer WHERE name like '%' + /* TYPE:String(1500) */ @name + '%'")]
            IList<string> SelectWithContains(string name);

            [Sql("SELECT /* TYPE:Decimal(10) */ @value")]
            decimal SelectDecimalWithPrecision(decimal value);

            [Sql("SELECT /* TYPE:Decimal(10,2) */ @value")]
            decimal SelectDecimalWithPrecisionAndScale(decimal value);
        }

        [Test]
        public void TestIssue512()
        {
            Connection().ExecuteSql("INSERT INTO [Beer] VALUES ('google ipa', 'http://www.google.com')");
            Connection().ExecuteSql("INSERT INTO [Beer] VALUES ('microsoft pale ale', 'http://www.microsoft.com')");

            var result = Connection().As<ITestIssue512>().SelectWithContains("google");
            ClassicAssert.IsTrue(result.Contains("google ipa"));

            var decimalValue = Connection().As<ITestIssue512>().SelectDecimalWithPrecision(123.45678m);
            ClassicAssert.AreEqual(123.45678m, decimalValue);

            var decimalValueWithScale = Connection().As<ITestIssue512>().SelectDecimalWithPrecisionAndScale(123.45678m);
            ClassicAssert.AreEqual(123.45m, decimalValueWithScale);
        }
        #endregion
    }
}
