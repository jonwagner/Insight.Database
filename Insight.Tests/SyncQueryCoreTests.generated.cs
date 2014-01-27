using System;
using System.Linq;
using Insight.Database;
using Insight.Database.Structure;
using Insight.Tests.Cases;
using NUnit.Framework;

namespace Insight.Tests
{
	public partial class SyncQueryCoreTests
    {
		///////////////////////////////////////////////////////
		// Read Results with Query Structure
		///////////////////////////////////////////////////////
		[Test]
		public void ReadResultsWithQueryStructure1()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(1),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
		}
		[Test]
		public void ReadResultsWithQueryStructure2()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(2),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
		}
		[Test]
		public void ReadResultsWithQueryStructure3()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(3),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
		}
		[Test]
		public void ReadResultsWithQueryStructure4()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(4),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
		}
		[Test]
		public void ReadResultsWithQueryStructure5()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(5),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
		}
		[Test]
		public void ReadResultsWithQueryStructure6()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(6),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
		}
		[Test]
		public void ReadResultsWithQueryStructure7()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(7),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
		}
		[Test]
		public void ReadResultsWithQueryStructure8()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(8),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
		}
		[Test]
		public void ReadResultsWithQueryStructure9()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(9),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
		}
		[Test]
		public void ReadResultsWithQueryStructure10()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(10),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
		}
		[Test]
		public void ReadResultsWithQueryStructure11()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(11),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
		}
		[Test]
		public void ReadResultsWithQueryStructure12()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(12),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
		}
		[Test]
		public void ReadResultsWithQueryStructure13()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(13),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
		}
		[Test]
		public void ReadResultsWithQueryStructure14()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(14),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
		}
		[Test]
		public void ReadResultsWithQueryStructure15()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(15),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
			Beer.VerifyAll(result.Set15);
		}
		[Test]
		public void ReadResultsWithQueryStructure16()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllProcMultipleTimes(16),
				null,
				Query.ReturnsResults(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
					.Then(Some<Beer>.Records)
				);

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
			Beer.VerifyAll(result.Set15);
			Beer.VerifyAll(result.Set16);
		}

		///////////////////////////////////////////////////////
		// Read Results with Generic Arguments
		///////////////////////////////////////////////////////
		[Test]
		public void ReadResultsWithGenerics2()
		{
			var result = Connection().QueryResultsSql<Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(2));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
		}
		[Test]
		public void ReadResultsWithGenerics3()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(3));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
		}
		[Test]
		public void ReadResultsWithGenerics4()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(4));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
		}
		[Test]
		public void ReadResultsWithGenerics5()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(5));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
		}
		[Test]
		public void ReadResultsWithGenerics6()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(6));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
		}
		[Test]
		public void ReadResultsWithGenerics7()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(7));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
		}
		[Test]
		public void ReadResultsWithGenerics8()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(8));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
		}
		[Test]
		public void ReadResultsWithGenerics9()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(9));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
		}
		[Test]
		public void ReadResultsWithGenerics10()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(10));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
		}
		[Test]
		public void ReadResultsWithGenerics11()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(11));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
		}
		[Test]
		public void ReadResultsWithGenerics12()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(12));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
		}
		[Test]
		public void ReadResultsWithGenerics13()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(13));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
		}
		[Test]
		public void ReadResultsWithGenerics14()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(14));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
		}
		[Test]
		public void ReadResultsWithGenerics15()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(15));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
			Beer.VerifyAll(result.Set15);
		}
		[Test]
		public void ReadResultsWithGenerics16()
		{
			var result = Connection().QueryResultsSql<Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer, Beer>(
				Beer.GetSelectAllProcMultipleTimes(16));

			Beer.VerifyAll(result.Set1);
			Beer.VerifyAll(result.Set2);
			Beer.VerifyAll(result.Set3);
			Beer.VerifyAll(result.Set4);
			Beer.VerifyAll(result.Set5);
			Beer.VerifyAll(result.Set6);
			Beer.VerifyAll(result.Set7);
			Beer.VerifyAll(result.Set8);
			Beer.VerifyAll(result.Set9);
			Beer.VerifyAll(result.Set10);
			Beer.VerifyAll(result.Set11);
			Beer.VerifyAll(result.Set12);
			Beer.VerifyAll(result.Set13);
			Beer.VerifyAll(result.Set14);
			Beer.VerifyAll(result.Set15);
			Beer.VerifyAll(result.Set16);
		}

		///////////////////////////////////////////////////////
		// Read Nested With Query Structure
		///////////////////////////////////////////////////////
		[Test]
		public void ReadNestedWithQueryStructure1()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(1),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

		}
		[Test]
		public void ReadNestedWithQueryStructure2()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(2),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure3()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(3),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure4()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(4),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure5()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(5),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure6()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(6),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure7()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(7),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure8()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(8),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure9()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(9),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure10()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(10),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure11()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(11),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure12()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(12),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure13()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(13),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure14()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(14),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure15()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(15),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithQueryStructure16()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(16),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records));
			var beer = result.Set1.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}

		///////////////////////////////////////////////////////
		// Read Nested With Generics
		///////////////////////////////////////////////////////
		[Test]
		public void ReadNestedWithGenerics1()
		{
			var result = Connection().QuerySql<InfiniteBeer>(Beer.GetSelectNested(1));
			var beer = result.First();
			beer.Verify();

		}
		[Test]
		public void ReadNestedWithGenerics2()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(2));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics3()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(3));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics4()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(4));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics5()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(5));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics6()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(6));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics7()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(7));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics8()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(8));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics9()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(9));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics10()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(10));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics11()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(11));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics12()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(12));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics13()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(13));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics14()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(14));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics15()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(15));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadNestedWithGenerics16()
		{
			var result = Connection().QuerySql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(16));
			var beer = result.First();
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}

		///////////////////////////////////////////////////////
		// Single Nested With Query Structure
		///////////////////////////////////////////////////////
		[Test]
		public void ReadSingleWithQueryStructure1()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(1),
				null,
				SingleReader<InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

		}
		[Test]
		public void ReadSingleWithQueryStructure2()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(2),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure3()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(3),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure4()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(4),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure5()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(5),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure6()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(6),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure7()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(7),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure8()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(8),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure9()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(9),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure10()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(10),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure11()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(11),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure12()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(12),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure13()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(13),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure14()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(14),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure15()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(15),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithQueryStructure16()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectNested(16),
				null,
				SingleReader<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Default);
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}

		///////////////////////////////////////////////////////
		// Single Nested With Generics
		///////////////////////////////////////////////////////
		[Test]
		public void ReadSingleWithGenerics1()
		{
			var result = Connection().SingleSql<InfiniteBeer>(Beer.GetSelectNested(1));
			var beer = result;
			beer.Verify();

		}
		[Test]
		public void ReadSingleWithGenerics2()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(2));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics3()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(3));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics4()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(4));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics5()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(5));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics6()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(6));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics7()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(7));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics8()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(8));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics9()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(9));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics10()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(10));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics11()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(11));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics12()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(12));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics13()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(13));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics14()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(14));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics15()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(15));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}
		[Test]
		public void ReadSingleWithGenerics16()
		{
			var result = Connection().SingleSql<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>(Beer.GetSelectNested(16));
			var beer = result;
			beer.Verify();

			beer.More.Verify();
			beer.More.More.Verify();
			beer.More.More.More.Verify();
			beer.More.More.More.More.Verify();
			beer.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
			beer.More.More.More.More.More.More.More.More.More.More.More.More.More.More.More.Verify();
		}

		///////////////////////////////////////////////////////
		// MixedHierarchy with Query Structure
		///////////////////////////////////////////////////////
		[Test]
		public void TestMixedHierarchy_1_1()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 1).Select(_ => Beer.GetSelectNested(1)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_2_2()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 2).Select(_ => Beer.GetSelectNested(2)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_3_3()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 3).Select(_ => Beer.GetSelectNested(3)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_4_4()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 4).Select(_ => Beer.GetSelectNested(4)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_5_5()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 5).Select(_ => Beer.GetSelectNested(5)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_6_6()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 6).Select(_ => Beer.GetSelectNested(6)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_7_7()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 7).Select(_ => Beer.GetSelectNested(7)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_8_8()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 8).Select(_ => Beer.GetSelectNested(8)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_9_9()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 9).Select(_ => Beer.GetSelectNested(9)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_10_10()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 10).Select(_ => Beer.GetSelectNested(10)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_11_11()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 11).Select(_ => Beer.GetSelectNested(11)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_12_12()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 12).Select(_ => Beer.GetSelectNested(12)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
			Assert.AreEqual(1, result.Set12.Count());
			result.Set12.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_13_13()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 13).Select(_ => Beer.GetSelectNested(13)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
			Assert.AreEqual(1, result.Set12.Count());
			result.Set12.First().Verify();
			Assert.AreEqual(1, result.Set13.Count());
			result.Set13.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_14_14()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 14).Select(_ => Beer.GetSelectNested(14)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
			Assert.AreEqual(1, result.Set12.Count());
			result.Set12.First().Verify();
			Assert.AreEqual(1, result.Set13.Count());
			result.Set13.First().Verify();
			Assert.AreEqual(1, result.Set14.Count());
			result.Set14.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_15_15()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 15).Select(_ => Beer.GetSelectNested(15)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
			Assert.AreEqual(1, result.Set12.Count());
			result.Set12.First().Verify();
			Assert.AreEqual(1, result.Set13.Count());
			result.Set13.First().Verify();
			Assert.AreEqual(1, result.Set14.Count());
			result.Set14.First().Verify();
			Assert.AreEqual(1, result.Set15.Count());
			result.Set15.First().Verify();
		}
		[Test]
		public void TestMixedHierarchy_16_16()
		{
			var result = Connection().QuerySql(
				String.Join(";", Enumerable.Range(1, 16).Select(_ => Beer.GetSelectNested(16)).ToArray()),
				null,
				Query.ReturnsResults(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					.Then(OneToOne<InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer, InfiniteBeer>.Records)
					);

			Assert.AreEqual(1, result.Set1.Count());
			result.Set1.First().Verify();
			Assert.AreEqual(1, result.Set2.Count());
			result.Set2.First().Verify();
			Assert.AreEqual(1, result.Set3.Count());
			result.Set3.First().Verify();
			Assert.AreEqual(1, result.Set4.Count());
			result.Set4.First().Verify();
			Assert.AreEqual(1, result.Set5.Count());
			result.Set5.First().Verify();
			Assert.AreEqual(1, result.Set6.Count());
			result.Set6.First().Verify();
			Assert.AreEqual(1, result.Set7.Count());
			result.Set7.First().Verify();
			Assert.AreEqual(1, result.Set8.Count());
			result.Set8.First().Verify();
			Assert.AreEqual(1, result.Set9.Count());
			result.Set9.First().Verify();
			Assert.AreEqual(1, result.Set10.Count());
			result.Set10.First().Verify();
			Assert.AreEqual(1, result.Set11.Count());
			result.Set11.First().Verify();
			Assert.AreEqual(1, result.Set12.Count());
			result.Set12.First().Verify();
			Assert.AreEqual(1, result.Set13.Count());
			result.Set13.First().Verify();
			Assert.AreEqual(1, result.Set14.Count());
			result.Set14.First().Verify();
			Assert.AreEqual(1, result.Set15.Count());
			result.Set15.First().Verify();
			Assert.AreEqual(1, result.Set16.Count());
			result.Set16.First().Verify();
		}

		///////////////////////////////////////////////////////
		// Child Records
		///////////////////////////////////////////////////////
		[Test]
		public void TestHierarchy3()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(3),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy4()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(4),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy5()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(5),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy6()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(6),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy7()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(7),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy8()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(8),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy9()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(9),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy10()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(10),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy11()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(11),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy12()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(12),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy13()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(13),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy14()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(14),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy15()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(15),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}
		[Test]
		public void TestHierarchy16()
		{
			var result = Connection().QuerySql(
				Beer.GetSelectAllChildren(16),
				null,
				Query.Returns(Some<InfiniteBeerList>.Records)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List, b => b.ID, (b, list) => b.List = list)
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					.ThenChildren(Some<InfiniteBeerList>.Records, b => b.List.SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List).SelectMany(b2 => b2.List), b => b.ID, (b, list) => b.List = list.ToList())
					);
			InfiniteBeerList.VerifyAll(result);
		}

	}
}