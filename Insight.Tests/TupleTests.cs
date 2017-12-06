using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;

namespace Insight.Tests
{
    [TestFixture]
    public class TupleTests : BaseTest
    {
        #region Structure Property Support
        struct FakeTuple
        {
            public int Item1;
            public int Item2;
        }

        // loading properties from structures was not working
        [Test]
        public void StructShouldBindToItemXFields()
        {
            var tuple = new FakeTuple { Item1 = 1, Item2 = 2 };
            var (one, two) = Connection().QuerySql<ValueTuple<int, int>>("SELECT Item1=@Item1, Item2=@Item2", tuple).First();
            Assert.AreEqual(1, one);
            Assert.AreEqual(2, two);
        }
        #endregion

        #region Basic Value Tuple Tests
        [Test]
        public void ReturningTuplesShouldBindToItemXColumns()
        {
            var (one, two) = Connection().QuerySql<ValueTuple<int, int>>("SELECT Item1=1, Item2=2").First();
            Assert.AreEqual(1, one);
            Assert.AreEqual(2, two);
        }

        [Test]
        public void TupleParametersShouldBindToItemXFields()
        {
            var tuple = (one: 1, two: 2);
            var (one, two) = Connection().QuerySql<ValueTuple<int, int>>("SELECT Item1=@Item1, Item2=@Item2", tuple).First();
            Assert.AreEqual(1, one);
            Assert.AreEqual(2, two);
        }
        #endregion

        #region Tuple Interface Tests
        public interface IHandleTuples
        {
            [Sql("SELECT one=1, two=2")]
            List<(int one, int two)> GetTuples();

            [Sql("SELECT one=1, two=2")]
            (int one, int two) GetTuple();
        }

        [Test]
        public void TupleInterfaceListShouldBeDeconstructible()
        {
            var (one, two) = Connection().As<IHandleTuples>().GetTuples().First();
            Assert.AreEqual(1, one);
            Assert.AreEqual(2, two);
        }

        [Test]
        public void TupleInterfaceListShouldHaveNamedProperties()
        {
            var tuple = Connection().As<IHandleTuples>().GetTuples().First();
            Assert.AreEqual(1, tuple.one);
            Assert.AreEqual(2, tuple.two);
        }

        [Test]
        public void TupleInterfaceShouldHaveNamedProperties()
        {
            var tuple = Connection().As<IHandleTuples>().GetTuple();
            Assert.AreEqual(1, tuple.one);
            Assert.AreEqual(2, tuple.two);
        }

        [Test]
        public void TupleInterfaceShouldBeDeconstructible()
        {
            var tuple = Connection().As<IHandleTuples>().GetTuple();
            Assert.AreEqual(1, tuple.one);
            Assert.AreEqual(2, tuple.two);
        }
        #endregion
    }
}
