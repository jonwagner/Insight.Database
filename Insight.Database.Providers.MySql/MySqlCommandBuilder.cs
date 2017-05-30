#if NETSTANDARD1_6 || NETSTANDARD2_0
using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using System.Text;
using MySql.Data.Common;
using System.Collections;
using MySql.Data.Types;
using System.Globalization;
using System.Collections.Generic;
using Insight.Database;

namespace MySql.Data.MySqlClient
{
	public static class MySqlCommandBuilder
	{
		public static void DeriveParameters(MySqlCommand command)
		{
			if (command.CommandType != CommandType.StoredProcedure)
				throw new InvalidOperationException(Resources.CanNotDeriveParametersForTextCommands);

			foreach (dynamic row in command.Connection.QuerySql("SELECT * FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_SCHEMA = @dbname AND SPECIFIC_NAME = @procname",
														   new { dbname = command.Connection.Database, procname = command.CommandText }))
			{
				var p = new MySqlParameter();
				p.ParameterName = row.PARAMETER_NAME.ToString();
				if (row.ORDINAL_POSITION.Equals(0) && p.ParameterName == "@")
					p.ParameterName = "@RETURN_VALUE";
				p.Direction = GetDirection(row);

				bool unsigned = row["DTD_IDENTIFIER"].ToString().ToUpper().IndexOf("UNSIGNED") != -1;
				p.MySqlDbType = NameToType(row["DATA_TYPE"].ToString(), unsigned, command.Connection);
				if (row.CHARACTER_MAXIMUM_LENGTH != null)
					p.Size = (int)row.CHARACTER_MAXIMUM_LENGTH;
				if (row.NUMERIC_PRECISION != null)
					p.Precision = Convert.ToByte(row.NUMERIC_PRECISION);
				if (row.NUMERIC_SCALE != null)
					p.Scale = Convert.ToByte(row.NUMERIC_SCALE);

				command.Parameters.Add(p);
			}

/*
-		data	Count = 15	System.Collections.Generic.IDictionary<string, object> {System.Collections.Generic.Dictionary<string, object>}
+		[0]	{[SPECIFIC_CATALOG, def]}	System.Collections.Generic.KeyValuePair<string, object>
+		[1]	{[SPECIFIC_SCHEMA, test]}	System.Collections.Generic.KeyValuePair<string, object>
+		[2]	{[SPECIFIC_NAME, MySqlTestExecute]}	System.Collections.Generic.KeyValuePair<string, object>
+		[3]	{[ORDINAL_POSITION, 1]}	System.Collections.Generic.KeyValuePair<string, object>
+		[4]	{[PARAMETER_MODE, IN]}	System.Collections.Generic.KeyValuePair<string, object>
+		[5]	{[PARAMETER_NAME, i]}	System.Collections.Generic.KeyValuePair<string, object>
+		[6]	{[DATA_TYPE, int]}	System.Collections.Generic.KeyValuePair<string, object>
+		[7]	{[CHARACTER_MAXIMUM_LENGTH, ]}	System.Collections.Generic.KeyValuePair<string, object>
+		[8]	{[CHARACTER_OCTET_LENGTH, ]}	System.Collections.Generic.KeyValuePair<string, object>
+		[9]	{[NUMERIC_PRECISION, 10]}	System.Collections.Generic.KeyValuePair<string, object>
+		[10]	{[NUMERIC_SCALE, 0]}	System.Collections.Generic.KeyValuePair<string, object>
+		[11]	{[CHARACTER_SET_NAME, ]}	System.Collections.Generic.KeyValuePair<string, object>
+		[12]	{[COLLATION_NAME, ]}	System.Collections.Generic.KeyValuePair<string, object>
+		[13]	{[DTD_IDENTIFIER, int(11)]}	System.Collections.Generic.KeyValuePair<string, object>
+		[14]	{[ROUTINE_TYPE, PROCEDURE]}	System.Collections.Generic.KeyValuePair<string, object>
*/
		}

		private static ParameterDirection GetDirection(dynamic parameter)
		{
			string mode = parameter.PARAMETER_MODE.ToString();
			int ordinal = Convert.ToInt32(parameter.ORDINAL_POSITION);

			if (0 == ordinal)
				return ParameterDirection.ReturnValue;
			else if (mode == "IN")
				return ParameterDirection.Input;
			else if (mode == "OUT")
				return ParameterDirection.Output;
			return ParameterDirection.InputOutput;
		}

		public static MySqlDbType NameToType(string typeName, bool unsigned, MySqlConnection connection)
		{
			switch (StringUtility.ToUpperInvariant(typeName))
			{
				case "CHAR": return MySqlDbType.String;
				case "VARCHAR": return MySqlDbType.VarChar;
				case "DATE": return MySqlDbType.Date;
				case "DATETIME": return MySqlDbType.DateTime;
				case "NUMERIC":
				case "DECIMAL":
				case "DEC":
				case "FIXED":
					return MySqlDbType.NewDecimal;
				case "YEAR":
					return MySqlDbType.Year;
				case "TIME":
					return MySqlDbType.Time;
				case "TIMESTAMP":
					return MySqlDbType.Timestamp;
				case "SET": return MySqlDbType.Set;
				case "ENUM": return MySqlDbType.Enum;
				case "BIT": return MySqlDbType.Bit;

				case "TINYINT":
					return unsigned ? MySqlDbType.UByte : MySqlDbType.Byte;
				case "BOOL":
				case "BOOLEAN":
					return MySqlDbType.Byte;
				case "SMALLINT":
					return unsigned ? MySqlDbType.UInt16 : MySqlDbType.Int16;
				case "MEDIUMINT":
					return unsigned ? MySqlDbType.UInt24 : MySqlDbType.Int24;
				case "INT":
				case "INTEGER":
					return unsigned ? MySqlDbType.UInt32 : MySqlDbType.Int32;
				case "SERIAL":
					return MySqlDbType.UInt64;
				case "BIGINT":
					return unsigned ? MySqlDbType.UInt64 : MySqlDbType.Int64;
				case "FLOAT": return MySqlDbType.Float;
				case "DOUBLE": return MySqlDbType.Double;
				case "REAL":
					return MySqlDbType.Double;
				case "TEXT":
					return MySqlDbType.Text;
				case "BLOB":
					return MySqlDbType.Blob;
				case "LONGBLOB":
					return MySqlDbType.LongBlob;
				case "LONGTEXT":
					return MySqlDbType.LongText;
				case "MEDIUMBLOB":
					return MySqlDbType.MediumBlob;
				case "MEDIUMTEXT":
					return MySqlDbType.MediumText;
				case "TINYBLOB":
					return MySqlDbType.TinyBlob;
				case "TINYTEXT":
					return MySqlDbType.TinyText;
				case "BINARY":
					return MySqlDbType.Binary;
				case "VARBINARY":
					return MySqlDbType.VarBinary;
			}
			throw new Exception("Unhandled type encountered");
		}

	}
}
#endif
