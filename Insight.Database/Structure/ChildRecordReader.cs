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
		/// The function to extract the ID.
		/// </summary>
		Func<TRecord, TId> _getid;

		/// <summary>
		/// The function to extract the object.
		/// </summary>
		Func<TRecord, TResult> _getobject;

		/// <summary>
		/// Initializes a new instance of the ChildRecordReader class.
		/// </summary>
		/// <param name="recordReader">The reader to get the whole record.</param>
		/// <param name="getid">The function to extract the id.</param>
		/// <param name="getobject">The function to extract the object.</param>
		public ChildRecordReader(IRecordReader<TRecord> recordReader, Func<TRecord, TId> getid, Func<TRecord, TResult> getobject)
		{
			_recordReader = recordReader;
			_getid = getid;
			_getobject = getobject;
		}

		/// <inheritdoc/>
		IEnumerable<IGrouping<TId, TResult>> IChildRecordReader<TResult, TId>.Read(IDataReader reader)
		{
			return reader.ToList(_recordReader).GroupBy(_getid, _getobject);
		}

		/// <inheritdoc/>
		Task<IEnumerable<IGrouping<TId, TResult>>> IChildRecordReader<TResult, TId>.ReadAsync(IDataReader reader, CancellationToken cancellationToken)
		{
			return reader.ToListAsync(_recordReader, cancellationToken)
				.ContinueWith(t => t.Result.GroupBy(_getid, _getobject), TaskContinuationOptions.ExecuteSynchronously);
		}
	}
}
