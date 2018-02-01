using Insight.Database;
using Insight.Database.Mapping;
using Insight.Tests.Cases;
using NUnit.Framework;
using System;
using System.Data;

namespace Insight.Tests
{
	[TestFixture]
	public class ParameterDataTypeMapperTests : BaseTest
	{
		[Test]
		public void TestWithMappingChangeTypeToVarChar()
		{
			ColumnMapping.ParameterDataTypes.AddMapper(new DataTypeMapper());

			ConnectionStateCase.ForEach(c =>
			{
				var command = c.CreateCommand
				(
					sql: "SELECT * FROM dbo.Beer WHERE Style = @mystyle",
					parameters: new { myStyle = "Lager" },
					commandType: CommandType.Text,
					commandTimeout: 10,
					transaction: null
				);

				Assert.AreEqual(((IDataParameter)command.Parameters[0]).DbType, DbType.AnsiString);
			});
		}

		[Test]
		public void TestWithMappingTypeDoesNotChange()
		{
			ColumnMapping.ParameterDataTypes.AddMapper(new DataTypeMapper2());

			ConnectionStateCase.ForEach(c =>
			{
				var command = c.CreateCommand
				(
					sql: "SELECT * FROM dbo.Beer WHERE Style = @changeMyType",
					parameters: new { changeMyType = "Lager" },
					commandType: CommandType.Text,
					commandTimeout: 10,
					transaction: null
				);

				Assert.AreEqual(((IDataParameter)command.Parameters[0]).DbType, DbType.String);
			});
		}

		#region Support Types

		public class DataTypeMapper : IParameterDataTypeMapper
		{
			public DbType MapParameterType(Type type, IDbCommand command, IDataParameter parameter, DbType dbType)
			{
				if (command.CommandType == CommandType.Text && parameter.ParameterName.Equals("mystyle", StringComparison.OrdinalIgnoreCase))
				{
					return DbType.AnsiString;
				}
				return dbType;
			}
		}

		public class DataTypeMapper2 : IParameterDataTypeMapper
		{
			public DbType MapParameterType(Type type, IDbCommand command, IDataParameter parameter, DbType dbType)
			{
				if (command.CommandType == CommandType.Text
						&& parameter.ParameterName.Equals("ChangeMyType", StringComparison.OrdinalIgnoreCase)
						&& command.CommandText == "SELECT 1 WHERE @changeMyType IS NULL")
				{
					return DbType.Date;
				}

				return dbType;
			}
		}

		#endregion
	}
}
