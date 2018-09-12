using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Implements a custom record reader that can read type T.
	/// </summary>
	/// <typeparam name="T">The type to read.</typeparam>
	public class CustomRecordReader<T> : RecordReader<T>
	{
		/// <summary>
		/// The reader function.
		/// </summary>
		private Func<IDataReader, T> _read;

		/// <summary>
		/// Initializes a new instance of the CustomRecordReader class.
		/// </summary>
		/// <param name="read">The function used to read the object.</param>
		public CustomRecordReader(Func<IDataReader, T> read)
		{
			_read = read;
		}

		/// <summary>
		/// Constructs a CustomRecordReader from a function.
		/// </summary>
		/// <param name="reader">The function to read the record.</param>
		/// <returns>A CustomRecordReader.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static CustomRecordReader<T> Read(Func<IDataReader, T> reader)
		{
			return new CustomRecordReader<T>(reader);
		}

		/// <inheritdoc/>
		public override Func<IDataReader, T> GetRecordReader(IDataReader reader)
		{
			return r => _read(r);
		}

		/// <inheritdoc/>
		public override IRecordReader<TGuardian> GetGuardianReader<TGuardian>()
		{
			return new CustomChildReader<TGuardian>(_read);
		}

		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			var c = other as CustomRecordReader<T>;

			if (c == null)
				return false;

			return c._read == this._read;
		}

		/// <summary>
		/// Reads the child record from the database.
		/// </summary>
		/// <typeparam name="TGuardian">The type of guardian record to use.</typeparam>
		class CustomChildReader<TGuardian> : IRecordReader<TGuardian> where TGuardian : Guardian<T>, new()
		{
			/// <summary>
			/// The function to read a record.
			/// </summary>
			private Func<IDataReader, T> _read;

			/// <summary>
			/// Initializes a new instance of the CustomChildReader class.
			/// </summary>
			/// <param name="read">The function to read the record.</param>
			public CustomChildReader(Func<IDataReader, T> read)
			{
				_read = read;
			}

			/// <inheritdoc/>
			public virtual bool RequiresDeduplication { get { return false; } }

			/// <inheritdoc/>
			Func<IDataReader, TGuardian> IRecordReader<TGuardian>.GetRecordReader(IDataReader reader)
			{
				return r =>
				{
					var guardian = new TGuardian();
					guardian.ReadCurrent(r);
					guardian.Object = _read(r);

					return guardian;
				};
			}

			/// <inheritdoc/>
			bool IEquatable<IRecordReader>.Equals(IRecordReader other)
			{
				var o = other as CustomChildReader<TGuardian>;
				if (o == null)
					return false;

				return o._read == _read;
			}
		}
	}
}
