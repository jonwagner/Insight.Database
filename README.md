# Insight.Database #

**Insight.Database** is a fast, lightweight, (and dare we say awesome) micro-orm for .NET. It's available as a [NuGet Package](http://www.nuget.org/packages/Insight.Database/).

Let's say you have a database and a class and you want them to work together. Something like this:

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

	var beer = new Beer() { Type = "ipa", Description = "Sly Fox 113" };

Let's get Insight.Database:

	PM> Install-Package Insight.Database

Now, wire up those stored procedures to an interface with a single `connection.As<T>`:

	public interface IBeerRepository
	{
		void InsertBeer(Beer beer);
		IList<Beer> GetBeerByType(string type);
		void UpdateBeerList(IList<Beer> beerList);
	}

	var repo = connection.As<IBeerRepository>();

	repo.Insert(beer);
	IList<Beer> beerList = repo.GetBeerByType("ipa");
	repo.UpdateBeerList(beerList);

Look, ma! No mapping code! (Plus, you can now inject that interface with a DI framework or mock the interface for testing.)

Want to work at a lower level? Let's call a stored proc with an anonymous object. (It also automatically opens and closes the connection.)

	// auto open/close
	conn.Execute("AddBeer", new { Name = "IPA", Flavor = "Bitter"});

Objects are mapped automatically. No config files, no attributes. Just pure, clean magic:

	// auto object mapping
	Beer beer = new Beer();
	conn.Execute("InsertBeer", beer);
	List<Beer> beers = conn.Query<Beer>("FindBeer", new { Name = "IPA" });

Yes, even nested objects. Insight will just figure it out for you:

	// auto object graphs
	var servings = conn.Query<Serving, Beer, Glass>("GetServings");
	foreach (var serving in servings)
		Console.WriteLine("{0} {1}", serving.Beer.Name, serving.Glass.Ounces);

Feel free to return multiple result sets. We can handle them:

	// multiple result sets
	var results = conn.QueryResults<Beer, Chip>("GetBeerAndChips", new { Pub = "Fergie's" }));
	IList<Beer> beer = results.Set1;
	IList<Chip> chips = results.Set2;

Full async support. Just add `Async`:

	var task = c.QueryAsync<Beer>("FindBeer", new { Name = "IPA" });

Send whole lists of objects to databases that support table parameters. No more multiple queries or weird parameter mapping:

	// auto table parameters
	CREATE TYPE BeerTable (Name [nvarchar](256), Flavor [nvarchar](256))
	CREATE PROCEDURE InsertBeer (@BeerList [BeerTable])
	List<Beer> beerList = new List<Beer>();
	conn.Execute("InsertBeer", new { BeerList = beerList });

Insight can also stream objects over your database's BulkCopy protocol. 

	// auto bulk-copy objects
	IEnumerable<Beer> beerList; // from somewhere
	conn.BulkCopy("Beer", beerList);

Oh, wait. You want to inline your SQL. We do that too. Just add Sql.

	var beerList = conn.QuerySql<Beer>("SELECT * FROM Beer WHERE Name LIKE @Name", new { Name = "%ipa%" });
	conn.ExecuteSql ("INSERT INTO Beer VALUES (ID, Name)", beer);

But if you *really* want to control every aspect of mapping, see the [wiki](https://github.com/jonwagner/Insight.Database/wiki).

# v3.0 - Now Supporting Lots of Databases and Tools #

Insight.Database v3.0 now supports multiple database providers and several testing frameworks. See [the list of supported providers](https://github.com/jonwagner/Insight.Database/wiki/Insight-and-Data-Providers).

If your database or toolset isn't listed, just open an issue on Github. Good things have been known to happen.

# v2.1 - Now with Automatic Interface Mapping #

- Insight will now automatically convert your .NET interface to SQL calls! See the wiki for some automajikal love!
- Also now with handy wrappers for managing transactions.

# v2.0 - Faster Than Ever #

- v2.0 has full async reads in .NET 4.5, automatic multi-recordset processing, customizable binding rules, tons of optimizations, and more code than I can remember.
- v2.0 should be compile-compatible with v1.x. (Except for a few APIs I'm pretty sure nobody is using.) It's not binary-compatible with v1.x.
- If you are using Insight.Database.Schema, please upgrade to v2.0.8, which no longer has a dependency on Insight.Database v1.0.

# Documentation #

**Full documentation is available on the [wiki](https://github.com/jonwagner/Insight.Database/wiki)**

