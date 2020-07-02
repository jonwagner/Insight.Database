#if !NO_OLEDB
using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Providers.OleDb;

namespace Insight.Tests
{
    [TestFixture]
    public class OleDbTests : BaseTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void NamedParametersAreConvertedToPositionalParameters()
        {
            var c = new System.Data.OleDb.OleDbConnection(
                        String.Format("Provider=MSOLEDBSQL;Server={0};Uid={1};Pwd={2};Trusted_Connection=no",
                            TestHost, "sa", Password));
            dynamic i = c.QuerySql("SELECT p=@p, q=@q, r=@p", new { p = 5, q = 9 }).First();
            Assert.AreEqual(5, i.p);
            Assert.AreEqual(9, i.q);
            Assert.AreEqual(5, i.r);
        }
    }
}
#endif
