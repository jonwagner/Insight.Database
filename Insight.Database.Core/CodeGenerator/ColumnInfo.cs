using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Represents a column identity.
	/// </summary>
	public class ColumnInfo : IEquatable<ColumnInfo>
	{
		#region Properties
		/// <summary>
		/// Gets the name of the column.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the type of the column.
		/// </summary>
		public Type DataType { get; private set; }

		/// <summary>
		/// Gets the name of the type of the column.
		/// </summary>
		public string DataTypeName { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the column is nullable.
		/// </summary>
		public bool IsNullable { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the column is the row identity.
		/// </summary>
		public bool IsIdentity { get; private set; }

		/// <summary>
		/// Gets  a value indicating whether the column is readonly.
		/// </summary>
		public bool IsReadOnly { get; private set; }

		/// <summary>
		/// Gets the size of the column.
		/// </summary>
		public long? ColumnSize { get; private set; }

		/// <summary>
		/// Gets the precision of the column.
		/// </summary>
		public int? NumericPrecision { get; private set; }

		/// <summary>
		/// Gets the scale of the column.
		/// </summary>
		public int? NumericScale { get; private set; }
		#endregion

		#region Static Methods
#if !NO_COLUMN_SCHEMA
		/// <inheritdoc/>
		public static List<ColumnInfo> FromDataReader(IDataReader reader)
		{
			var schemaGenerator = reader as IDbColumnSchemaGenerator;
			if (schemaGenerator != null)
			{
				var schema = schemaGenerator.GetColumnSchema();

				return schemaGenerator.GetColumnSchema().Select(column =>
					new ColumnInfo()
					{
						Name = column.ColumnName,
						DataType = column.DataType,
						DataTypeName = column.DataTypeName,
						IsNullable = column.AllowDBNull ?? false,
						IsReadOnly = column.IsReadOnly ?? false,
						IsIdentity = column.IsIdentity ?? false,
						NumericPrecision = column.NumericPrecision,
						NumericScale = column.NumericScale,
						ColumnSize = column.ColumnSize
					}).ToList();
			}
			else
			{
				// if the provider doesn't implement IDbColumnSchemaGenerator, then this should be enough to read a schema
				var columns = new List<ColumnInfo>();
				for (int i = 0; i < reader.FieldCount; i++)
				{
					columns.Add(new ColumnInfo()
					{
						Name = reader.GetName(i),
						DataType = reader.GetFieldType(i),
						DataTypeName = reader.GetDataTypeName(i),
						IsNullable = true,
						IsReadOnly = false,
						IsIdentity = false,
						NumericPrecision = null,
						NumericScale = null,
						ColumnSize = null
					});
				}

				return columns;
			}
		}
#else
		/// <summary>
		/// Gets the list of columns from a data reader.
		/// </summary>
		/// <param name="reader">The data reader to parse.</param>
		/// <returns>The columns in the reader.</returns>
		public static List<ColumnInfo> FromDataReader(IDataReader reader)
		{
			int fieldCount = (reader.IsClosed) ? 0 : reader.FieldCount;

			var columns = new List<ColumnInfo>();

			// if there are no fields, then this is a simple identity
			if (fieldCount == 0)
				return columns;

			// we have to compare nullable, readonly and identity because it affects bulk copy
			var schemaTable = reader.GetSchemaTable();
			var isNullableColumn = schemaTable.Columns.IndexOf("AllowDbNull");
			var isReadOnlyColumn = schemaTable.Columns.IndexOf("IsReadOnly");
			var isIdentityColumn = schemaTable.Columns.IndexOf("IsIdentity");
			var dataTypeColumn = schemaTable.Columns.IndexOf("DataType");
			var dataTypeNameColumn = schemaTable.Columns.IndexOf("DataTypeName");
			var precisionColumn = schemaTable.Columns.IndexOf("NumericPrecision");
			var scaleColumn = schemaTable.Columns.IndexOf("NumericScale");
			var columnSizeColumn = schemaTable.Columns.IndexOf("ColumnSize");

			for (int i = 0; i < fieldCount; i++)
			{
				var row = schemaTable.Rows[i];

				var column = new ColumnInfo()
				{
					Name = reader.GetName(i),
					IsNullable = (isNullableColumn == -1) ? false : row.IsNull(isNullableColumn) ? false : Convert.ToBoolean(row[isNullableColumn], CultureInfo.InvariantCulture),
					IsReadOnly = (isReadOnlyColumn == -1) ? false : row.IsNull(isReadOnlyColumn) ? false : Convert.ToBoolean(row[isReadOnlyColumn], CultureInfo.InvariantCulture),
					IsIdentity = (isIdentityColumn == -1) ? false : row.IsNull(isIdentityColumn) ? false : Convert.ToBoolean(row[isIdentityColumn], CultureInfo.InvariantCulture),
					DataType = (Type)row[dataTypeColumn],
					NumericPrecision = (precisionColumn == -1) ? (int?)null : row.IsNull(precisionColumn) ? (int?)null : Convert.ToInt32(row[precisionColumn]),
					NumericScale = (scaleColumn == -1) ? (int?)null : row.IsNull(scaleColumn) ? (int?)null : Convert.ToInt32(row[scaleColumn]),
					ColumnSize = (columnSizeColumn == -1) ? (int?)null : row.IsNull(columnSizeColumn) ? (int?)null : Convert.ToInt32(row[columnSizeColumn])
				};

				if (dataTypeNameColumn != -1)
				{
					string dataType = row[dataTypeNameColumn].ToString();
					if (String.Equals(dataType, "money", StringComparison.OrdinalIgnoreCase))
						column.NumericScale = 4;
					else if (String.Equals(dataType, "smallmoney", StringComparison.OrdinalIgnoreCase))
						column.NumericScale = 4;
					else if (String.Equals(dataType, "date", StringComparison.OrdinalIgnoreCase))
						column.NumericScale = 0;
				}

				columns.Add(column);
			}

			return columns;
		}
#endif

#if !NO_SCHEMA_TABLE
        /// <summary>
        /// Converts a list of columns to a SchemaTable.
        /// </summary>
        /// <param name="columns">The columns to convert.</param>
        /// <returns>A schema table.</returns>
        public static DataTable ToSchemaTable(List<ColumnInfo> columns)
		{
			var table = new DataTable("SchemaTable");
			try
			{
				table.Locale = CultureInfo.InvariantCulture;

				table.Columns.Add("AllowDBNull", typeof(bool));
				table.Columns.Add("BaseCatalogName", typeof(string));
				table.Columns.Add("BaseColumnName", typeof(string));
				table.Columns.Add("BaseSchemaName", typeof(string));
				table.Columns.Add("BaseTableName", typeof(string));
				table.Columns.Add("ColumnName", typeof(string));
				table.Columns.Add("ColumnOrdinal", typeof(int));
				table.Columns.Add("ColumnSize", typeof(int));
				table.Columns.Add("DataType", typeof(Type));
				table.Columns.Add("DataTypeName", typeof(string));
				table.Columns.Add("IsUnique", typeof(bool));
				table.Columns.Add("IsKey", typeof(bool));
				table.Columns.Add("IsAliased", typeof(bool));
				table.Columns.Add("IsExpression", typeof(bool));
				table.Columns.Add("IsIdentity", typeof(bool));
				table.Columns.Add("IsAutoIncrement", typeof(bool));
				table.Columns.Add("IsRowVersion", typeof(bool));
				table.Columns.Add("IsHidden", typeof(bool));
				table.Columns.Add("IsLong", typeof(bool));
				table.Columns.Add("IsReadOnly", typeof(bool));
				table.Columns.Add("NumericPrecision", typeof(int));
				table.Columns.Add("NumericScale", typeof(int));
				table.Columns.Add("ProviderSpecificDataType", typeof(Type));
				table.Columns.Add("ProviderType", typeof(Type));

				for (int i = 0; i < columns.Count; i++)
				{
					var column = columns[i];
					var row = table.NewRow();

					row["AllowDBNull"] = column.IsNullable;
					row["BaseColumnName"] = column.Name;
					row["ColumnName"] = column.Name;
					row["ColumnSize"] = column.ColumnSize;
					row["ColumnOrdinal"] = i;
					row["DataType"] = column.DataType;
					row["DataTypeName"] = column.DataTypeName;
					row["IsIdentity"] = column.IsIdentity;
					row["IsReadOnly"] = column.IsReadOnly;
					row["NumericPrecision"] = column.NumericPrecision;
					row["NumericScale"] = column.NumericScale;

					table.Rows.Add(row);
				}

				return table;
			}
			catch
			{
				table.Dispose();
				throw;
			}
		}
#endif
#endregion

        /// <summary>
        /// Determines whether two columns are equal.
        /// </summary>
        /// <param name="other">The other column.</param>
        /// <returns>True if they are equal.</returns>
        public bool Equals(ColumnInfo other)
		{
			if (other == null)
				return false;

			if (Name != other.Name)
				return false;
			if (DataType != other.DataType)
				return false;
			if (IsNullable != other.IsNullable)
				return false;
			if (IsIdentity != other.IsIdentity)
				return false;
			if (IsReadOnly != other.IsReadOnly)
				return false;

			return true;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int hashCode = 17;

			unchecked
			{
				hashCode += Name.GetHashCode();
				hashCode *= 23;
				hashCode += DataType.GetHashCode();
				hashCode *= 23;
				hashCode += IsNullable.GetHashCode();
				hashCode *= 23;
				hashCode += IsIdentity.GetHashCode();
				hashCode *= 23;
				hashCode += IsReadOnly.GetHashCode();
			}

			return hashCode;
		}
	}
}
