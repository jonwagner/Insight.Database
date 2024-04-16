﻿using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Insight.Tests.Cases;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Insight.Database.Mapping;
#if !NO_SQL_TYPES
using Microsoft.SqlServer.Types;
#endif

#pragma warning disable 0649

namespace Insight.Tests
{
    [TestFixture]
    public class MappingTests : BaseTest
    {
        [TearDown]
        public void TearDown()
        {
            ColumnMapping.All.ResetTransforms();
            ColumnMapping.All.ResetMappers();
            ColumnMapping.All.ResetChildBinding();
            DbSerializationRule.ResetRules();
        }

        #region Table Tests
        [Test]
        public void RegexReplaceShouldAlterColumnName()
        {
            ColumnMapping.Tables.ReplaceRegex("_", String.Empty);

            var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X");
            ClassicAssert.AreNotEqual(sql, ParentTestData.Sql);
            var results = Connection().QuerySql<ParentTestData, TestData>(sql);
            ParentTestData.Verify(results);
        }

        [Test]
        public void PrefixRemoveShouldAlterColumnName()
        {
            ColumnMapping.Tables.RemovePrefixes("int");

            var sql = ParentTestData.Sql.Replace("ParentX", "intParentX");
            ClassicAssert.AreNotEqual(sql, ParentTestData.Sql);
            var results = Connection().QuerySql<ParentTestData, TestData>(sql);
            ParentTestData.Verify(results);
        }

        [Test]
        public void SuffixRemoveShouldAlterColumnName()
        {
            ColumnMapping.Tables.RemoveSuffixes("int");

            var sql = ParentTestData.Sql.Replace("ParentX", "ParentXint");
            ClassicAssert.AreNotEqual(sql, ParentTestData.Sql);
            var results = Connection().QuerySql<ParentTestData, TestData>(sql);
            ParentTestData.Verify(results);
        }

        [Test]
        public void ReplaceCanBeChained()
        {
            ColumnMapping.Tables.RemovePrefixes("int").RemoveSuffixes("Foo").RemoveStrings("_");

            var sql = ParentTestData.Sql.Replace("ParentX", "_Parent_X_Foo");
            ClassicAssert.AreNotEqual(sql, ParentTestData.Sql);
            var results = Connection().QuerySql<ParentTestData, TestData>(sql);
            ParentTestData.Verify(results);
        }
        #endregion

        #region Issue 423
        public class NullMapper : IParameterMapper
        {
            public string MapParameter(Type type, IDbCommand command, IDataParameter parameter)
            {
                return null;
            }
        }

        public class XMapper : IParameterMapper
        {
            public string MapParameter(Type type, IDbCommand command, IDataParameter parameter)
            {
                return "x"; // map everything to x
            }
        }

        [Test]
        public void ParameterMappersCanBeChained()
        {
            ColumnMapping.Parameters.AddMapper(new NullMapper()).AddMapper(new XMapper());

            var result = Connection().ExecuteScalarSql<int>("SELECT @y", new { x = 5 });
            ClassicAssert.AreEqual(result, 5);
        }
        #endregion

        #region Output Parameter Tests
        class OutputParameters
        {
            public int out_foo;
            public int foo;
        }

        [Test]
        public void OutputParameterShouldHonorParameterMappings()
        {
            ColumnMapping.Parameters.RemovePrefixes("out_");

            var output = new OutputParameters();
            Connection().Execute("OutputParameterMappingTest", outputParameters: output);
            ClassicAssert.AreEqual(0, output.out_foo);
            ClassicAssert.AreEqual(5, output.foo);
        }
        #endregion

        #region Serialization Tests
        public class JsonClass
        {
            [Column(SerializationMode = SerializationMode.Json)]
            public JsonSubClass SubClass;
        }

        public class JsonSubClass
        {
            public string Foo;
            public int Bar;
        }

