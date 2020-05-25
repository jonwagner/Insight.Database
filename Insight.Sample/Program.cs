using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;
using Insight.Database;
using Insight.Database.Sample;
using Insight.Database.Providers.SqlInsightDbProvider;
using System.Runtime.CompilerServices;

#pragma warning disable 0649

[assembly: InternalsVisibleTo("Insight.Database.DynamicAssembly")]

namespace Insight.Database.Sample
{
    public class Program
    {
        public static readonly string connectionString = "Data Source = .; Initial Catalog = InsightDbTests; Integrated Security = true";
        private static SqlConnectionStringBuilder Database = new SqlConnectionStringBuilder(connectionString);

        static void Main(string[] args)
        {
            // Registering a provider is usually not necessary
            // SqlInsightDbProvider.RegisterProvider();

            #region Opening Connections
            IDBConnection_OpenConnection();
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

            #region Insert
            Insert_Sql();
            #endregion

            #region Dynamic Objects
            Dynamic_Query();
            Dynamic_ForEach();
            #endregion

            #region Dynamic Database Calls
            DynamicCall_Named();
            DynamicCall_Transaction();
            #endregion

            #region Lists of Objects
            List_ValueTypeSql();
            List_ClassSql();
            #endregion

            #region Async Queries
            Async_Execute();
            Async_Query();
            #endregion

            #region Bulk Copy
            BulkCopy();
            Async_BulkCopy();
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

        class Glass
        {
            public int Id;
            public string Name;
            public int Ounces;
        }

        class Serving
        {
            public int ID;
            public DateTime When;
            public int BeerID { get { return Beer.Id; } }
            public Beer Beer;
            public int GlassesID { get { return Glass.Id; } }
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
            using (IDbConnection c = database.Open())
            {
                c.QuerySql("SELECT * FROM Beer", Parameters.Empty);
            }
        }

        static void SqlConnectionStringBuilder_Open2()
        {
            SqlConnection database = new SqlConnection(connectionString);
            // make other changes here

            // manage the lifetime ourselves
            using (IDbConnection c = database.OpenWithTransaction("test"))
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
            Database.Connection().Insert("InsertBeer", beer);

            // map an anonymous object to the stored procedure parameters
            Database.Connection().Execute("DeleteBeer", beer);
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
            using (IDbConnection connection = Database.Open())
            {
                IDbCommand command = connection.CreateCommand("FindBeers", new { Name = "IPA" });
            }
        }

        static void IDbConnection_CreateCommandSql()
        {
            using (IDbConnection connection = Database.Open())
            {
                IDbCommand command = connection.CreateCommandSql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" });
            }
        }
        #endregion

        #region Common Method Parameters
        static void CommonParameter_Transaction()
        {
            using (var connection = Database.Connection().OpenWithTransaction())
            {
                Beer beer = new Beer("Sly Fox IPA");
                connection.Execute("InsertBeer", beer);

                // without a commit this rolls back
            }
        }
        #endregion

        #region Manual Transformations
        static void ManualTransform()
        {
            List<Beer> beer = Database.Connection().Query(
                "FindBeers",
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
                "FindBeers",
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
            using (IDbConnection connection = Database.Open())
            using (IDataReader reader = connection.GetReader("FindBeers", new { Name = "IPA" }))
            {
                while (reader.Read())
                {
                    // do stuff with the reader here
                }
            }
        }

        static void Async_ManualTransform_GetReader()
        {
            using (IDbConnection connection = Database.Open())
            using (DbDataReader reader = connection.GetReaderAsync("FindBeers", new { Name = "IPA" }).Result)
            {
                while (reader.ReadAsync().Result)
                {

                }
            }
        }

        #endregion

        #region Querying for Objects
        static void Query_Query()
        {
            IList<Beer> beer = Database.Connection().Query<Beer>("FindBeers", new { Name = "IPA" });
        }

        static void Query_QuerySql()
        {
            IList<Beer> beer = Database.Connection().QuerySql<Beer>("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" });
        }

        static void Query_ToList()
        {
            using (IDbConnection connection = Database.Open())
            using (IDataReader reader = connection.GetReader("FindBeers", new { Name = "IPA" }))
            {
                IList<Beer> beer = reader.ToList<Beer>();
            }
        }

        static void Query_AsEnumerable()
        {
            using (IDbConnection connection = Database.Open())
            using (IDataReader reader = connection.GetReader("FindBeers", new { Name = "IPA" }))
            {
                foreach (Beer beer in reader.AsEnumerable<Beer>())
                {
                    // drink?
                }
            }
        }
        #endregion

        #region Insert
        static void Insert_Sql()
        {
            Beer beer = new Beer()
            {
                Name = "HopDevil",
                Style = "Hoppy"
            };

            var insertedBeer = Database.Connection().InsertSql("INSERT INTO Beer (Name, Style) VALUES (@Name, @Style) SELECT SCOPE_IDENTITY() AS [Id]", beer);
        }
        #endregion

        #region Dynamic Objects
        static void Dynamic_Query()
        {
            Beer ipa = new Beer() { Name = "IPA" };
            Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

            foreach (dynamic beer in Database.Connection().QuerySql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" }))
            {
                beer.Style = "crisp";

                // extension methods cannot be dynamically dispatched, so we have to cast it to object
                // good news: it still works
                Database.Connection().ExecuteSql("UPDATE Beer Set Style = @Style WHERE Name = @Name", (object)beer);
            }

            Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
        }

        static void Dynamic_ForEach()
        {
            Beer ipa = new Beer() { Name = "IPA" };
            Database.Connection().ExecuteSql("INSERT INTO Beer (Name) VALUES (@Name)", ipa);

            Database.Connection().ForEachDynamicSql("SELECT * FROM Beer WHERE Name = @Name", new { Name = "IPA" },
                beer =>
                {
                    beer.Style = "crisp";

                    // extension methods cannot be dynamically dispatched, so we have to cast it to object
                    // good news: it still works
                    Database.Connection().ExecuteSql("UPDATE Beer Set Style = @Style WHERE Name = @Name", (object)beer);
                });

            Database.Connection().ExecuteSql("DELETE FROM Beer WHERE Name = @Name", ipa);
        }
        #endregion

        #region Dynamic Database Calls
        static void DynamicCall_Named()
        {
            IList<Beer> beer = Database.Connection().Dynamic<Beer>().FindBeers(name: "IPA");
        }

        static void DynamicCall_Transaction()
        {
            using (var connection = Database.Connection().OpenWithTransaction())
            {
                IList<Beer> beer = connection.Dynamic<Beer>().FindBeers(name: "IPA");
            }
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

        static void List_ClassSql()
        {
            List<Beer> beer = new List<Beer>();
            beer.Add(new Beer() { Name = "Sly Fox IPA", Style = "yummy" });
            beer.Add(new Beer() { Name = "Hoppopotamus", Style = "hoppy" });

            Database.Connection().ExecuteSql("INSERT INTO Beer (Name, Style) SELECT Name, Style FROM @Beer", new { Beer = beer });
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
            Task<IList<Beer>> task = Database.Connection().QueryAsync<Beer>("FindBeers", new { Name = "IPA" });

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

        #region Bulk Copy Async
        static void Async_BulkCopy()
        {
            List<Beer> beer = new List<Beer>();
            beer.Add(new Beer() { Name = "Sly Fox IPA", Flavor = "yummy", OriginalGravity = 4.2m });
            beer.Add(new Beer() { Name = "Hoppopotamus", Flavor = "hoppy", OriginalGravity = 3.0m });

            Database.Connection().BulkCopyAsync("Beer", beer);
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
            Console.WriteLine("{0}", d.Name);
            Console.WriteLine("{0}", d.Ounces);
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
                "FindBeers",
                new { Name = "IPA" },
                beer => Drink(beer));
        }

        static void AsEnumerable()
        {
            using (IDbConnection connection = Database.Open())
            using (var reader = connection.GetReaderSql("SELECT * FROM Beer", Parameters.Empty))
            {
                foreach (Beer beer in reader.AsEnumerable<Beer>())
                {
                    Drink(beer);
                }
            }
        }

        static void Drink(Beer b)
        {
            Console.WriteLine("YUM");
        }
        #endregion
    }
}
