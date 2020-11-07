using System;
using System.Data.SqlClient;
using Insight.Database;

namespace HelloWorld
{
    class Program
    {
		public static readonly string connectionString = "Data Source = .; Initial Catalog = InsightDbTests; Integrated Security = false; User ID = sa; Password = Insight!!!Test";
        private static SqlConnectionStringBuilder Database = new SqlConnectionStringBuilder(connectionString);

        static void Main(string[] args)
        {
            using (var c = Database.Open())
			{
				Console.WriteLine(c.ExecuteScalarSql<String>("SELECT 'Hello, World.'"));
			}
        }
    }
}
