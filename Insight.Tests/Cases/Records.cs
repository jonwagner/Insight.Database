﻿using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests.Cases
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
			ClassicAssert.AreEqual(1, ID);
			ClassicAssert.AreEqual("Sly Fox 113", Name);
			ClassicAssert.AreEqual("IPA", Style);
		}

		public void VerifySample()
		{
			ClassicAssert.AreNotEqual(0, ID);
			ClassicAssert.AreEqual("HopDevil", Name);
			ClassicAssert.AreEqual("Hoppy", Style);
		}

		public static void VerifyAll(IEnumerable<Beer> beer)
		{
			ClassicAssert.AreEqual(3, beer.Count());

			var slyFox = beer.First(b => b.ID == 1);
			ClassicAssert.IsNotNull(slyFox);
			slyFox.Verify();
		}
	}

	class InfiniteBeer : Beer
	{
		public InfiniteBeer More;

		public override void Verify()
		{
			base.Verify();
			if (More != null)
				More.Verify();
		}
	}

	class InfiniteBeerList : Beer
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

	class BeerWithFields
	{
		public int ID;
		public string Name;
		public string Style;
	}

	class InfiniteBeerListWithFields : BeerWithFields
	{
		public IList<InfiniteBeerListWithFields> List;
	}

	class Wine
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	class LiquorStore
	{
		public int ID;
		public IList<Beer> Beer;
		public IList<Wine> Wine;
	}

	class LiquorStoreNeedingFieldOverrides
	{
		public int Foo;

		public IList<Beer> Beer;
		public IList<Wine> Wine;

		// for testing field overrides
		public IList<Beer> OtherBeer;
	}

	class PageData
	{
		public int TotalCount;
	}
	class PageData<T> : Results<T, PageData>
	{
		public int TotalCount { get { return Set2.First().TotalCount; } }
	}
}
