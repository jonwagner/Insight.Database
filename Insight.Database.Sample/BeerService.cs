using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Insight.Database;

#pragma warning disable 0649

namespace Insight.Database.Sample
{
	/// <summary>
	/// The beer service is a business logic service that handles ordering a beer.
	/// It uses Ninject to inject the IBeerRepository interface, which is automatically implmemented by Insight.
	/// </summary>
	class BeerService
	{
		/// <summary>
		/// We use Ninject to inject the beer repository into the service.
		/// </summary>
		[Inject]
		public IBeerRepository BeerRepository { get; set; }

		public Beer OrderBeer(int id)
		{
			// we are going to query the database with auto-open/close semantics
			var beer = BeerRepository.SelectBeer(id);

			return beer;
		}

		public void AddBeer(IEnumerable<Beer> beers)
		{
			// send an entire list of beer to the database
			BeerRepository.InsertBeers(beers);
		}

		public void StockBeer(IEnumerable<int> beerIDs)
		{
			// tell the repository to stay open and perform the following operation in a transaction
			using (var tx = BeerRepository.OpenWithTransaction())
			{
				// add a 6-pack of each beer
				foreach (int id in beerIDs)
				{
					Beer b = BeerRepository.SelectBeer(id);
					b.InStock += 6;
					BeerRepository.UpdateBeer(b);
				}

				// don't forget to commit the transaction
				tx.Commit();
			}
		}
	}
}
