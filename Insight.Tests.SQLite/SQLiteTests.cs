using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Insight.Tests.SQLite
{
	[TestFixture]
    public class SQLiteTests
    {
		private static string _filename = "testdb.sqlite";
		private SqliteConnectionStringBuilder _builder;
		private SqliteConnection _connection;

		[SetUp]
		public void SetUp()
		{
			_builder = new SqliteConnectionStringBuilder() { DataSource = _filename };
			_connection = new SqliteConnection(_builder.ConnectionString);
		}

		[TearDown]
		public void TearDown()
		{
			try
			{
				File.Delete(_filename);
			}
			catch { }
		}

		public class Beer
		{
			public int id;
			public string name;
		}
		
		[Test]
		public void BasicTests()
		{
			var beer = new Beer() { id = 1, name = "Troeg's Mad Elf" };

			_connection.ExecuteSql("CREATE TABLE Beer (id int, name varchar(400))");
			_connection.ExecuteSql("INSERT INTO Beer (id, name) values (@id, @name)", beer);

			var pour = _connection.QuerySql<Beer>("SELECT * FROM Beer WHERE ID = @id", new { id = 1 }).FirstOrDefault();
			ClassicAssert.IsNotNull(pour);
			ClassicAssert.AreEqual(beer.id, pour.id);
			ClassicAssert.AreEqual(beer.name, pour.name);
		}
    }
}
