using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace Insight.Tests.TestDataClasses
{
    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    class ParentTestData
    {
        public TestData TestData;
        public int ParentX;

        public static readonly string Sql = "SELECT ParentX=2, X=5 ";

        public static void Verify(IEnumerable results, bool withGraph = true)
        {
            var list = results.OfType<ParentTestData>().ToList();

            Assert.IsNotNull(results);
            Assert.AreEqual(1, list.Count);

            var data = list[0];
            Assert.AreEqual(2, data.ParentX);

            if (withGraph)
            {
                Assert.IsNotNull(data.TestData);
                Assert.AreEqual(5, data.TestData.X);
            }
            else
                Assert.IsNull(data.TestData);
        }
    }

    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    [DefaultGraph(typeof(Graph<ParentTestDataWithDefaultGraph, TestData>))]
    class ParentTestDataWithDefaultGraph : ParentTestData
    {
    }

    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    class TestData
    {
        public int X;
    }

    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    class TestData2
    {
        public int Y;

        public static readonly string Sql = "SELECT Y=7 ";

        public static void Verify(IList<TestData2> results)
        {
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            var data = results[0];
            Assert.AreEqual(7, data.Y);
        }
    }
}
