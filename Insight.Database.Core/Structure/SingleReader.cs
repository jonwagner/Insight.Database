using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
    /// <summary>
    /// Reads a single record from a data stream.
    /// </summary>
    /// <typeparam name="T">The type of object to read.</typeparam>
    public class SingleReader<T> : QueryReader<T>, IQueryReader<T>
    {
        #region Fields
        /// <summary>
        /// The default reader to read a list of type T.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly SingleReader<T> Default = new SingleReader<T>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SingleReader class.
        /// </summary>
        public SingleReader() : this(OneToOne<T>.Records)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingleReader class.
        /// </summary>
        /// <param name="mapping">The mapping to use to read objects from each record.</param>
        public SingleReader(IRecordReader<T> mapping) : base(mapping)
        {
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public virtual Type ReturnType { get { return typeof(T); } }

        /// <inheritdoc/>
        public T Read(IDbCommand command, IDataReader reader)
        {
            var results = reader.Single<T>(RecordReader);

            // read in the children
            ReadChildren(reader, Enumerable.Range(1, 1).Select(i => results));

            return results;
        }

        /// <inheritdoc/>
        public async Task<T> ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
        {
            IList<T> results = await reader.ToListAsync(RecordReader, cancellationToken, firstRecordOnly: true);
            await ReadChildrenAsync(reader, results, cancellationToken);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a child reader to this reader.
        /// </summary>
        /// <param name="child">The child reader to add.</param>
        /// <returns>A list reader that also reads the specified child.</returns>
        internal new SingleReader<T> AddChild(Children<T> child)
        {
            return (SingleReader<T>)base.AddChild(child);
        }
        #endregion
    }
}