        [Test]
        public void SerializationHandlerSwitchesObjectToXmlWhenRecordTypeMatches()
        {
            DbSerializationRule.Serialize<JsonSubClass>(SerializationMode.Xml);

            var input = new JsonClass();
            input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

            var s = Connection().Single<string>("MappingAsJson1", input);
            ClassicAssert.IsTrue(Connection().Single<string>("MappingAsJson1", input).StartsWith("<MappingTests."));
        }

        [Test]
        public void SerializationHandlerSwitchesObjectToXmlWhenNameMatches()
        {
            DbSerializationRule.Serialize<JsonClass>("Subclass", SerializationMode.Xml);

            var input = new JsonClass();
            input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

            var s = Connection().Single<string>("MappingAsJson2", input);
            ClassicAssert.IsTrue(Connection().Single<string>("MappingAsJson2", input).StartsWith("<MappingTests."));
        }

        [Test]
        public void SerializationHandlerSwitchesObjectToXmlWhenTypeMatches()
        {
            DbSerializationRule.Serialize<JsonSubClass>(SerializationMode.Xml);

            var input = new JsonClass();
            input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

            var s = Connection().Single<string>("MappingAsJson3", input);
            ClassicAssert.IsTrue(Connection().Single<string>("MappingAsJson3", input).StartsWith("<MappingTests."));
        }

        [Test]
        public void SerializationHandlerDoesNotSwitchObjectToXmlWhenNameDoesNotMatch()
        {
            DbSerializationRule.Serialize<JsonClass>("foo", SerializationMode.Xml);

            var input = new JsonClass();
            input.SubClass = new JsonSubClass() { Foo = "foo", Bar = 5 };

            var s = Connection().Single<string>("MappingAsJson4", input);
            ClassicAssert.IsFalse(Connection().Single<string>("MappingAsJson4", input).StartsWith("<MappingTests."));
        }
        #endregion

        #region Multiple Level Mapping Tests
        public class FlattenedParameters
        {
            public int Y1;
            public int Y2;
        }

        public class FlattenedParameters2
        {
            public int Z1;
            public int Z2;
        }

        public interface IFlattenParameters
        {
            [Sql("SELECT x=@x, y1=@y1, y2=@y2")]
            FastExpando Flatten(int x, FlattenedParameters y);

            [Sql("SELECT x=@x, y1=@y1, y2=@y2, z1=@z1, z2=@z2")]
            FastExpando Flatten(int x, FlattenedParameters y, FlattenedParameters2 z);

            [BindChildren(BindChildrenFor.None)]
            [Sql("SELECT x=@x, y1=@y1, y2=@y2")]
            FastExpando DontFlatten(int x, FlattenedParameters y);
        }

        [Test]
        public void IntefaceMethodsFlattenParameters()
        {
            FastExpando result = Connection().As<IFlattenParameters>().Flatten(1, new FlattenedParameters() { Y1 = 2, Y2 = 3 });

            ClassicAssert.AreEqual(1, result["x"]);
            ClassicAssert.AreEqual(2, result["y1"]);
            ClassicAssert.AreEqual(3, result["y2"]);
        }

        [Test]
        public void IntefaceMethodsFlattenParameters2()
        {
            FastExpando result = Connection().As<IFlattenParameters>().Flatten(1, new FlattenedParameters() { Y1 = 2, Y2 = 3 }, new FlattenedParameters2() { Z1 = 4, Z2 = 5 });

            ClassicAssert.AreEqual(1, result["x"]);
            ClassicAssert.AreEqual(2, result["y1"]);
            ClassicAssert.AreEqual(3, result["y2"]);
            ClassicAssert.AreEqual(4, result["z1"]);
            ClassicAssert.AreEqual(5, result["z2"]);
        }

        [Test]
        public void AttributeCanDisableFlattening()
        {
            Assert.Throws<SqlException>(() => Connection().As<IFlattenParameters>().DontFlatten(1, new FlattenedParameters() { Y1 = 2, Y2 = 3 }));
        }

