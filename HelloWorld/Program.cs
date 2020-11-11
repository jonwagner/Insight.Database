using System;
using Microsoft.Data.SqlClient;
using Insight.Database;

namespace HelloWorld
{
    class Program
    {
		public static readonly string connectionString = "Data Source = .; Initial Catalog = InsightDbTests; Integrated Security = false; User ID = sa; Password = Insight!!!Test";
        private static SqlConnectionStringBuilder Database = new SqlConnectionStringBuilder(connectionString);

        static void Main(string[] args)
        {
			SqlInsightDbProvider.RegisterProvider();

            using (var c = Database.Open())
			{
				try
				{
					c.ExecuteSql("DROP PROCEDURE HelloWorld", null);					
				}
				catch
				{
				}
				try
				{
					c.ExecuteSql("CREATE PROCEDURE HelloWorld AS SELECT 'Hello, World.'", null);
					Console.WriteLine(c.ExecuteScalar<String>("HelloWorld", null));
				}
				finally
				{
					c.ExecuteSql("DROP PROCEDURE HelloWorld", null);					
				}
			}
        }
    }
}
