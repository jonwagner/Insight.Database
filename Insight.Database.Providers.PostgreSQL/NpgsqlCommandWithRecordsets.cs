using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Insight.Database.Providers.PostgreSQL
{
	/// <summary>
	/// Implements a postgres command that automatically deferences refcursors into recordsets.
	/// </summary>
	public class NpgsqlCommandWithRecordsets : DbCommandWrapper
	{
#pragma warning disable CA2213
		/// <summary>
		/// The inner Npgsql connection;
		/// </summary>
		private NpgsqlConnection _innerConnection;
#pragma warning restore CA2213

		/// <summary>
		/// Initializes a new instance of the NpgsqlCommandWithRecordsets class.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="innerCommand">The inner command to wrap.</param>
		public NpgsqlCommandWithRecordsets(DbConnectionWrapper connection, DbCommand innerCommand) : base(connection, innerCommand)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			_innerConnection = (NpgsqlConnection)connection.InnerConnection;
		}

		/// <inheritdoc/>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var reader = base.ExecuteDbDataReader(behavior);

			if (!ShouldDereference(reader))
				return reader;

			using (var command = CreateDereferenceDbCommand(reader))
			{
				return command.ExecuteReader(behavior);
			}
		}

		/// <inheritdoc/>
		protected async override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			var reader = await base.ExecuteDbDataReaderAsync(behavior, cancellationToken).ConfigureAwait(false);

			if (!ShouldDereference(reader))
				return reader;

			using (var command = CreateDereferenceDbCommand(reader))
			{
				return await command.ExecuteReaderAsync(behavior).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Determines whether the reader contains refcursors that need to be deferenced.
		/// </summary>
		/// <param name="reader">The reader to check.</param>
		/// <returns>True if the reader contains refcursors.</returns>
		/// <exception cref="InvalidOperationException">Throws InvalidOperationException if the reader contains cursors and other data.</exception>
		private static bool ShouldDereference(DbDataReader reader)
		{
			// Transparently dereference returned cursors, where possible
			bool cursors = false;
			for (int i = 0; i < reader.FieldCount; i++)
			{
				if (reader.GetDataTypeName(i) == "refcursor")
					cursors = true;
			}

			return cursors;
		}

		/// <summary>
		/// Creates a new DbCommand to use to dereference the reader.
		/// </summary>
		/// <param name="reader">The original reader.</param>
		/// <returns>A new command to use to retrieve the cursor data.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Data is refcursor names returned from the server.")]
		private NpgsqlCommand CreateDereferenceDbCommand(DbDataReader reader)
		{
			// Supports 1x1 1xN Nx1 (and NXM!) patterns of cursor data
			// The resultant FETCH command(s) *will* properly stream the cursored data
			var sb = new StringBuilder();
			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					if (reader.GetDataTypeName(i) == "refcursor")
						sb.AppendFormat(@"FETCH ALL FROM ""{0}"";", reader.GetString(i));
				}
			}

			reader.Dispose();

			return new NpgsqlCommand(sb.ToString(), _innerConnection);
		}
	}
}
