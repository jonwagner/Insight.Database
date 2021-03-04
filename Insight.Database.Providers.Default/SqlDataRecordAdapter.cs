using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Insight.Database.CodeGenerator;
using Microsoft.SqlServer.Server;

namespace Insight.Database.Providers.Default
{
	/// <summary>
	/// Adapts an ObjectReader to return an enumeration of SqlDataRecord. This is the best way to send a TVP to SQLServer.
	/// </summary>
	class SqlDataRecordAdapter : IEnumerable<SqlDataRecord>
	{
		/// <summary>
		/// Mapping from object types to DbTypes.
		/// </summary>
		private static Dictionary<Type, SqlDbType> _typeToDbTypeMap = new Dictionary<Type, SqlDbType>()
		{
			{ typeof(byte), SqlDbType.TinyInt },
			{ typeof(sbyte), SqlDbType.TinyInt },
			{ typeof(short), SqlDbType.SmallInt },
			{ typeof(ushort), SqlDbType.SmallInt },
			{ typeof(int), SqlDbType.Int },
			{ typeof(uint), SqlDbType.Int },
			{ typeof(long), SqlDbType.BigInt },
			{ typeof(ulong), SqlDbType.BigInt },
			{ typeof(float), SqlDbType.Real },
			{ typeof(double), SqlDbType.Float },
			{ typeof(decimal), SqlDbType.Decimal },
			{ typeof(bool), SqlDbType.Bit },
			{ typeof(string), SqlDbType.NVarChar },
			{ typeof(char), SqlDbType.NChar },
			{ typeof(Guid), SqlDbType.UniqueIdentifier },
			{ typeof(DateTime), SqlDbType.DateTime },
			{ typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
			{ typeof(TimeSpan), SqlDbType.Time },
			{ typeof(byte[]), SqlDbType.VarBinary },
		};

		private ObjectReader _objectReader;
		private IEnumerable _list;
		private SqlMetaData[] _metadata;

		/// <summary>
		/// Constucts an instance of the SqlDataRecordAdapter class.
		/// </summary>
		/// <param name="objectReader">The ObjectReader to use to extract properties from the object.</param>
		/// <param name="list">The list of objects to read.</param>
		public SqlDataRecordAdapter(ObjectReader objectReader, IEnumerable list)
		{
			_objectReader = objectReader;
			_list = list;

			_metadata = objectReader.Columns.Select(c => new SqlMetaData(
				c.Name,
				GetSqlDbType(c.DataType, c.DataTypeName),
				GetColumnSize(c),
				Convert.ToByte((c.NumericPrecision == 0xff) ? 0 : c.NumericPrecision ?? 0),
				Convert.ToByte((c.NumericScale == 0xff) ? 0 : c.NumericScale ?? 0),
				0, // locale
				SqlCompareOptions.None,
				null, // user defined type
				c.IsIdentity || c.IsReadOnly, // use server default
				false, // is unique key
				System.Data.SqlClient.SortOrder.Unspecified,
				-1)).ToArray();
		}

		/// <inheritdoc/>
		IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
		{
			foreach (object o in _list)
			{
				var record = new SqlDataRecord(_metadata);

				if (o != null)
				{
					for (int i = 0; i < _objectReader.FieldCount; i++)
					{
						var accessor = _objectReader.GetAccessor(i);
						if (accessor != null)
						{
							var value = accessor(o);
							if (value != null)
								record.SetValue(i, value);
						}
					}
				}

				yield return record;
			}
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<SqlDataRecord>)this).GetEnumerator();
		}

		/// <inheritdoc/>
		private SqlDbType GetSqlDbType(Type type, string dataTypeName)
		{
			if (!String.IsNullOrEmpty(dataTypeName))
			{
				switch (dataTypeName.ToLowerInvariant())
				{
					case "date":
						return SqlDbType.Date;
					case "datetime":
						return SqlDbType.DateTime;
					case "datetime2":
						return SqlDbType.DateTime2;
				}
			}

			type = Nullable.GetUnderlyingType(type) ?? type;
			return _typeToDbTypeMap[type];
		}

		private long GetColumnSize(ColumnInfo c)
		{
			if (c.ColumnSize == 0x7fffffff)
				return SqlMetaData.Max;

			long maxLen = -1;
			switch (GetSqlDbType(c.DataType, c.DataTypeName))
			{
				case SqlDbType.NChar:
				case SqlDbType.NVarChar:
					maxLen = 4000;
					break;

				case SqlDbType.Char:
				case SqlDbType.VarChar:
					maxLen = 8000;
					break;
			}

			long length = c.ColumnSize ?? 0;
			if (length > maxLen)
				return SqlMetaData.Max;

			return maxLen;
		}
	}
}
