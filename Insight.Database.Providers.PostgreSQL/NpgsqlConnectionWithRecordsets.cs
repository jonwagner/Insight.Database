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
	/// Opens a postgres connection that automatically deferences refcursors into recordsets.
	/// </summary>
	public class NpgsqlConnectionWithRecordsets : DbConnectionWrapper
	{
		/// <summary>
		/// Initializes a new instance of the NpgsqlConnectionWithRecordsets class.
		/// </summary>
		/// <param name="connectionString">The connectionstring to use to connect to the database.</param>
		public NpgsqlConnectionWithRecordsets(NpgsqlConnectionStringBuilder connectionString) : base(connectionString.Connection())
        {
		}

		/// <inheritdoc/>
		protected override DbCommand CreateDbCommand()
		{
			return new NpgsqlCommandWithRecordsets(this, InnerConnection.CreateCommand());
		}
	}
}