        [Test]
        public void InputParametersCanMapChildFields()
        {
            ColumnMapping.All.EnableChildBinding();

            // we should be able to read in child fields
            FastExpando result = (FastExpando)Connection().QuerySql("SELECT x=@x, y1=@y1, y2=@y2", new { X = 1, Y = new { Y1 = 2, Y2 = 3 } }).First();

            ClassicAssert.AreEqual(1, result["x"]);
            ClassicAssert.AreEqual(2, result["y1"]);
            ClassicAssert.AreEqual(3, result["y2"]);
        }

        [Test]
        public void DeepBindingDisabledByDefault()
        {
            Assert.Throws<SqlException>(() => Connection().QuerySql("SELECT xx=@x, y1=@y1, y2=@y2", new { XX = 1, Y = new { Y1 = 2, Y2 = 3 } }).First());
        }

        [Test]
        public void InputParametersCanMapChildFields2()
        {
            ColumnMapping.All.EnableChildBinding();

            // we should be able to read in child fields
            FastExpando result = (FastExpando)Connection().QuerySql("SELECT x=@x, y1=@y1, y2=@y2, z1=@z1, z2=@z2", new { X = 1, Y = new { Y1 = 2, Y2 = 3 }, Z = new { Z1 = 4, Z2 = 5 } }).First();

            ClassicAssert.AreEqual(1, result["x"]);
            ClassicAssert.AreEqual(2, result["y1"]);
            ClassicAssert.AreEqual(3, result["y2"]);
            ClassicAssert.AreEqual(4, result["z1"]);
            ClassicAssert.AreEqual(5, result["z2"]);
        }

        [Test]
        public void InputParametersAreUndefinedWhenChildrenAreNull()
        {
            ColumnMapping.All.EnableChildBinding();

            var parent = new ParameterParent() { parent = 3 };

            // we should be able to read in child fields
            Assert.Throws<SqlException>(() => Connection().QuerySql("SELECT parent=@parent, child=@foo", parent).First());
        }

        class ParameterParent
        {
            public int parent;
            public OutputParameters Child;
        }

        [Test]
        public void OutputParametersCanMapChildFields()
        {
            ColumnMapping.All.EnableChildBinding();

            var output = new ParameterParent()
            {
                Child = new OutputParameters()
            };

            // we should be able to read in child fields
            Connection().Execute("OutputParameterParentMappingTest", outputParameters: output);

            ClassicAssert.AreEqual(1, output.parent);
            ClassicAssert.AreEqual(2, output.Child.foo);
        }

        [Test]
        public void MergeCanMapChildFields()
        {
            ColumnMapping.All.EnableChildBinding();

            var parent = new ParameterParent()
            {
                Child = new OutputParameters()
            };

            Connection().InsertSql("SELECT parent=1, Foo=2", parent);

            ClassicAssert.AreEqual(1, parent.parent);
            ClassicAssert.AreEqual(2, parent.Child.foo);
        }

        public class ParentOfMissingChild<T>
        {
            public MissingChild<T> Child;
        }

        public class MissingChild<T>
        {
            public T Member;
        }

        private void TestMissingChildCanBeNulled<T>()
        {
            ColumnMapping.All.EnableChildBinding();

            var p = new ParentOfMissingChild<T>();

            try
            {
                Connection().ExecuteSql("SELECT @Member", p);
            }
            catch (SqlException)
            {
                // should get a missing parameter exception
            }

            // this should map a DbNull, since the child isn't missing
            p.Child = new MissingChild<T>();
            ClassicAssert.AreEqual(default(T), Connection().ExecuteScalarSql<T>("SELECT @Member", p));
        }

        [Test]
        public void MissingChildrenAreNulledSuccessfully()
        {
            ColumnMapping.All.EnableChildBinding();

            TestMissingChildCanBeNulled<int>();         // value
            TestMissingChildCanBeNulled<int?>();        // value?
            TestMissingChildCanBeNulled<Guid>();        // struct
            TestMissingChildCanBeNulled<Guid?>();       // struct?
            TestMissingChildCanBeNulled<TestData>();    // object
        }

