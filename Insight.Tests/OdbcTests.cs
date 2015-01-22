using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests
{
    [TestFixture]
    public class OdbcTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            OdbcInsightDbProvider.RegisterProvider();
        }

#if !NODYNAMIC
        [Test]
        public void NamedParametersAreConvertedToPositionalParameters()
        {
            var c = new System.Data.Odbc.OdbcConnection(ConfigurationManager.ConnectionStrings["Odbc"].ConnectionString);
            dynamic i = c.QuerySql("SELECT p=@p, q=@q, r=@p", new { p = 5, q = 9 }).First();
            Assert.AreEqual(5, i.p);
            Assert.AreEqual(9, i.q);
            Assert.AreEqual(5, i.r);
        }
#endif
    }
}
