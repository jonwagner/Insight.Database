# Insight.Database #

**Insight.Database** is a fast, lightweight, (and dare we say awesome) micro-orm for .NET.

If you are thinking that you need something that is simple and just works for almost any use case that you can think of, Insight probably does it.

	// auto open/close
	var c = new SqlConnection(connectionString);
	c.Execute("AddBeer", new { Name = "IPA", Flavor = "Bitter"});

	// auto object mapping
	Beer beer = new Beer();
	c.Execute("InsertBeer", beer);
	List<Beer> beers = c.Query<Beer>("FindBeer", new { Name = "IPA" });

	// auto object graphs
	var servings = c.Query<Serving, Beer, Glass>("GetServings");
	foreach (var serving in servings)
		Console.WriteLine("{0} {1}", serving.Beer.Name, serving.Glass.Ounces);

	// multiple result sets
	var results = c.QueryResultsSql<Beer, Chip>("GetBeerAndChips", new { Pub = "Fergie's" }));
	IList<Beer> beer = results.Set1;
	IList<Chip> chips = results.Set2;

	// full async support
	var task = c.QueryAsync<Beer>("FindBeer", new { Name = "IPA" });

	// auto table parameters
	CREATE TYPE BeerTable (Name [nvarchar](256), Flavor [nvarchar](256))
	CREATE PROCEDURE InsertBeer (@Beer [BeerTable])
	List<Beer> beer = new List<Beer>();
	c.Execute("InsertBeer", new { Beer = beer });

	// auto bulk-copy objects
	IEnumerable<Beer> listOBeer; // from somewhere
	c.BulkCopy("Beer", listOfBeer);

# v2.0 (Release build) is now in NuGet!

- v2.0 has full async reads in .NET 4.5, automatic multi-recordset processing, customizable binding rules, tons of optimizations, and more code than I can remember.
- In Package Manager, turn on "Include PreRelease" packages to get the new package.
- v2.0 should be compile-compatible with v1.x. (Except for a few APIs I'm pretty sure nobody is using.) It's not binary-compatible with v1.x.
- If you are using Insight.Database.Schema, please upgrade to v2.0.7, which no longer has a dependency on Insight.Database v1.0.
- v2.0 Documentation is online!

# Documentation #

**Full documentation is available on the [wiki](https://github.com/jonwagner/Insight.Database/wiki)!**

