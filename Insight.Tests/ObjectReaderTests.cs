using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database;
using NUnit.Framework;
using Insight.Database.Reliable;
using System.Data.Common;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	class ObjectReaderTests : BaseTest
	{
		#region Converting Types While Reading
		/// <summary>
		/// Tests the conversion of types when reading a value from an object (say, as a string)
		/// and sending it to a TVP column of a different type (say, a guid).
		/// </summary>
		[Test]
		public void TestConvertToTypeWhileReading()
		{
			TestConvertToTypeWhileReading<Guid>("uniqueidentifier", Guid.NewGuid());
			TestConvertToTypeWhileReading<byte>("tinyint");
			TestConvertToTypeWhileReading<short>("smallint");
			TestConvertToTypeWhileReading<int>("int");
			TestConvertToTypeWhileReading<long>("bigint");
			TestConvertToTypeWhileReading<float>("real");
			TestConvertToTypeWhileReading<double>("float");
			TestConvertToTypeWhileReading<decimal>("decimal(18,5)");
			TestConvertToTypeWhileReading<bool>("bit");
			TestConvertToTypeWhileReading<char>("char(1)");
			TestConvertToTypeWhileReading<DateTime>("date", DateTime.Now);
			TestConvertToTypeWhileReading<DateTimeOffset>("datetimeoffset");
			TestConvertToTypeWhileReading<TimeSpan>("time");
		}

		private void TestConvertToTypeWhileReading<T>(string sqlType, T value = default(T)) where T : struct
		{
			List<T> list = new List<T>() { value };

			string tableName = String.Format("ObjectReaderTable_{0}", typeof(T).Name);
			string procName = String.Format("ObjectReaderProc_{0}", typeof(T).Name);

			try
			{
				Connection().ExecuteSql(String.Format("CREATE TYPE {1} AS TABLE (value {0})", sqlType, tableName));

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql(String.Format("CREATE PROC {0} @values {1} READONLY AS SELECT value FROM @values", procName, tableName));

					// convert a string value to the target type
					connection.Execute(procName, list.Select(item => new { Value = item.ToString() }));

					// convert a null nullable<T> to T
					connection.Execute(procName, list.Select(item => new { Value = (Nullable<T>)null }));

					// convert a non-null nullable<T> to T
					connection.Execute(procName, list.Select(item => new { Value = (Nullable<T>)value }));
				}
			}
			finally
			{
				Connection().ExecuteSql(String.Format("DROP TYPE {0}", tableName));
			}
		}
#endregion

#region Using Implicit/Explicit Operators For Conversions
		struct Money
		{
			private decimal _d;

			public static implicit operator decimal(Money m)
			{
				return m._d;
			}
		}

		[Test]
		public void ImplicitOperatorsShouldBeUsedIfAvailable()
		{
			try
			{
				Connection().ExecuteSql("CREATE TYPE ObjectReader_ImplicitTable AS TABLE (value decimal(18,5))");

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql("CREATE PROC ObjectReader_Implicit @values ObjectReader_ImplicitTable READONLY AS SELECT value FROM @values");

					Money m = new Money();
					List<Money> list = new List<Money>() { m };
					connection.Execute("ObjectReader_Implicit", list.Select(item => new { Value = item }));
				}
			}
			finally
			{
				Connection().ExecuteSql("DROP TYPE ObjectReader_ImplicitTable");
			}
		}
#endregion