        public class ParentWithConflictedField
        {
            public int ID;
            public ChildWithConflictedField Child;
        }

        public class ChildWithConflictedField
        {
            public int ID;
            public int OtherFieldToTriggerDepthSearch;
        }

        [Test]
        public void ParentFieldOverridesChildField()
        {
            ColumnMapping.All.EnableChildBinding();

            var p = new ParentWithConflictedField() { ID = 1 };
            p.Child = new ChildWithConflictedField() { ID = 2 };

            ClassicAssert.AreEqual(1, Connection().ExecuteScalarSql<int>("SELECT @ID, @OtherFieldToTriggerDepthSearch", p));
        }

        class TVPParent
        {
            public int X;
            public TVPChild Child;
        }
        class TVPChild
        {
            public int Z;
        }

        [Test]
        public void TVPCanMapChildField()
        {
            ColumnMapping.All.EnableChildBinding();

            var parent = new TVPParent() { Child = new TVPChild() { Z = 2 } };
            var list = new List<TVPParent>();
            list.Add(parent);

            Connection().InsertList("InsertMultipleTestData", list);
            ClassicAssert.AreNotEqual(0, parent.X);

            var result = Connection().QuerySql<TVPParent, TVPChild>("SELECT * FROM InsertTestDataTable WHERE X = @x", parent).Single();
            ClassicAssert.AreEqual(parent.X, result.X);
            ClassicAssert.AreEqual(parent.Child.Z, result.Child.Z);
        }
        #endregion

        #region Test for Issue #156
        public class BaseForRenaming
        {
            public virtual string RenamedInDerived { get; set; }
            [Column("BaseRenamed")]
            public virtual string RenamedInBase { get; set; }
        }

        public class DerivedForRenaming : BaseForRenaming
        {
            [Column("DerivedRenamed")]
            public override string RenamedInDerived { get; set; }
            public override string RenamedInBase { get; set; }
        }

        [Test]
        public void CanUseColumnAttributeOnDerivedClass()
        {
            var result = Connection().QuerySql<DerivedForRenaming>("SELECT DerivedRenamed='derived', BaseRenamed='base'", null).First();
            ClassicAssert.AreEqual("derived", result.RenamedInDerived);
            ClassicAssert.AreEqual("base", result.RenamedInBase);
        }
        #endregion

        #region "Object Access tests"

        // Test access levels for an auto implemented repo interface
        // Note that this project allows Insight.Database access to its internals (InternalsVisibleTo)
        // Interestingly this test does not need InternalsVisibleTo set to work
        [Test]
        public void PrivateFieldsAreAccessible()
        {
            var p = new ObjectAccess_PrivateParams();
            var result = Connection().QuerySql<ObjectAccess_Result>("SELECT MyValue=@ImPrivate", p).First();
            ClassicAssert.AreEqual("abc", result.MyValue);
        }

        private class ObjectAccess_PrivateParams
        {
            internal ObjectAccess_PrivateParams()
            {
                ImPrivate = "abc";
            }
            private string ImPrivate { get; set; }
        }

        private class ObjectAccess_Result { internal String MyValue; }

        // Test access levels for an auto implemented repo interface
        // Note that this project allows Insight.Database access to its internals (internalsvisibleto)
        [Test]
        public void CanAccessAutoRepo_Interface()
        {
            IMyRepoInterface repo = Connection().As<IMyRepoInterface>();

            IList<String> listOfFoo = repo.GetOneFooString();

            ClassicAssert.AreEqual(1, listOfFoo.Count());
            ClassicAssert.AreEqual("foo", listOfFoo[0]);
        }

        internal interface IMyRepoInterface
        {
            [Sql("Select 'foo'")]
            IList<String> GetOneFooString();
        }

