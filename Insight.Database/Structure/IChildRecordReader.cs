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
	/// Knows how to read a record containing an object and its parent id.
	/// </summary>
	/// <typeparam name="T">The type of object to read.</typeparam>
	/// <typeparam name="TId">The type of ID for the object.</typeparam>
	public interface IChildRecordReader<T, TId>
	{
		/// <summary>
		/// Reads chidren from the reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns>The children grouped by id.</returns>
		IEnumerable<IGrouping<TId, T>> Read(IDataReader reader);

		/// <summary>
		/// Asynchronously reads chidren from the reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task containing the children grouped by id.</returns>
		Task<IEnumerable<IGrouping<TId, T>>> ReadAsync(IDataReader reader, CancellationToken cancellationToken);
	}
}
