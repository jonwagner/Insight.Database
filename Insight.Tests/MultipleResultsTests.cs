using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;

#pragma warning disable 0649

namespace Insight.Tests
{
    [TestFixture]
    public class MultipleResultsTests : BaseDbTest
    {
        #region Test Classes
        class TestData
        {
            public int X;
        }

        class TestData2
        {
            public int Y;
        }

        [DefaultGraph(typeof(Graph<SuperTestData, TestData>))]
        class SuperTestData
        {
            public TestData TestData;
        }
        #endregion

        #region Multiple Recordset Tests
        [Test]
        public void TwoRecordsetsAreReturned()
        {
            var results = _connection.QueryResults<TestData, TestData2>(@"SELECT X=5 SELECT Y=7", commandType: System.Data.CommandType.Text);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Set1);
            Assert.AreEqual(1, results.Set1.Count); 
            Assert.AreEqual(5, results.Set1[0].X);
            Assert.IsNotNull(results.Set2);
            Assert.AreEqual(1, results.Set2.Count);
            Assert.AreEqual(7, results.Set2[0].Y);
        }
        #endregion

        #region Multiple Recordset WithGraph Tests
        [Test]
        public void RecordsetWithDefaultGraphIsReturned()
        {
            var results = _connection.QueryResults<SuperTestData, TestData2>(@"SELECT X=5 SELECT Y=7", commandType: System.Data.CommandType.Text);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Set1);
            Assert.AreEqual(1, results.Set1.Count);
            Assert.IsNotNull(results.Set1[0].TestData);
            Assert.AreEqual(5, results.Set1[0].TestData.X);
            Assert.IsNotNull(results.Set2);
            Assert.AreEqual(1, results.Set2.Count);
            Assert.AreEqual(7, results.Set2[0].Y);
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
            var results = _connection.QueryResults<PageData<TestData>>(@"SELECT X=5 UNION SELECT X=7 SELECT TotalCount=70", commandType: System.Data.CommandType.Text);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Set1);
            Assert.AreEqual(2, results.Set1.Count);
            Assert.AreEqual(5, results.Set1[0].X);
            Assert.AreEqual(7, results.Set1[1].X);
            Assert.IsNotNull(results.Set2);
            Assert.AreEqual(1, results.Set2.Count);
            Assert.AreEqual(70, results.TotalCount);
        }
        #endregion
    }
}