        // Test an auto implemented repo via abstract class 
        // Note that this project allows Insight.Database access to its internals (internalsvisibleto)
        [Test]
        public void CanAccessAutoRepo_Abstract()
        {
            MyAbstractRepo repo = Connection().As<MyAbstractRepo>();

            IList<String> listOfFoo = repo.GetOneFooString();

            ClassicAssert.AreEqual(1, listOfFoo.Count());
            ClassicAssert.AreEqual("foo", listOfFoo[0]);
        }

        internal abstract class MyAbstractRepo
        {
            [Sql("Select 'foo'")]
            public abstract IList<String> GetOneFooString();
        }

        #endregion

        #region IColumnMapper Tests
        class MyColumnMapper : IColumnMapper
        {
            string IColumnMapper.MapColumn(Type type, IDataReader reader, int column)
            {
                if (type != typeof(ParentTestData))
                    return null;            // null allows another handler to try

                if (reader.GetName(column) == "ParentX")
                    return "__unmapped__";         // a string value specifies the name of the field/property

                if (reader.GetName(column) == "ParentY")
                    return "ParentX";

                return null;
            }
        }

        [Test]
        public void CustomMapperCanMapFields()
        {
            ColumnMapping.Tables.AddMapper(new MyColumnMapper());

            var results = Connection().QuerySql<ParentTestData>("SELECT ParentY = 2, WithMapping = 1");
            ClassicAssert.AreEqual(results.First().ParentX, 2);
        }

        [Test]
        public void CustomMapperShouldNotThrowWhenFieldIsUnmapped()
        {
            ColumnMapping.Tables.AddMapper(new MyColumnMapper());

            var results = Connection().QuerySql<ParentTestData>("SELECT ParentX = 2, NoMapping = 1");
            ClassicAssert.AreEqual(results.First().ParentX, 0);
        }
        #endregion
    }

    /// <summary>
    /// Tests dynamic connection.
    /// </summary>
    [TestFixture]
    public class MappingProcTests : BaseTest
    {
        #region SetUp and TearDown
        [TearDown]
        public void TearDownFixture()
        {
            ColumnMapping.Tables.ResetTransforms();
        }
        #endregion

        #region Table Valued Parameter Tests
        [Test]
        public void MappingsAreAppliedToTableValuedParameters()
        {
            // get a stanard set of objects from the server
            var original = Connection().QuerySql<ParentTestData>(ParentTestData.Sql);
            ParentTestData.Verify(original, false);

            ColumnMapping.Tables.RemovePrefixes("int");

            // send the object up to the server and get them back
            var results = Connection().Query<ParentTestData>("MappingTestProc", original);
            ParentTestData.Verify(results, false);
        }

        class ParentTestDataWithColumn
        {
            [Column("IntParentX")]
            public int ParentX { get; set; }

            [Column("IntX")]
            public int X { get; set; }
        }

        [Test]
        public void ColumnAttributesAreAppliedToTableValuedParameters()
        {
            var original = new ParentTestDataWithColumn() { ParentX = 5, X = 7 };
            var list = new List<ParentTestDataWithColumn>() { original };

            // send the object up to the server and get them back
            var results = Connection().Query<ParentTestDataWithColumn>("MappingTestProc", list).First();
            ClassicAssert.AreEqual(original.ParentX, results.ParentX);
            ClassicAssert.AreEqual(original.X, results.X);
        }
        #endregion

        #region BulkCopy Tests
        [Test]
        public void MappingsAreAppliedToBulkCopy()
        {
            ColumnMapping.Tables.RemovePrefixes("int");

            for (int i = 0; i < 3; i++)
            {
                // build test data
                ParentTestData[] array = new ParentTestData[i];
                for (int j = 0; j < i; j++)
                    array[j] = new ParentTestData() { ParentX = j };

                // bulk load the data
                Connection().ExecuteSql("DELETE FROM MappingBulkCopyTestTable");
                Connection().BulkCopy("MappingBulkCopyTestTable", array);

                // run the query
                var items = Connection().QuerySql<ParentTestData>("SELECT * FROM MappingBulkCopyTestTable");
                ClassicAssert.IsNotNull(items);
                ClassicAssert.AreEqual(i, items.Count);
                for (int j = 0; j < i; j++)
                    ClassicAssert.AreEqual(j, items[j].ParentX);
            }
        }
        #endregion

