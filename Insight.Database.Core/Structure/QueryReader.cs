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
	/// Reads objects from a Data Reader.
	/// </summary>
	/// <typeparam name="T">The type of object read from the reader.</typeparam>
	public abstract class QueryReader<T>
	{
		#region Fields
		/// <summary>
		/// The set of child records to read from the stream.
		/// </summary>
		private List<Children<T>> _children;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the QueryReader class.
		/// </summary>
		/// <param name="recordReader">A mapping that can be used to read a single record from the stream.</param>
		protected QueryReader(IRecordReader<T> recordReader)
		{
			RecordReader = recordReader;
			_children = new List<Children<T>>();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the mapping used to read an individual record from this stream.
		/// </summary>
		protected IRecordReader<T> RecordReader { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Adds a child reader to this reader.
		/// </summary>
		/// <param name="child">The child reader to add.</param>
		/// <returns>A QueryReader that also reads the specified children.</returns>
		internal QueryReader<T> AddChild(Children<T> child)
		{
			var clone = (QueryReader<T>)MemberwiseClone();
			clone._children = _children.ToList();
			clone._children.Add(child);

			return clone;
		}

		/// <summary>
		/// Reads all of the children from the stream.
		/// </summary>
		/// <param name="reader">The data reader to read from.</param>
		/// <param name="results">The results of the current read operation.</param>
		protected void ReadChildren(IDataReader reader, IEnumerable<T> results)
		{
			// read in the children
			foreach (var child in _children)
				child.Read(results, reader);
		}

		/// <summary>
		/// Reads all of the children from the stream asynchronously.
		/// </summary>
		/// <param name="reader">The data reader to read from.</param>
		/// <param name="results">The results of the current read operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task representing the completion of the read.</returns>
		protected async Task ReadChildrenAsync(IDataReader reader, IEnumerable<T> results, CancellationToken cancellationToken)
		{
			// read in the children
			foreach (var child in _children)
				await child.ReadAsync(results, reader, cancellationToken);
		}
		#endregion
	}
}
