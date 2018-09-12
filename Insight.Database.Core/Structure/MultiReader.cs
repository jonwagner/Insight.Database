using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// A RecordReader that can read more than one type of class from a record stream.
	/// </summary>
	/// <typeparam name="TBase">The base type of all of the records returned.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
	public class MultiReader<TBase> : IRecordReader<TBase>
	{
		/// <summary>
		/// The selector for the record readers.
		/// </summary>
		private Func<IDataReader, IRecordReader<TBase>> _selector;

		/// <summary>
		/// Initializes a new instance of the MultiReader class.
		/// </summary>
		/// <param name="selector">The function used to select the record reader for an individual record.</param>
		public MultiReader(Func<IDataReader, IRecordReader<TBase>> selector)
		{
			_selector = selector;
		}

		/// <inheritdoc/>
		public virtual bool RequiresDeduplication { get { return false; } }

		/// <inheritdoc/>
		public Func<IDataReader, TBase> GetRecordReader(IDataReader reader)
		{
			return r =>
			{
				// wrap the reader so we can go out of order when reading the columns
				using (var wrapped = new CachedDbDataReader(reader))
				{
					return _selector(wrapped).GetRecordReader(wrapped)(wrapped);
				}
			};
		}

		/// <inheritdoc/>
		public bool Equals(IRecordReader other)
		{
			return other is MultiReader<TBase>;
		}
	}
}