        [Test]
        public void MappingsAreAppliedToParameters()
        {
            ColumnMapping.Parameters.RemovePrefixes("int");

            var parentTestData = new ParentTestData() { ParentX = 5 };

            var results = Connection().Query<int>("MappingTestProc2", parentTestData);
            int data = results.First();

            ClassicAssert.AreEqual(parentTestData.ParentX, data);
        }

#if !NO_SQL_TYPES
        #region Geography Tests
		class TestGeography
		{
			public Microsoft.SqlServer.Types.SqlGeography Geo;
		}

		[Test]
		public void GeographyParametersArePassedCorrectly()
		{
			// single value query
			var point = SqlGeography.Point(0, 0, 4326);
			var results = Connection().Query<SqlGeography>("MappingTestProcGeography", new { geo = point });
			Assert.That(results[0].STEquals(point).IsTrue);

			// class return value
			var list = Connection().Query<TestGeography>("MappingTestProcGeography", new { geo = point });
			Assert.That(list[0].Geo.STEquals(point).IsTrue);

			// dynamic parameter
			dynamic p = new FastExpando();
			p["Geo"] = point;
			results = Connection().Query<SqlGeography>("MappingTestProcGeography", (object)p);
			Assert.That(results[0].STEquals(point).IsTrue);

			// dynamic results
			var dynamicList = Connection().Query("MappingTestProcGeography", new { geo = point });
			Assert.That(results[0].STEquals(point).IsTrue);
		}
        #endregion
#endif

        public class A388
        {
            public int _a;
        }
        public class B388
        {
            public int A;
        }

        [Test]
        public void TestIssue388()
        {
            ColumnMapping.Tables.RemoveStrings<A388>("_");

            // this will throw because one of the columns has a NULL column name
            Connection().SingleSql<A388>("SELECT A=1, 2");
            Connection().SingleSql<B388>("SELECT A=1, 2");
        }

#region Test459
        public class Test459 : BaseTest
        {
            [Test]
            public void TestReadingPrivateSetters()
            {
                var o = Connection().SingleSql<PrivateSetterInt>("SELECT X=1");
				// should map
                ClassicAssert.AreEqual(1, o.X);
            }

            [Test]
            public void TestReadingPrivateNoSetter()
            {
                var o = Connection().SingleSql<PrivateNoSetterInt>("SELECT X=1");
				// should not map
                ClassicAssert.AreEqual(0, o.X);
            }

            [Test]
            public void TestReadingPrivateNoSetterWithConversion()
            {
                var o = Connection().SingleSql<PrivateNoSetterInt>("SELECT X='string'");
				// should not map and should not throw due to incompatible type
                ClassicAssert.AreEqual(0, o.X);
            }

            private class PrivateSetterInt
            {
                public int X { get; private set; }
            }

            private class PrivateNoSetterInt
            {
                public int X { get; }
            }
        }
#endregion

#region Test 466
		public class Test466 : BaseTest
		{
			[Test]
			public void TestUmlautParamsProc()
			{
				try
				{
					Connection().ExecuteSql("CREATE PROCEDURE ProcWithUmlaut(@paräm int) AS SELECT @paräm");

					var result = Connection().ExecuteScalar<int>("ProcWithUmlaut", new { paräm = 5 });
					ClassicAssert.AreEqual(result, 5);
				}
				finally
				{
					Connection().ExecuteSql("DROP PROCEDURE ProcWithUmlaut");
				}
			}

			[Test]
			public void TestUmlautParamsSql()
			{
				var result = Connection().ExecuteScalarSql<int>("SELECT @paräm", new { paräm = 5 });
				ClassicAssert.AreEqual(result, 5);
			}
		}
#endregion
    }
}
