#if NO_COMMAND_BUILDER
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Insight.Database.MissingExtensions;

namespace Insight.Database.Providers.Default
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "This class is an implementation wrapper.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "This class is an implementation wrapper.")]
	static class SqlParameterHelper
	{
		/// <summary>
		/// Get the parameters for the stored the proc specified in the command. Only implemented for SQL 2008 or higher.
		/// </summary>
		/// <param name="cmdToPopulate">The command to populate.</param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void DeriveParameters(SqlCommand cmdToPopulate)
		{
			AssertCommandIsValid(cmdToPopulate, "DeriveParameters");
			AssertConnectionIsValid(cmdToPopulate);

			var newParameters = new List<SqlParameter>();

			var procToPopulateName = new SqlObjectName(cmdToPopulate.CommandText);
			var parameterRequestProc = new SqlObjectName(procToPopulateName.Database, "sys", "sp_procedure_params_100_managed");

			var parameterRequestCmd = new SqlCommand(parameterRequestProc.Name, cmdToPopulate.Connection, cmdToPopulate.Transaction);
			parameterRequestCmd.CommandType = CommandType.StoredProcedure;
			parameterRequestCmd.Parameters.Add(CreateNVarCharParameter("@procedure_name", procToPopulateName.Name));

			if (!procToPopulateName.Schema.IsNullOrWhiteSpace())
				parameterRequestCmd.Parameters.Add(CreateNVarCharParameter("@procedure_schema", procToPopulateName.Schema));

			using (SqlDataReader parameterReader = parameterRequestCmd.ExecuteReader())
			{
				var columnMap = new ParameterColumnMap(parameterReader);

				while (parameterReader.Read())
				{
					SqlParameter parameter = CreateParameter(parameterReader, columnMap);
					newParameters.Add(parameter);
				}
			}

			if (newParameters.Count == 0)
				throw new InvalidOperationException($"The stored procedure '{cmdToPopulate.CommandText}' doesn't exist.");

			cmdToPopulate.Parameters.Clear();
			cmdToPopulate.Parameters.AddRange(newParameters.ToArray());
		}

		/// <summary>
		/// The the stored procedure parameter parameters for the proc specified in the command
		/// </summary>
		/// <param name="cmdToPopulate">The command containing the stored procedure</param>
		/// <returns>A SqlDataReader containing the parameters.</returns>
		private static SqlDataReader GetParametersAsReader(SqlCommand cmdToPopulate)
		{
			var procToPopulateName = new SqlObjectName(cmdToPopulate.CommandText);
			var parameterRequestProc = new SqlObjectName(procToPopulateName.Database, "sys", "sp_procedure_params_100_managed");

			var parameterRequestCmd = new SqlCommand(parameterRequestProc.Name, cmdToPopulate.Connection, cmdToPopulate.Transaction);
			parameterRequestCmd.CommandType = CommandType.StoredProcedure;
			parameterRequestCmd.Parameters.Add(CreateNVarCharParameter("@procedure_name", procToPopulateName.Name));

			if (!procToPopulateName.Schema.IsNullOrWhiteSpace())
				parameterRequestCmd.Parameters.Add(CreateNVarCharParameter("@procedure_schema", procToPopulateName.Schema));

			SqlDataReader parameterReader = parameterRequestCmd.ExecuteReader();
			return parameterReader;
		}

		private static SqlParameter CreateParameter(SqlDataReader reader, ParameterColumnMap columnMap)
		{
			SqlParameter parameter = new SqlParameter();

			parameter.ParameterName = ReadDbNullStringSafely(reader, columnMap.ParamName);

			int managedTypeId = reader.GetInt16(columnMap.ParamDataTypeNum);
			parameter.SqlDbType = GetSqlDbTypeEnumFromManagedTypeId(managedTypeId);

			string sqlDbTypeName = ReadDbNullStringSafely(reader, columnMap.TypeName);

			if (sqlDbTypeName == null)  // Fixes unit test issue where simple UDT's have a null type (Insight fixes the null later)
				parameter.SqlDbType = SqlDbType.Udt; // Set the type to UDT so insight knows its a UDT

			parameter.Direction = GetParameterDirection(reader.GetInt16(columnMap.ParamDirType));

			object charLengthObj = reader[columnMap.Length];

			if (charLengthObj is int)
			{
				int charLength = (int)charLengthObj;

				// Sql sometimes returns zero rather than -1, so fix it (it failed output parameters unit tests)
				if (charLength == 0 && (parameter.SqlDbType == SqlDbType.NVarChar || parameter.SqlDbType == SqlDbType.VarBinary
										|| parameter.SqlDbType == SqlDbType.VarChar))
					charLength = -1;

				parameter.Size = charLength;
			}

			if (parameter.SqlDbType == SqlDbType.Decimal)
			{
				parameter.Scale = (byte)reader.GetInt16(columnMap.Scale);
				parameter.Precision = (byte)reader.GetInt16(columnMap.Precision);
			}

			if (parameter.SqlDbType == SqlDbType.Structured)
			{
				parameter.TypeName = ReadDbNullStringSafely(reader, columnMap.TypeCatalogName) + "." +
										ReadDbNullStringSafely(reader, columnMap.TypeSchemaName) + "." +
										ReadDbNullStringSafely(reader, columnMap.TypeName);
			}

#if !NO_UDT
			if (parameter.SqlDbType == SqlDbType.Udt)
			{
				parameter.UdtTypeName = ReadDbNullStringSafely(reader, columnMap.TypeCatalogName) + "." +
										   ReadDbNullStringSafely(reader, columnMap.TypeSchemaName) + "." +
										   ReadDbNullStringSafely(reader, columnMap.TypeName);
			}
#endif

			if (parameter.SqlDbType == SqlDbType.Xml)
			{
				parameter.XmlSchemaCollectionDatabase = ReadDbNullStringSafely(reader, columnMap.XmlCatalogName);
				parameter.XmlSchemaCollectionOwningSchema = ReadDbNullStringSafely(reader, columnMap.XmlSchemaName);
				parameter.XmlSchemaCollectionName = ReadDbNullStringSafely(reader, columnMap.XmlCollectionName);
			}

			if ((parameter.SqlDbType == SqlDbType.DateTime2 || parameter.SqlDbType == SqlDbType.Time
				|| parameter.SqlDbType == SqlDbType.DateTimeOffset))
			{
				object value = reader.GetValue(columnMap.DateTimePrecision);
				if (value is int)
					parameter.Scale = (byte)(((int)value) & 0xff);
			}

			return parameter;
		}

		private static string ReadDbNullStringSafely(SqlDataReader reader, int columnNum)
		{
			if (reader.IsDBNull(columnNum))
				return string.Empty;

			return reader.GetString(columnNum);
		}

		private static ParameterDirection GetParameterDirection(short sqlParamType)
		{
			switch (sqlParamType)
			{
				case 1: return ParameterDirection.Input;
				case 2: return ParameterDirection.InputOutput;
				case 3: return ParameterDirection.Output;
				case 4: return ParameterDirection.ReturnValue;
			}

			return ParameterDirection.Input;
		}

		private static SqlParameter CreateNVarCharParameter(string parameterName, string value)
		{
			// Length = MaxValue so the query plan can be cached
			SqlParameter p = new SqlParameter(parameterName, SqlDbType.NVarChar, (int)byte.MaxValue);
			p.Value = value;
			return p;
		}

		private static SqlDbType GetSqlDbTypeEnumFromManagedTypeId(int managedTypeId)
		{
			return (SqlDbType)managedTypeId;
		}

		private static void AssertCommandIsValid(SqlCommand cmd, string method)
		{
			if (cmd.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException(
					$"DeriveParameters supports CommandType.StoredProcedure, not CommandType.{cmd.CommandType}");
			}

			if (cmd.CommandText.Length == 0)
				throw new InvalidOperationException("CommandText has not been set for this Command.");
		}

		private static void AssertConnectionIsValid(SqlCommand cmd)
		{
			if (cmd.Connection == null)
				throw new InvalidOperationException($"The command does not have a connection.");

			if (cmd.Connection.State != ConnectionState.Open)
				throw new InvalidOperationException($"The connection is not open.");

			if (cmd.Transaction != null && cmd.Transaction.Connection != cmd.Connection)
				throw new InvalidOperationException("The connection does not have the same transaction as the command.");
		}

		private class ParameterColumnMap
		{
			internal ParameterColumnMap(SqlDataReader reader)
			{
				ParamName = reader.GetOrdinal("PARAMETER_NAME");
				ParamDirType = reader.GetOrdinal("PARAMETER_TYPE");
				ParamDataTypeNum = reader.GetOrdinal("MANAGED_DATA_TYPE");
				Length = reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH");
				Precision = reader.GetOrdinal("NUMERIC_PRECISION");
				Scale = reader.GetOrdinal("NUMERIC_SCALE");
				TypeCatalogName = reader.GetOrdinal("TYPE_CATALOG_NAME");
				TypeSchemaName = reader.GetOrdinal("TYPE_SCHEMA_NAME");
				TypeName = reader.GetOrdinal("TYPE_NAME");
				XmlCatalogName = reader.GetOrdinal("XML_CATALOGNAME");
				XmlSchemaName = reader.GetOrdinal("XML_SCHEMANAME");
				XmlCollectionName = reader.GetOrdinal("XML_SCHEMACOLLECTIONNAME");
				DateTimePrecision = reader.GetOrdinal("SS_DATETIME_PRECISION");
			}

			internal int ParamName { get; }
			internal int ParamDirType { get; }
			internal int ParamDataTypeNum { get; }
			internal int Length { get; }
			internal int Precision { get; }
			internal int Scale { get; }
			internal int TypeCatalogName { get; }
			internal int TypeSchemaName { get; }
			internal int TypeName { get; }
			internal int XmlCatalogName { get; }
			internal int XmlSchemaName { get; }
			internal int XmlCollectionName { get; }
			internal int DateTimePrecision { get; }
		}
	}
}
#endif
