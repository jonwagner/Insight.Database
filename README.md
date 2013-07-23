# Insight.Database #

**Insight.Database** is a fast, lightweight, (and dare we say awesome) micro-orm for .NET.

Insight.Database lets you call your database with almost no code, and makes it easy to send objects to your database and get them back.

Here is Insight implementing a repository automatically:

	CREATE TABLE Beer ([ID] [int], [Type] varchar(128), [Description] varchar(128)) GO
	CREATE PROC InsertBeer @type varchar(128), @description varchar(128) AS
		INSERT INTO Beer (Type, Description) OUTPUT inserted.ID
			VALUES (@type, @description)
			GO
	CREATE PROC GetBeerByType @type [varchar] AS SELECT * FROM Beer WHERE Type = @type GO

	public class Beer
	{
		public int ID { get; private set; }
		public string Type { get; set; }
		public string Description { get; set; }
	}

	public interface IBeerRepository
	{
		void InsertBeer(Beer beer);
		IList<Beer> GetBeerByType(string type);
	}

	ConnectionStringBuilder builder = "blah blah";

	// insight will connect your interface to the stored proc automatically
	var repo = builder.OpenAs<IBeerRepository>();
	repo.Insert(new Beer() { Type = "ipa", Description = "Sly Fox 113" });
	IList<Beer> beer = repo.GetBeerByType("ipa");

Here is Insight letting you call your database directly with almost no code:

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

# v3.0 - IS NOW IN NUGET! #

Insight.Database v3.0 now supports multiple database providers. The following providers are currently available:

- Sql Server
- MySQL
- Oracle
- Postgres
- ODBC drivers
- OLEDB drivers

# v2.1 now with automatic interface mapping #

- Insight will now automatically convert your .NET interface to SQL calls! See the wiki for some automajikal love!
- Also now with handy wrappers for managing transactions.

# v2.0 is now in NuGet! #

- v2.0 has full async reads in .NET 4.5, automatic multi-recordset processing, customizable binding rules, tons of optimizations, and more code than I can remember.
- v2.0 should be compile-compatible with v1.x. (Except for a few APIs I'm pretty sure nobody is using.) It's not binary-compatible with v1.x.
- If you are using Insight.Database.Schema, please upgrade to v2.0.8, which no longer has a dependency on Insight.Database v1.0.

# Documentation #

**Full documentation is available on the [wiki](https://github.com/jonwagner/Insight.Database/wiki)!**

