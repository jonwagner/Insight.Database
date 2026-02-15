using Insight.Database;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Data;
using System.Runtime.Serialization;

namespace Insight.Tests.MsSqlClient
{
	[TestFixture]
	public class JsonTests : MsSqlClientBaseTest
	{
		[DataContract]
		public class NativeJsonData
		{
			[DataMember]
			public int P;

			[DataMember]
			public string Text;
		}

		public class NativeJsonParameters
		{
			[Column(SerializationMode = SerializationMode.Json)]
			public NativeJsonData Data;
		}

		[Test]
		public void ObjectCanAutoSerializeToNativeJsonParameterAndStoreInJsonColumn()
		{
			using (var connection = ConnectionWithTransaction())
			{
				if (!SupportsNativeJson(connection))
					Assert.Ignore("Native json type is not available on this SQL Server instance.");

				connection.ExecuteSql("CREATE TABLE NativeJsonDataTable (Data json NOT NULL)");
				connection.ExecuteSql("CREATE PROC InsertNativeJsonData (@Data json) AS INSERT INTO NativeJsonDataTable (Data) VALUES (@Data)");

				var input = new NativeJsonParameters() { Data = new NativeJsonData() { P = 5, Text = "foo" } };
				connection.Execute("InsertNativeJsonData", input);

				ClassicAssert.AreEqual("5", connection.ExecuteScalarSql<string>("SELECT JSON_VALUE(CONVERT(nvarchar(max), Data), '$.P') FROM NativeJsonDataTable"));
				ClassicAssert.AreEqual("foo", connection.ExecuteScalarSql<string>("SELECT JSON_VALUE(CONVERT(nvarchar(max), Data), '$.Text') FROM NativeJsonDataTable"));
			}
		}

		[Test]
		public void ObjectCanAutoDeserializeFromNativeJsonResult()
		{
			using (var connection = ConnectionWithTransaction())
			{
				if (!SupportsNativeJson(connection))
					Assert.Ignore("Native json type is not available on this SQL Server instance.");

				connection.ExecuteSql("CREATE PROC ReflectNativeJsonResult (@Data json) AS SELECT Data=CONVERT(nvarchar(max), @Data)");

				var input = new NativeJsonParameters() { Data = new NativeJsonData() { P = 7, Text = "bar" } };
				var outputList = connection.Query<NativeJsonParameters>("ReflectNativeJsonResult", input);
				ClassicAssert.AreEqual(1, outputList.Count);
				var output = outputList[0];

				ClassicAssert.IsNotNull(output.Data);
				ClassicAssert.AreEqual(7, output.Data.P);
				ClassicAssert.AreEqual("bar", output.Data.Text);
			}
		}

		private static bool SupportsNativeJson(IDbConnection connection)
		{
			return connection.ExecuteScalarSql<int>(
				@"SELECT COUNT(*) FROM sys.types t
					INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
					WHERE s.name = 'sys' AND t.name = 'json'") > 0;
		}
	}
}
