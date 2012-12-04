using Insight.Database;
using NUnit.Framework;
using System;
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
    class TestData
    {
        public TestSubData SubData;

        public static readonly string Sql = "SELECT X=5";

        public static void Verify(IList<TestData> results)
        {
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            var data = results[0];
            Assert.IsNotNull(data.SubData);
            Assert.AreEqual(5, data.SubData.X);
        }
    }

    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    [DefaultGraph(typeof(Graph<TestDataWithDefaultGraph, TestSubData>))]
    class TestDataWithDefaultGraph
    {
        public TestSubData SubData;

        public static readonly string Sql = "SELECT X=5";

        public static void Verify(IList<TestDataWithDefaultGraph> results)
        {
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            var data = results[0];
            Assert.IsNotNull(data.SubData);
            Assert.AreEqual(5, data.SubData.X);
        }
    }

    /// <summary>
    /// Test class to remove some repetition from test cases.
    /// </summary>
    class TestSubData
    {
        public int X;
    }
}
