#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

// This file has been adapted from its original at https://raw.githubusercontent.com/npgsql/npgsql/dev/src/Npgsql/NpgsqlCommandBuilder.cs

#if NETSTANDARD2_0

using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;

namespace Insight.Database.Providers.PostgreSQL.Compatibility
{
	///<summary>
	/// This class is responsible to create database commands for automatic insert, update and delete operations.
	///</summary>
	[System.ComponentModel.DesignerCategory("")]
	public static class NpgsqlCommandBuilder
	{
		///<summary>
		///
		/// This method is reponsible to derive the command parameter list with values obtained from function definition.
		/// It clears the Parameters collection of command. Also, if there is any parameter type which is not supported by Npgsql, an InvalidOperationException will be thrown.
		/// Parameters name will be parameter1, parameter2, ...
		///</summary>
		/// <param name="command">NpgsqlCommand whose function parameters will be obtained.</param>
		public static void DeriveParameters(NpgsqlCommand command)
		{
			try
			{
				DoDeriveParameters(command);
			}
			catch
			{
				command.Parameters.Clear();
				throw;
			}
		}

		private const string DeriveParametersQuery = @"
SELECT
CASE
	WHEN pg_proc.proargnames IS NULL THEN array_cat(array_fill(''::name,ARRAY[pg_proc.pronargs]),array_agg(pg_attribute.attname ORDER BY pg_attribute.attnum))
	ELSE pg_proc.proargnames
END AS proargnames,
pg_proc.proargtypes,
CASE
	WHEN pg_proc.proallargtypes IS NULL AND (array_agg(pg_attribute.atttypid))[1] IS NOT NULL THEN array_cat(string_to_array(pg_proc.proargtypes::text,' ')::oid[],array_agg(pg_attribute.atttypid ORDER BY pg_attribute.attnum))
	ELSE pg_proc.proallargtypes
END AS proallargtypes,
CASE
	WHEN pg_proc.proargmodes IS NULL AND (array_agg(pg_attribute.atttypid))[1] IS NOT NULL THEN array_cat(array_fill('i'::""char"",ARRAY[pg_proc.pronargs]),array_fill('o'::""char"",ARRAY[array_length(array_agg(pg_attribute.atttypid), 1)]))
    ELSE pg_proc.proargmodes
END AS proargmodes
FROM pg_proc
LEFT JOIN pg_type ON pg_proc.prorettype = pg_type.oid
LEFT JOIN pg_attribute ON pg_type.typrelid = pg_attribute.attrelid AND pg_attribute.attnum >= 1
WHERE pg_proc.oid = :proname::regproc
GROUP BY pg_proc.proargnames, pg_proc.proargtypes, pg_proc.proallargtypes, pg_proc.proargmodes, pg_proc.pronargs;
";

		private static void DoDeriveParameters(NpgsqlCommand command)
		{
			// See http://www.postgresql.org/docs/current/static/catalog-pg-proc.html
			command.Parameters.Clear();
			using (var c = new NpgsqlCommand(DeriveParametersQuery, command.Connection))
			{
				c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Text));
				c.Parameters[0].Value = command.CommandText;

				string[] names = null;
				uint[] types = null;
				char[] modes = null;

				using (var rdr = c.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
				{
					if (rdr.Read())
					{
						if (!rdr.IsDBNull(0))
							names = rdr.GetValue(0) as string[];
						if (!rdr.IsDBNull(2))
							types = rdr.GetValue(2) as uint[];
						if (!rdr.IsDBNull(3))
							modes = rdr.GetValue(3) as char[];
						if (types == null)
						{
							if (rdr.IsDBNull(1) || rdr.GetFieldValue<uint[]>(1).Length == 0)
								return;  // Parameterless function
							types = rdr.GetFieldValue<uint[]>(1);
						}
					}
					else
						throw new InvalidOperationException($"{command.CommandText} does not exist in pg_proc");
				}

				command.Parameters.Clear();
				for (var i = 0; i < types.Length; i++)
				{
					var param = new NpgsqlParameter();

					// TODO: Fix enums, composite types
//					var npgsqlDbType = c.Connection.Connector.TypeHandlerRegistry[types[i]].PostgresType.NpgsqlDbType;
//					if (!npgsqlDbType.HasValue)
//					throw new InvalidOperationException($"Invalid parameter type: {types[i]}");
//					param.NpgsqlDbType = npgsqlDbType.Value;

					if (names != null && i < names.Length)
						param.ParameterName = ":" + names[i];
					else
						param.ParameterName = "parameter" + (i + 1);

					if (modes == null) // All params are IN, or server < 8.1.0 (and only IN is supported)
						param.Direction = ParameterDirection.Input;
					else
					{
						switch (modes[i])
						{
							case 'i':
								param.Direction = ParameterDirection.Input;
								break;
							case 'o':
							case 't':
								param.Direction = ParameterDirection.Output;
								break;
							case 'b':
								param.Direction = ParameterDirection.InputOutput;
								break;
							case 'v':
								throw new NotImplementedException("Cannot derive function parameter of type VARIADIC");
							default:
								throw new ArgumentOutOfRangeException("proargmode", modes[i],
									"Unknown code in proargmodes while deriving: " + modes[i]);
						}
					}

					command.Parameters.Add(param);
				}
			}
		}
	}
}
#endif
