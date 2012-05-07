using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database.Sample;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace Insight.Database.Sample
{
	class Program
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
		private static ConnectionStringSettings Database = ConfigurationManager.ConnectionStrings["MyDatabase"];

		static void Main(string[] args)
		{
			#region Opening Connections
			IDBConnection_OpenConnection();
			ConnectionStringSettings_Connection();
			ConnectionStringSettings_Open();
			SqlConnectionStringBuilder_Connection();
			SqlConnectionStringBuilder_Open();
			#endregion

			#region Executing Database Commands
			Execute();
			ExecuteSql();
			#endregion

			#region Querying for Objects
			Query_Query();
			Query_QuerySql();
			Query_ToList();
			Query_AsEnumerable();
			#endregion

			#region Dynamic Objects
			Dynamic_Query();
			Dynamic_ForEach();
			#endregion

			#region Lists of Objects
			List_ValueTypeSql();
			List_ValueTypeStoredProcedure();
			List_ClassStoredProcedure();
			List_ClassSql();
			#endregion

			#region Async Queries
			Async_Execute();
			Async_Query();
			#endregion

			#region Bulk Copy
			BulkCopy();
			#endregion

			#region Multiple Result Sets
			MultipleResultSets();
			#endregion

			#region Object Hierarchies
			ObjectHierarchy();
			#endregion

			#region Creating Commands
			IDbConnection_CreateCommand();
			IDbConnection_CreateCommandSql();
			#endregion

			#region Common Parameters
			CommonParameter_Transaction();
			#endregion

			#region Manual Transformation
			ManualTransform();
			ManualTransform_Sum();
			ManualTransform_GetReader();
			#endregion

			#region Expando Expansion
			Expando_Expand();
			#endregion

			#region Expando Mutation
			Expando_Mutate();
			Expando_Transform();
			Expando_TransformList();
			#endregion

			#region ForEach
			ForEach();
			AsEnumerable();
			#endregion
		}

		class Beer
		{
			public string Name { get; internal set; }
			public string Flavor { get; internal set; }
			public decimal? OriginalGravity { get; internal set; }

			public Beer()
			{
			}

			public Beer(string name)
			{
				Name = name;
			}
		}

		class Glass
		{
			public int Ounces;
		}

		class Serving
		{
			public DateTime When;
			public Beer Beer;
			public Glass Glass;
		}

		#region Opening Connections
		static void IDBConnection_OpenConnection()
		{
			// open the connection and return it
			using (SqlConnection c = new SqlConnection(connectionString).OpenConnection())
			{
				c.QuerySql("SELECT * FROM Beer", Parameters.Empty);
			}
		}

		static void ConnectionStringSettings_Connection()
		{
			ConnectionStringSettings database = ConfigurationManager.ConnectionStrings["MyDatabase"];

			// run a query right off the connection (this performs an auto-open/close)
			database.Connection().QuerySql("SELECT * FROM Beer", Parameters.Empty);
		}
		
		static void ConnectionStringSettings_Open()
		{
			ConnectionStringSettings database = ConfigurationManager.ConnectionStrings["MyDatabase"];

			// get an open connection from the ConnectionStringSettings
			using (SqlConnection c = database.Open())
			{
				c.QuerySql("SELECT * FROM Beer", Parameters.Empty);
			}
		}

		static void SqlConnectionStringBuilder_Connection()
		{
			SqlConnectionStringBuilder database = new SqlConnectionStringBuilder(connectionString);
			// make other changes here

			// run a query right off the connection (this performs an auto-open/close)
			database.Connection().QuerySql("SELECT * FROM Beer", Parameters.Empty);
		}

		static void SqlConnectionStringBuilder_Open()
		{
			SqlConnectionStringBuilder database = new SqlConnectionStringBuilder(connectionString);
			// make other changes here

			// manage the lifetime ourselves
			using (SqlConnection c = database.Open())
			{
				c.QuerySql("SELECT * FROM Beer", Parameters.Empty);
			}
		}
		#endregion

		#region Executing Database Commands
		static void Execute()
		{
			Beer beer = new Beer() { Name = "IPA" };

			// map a beer the stored procedure parameters
			Database.Connection().Execute("InsertBeer", beer);

			// map an anonymous object to the stored procedure parameters
			Database.Connection().Execute("DeleteBeer", new { Name = "IPA" });
		}

		static void ExecuteSql()
		{
			Beer beer = new Beer() { Name = "IPA" };

			// map a beer the stored procedure parameters
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", beer);

			// map an anonymous object to the stored procedure parameters
			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", new { Name = "IPA" });
		}

		static void ExecuteScalar()
		{
			int count = Database.Connection().ExecuteScalar<int>("CountBeer", new { Name = "IPA" });

			int count2 = Database.Connection().ExecuteScalarSql<int>("SELECT COUNT(*) FROM Beer WHERE Name LIKE @Name", new { Name = "IPA" });
		}
		#endregion

		#region Creating Commands
		static void IDbConnection_CreateCommand()
		{
			using (SqlConnection connection = Database.Open())
			{
				IDbCommand command = connection.CreateCommand("FindBeer", new { Name = "IPA" });
			}
		}

		static void IDbConnection_CreateCommandSql()
		{
			using (SqlConnection connection = Database.Open())
			{
				IDbCommand command = connection.CreateCommandSql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" });
			}
		}
		#endregion

		#region Common Method Parameters
		static void CommonParameter_Transaction()
		{
			using (SqlConnection connection = Database.Open())
			using (SqlTransaction t = connection.BeginTransaction())
			{
				Beer beer = new Beer("Sly Fox IPA");
				connection.Execute("InsertBeer", beer, transaction: t);

				// without a commit this rolls back
			}
		}
		#endregion

		#region Manual Transformations
		static void ManualTransform()
		{
			List<Beer> beer = Database.Connection().Query(
				"FindBeer",
				new { Name = "IPA" },
				reader =>
				{
					List<Beer> b = new List<Beer>();

					while (reader.Read())
					{
						b.Add(new Beer(reader["Name"].ToString()));
					}

					return b;
				});
		}

		static void ManualTransform_Sum()
		{
			int totalNameLength = Database.Connection().Query(
				"FindBeer", 
				new { Name = "IPA" }, 
				reader => 
					{
						int total = 0;
						while (reader.Read())
						{
							total += reader["Name"].ToString().Length;
						}
						return total;
					});
			
			Console.WriteLine(totalNameLength);
		}

		static void ManualTransform_GetReader()
		{
			using (SqlConnection connection = Database.Open())
			using (IDataReader reader = connection.GetReader("FindBeer", new { Name = "IPA" }))
			{
				while (reader.Read())
				{
					// do stuff with the reader here
				}
			}
		}
		#endregion

		#region Querying for Objects
		static void Query_Query()
		{
			IList<Beer> beer = Database.Connection().Query<Beer>("FindBeer", new { Name = "IPA" });
		}

		static void Query_QuerySql()
		{
			IList<Beer> beer = Database.Connection().QuerySql<Beer>("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" });
		}

		static void Query_ToList()
		{
			using (SqlConnection connection = Database.Open())
			using (IDataReader reader = connection.GetReader("FindBeer", new { Name = "IPA" }))
			{
				IList<Beer> beer = reader.ToList<Beer>();
			}
		}

		static void Query_AsEnumerable()
		{
			using (SqlConnection connection = Database.Open())
			using (IDataReader reader = connection.GetReader("FindBeer", new { Name = "IPA" }))
			{
				foreach (Beer beer in reader.AsEnumerable<Beer>())
				{
					// drink?
				}
			}
		}
		#endregion

		#region Dynamic Objects
		static void Dynamic_Query()
		{
			Beer ipa = new Beer() { Name = "IPA" };
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

			foreach (dynamic beer in Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" }))
			{
				beer.OriginalGravity = 4.2m;

				// extension methods cannot be dynamically dispatched, so we have to cast it to object
				// good news: it still works
				Database.Connection().ExecuteSql("UPDATE Beer Set OriginalGravity = @OriginalGravity WHERE Name = @Name", (object)beer);
			}

			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
		}

		static void Dynamic_ForEach()
		{
			Beer ipa = new Beer() { Name = "IPA" };
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

			Database.Connection().ForEachSql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" },
				(dynamic beer) =>
				{
					beer.OriginalGravity = 4.2m;

					// extension methods cannot be dynamically dispatched, so we have to cast it to object
					// good news: it still works
					Database.Connection().ExecuteSql("UPDATE Beer Set OriginalGravity = @OriginalGravity WHERE Name = @Name", (object)beer);
				});

			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
		}
		#endregion

		#region Lists of Objects
		static void List_ValueTypeSql()
		{
			IEnumerable<String> names = new List<String>() { "Sly Fox IPA", "Hoppapotamus" };
			var beer = Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name IN (@Name)", new { Name = names });

			names = new string[] { "Sly Fox IPA", "Hoppapotamus" };
			beer = Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name IN (@Name)", new { Name = names });
		}

		static void List_ValueTypeStoredProcedure()
		{
			IEnumerable<String> names = new List<String>() { "Sly Fox IPA", "Hoppapotamus" };
			var beer = Database.Connection().Query("GetBeer", new { Name = names });
		}

		static void List_ClassStoredProcedure()
		{
			List<Beer> beer = new List<Beer>();
			beer.Add(new Beer() { Name = "Sly Fox IPA", Flavor = "yummy", OriginalGravity = 4.2m });
			beer.Add(new Beer() { Name = "Hoppopotamus", Flavor = "hoppy", OriginalGravity = 3.0m });

			Database.Connection().Execute("UpdateBeer", new { Beer = beer });
		}

		static void List_ClassSql()
		{
			List<Beer> beer = new List<Beer>();
			beer.Add(new Beer() { Name = "Sly Fox IPA", Flavor = "yummy", OriginalGravity = 4.2m });
			beer.Add(new Beer() { Name = "Hoppopotamus", Flavor = "hoppy", OriginalGravity = 3.0m });

			Database.Connection().ExecuteSql("INSERT INTO Beer SELECT * FROM @Beer", new { Beer = beer });
		}
		#endregion

		#region Async
		static void Async_Execute()
		{
			Beer beer = new Beer() { Name = "Sly Fox IPA" };

			Task task = Database.Connection().ExecuteAsync("InsertBeer", beer);

			// do stuff

			task.Wait();
		}

		static void Async_Query()
		{
			Task<IList<Beer>> task = Database.Connection().QueryAsync<Beer>("FindBeer", new { Name = "IPA" });

			// do stuff

			var results = task.Result;

			foreach (Beer b in results)
				Console.WriteLine(b.Name);
		}
		#endregion

		#region Bulk Copy
		static void BulkCopy()
		{
			List<Beer> beer = new List<Beer>();
			beer.Add(new Beer() { Name = "Sly Fox IPA", Flavor = "yummy", OriginalGravity = 4.2m });
			beer.Add(new Beer() { Name = "Hoppopotamus", Flavor = "hoppy", OriginalGravity = 3.0m });

			Database.Connection().BulkCopy("Beer", beer);
		}
		#endregion

		#region Multiple Result Sets
		static void MultipleResultSets()
		{
			using (SqlConnection connection = Database.Open())
			using (var reader = connection.GetReaderSql("SELECT * FROM Beer SELECT * FROM Glasses", Parameters.Empty))
			{
				var beer = reader.ToList<Beer>();
				var glasses = reader.ToList<Glass>();
			}
		}
		#endregion

		#region Object Hierarchies
		static void ObjectHierarchy()
		{
			Database.Connection().ExecuteSql("INSERT INTO Beer VALUES ('Sly Fox IPA', 'yummy', 4.2)");
			Database.Connection().ExecuteSql("INSERT INTO Glasses VALUES ('Pilsner', 16)");
			Database.Connection().ExecuteSql("INSERT INTO Servings VALUES ('1/1/2012', 'Sly Fox IPA', 'Pilsner')");

			var servings = Database.Connection().Query<Serving, Beer, Glass>("GetServings", Parameters.Empty);
			foreach (var serving in servings)
				Console.WriteLine("{0} {1} {2}", serving.When, serving.Beer.Name, serving.Glass.Ounces);
		}
		#endregion

		#region Expando Expansion
		static void Expando_Expand()
		{
			Beer beer = new Beer() { Name = "Sly Fox IPA" };
			Glass glass = new Glass() { Ounces = 32 };

			// create an expando and combine the objects
			FastExpando x = beer.Expand();
			x.Expand(glass);

			// look! a dynamic object
			dynamic d = x;
			Console.WriteLine(d.Name);
			Console.WriteLine(d.Ounces);
		}
		#endregion

		#region Expando Mutations
		static void Expando_Mutate()
		{
			Beer ipa = new Beer() { Name = "IPA" };
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

			var mapping = new Dictionary<string, string>() { { "Name", "TheName" } };
			dynamic beer = Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" }).First();
			beer.Mutate(mapping);
			Console.WriteLine(beer.TheName);

			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
		}

		static void Expando_Transform()
		{
			Beer ipa = new Beer() { Name = "IPA" };
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

			var mapping = new Dictionary<string, string>() { { "Name", "TheName" } };
			dynamic beer = Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" }).First().Transform(mapping);
			Console.WriteLine(beer.TheName);

			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
		}

		static void Expando_TransformList()
		{
			Beer ipa = new Beer() { Name = "IPA" };
			Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

			var mapping = new Dictionary<string, string>() { { "Name", "TheName" } };
			foreach (dynamic beer in Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" }).Transform(mapping))
				Console.WriteLine(beer.TheName);

			Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
		}
		#endregion

		#region ForEach
		static void ForEach()
		{
			Database.Connection().ForEach<Beer>(
				"FindBeer", 
				new { Name = "IPA" },
				beer => Drink(beer));
		}

		static void AsEnumerable()
		{
			using (SqlConnection connection = Database.Open())
			using (var reader = connection.GetReaderSql("SELECT * FROM Beer", Parameters.Empty))
			{
				foreach (Beer beer in reader.AsEnumerable<Beer>())
				{
					Drink(beer);
				}
			}
		}

		static void Drink (Beer b)
		{
			Console.WriteLine("YUM");
		}
		#endregion
	}
}
