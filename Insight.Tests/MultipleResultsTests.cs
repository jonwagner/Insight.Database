using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using Insight.Tests.TestDataClasses;

#pragma warning disable 0649

namespace Insight.Tests
{
    [TestFixture]
    public class MultipleResultsTests : BaseDbTest
    {
        #region Multiple Recordset Tests
        [Test]
        public void TwoRecordsetsAreReturned()
        {
            var results = _connection.QueryResultsSql<ParentTestData, TestData2>(ParentTestData.Sql + TestData2.Sql);

            Assert.IsNotNull(results);
            ParentTestData.Verify(results.Set1, withGraph: false);
            TestData2.Verify(results.Set2);
        }
        #endregion

        #region Multiple Recordset WithGraph Tests
        [Test]
        public void RecordsetWithDefaultGraphIsReturned()
        {
            var results = _connection.QueryResultsSql<ParentTestDataWithDefaultGraph, TestData2>(ParentTestData.Sql + TestData2.Sql);

            Assert.IsNotNull(results);
            ParentTestData.Verify(results.Set1, withGraph: true);
            TestData2.Verify(results.Set2);
        }

        [Test]
        public void RecordsetWithGraphIsReturned()
        {
            var results = _connection.QueryResultsSql<ParentTestData, TestData2>(
                ParentTestData.Sql + TestData2.Sql,
                withGraphs: new[] { typeof(Graph<ParentTestData, TestData>) });

            Assert.IsNotNull(results);
            ParentTestData.Verify(results.Set1, withGraph: true);
            TestData2.Verify(results.Set2);
        }
        #endregion

        #region Derived Recordsets Tests
        class PageData
        {
            public int TotalCount;
        }
        class PageData<T> : Results<T, PageData>
        {
            public int TotalCount { get { return Set2.First().TotalCount; } }
        }

        [Test]
        public void DerivedRecordsetsCanBeReturned()
        {
            var results = _connection.QueryResultsSql<PageData<ParentTestData>>(ParentTestData.Sql + "SELECT TotalCount=70");

            Assert.IsNotNull(results);
            ParentTestData.Verify(results.Set1, withGraph: false);

            Assert.IsNotNull(results.Set2);
            Assert.AreEqual(1, results.Set2.Count);
            Assert.AreEqual(70, results.TotalCount);
        }
        #endregion
    }
}