#region Using IConvertible For Conversions
		struct ConvertibleMoney : IConvertible
		{
			private decimal _d;

			public TypeCode GetTypeCode()
			{
				throw new NotImplementedException();
			}

			public bool ToBoolean(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public byte ToByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public char ToChar(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public DateTime ToDateTime(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public decimal ToDecimal(IFormatProvider provider)
			{
				return _d;
			}

			public double ToDouble(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public short ToInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public int ToInt32(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public long ToInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public sbyte ToSByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public float ToSingle(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public string ToString(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public object ToType(Type conversionType, IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public ushort ToUInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public uint ToUInt32(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			public ulong ToUInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// For object reads, the system should be able to use IConvertible for types if available.
		/// </summary>
		[Test]
		public void IConvertibleShouldBeUsedIfAvailable()
		{
			try
			{
				Connection().ExecuteSql("CREATE TYPE ObjectReader_IConvertibleTable AS TABLE (value decimal(18,5))");

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql("CREATE PROC ObjectReader_IConvertible @values ObjectReader_IConvertibleTable READONLY AS SELECT value FROM @values");

					ConvertibleMoney m = new ConvertibleMoney();
					List<ConvertibleMoney> list = new List<ConvertibleMoney>() { m };
					connection.Execute("ObjectReader_IConvertible", list.Select(item => new { Value = item }));
				}
			}
			finally
			{
				Connection().ExecuteSql("DROP TYPE ObjectReader_IConvertibleTable");
			}
		}
#endregion

#region Reading Special Types
		/// <summary>
		/// Tests the conversion of types when reading a value from an object (say, as a string)
		/// and sending it to a TVP column of a different type (say, a guid).
		/// </summary>
		[Test]
		public void TestReadingClassTypesFromObjects()
		{
			TestReadingClassType<byte[]>("varbinary(MAX)", new byte[10], "bytearray");
			TestReadingClassType<string>("varchar(MAX)", "", "string");

			// type mismatches should throw
			Assert.Throws<InvalidCastException>(() => TestReadingClassType<string>("varbinary(MAX)", "", "bytearray"));
		}

		private void TestReadingClassType<T>(string sqlType, T value, string tableBaseName) where T : class
		{
			List<T> list = new List<T>() { value };

			string tableName = String.Format("ObjectReaderTable_{0}", tableBaseName);
			string procName = String.Format("ObjectReaderProc_{0}", tableBaseName);

			try
			{
				Connection().ExecuteSql(String.Format("CREATE TYPE {1} AS TABLE (value {0})", sqlType, tableName));

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql(String.Format("CREATE PROC {0} @values {1} READONLY AS SELECT value FROM @values", procName, tableName));

					// convert a string value to the target type
					connection.Execute(procName, list);
				}
			}
			finally
			{
				Connection().ExecuteSql(String.Format("DROP TYPE {0}", tableName));
			}
		}		
#endregion

#region Missing Table Tests
		[Test]
		public void ProcWithMissingTableParameterShouldThrow()
		{
			try
			{
				Connection().ExecuteSql("CREATE TYPE Missing_Table AS TABLE (value int)");

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql("CREATE PROC ProcWithMissingTable (@table Missing_Table READONLY) AS SELECT * FROM @table");

					// sql will silently eat table parameters that are not specified, and that can be difficult to debug
					Assert.Throws<InvalidOperationException>(() => connection.Query("ProcWithMissingTable"));

					// so you should be able to specify an empty list
					connection.Query("ProcWithMissingTable", new { Table = Parameters.EmptyListOf<int>() });
				}
			}
			finally
			{
				Connection().ExecuteSql("DROP TYPE Missing_Table");
			}
		}

		[Test]
		public void DynamicProcWithMissingTableParameterShouldThrow()
		{
			try
			{
				Connection().ExecuteSql("CREATE TYPE Missing_Table AS TABLE (value int)");

				using (var connection = ConnectionWithTransaction())
				{
					connection.ExecuteSql("CREATE PROC ProcWithMissingTable (@table Missing_Table READONLY) AS SELECT * FROM @table");

					// sql will silently eat table parameters that are not specified, and that can be difficult to debug
					Assert.Throws<InvalidOperationException>(() => connection.Dynamic().ProcWithMissingTable());

					// so you should be able to specify an empty list
					connection.Dynamic().ProcWithMissingTable(new { Table = Parameters.EmptyList });
					connection.Dynamic().ProcWithMissingTable(Table: Parameters.EmptyList);
				}
			}
			finally
			{
				Connection().ExecuteSql("DROP TYPE Missing_Table");
			}
		}
#endregion

#region Table Schema Tests
		[Test]
		public void TestTablesInSchemasWithDots()
		{
			try
			{
				Connection().ExecuteSql("CREATE SCHEMA [vk.common]");
				Connection().ExecuteSql("CREATE TYPE [vk.common].TableOfInt AS TABLE (id int)");
				Connection().ExecuteSql("CREATE PROC [vk.common].MyProc (@table [vk.common].TableOfInt READONLY) AS SELECT 1");

				Connection().Execute("[vk.common].MyProc", new { table = Enumerable.Range(1, 10) });
			}
			finally
			{
				try { Connection().ExecuteSql("DROP PROC [vk.common].MyProc"); } catch { }
				try { Connection().ExecuteSql("DROP TYPE [vk.common].TableOfInt"); } catch { }
				try { Connection().ExecuteSql("DROP SCHEMA [vk.common]"); } catch { }
			}
		}
#endregion

#region Retry Tests
		public class TestIssue215 : BaseTest
		{
			class TestData
			{
				public int i;
			}

			class MyRetryStrategy : RetryStrategy
			{
				public override bool IsTransientException(Exception exception)
				{
					return true;
				}
			}

			/// <summary>
			/// When a TVP is retried, the ObjectList has already been read in the first pass and doesn't get sent up in the second pass.
			/// </summary>
			[Test]
			public void RetryWithTableParameterShouldSendRecords()
			{
				var retryStrategy = new MyRetryStrategy();
				retryStrategy.MaxRetryCount = 1;
				retryStrategy.MaxBackOff = new TimeSpan(0, 0, 0, 0, 1);

				try
				{
					Connection().ExecuteSql("CREATE TYPE TestTable AS TABLE (value int)");

					using (var connection = Connection())
					{
						var reliable = new ReliableConnection((DbConnection)connection, retryStrategy);

						reliable.ExecuteSql("CREATE TABLE TestTableFlag (id int)");
						reliable.ExecuteSql(@"
							CREATE PROC ProcWithTestTable (@table TestTable READONLY) AS 
								IF NOT EXISTS (SELECT * FROM TestTableFlag)
								BEGIN
									INSERT INTO TestTableFlag VALUES (1);
									RAISERROR ('Force a Retry', 16, 1);
								END
								ELSE
									SELECT COUNT(*) FROM @table;
						");

						var list = new List<TestData>() { new TestData() { i = 1 } };
						var count = reliable.ExecuteScalar<int>("ProcWithTestTable", new { table = list });

						Assert.AreEqual(list.Count, count);
					}
				}
				finally
				{
					Connection().ExecuteSql("DROP PROC ProcWithTestTable");
					Connection().ExecuteSql("DROP TABLE TestTableFlag");
					Connection().ExecuteSql("DROP TYPE TestTable");
				}
			}
		}
		#endregion

		#region TVP with Identity
		class TVPWithIdentityData
		{
			public int ID;
			public string Stuff;
		}

		[Test]
		public void CanInsertDataIntoTVPWithIdentityColumn()
		{
			var data = new TVPWithIdentityData[]
			{
				new TVPWithIdentityData(),
				new TVPWithIdentityData(),
				new TVPWithIdentityData()
			};

			var result = Connection().Query<int>("[CallTVPWithIdentityColumn]", data);
			Assert.AreEqual(new int[] { 1, 2, 3 }, result);
		}
		#endregion
	}
}
