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
    /// Reads child records and groups them by ID.
    /// </summary>
    /// <typeparam name="TRecord">The type of object to read as the record.</typeparam>
    /// <typeparam name="TId">The type of the key of the record.</typeparam>
    /// <typeparam name="TResult">The type of the data of the record.</typeparam>
    class ChildRecordReader<TRecord, TId, TResult> : IChildRecordReader<TResult, TId>
    {
        /// <summary>
        /// The reader to get the whole record.
        /// </summary>
        IRecordReader<TRecord> _recordReader;

        /// <summary>
        /// The function to group the results.
        /// </summary>
        Func<IEnumerable<TRecord>, IEnumerable<IGrouping<TId, TResult>>> _grouping;

        /// <summary>
		/// Initializes a new instance of the ChildRecordReader class.
		/// </summary>
		/// <param name="recordReader">The reader to get the whole record.</param>
        /// <param name="grouping">The function to group the results.</param>
		public ChildRecordReader(IRecordReader<TRecord> recordReader, Func<IEnumerable<TRecord>, IEnumerable<IGrouping<TId, TResult>>> grouping)
        {
            _recordReader = recordReader;
            _grouping = grouping;
        }

        /// <inheritdoc/>
        IEnumerable<IGrouping<TId, TResult>> IChildRecordReader<TResult, TId>.Read(IDataReader reader)
        {
			IEnumerable<TRecord> records = reader.ToList(_recordReader);

			if (_recordReader.RequiresDeduplication)
				records = records.Distinct();

            return _grouping(records);
        }

        /// <inheritdoc/>
        async Task<IEnumerable<IGrouping<TId, TResult>>> IChildRecordReader<TResult, TId>.ReadAsync(IDataReader reader, CancellationToken cancellationToken)
        {
			IEnumerable<TRecord> records = await reader.ToListAsync(_recordReader, cancellationToken);

			if (_recordReader.RequiresDeduplication)
				records = records.Distinct();

            return _grouping(records);
        }
    }
}
