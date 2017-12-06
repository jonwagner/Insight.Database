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
    /// Knows how to read a list of children into a parent object.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent object.</typeparam>
    /// <typeparam name="TChild">The type of the child object.</typeparam>
    class SingleChildren<TParent, TChild> : Children<TParent>
    {
        /// <summary>
        /// Can read the child records from the stream.
        /// </summary>
        private IRecordReader<TChild> _recordReader;

        /// <summary>
        /// Can map the children into the parent in the right place.
        /// </summary>
        private SingleChildMapper<TParent, TChild> _mapper;

        /// <summary>
        /// Initializes a new instance of the SingleChildren class.
        /// </summary>
        /// <param name="recordReader">The recordReader that can read the children from the data stream.</param>
        /// <param name="mapper">The mapper that puts the children into the parent.</param>
        public SingleChildren(IRecordReader<TChild> recordReader, SingleChildMapper<TParent, TChild> mapper)
        {
            _recordReader = recordReader;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public override void Read(IEnumerable<TParent> parents, IDataReader reader)
        {
            _mapper.MapChildren(parents, reader.ToList(_recordReader));
        }

        /// <inheritdoc/>
        public override async Task ReadAsync(IEnumerable<TParent> parents, IDataReader reader, CancellationToken ct)
        {
            var result = await reader.ToListAsync(_recordReader);
            _mapper.MapChildren(parents, result);
        }
    }
}
