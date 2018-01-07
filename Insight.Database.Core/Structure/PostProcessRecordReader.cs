using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Extends a record reader, allowing records to be postprocessed after reading them.
	/// </summary>
	/// <typeparam name="T">The type of record that is being read.</typeparam>
	public class PostProcessRecordReader<T> : RecordReader<T>
	{
		/// <summary>
		/// The base reader to read the record.
		/// </summary>
		private IRecordReader<T> _baseReader;

		/// <summary>
		/// The code to execute after reading the record.
		/// </summary>
		private Func<IDataReader, T, T> _postRead;

		/// <summary>
		/// Initializes a new instance of the PostProcessRecordReader class.
		/// </summary>
		/// <param name="postRead">The code to execute after reading the record.</param>
		public PostProcessRecordReader(Func<IDataReader, T, T> postRead)
			: this(OneToOne<T>.Records, postRead)
		{
		}

		/// <summary>
		/// Initializes a new instance of the PostProcessRecordReader class.
		/// </summary>
		/// <param name="baseReader">The base record reader to use to read the record.</param>
		/// <param name="postRead">The code to execute after reading the record.</param>
		public PostProcessRecordReader(IRecordReader<T> baseReader, Func<IDataReader, T, T> postRead)
		{
			_baseReader = baseReader;
			_postRead = postRead;
		}

		/// <inheritdoc/>
		public override IRecordReader<TGuardian> GetGuardianReader<TGuardian>()
		{
			return new PostProcessRecordReader<TGuardian>(OneToOne<TGuardian, T>.Records,
				(IDataReader reader, TGuardian guardian) =>
				{
					_postRead(reader, guardian.Object);
					return guardian;
				});
		}

		/// <inheritdoc/>
		public override Func<IDataReader, T> GetRecordReader(IDataReader reader)
		{
			var baseReader = _baseReader.GetRecordReader(reader);

			return r =>
			{
				using (var wrapped = new CachedDbDataReader(r))
				{
					var t = baseReader(wrapped);
					return _postRead(wrapped, t);
				}
			};
		}

		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			return true;
		}
	}
}
