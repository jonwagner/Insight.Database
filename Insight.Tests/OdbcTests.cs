﻿#if !NO_ODBC
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
    public class OdbcTests : BaseTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            ConnectionString = String.Format("Driver={{SQL Server}}; Server={0}; {1};",
                TestHost ?? ".",
                (Password != null) ? String.Format("Uid=sa; Pwd={0}", Password) : "Trusted_Connection=Yes");
        }

        [Test]
        public void NamedParametersAreConvertedToPositionalParameters()
        {
            var c = new System.Data.Odbc.OdbcConnection(ConnectionString);
            dynamic i = c.QuerySql("SELECT p=@p, q=@q, r=@p", new { p = 5, q = 9 }).First();
            ClassicAssert.AreEqual(5, i.p);
            ClassicAssert.AreEqual(9, i.q);
            ClassicAssert.AreEqual(5, i.r);
        }
    }
}
#endif
