using Insight.Database;
using Insight.Tests.TestDataClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests
{
    [TestFixture]
    public class MappingTests : BaseDbTest
    {
        [TearDown]
        public override void TearDown()
        {
            ColumnMapping.Configuration.ResetHandlers();

            base.TearDown();
        }

        [Test]
        public void RegexReplaceShouldAlterColumnName()
        {
            ColumnMapping.Configuration.ReplaceRegex("_", String.Empty);

            var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X");
            Assert.AreNotEqual(sql, ParentTestData.Sql);
            var results = _connection.QuerySqlAsync<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>)).Result;
            ParentTestData.Verify(results);
        }

        [Test]
        public void PrefixRemoveShouldAlterColumnName()
        {
            ColumnMapping.Configuration.RemovePrefixes("int");

            var sql = ParentTestData.Sql.Replace("ParentX", "intParentX");
            Assert.AreNotEqual(sql, ParentTestData.Sql);
            var results = _connection.QuerySqlAsync<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>)).Result;
            ParentTestData.Verify(results);
        }

        [Test]
        public void SuffixRemoveShouldAlterColumnName()
        {
            ColumnMapping.Configuration.RemoveSuffixes("int");

            var sql = ParentTestData.Sql.Replace("ParentX", "ParentXint");
            Assert.AreNotEqual(sql, ParentTestData.Sql);
            var results = _connection.QuerySqlAsync<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>)).Result;
            ParentTestData.Verify(results);
        }

        [Test]
        public void ReplaceCanBeChained()
        {
            ColumnMapping.Configuration.RemovePrefixes("int").RemoveSuffixes("Foo").RemoveStrings("_");

            var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X_Foo");
            Assert.AreNotEqual(sql, ParentTestData.Sql);
            var results = _connection.QuerySqlAsync<ParentTestData>(sql, withGraph: typeof(Graph<ParentTestData, TestData>)).Result;
            ParentTestData.Verify(results);
        }
    }
}
