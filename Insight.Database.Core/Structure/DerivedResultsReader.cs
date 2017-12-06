using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
    /// <summary>
    /// Can read classes derived from the Results class.
    /// </summary>
    /// <typeparam name="T">The type that can be read.</typeparam>
    public class DerivedResultsReader<T> : IQueryReader<T> where T : Results, new()
    {
        /// <summary>
        /// The default reader for this type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly IQueryReader<T> Default = new DerivedResultsReader<T>();

        /// <inheritdoc/>
        public Type ReturnType { get { return typeof(T); } }

        /// <inheritdoc/>
        public T Read(IDbCommand command, IDataReader reader)
        {
            var t = new T();
            t.Read(command, reader);
            return t;
        }

        /// <inheritdoc/>
        public async Task<T> ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
        {
            var t = new T();
            await t.ReadAsync(command, reader, cancellationToken);
            return t;
        }
    }
}
