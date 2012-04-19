# Insight.Database #

**Insight.Database** is a lightweight micro-ORM for .NET.

# Why You Want This #
- It just works. 
- Without a lot of effort or configuration. 
- It's fast.
- It supports structured, production quality coding.
- It supports ad-hoc, one-off, typeless, get-it-done coding.

# Some Examples #
## Getting Started ##
1. Get the nuGet package: [http://www.nuget.org/packages/Insight.Database](http://www.nuget.org/packages/Insight.Database)
1. Add a reference to `Insight.Database;` to your code. Insight.Database is wired up using extension methods.
1. Relax and enjoy.

## Executing a Stored Procedure ##
Executing a stored procedure is pretty easy. Just call Execute with the name of the procedure. Pass in an object for the parameters. Insight figures out the rest. Use an anonymous type if you like.

	using (SqlConnection conn = new SqlConnection(connectionString))
	{
		conn.Open();
		conn.Execute("AddBeer", new { Name = "IPA", Flavor = "Bitter"});
	}

## Auto-Open/Close ##
If you are too lazy to open and close your connection, Insight will do it for you. If you give it a closed connection, most methods will automatically open and close the connection for you at the right time. Many times, you are just making individual one-shot calls to the database, so why should you have to manage that lifetime?

	SqlConnection conn = new SqlConnection(connectionString);
	conn.Execute("AddBeer", new { Name = "IPA", Flavor = "Bitter"});

If you combine this with an DI tool like [Ninject](http://www.ninject.org/), your code can be very clean.

	class Bartender
	{
		[Inject]
		private SqlConnection Database;

		public void AddBeer(string name, string flavor)
		{
			Database.Execute("AddBeer", new { Name = name, Flavor = flavor});
		}
	}

## Query for Objects ##
You can use the Query and QuerySql methods to get objects back from your database.

	class Beer
	{
		public string Name;
		public string Flavor;		
	}
	...
	List<Beer> beers = Database.Query<Beer>("FindBeer", new { Name = "IPA" });

Don't forget that you can *send* objects to the database. Insight will map the object fields to the stored procedure parameters.

	Database.Execute("UpdateBeer", beer);
	
## Convenience ##
Don't like stored procedures? Use the ExecuteSql and QuerySql methods to execute SQL text. Insight will map the parameters into the @parameters in the SQL.

	Database.ExecuteSql("UPDATE Beer SET Flavor = @Flavor WHERE Name = @Name", beer);
	List<Beer> beers = Database.QuerySql<Beer>("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" });

Don't like classes? Use dynamic objects! This is great for those "utility projects"! Just omit the generic &lt;Type&gt; from the Query methods, and Insight will hand you untyped dynamic objects. (Technically a FastExpando, but it can quack like a duck if you like.)

	var beer = Database.Query("FindBeer", new Name = "IPA").First();
	beer.Flavor = "Yummy";
	Database.Execute("UpdateBeer", beer);

Your objects can go round trip.

## Multi-Class Result Sets ##
Returning a hierarchy of objects? Got that too. Simply pass a list of types into the Query method and we will figure out the rest.

	class Car
	{
		public int Doors;
		public Tire Tires;
	}

	class Tire
	{
		public string Brand;
		public string Size;
	}

	List<Car> cars = Database.Query<Car, Tire>("SELECT c.Doors, t.Brand, t.Size FROM Cars JOIN Tires...", Parameters.Empty);

Insight assumes the order of the columns and the order of the generic class parameters are the same. It detects the boundary between them as the first field that DOES NOT map to the first class but DOES map to the second class.

This supports up to 5 types, and there are ways to manually handle the mapping, or the object hierarchy.

## Multiple Result Sets ##
Sometimes a query returns multiple record sets. Sorry, gang, but you will have to do a *little* bit of work.

	using (SqlConnection conn = new SqlConnection(connectionString).Open())
	using (SqlDataReader reader = conn.GetReader("GetBeerAndChips", new { Pub = "Fergie's" }))
	{
		List<Beer> beer = reader.ToList<Beer>();
		List<Chips> chips = reader.ToList<Chips>();
	}

In this case we use the GetReader method to create a SqlCommand and execute it. Then we can use the object conversion routines to convert the results to objects. Note that GetReader *does not* support auto-close.

## Asynchronous SQL ##
If you want to do anything with any amount of load and you don't want the .NET ThreadPool to bite you (trust me, it will), then you need to write your code asynchronously. In general, it's pretty ugly, but Insight will take care of it for you. It even knows when to open and close the connection for you.

Simply call AsyncExecute or AsyncQuery and you will get a Task&lt;T&gt; representing the completion of the query.

	// need to enable async on the connection
	Database = new SqlConnection("...;AsynchronousProcessing=true;");

	Task<Beer> getMeABeerMenu = Database.AsyncQuery<Beer>("FindBeer", new { Name = "Sly Fox" });
	// go do other things. really. we'll be fine.
	List<Beer> beerMenu = getMeABeerMenu.Result;

Once you start running C# 4.5, it all becomes clear:

	List<Beer> beerMenu = await Database1.AsyncQuery<Beer>("FindBeer", new { Name = "Sly Fox" });
	List<Food> foodMenu = await Database2.AsyncQuery<Beer>("FindFood", new { Meal = "Lunch" });

	await Order(beerMenu.First(), foodMenu.First());
	
Just try to do this with the built in SqlCommand class. We dare you.

Note that in the second example, we needed two database connections. You can only run one query at a time against a connection. If you try it with the same connection, it might actually work, but that's just because your threads haven't collided yet. They will.

It's a bit ugly having two database connections, particularly if you are using the beautiful Ninject pattern above. You would have to inject two connections into the Bartender class. It's much better (and a little more lightweight if you don't always use the connections) to inject a SqlConnectionStringBuilder instead. The handy **.Connection()** extension method can convert the builder into a connection and off you go. There are probably even better ways to do this.

	class Bartender
	{
		// let your injection code send the proper connection string in
		[Inject]
		private SqlConnectionStringBuilder Database;

		public async void ServeTable()
		{
			List<Beer> beerMenu = await Database.Connection().AsyncQuery<Beer>("FindBeer", new { Name = "Sly Fox" });
			List<Food> foodMenu = await Database.Connection().AsyncQuery<Beer>("FindFood", new { Meal = "Lunch" });
		
			await Order(beerMenu.First(), foodMenu.First());
		}
	}

## Sending Lists of Values to a SQL Statement ##
If you are using SQL statements, rather than stored procedures, you can send lists of values as parameters. This works with any IEnumerable&lt;ValueType&gt;.

	int[] ids = new int[] { 1, 6, 8 };
	Database.ExecuteSql("DELETE FROM Beer WHERE ID in @ids", new { ids = ids });

In this case, Insight will **rewrite** your SQL and expand the parameters:

	DELETE FROM Beer WHERE ID in (@ids1, @ids2, @ids3)
	
We keep the parameters to avoid SQL injection attacks and to allow the query engine to cache the query plan.

## Sending Lists of Objects to a SQL Statement ##
If you send a list of *objects* to a SQL statement, Insight will look for a user-defined table type with the same name as the type you are sending. If you send Beer, Insight will look for a BeerTable table type. So you can do this:

	CREATE TYPE BeerTable (Name [nvarchar](256), Flavor [nvarchar](256))

	List<Beer> beer = new List<Beer>();
	// add beer here
	Database.ExecuteSql("INSERT INTO Beer SELECT Name, Flavor FROM @Beer", new { Beer = beer });

Insight takes care of the mapping.

## Sending Lists of Objects to the Database ##
If you send a list of objects to a *Stored Procedure*, Insight looks at the parameters on the Stored Procedure to determine which table type to use.

	CREATE TYPE BeerTable (Name [nvarchar](256), Flavor [nvarchar](256))
	CREATE PROCEDURE InsertBeer (@Beer [BeerTable])

	List<Beer> beer = new List<Beer>();
	Database.Execute("InsertBeer", new { Beer = beer });

## Streaming Objects ##
Sometimes you don't need to have all of the objects in a result set at the same time. If you have a large result set, you might just want to stream the objects rather than putting them in a list. There are two ways to do this.

Use GetReader and AsEnumerable&lt;T&gt;:

	using (SqlConnection conn = new SqlConnection(connectionString).Open())
	using (SqlDataReader reader = conn.GetReader("GetBeer", new { Pub = "Fergie's" }))
	{
		foreach (Beer b in reader.AsEnumerable<Beer>())
		{
			// do stuff one beer at a time
		}
	}

This way requires you to manage the lifetime of the connection, so it's a little bit of work. But it works with IEnumerable so you can send the enumerable into LINQ or other language goodness.

The other way is the ForEach extension method:

	Database.ForEach<Beer>("GetBeer", new { Pub = "Sly Fox" },
		// an action to apply to each beer as it is read
		beer => beer.Drink()
	);

Note that both of these methods will keep the database connection open, so only use this if you are going through a large recordset.

## Bulk Copying Objects ##
Lastly, there are some times where you just want to stream data to SQL Server, but all you have are objects. Never fear! Insight supports BulkCopy!

	IEnumerable<Beer> listOBeer; // from somewhere
	Database.BulkCopy("Beer", listOfBeer);

Now your beer is in the fridge getting cold. Insight automatically grabs the schema for the table and does the mapping.

## Putting it All Together ##
You would probably never actually do this, but you can see how we can combine streaming result sets to objects, transform them or act upon them, then bulk copy them to the database or send them to another stored procedure.

	var incomingBeer = Database1.GetReader("GetAllBeer", Parameters.Empty)
								.AsEnumerable<Beer>()
								.Select(b => new Beer (b) { Discount = 1.0m });
	Database2.BulkCopy("Beer", incomingBeer);

Good news. Free beer!

# Documentation #

Coming soon! (**Wanna help**?)

For now, check out the optional parameters and overloads for more goodies.


# Design Goals #
This section attempts to explain the design philosophy behind the Insight.Database library.

## Faster is Better ##
Insight.Database emits dynamic IL at runtime to bind the objects to parameters or result sets. the bindings are generated at the first call for each signature, and then reused. This keeps the runtime performance as fast as possible. Kudos to the Dapper team ([http://code.google.com/p/dapper-dot-net/](http://code.google.com/p/dapper-dot-net/)) for showing how this could be done efficiently.

## Stored Procedures are Your Friends ##
We are firm believers that you should call your database through stored procedures. It allows you to have control over the API to your database, and gives you a place to abstract the storage and logic and change it at runtime. So methods like *Query* and *Execute* default to CommandType.StoredProcedure.

## Convenience is the New Reality ##
Let's be real. You're not going to use stored procedures for everything, and there are times where you don't even want to bother creating classes for *just this one task*. So we also provide *QuerySql* and *ExecuteSql* that favor text-based queries, and support dynamic objects as both inputs and outputs. We also support auto-open/close semantics for connections so you don't have to. Just wait until we figure out how to integrate it with PowerShell...

## Lightweight is Better ##
Don't make me work to do the easy things. That's why there are no mapping attributes, or XML config files.  We assume you have access to the source code and probably the database schema. Insight.Database infers the mappings for your objects by matching up column names and property names. OK, so it also supports multi-object result sets, object hierarchies, and other complex patterns, but we try to keep it simple. (We'll probably support a little bit of mapping when we figure out how to do it without going down the slippery slope of config files.)

## Object Code is the Priority ##
Insight.Database is designed to let you write your code as objects, passing them in and out with the least amount of boilerplate code. Code readability is a high priority.

## Asynchronous is the Future ##
If you want to scale your system, your code needs to be asynchronous. But writing async database code is virtually impossible, particularly if you want to use an ORM. (And just try to figure out how to close your connection at *just* the right time.) Insight has full support for async code, as long as the database provider supports it (so far this is just SQL Server). And when C# 4.5 goes mainstream, you are going to want this.

## Batchy is Important Too ##
If you want to work in objects, but need high-performance, we can do that too. There are patterns to let you work on objects in result sets as they stream in rather than putting everything in memory at the same time. Did I mention bulk copy support too? Stream your objects into the database! Or get crazy and pull an object result set from one database, transform it, and bulk copy it into another database...all asynchronously!

## Tear at the Dotted Line ##
Insight.Database is a few magik black boxes put together, but we try to put the dotted lines in the right places. There is always a time when you just need to use one bit of the tool to solve a problem. You can use the result set reader separately from the Async extensions, or the command generator.