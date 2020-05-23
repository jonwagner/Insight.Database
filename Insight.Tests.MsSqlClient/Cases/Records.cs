using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0649

namespace Insight.Tests.MsSqlClient.Cases
{
    public class Beer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }

        public const string SelectAllProc = "SelectAllBeer";
        public const string SelectAllChildrenProc = "SelectAllBeerChildren";
        public const string InsertProc = "InsertBeer";
        public const string InsertProcScopeIdentity = "InsertBeer_ScopeIdentity";
        public const string InsertManyProc = "InsertBeers";

        public static Beer GetSample()
        {
            return new Beer()
            {
                Name = "HopDevil",
                Style = "Hoppy"
            };
        }

        public static string GetSelectAllProcMultipleTimes(int times)
        {
            return String.Join(";", Enumerable.Range(1, times).Select(i => "EXEC " + SelectAllProc).ToArray());
        }

        public static string GetSelectNested(int times)
        {
            return "SELECT " + String.Join(",", Enumerable.Range(1, times).Select(i => "ParentID=1, ID=1, Name='Sly Fox 113', Style='IPA'").ToArray());
        }

        public static string GetSelectNestedChildren(int times, int childrenTimes)
        {
            return GetSelectAllProcMultipleTimes(times) + ";" + GetSelectNested(childrenTimes);
        }

        public static string GetSelectAllChildren(int times)
        {
            return String.Join(";", Enumerable.Range(1, times).Select(i => "EXEC " + SelectAllChildrenProc).ToArray());
        }

        public virtual void Verify()
        {
            Assert.AreEqual(1, ID);
            Assert.AreEqual("Sly Fox 113", Name);
            Assert.AreEqual("IPA", Style);
        }

        public void VerifySample()
        {
            Assert.AreNotEqual(0, ID);
            Assert.AreEqual("HopDevil", Name);
            Assert.AreEqual("Hoppy", Style);
        }

        public static void VerifyAll(IEnumerable<Beer> beer)
        {
            Assert.AreEqual(3, beer.Count());

            var slyFox = beer.First(b => b.ID == 1);
            Assert.IsNotNull(slyFox);
            slyFox.Verify();
        }
    }

    internal class InfiniteBeer : Beer
    {
        public InfiniteBeer More;

        public override void Verify()
        {
            base.Verify();
            if (More != null)
                More.Verify();
        }
    }

    internal class InfiniteBeerList : Beer
    {
        public InfiniteBeer More;
        public IList<InfiniteBeerList> List { get; set; }

        public override void Verify()
        {
            base.Verify();

            if (ID == 1 && List != null)
                VerifyAll(List);
        }

        public static void VerifyAll(IEnumerable<InfiniteBeerList> list)
        {
            // all of the base beer should be in it
            Beer.VerifyAll(list.OfType<Beer>());
        }
    }

    internal class BeerWithFields
    {
        public int ID;
        public string Name;
        public string Style;
    }

    internal class InfiniteBeerListWithFields : BeerWithFields
    {
        public IList<InfiniteBeerListWithFields> List;
    }

    internal class Wine
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    internal class LiquorStore
    {
        public int ID;
        public IList<Beer> Beer;
        public IList<Wine> Wine;
    }

    internal class LiquorStoreNeedingFieldOverrides
    {
        public int Foo;

        public IList<Beer> Beer;
        public IList<Wine> Wine;

        // for testing field overrides
        public IList<Beer> OtherBeer;
    }

    internal class PageData
    {
        public int TotalCount;
    }

    internal class PageData<T> : Results<T, PageData>
    {
        public int TotalCount { get { return Set2.First().TotalCount; } }
    }
}
