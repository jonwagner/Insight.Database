using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Reliable;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for DbConnection to make it easier to call the database.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	public static partial class DBConnectionExtensions
	{
		#region Bulk Copy Members
		/// <summary>
		/// Bulk copy a list of objects to the server. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of the objects.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="list">The list of objects.</param>
		/// <param name="batchSize">An optional batch size.</param>
		/// <param name="closeConnection">True to close the connection when complete.</param>
		/// <param name="options">The options to use for the bulk copy.</param>
		/// <param name="transaction">An optional external transaction.</param>
		public static void BulkCopy<T>(
			this IDbConnection connection,
			string tableName,
			IEnumerable<T> list,
			int? batchSize = null,
			bool closeConnection = false,
			SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
			SqlTransaction transaction = null)
		{
			connection.BulkCopy<T>(
				tableName,
				list,
				bulk =>	
				{
					if (batchSize.HasValue)
						bulk.BatchSize = batchSize.Value;
				},
				closeConnection,
				options,
				transaction);
		}

		/// <summary>
		/// Bulk copy a list of objects to the server. This method supports auto-open.
		/// </summary>
		/// <typeparam name="T">The type of the objects.</typeparam>
		/// <param name="connection">The connection to use.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="list">The list of objects.</param>
		/// <param name="configure">An action that can be used to configure the bulk copy operation.</param>
		/// <param name="closeConnection">True to close the connection when complete.</param>
		/// <param name="options">The options to use for the bulk copy.</param>
		/// <param name="transaction">An optional external transaction.</param>
		public static void BulkCopy<T>(
			this IDbConnection connection,
			string tableName,
			IEnumerable<T> list,
			Action<SqlBulkCopy> configure,
			bool closeConnection = false,
			SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
			SqlTransaction transaction = null)
		{
			// bulk copy only works for sql server
			SqlConnection sqlConnection = connection as SqlConnection;
			if (sqlConnection == null)
				throw new ArgumentException("connection must be a SqlConnection", "connection");

			try
			{
				DetectAutoOpen(connection, ref closeConnection);

				// create a bulk copier
				using (SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection, options, transaction))
				{
					bulk.DestinationTableName = tableName;
					if (configure != null)
						configure(bulk);

					// see if we already have a mapping for the given table name and type
					// we can't use the schema mapping cache because we don't have the schema yet, just the name of the table
					var key = Tuple.Create<string, Type>(tableName, typeof(T));
					ObjectReader fieldReaderData = _tableReaders.GetOrAdd(
						key,
						t =>
						{
							// select a 0 row result set so we can determine the schema of the table
							string sql = String.Format(CultureInfo.InvariantCulture, "SELECT TOP 0 * FROM {0}", tableName);
							using (var sqlReader = connection.GetReaderSql(sql, commandBehavior: CommandBehavior.SchemaOnly))
								return ObjectReader.GetObjectReader(sqlReader, typeof(T));
						});

					// create a reader for the list
					using (ObjectListDbDataReader reader = new ObjectListDbDataReader(fieldReaderData, list))
					{
						// write the data to the server
						bulk.WriteToServer(reader);
					}
				}
			}
			finally
			{
				if (closeConnection)
					connection.Close();
			}
		}
		#endregion
	}
}
