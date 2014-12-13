using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Insight.Database.Providers.PostgreSQL
{
    /// <summary>
    /// Extension methods for Postgres.
    /// </summary>
    public static class NpgsqlExtensions
    {
        /// <summary>
        /// Creates a connection wrapper that automatically sets the schema when the connection is opened.
        /// </summary>
        /// <param name="connectionString">The connection string for the connection.</param>
        /// <param name="schema">The schema to select upon opening.</param>
        /// <returns>A wrapped connection.</returns>
        public static IDbConnection ConnectionWithSchema(this NpgsqlConnectionStringBuilder connectionString, string schema)
        {
            return new NpgsqlConnectionWithSchema(connectionString, schema);
        }
    }
}
