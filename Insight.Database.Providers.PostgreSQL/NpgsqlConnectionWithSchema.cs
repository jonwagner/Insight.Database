using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;

namespace Insight.Database.Providers.PostgreSQL
{
    /// <summary>
    /// A wrapped connection that automatically sets the schema when the connection is opened.
    /// </summary>
    public class NpgsqlConnectionWithSchema : DbConnectionWrapper
    {
        /// <summary>
        /// A regex defining a valild Postgres identifier. Note that it doesn't support quoted identifiers.
        /// </summary>
        private static string _validPostgresIdentifier = @"[\w\.\$_]+";

        /// <summary>
        /// The sql to use to select the schema upon opening.
        /// </summary>
        private string _switchSchemaSql;

        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionWithSchema class.
        /// </summary>
        /// <param name="connectionString">The connection string to wrap.</param>
        /// <param name="schema">The schema to select upon opening the connection.</param>
        public NpgsqlConnectionWithSchema(NpgsqlConnectionStringBuilder connectionString, string schema)
            : base(connectionString.Connection())
        {
            if (schema == null)
                throw new ArgumentNullException("schema");
            if (!Regex.Match(schema, String.Format(CultureInfo.InvariantCulture, "^{0}(,{0})*$", _validPostgresIdentifier)).Success)
                throw new ArgumentException("Schema contained invalid characters", "schema");

            Schema = schema;

            _switchSchemaSql = String.Format(CultureInfo.InvariantCulture, "SET SCHEMA '{0}'", Schema);
        }

        /// <summary>
        /// Gets the schema for this connection.
        /// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string Schema { get; private set; }

        /// <inheritdoc/>
        public override void Open()
        {
            base.Open();
            InnerConnection.ExecuteSql(_switchSchemaSql);
        }

        /// <inheritdoc/>
        public async override Task OpenAsync(System.Threading.CancellationToken cancellationToken)
        {
            await base.OpenAsync(cancellationToken);
            await InnerConnection.ExecuteSqlAsync(_switchSchemaSql, cancellationToken);
        }
    }
}
