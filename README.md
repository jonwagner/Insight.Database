# Insight.Database #

**Insight.Database** is a lightweight micro-ORM for .NET.

# Why You Want This #

# Some Examples #
## Getting Started ##
1. Get the nuGet package: [http://www.nuget.org/packages/Insight.Database](http://www.nuget.org/packages/Insight.Database)
1. Add a reference to `Insight.Database;` to your code. Insight.Database is wired up using extension methods.
1. Relax and enjoy.

## Execute a Stored Procedure ##
Executing a stored procedure is pretty easy. Just call Execute with the name of the procedure. Pass in an object for the parameters. Insight figures out the rest. Use an anonymous type if you like.

	using (SqlConnection conn = new SqlConnection(connectionString))
	{
		conn.Open();
		conn.Execute("AddBeer", new { Name = "IPA", Flavor = "Bitter"});
	}

## Auto-Open/Close ##
If you are too lazy to open and close your connection, Insight will do it for you. If you give it a closed connection, most methods will automatically open and close the connection for you. Many times, you are just making individual one-shot calls to the database, so why should you have to manage that lifetime?

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

Don't like classes? Use dynamic objects! This is great for those "utility projects"! Just omit the generic &lt;Type&gt; from the Query methods.

	var beer = Database.Query("FindBeer", new Name = "IPA").First();
	beer.Flavor = "Yummy";
	Database.Execute("UpdateBeer", beer);

Your objects can go round trip.

# Documentation #

Coming soon! (Wanna help?)

- Asynchronous SQL
- Multi-class Result Sets
- Multiple Record Sets
- Table Parameters
- Bulk Copy
- Streaming

# Design Goals #

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