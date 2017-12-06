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
    /// An instance of Children knows how to read children into a parent.
    /// This is a one-to-many relationship.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent to read into.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    abstract class Children<TParent>
    {
        /// <summary>
        /// Read a list of children into the given list of parents.
        /// </summary>
        /// <param name="parents">The list of parents.</param>
        /// <param name="reader">The reader to read from.</param>
        public abstract void Read(IEnumerable<TParent> parents, IDataReader reader);

        /// <summary>
        /// Read a list of children into the given list of parents.
        /// </summary>
        /// <param name="parents">The list of parents.</param>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>A task representing the completion of the read.</returns>
        public abstract Task ReadAsync(IEnumerable<TParent> parents, IDataReader reader, CancellationToken cancellationToken);
    }

    /// <summary>
    /// An instance of Children knows how to read children into a parent.
    /// This is a one-to-many relationship.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent to read into.</typeparam>
    /// <typeparam name="TChild">The type of the child object.</typeparam>
    /// <typeparam name="TId">The type of the ID value.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    class Children<TParent, TChild, TId> : Children<TParent>
    {
        /// <summary>
        /// Can read the child records from the stream.
        /// </summary>
        private IChildRecordReader<TChild, TId> _recordReader;

        /// <summary>
        /// Can map the children into the parent in the right place.
        /// </summary>
        private IChildMapper<TParent, TChild, TId> _mapper;

        /// <summary>
        /// Initializes a new instance of the Children class.
        /// </summary>
        /// <param name="recordReader">The recordReader that can read the children from the data stream.</param>
        /// <param name="mapper">The mapper that puts the children into the parent.</param>
        public Children(IChildRecordReader<TChild, TId> recordReader, IChildMapper<TParent, TChild, TId> mapper)
        {
            _recordReader = recordReader;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public override void Read(IEnumerable<TParent> parents, IDataReader reader)
        {
            _mapper.MapChildren(parents, _recordReader.Read(reader));
        }

        /// <inheritdoc/>
        public override async Task ReadAsync(IEnumerable<TParent> parents, IDataReader reader, CancellationToken ct)
        {
            var result = await _recordReader.ReadAsync(reader, ct);
            _mapper.MapChildren(parents, result);
        }
    }
}
