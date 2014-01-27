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
	/// Describes an object that can read the results of a query.
	/// </summary>
	public interface IQueryReader
	{
		/// <summary>
		/// Gets the type of object returned from the query reader.
		/// </summary>
		/// <returns>The type of object returned from the query reader.</returns>
		/// <remarks>This is used to support dynamic calls.</remarks>
		Type ReturnType { get; }
	}

	/// <summary>
	/// Describes an object that can read the results of a query.
	/// </summary>
	/// <typeparam name="T">The type of object read from the data raeder.</typeparam>
	public interface IQueryReader<T> : IQueryReader
	{
		/// <summary>
		/// Reads objects from the reader.
		/// </summary>
		/// <param name="command">The command that was executed.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>The output of the reader.</returns>
		T Read(IDbCommand command, IDataReader reader);

		/// <summary>
		/// Reads objects from the reader asynchronously.
		/// </summary>
		/// <param name="command">The command that was executed.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="cancellationToken">An optional CancellationToken that can cancel the operation.</param>
		/// <returns>The output of the reader.</returns>
		Task<T> ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken);
	}
}
