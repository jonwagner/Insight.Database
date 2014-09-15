using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// An object that can read a record.
	/// It needs to be equatable so serializer caching works properly.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
	public interface IRecordReader : IEquatable<IRecordReader>
	{
	}

	/// <summary>
	/// An object that can read a record of a given type.
	/// </summary>
	/// <typeparam name="T">The type of record that can be read.</typeparam>
#if NET35
	public interface IRecordReader<T> : IRecordReader
#else
	public interface IRecordReader<out T> : IRecordReader
#endif
	{
		/// <summary>
		/// Gets a function that can read a record from the given data reader.
		/// </summary>
		/// <param name="reader">The reader to be read from.</param>
		/// <returns>A function that can read a record.</returns>
		/// <remarks>This returns a function because each reader may have a different schema.</remarks>
		Func<IDataReader, T> GetRecordReader(IDataReader reader);
	}

	/// <summary>
	/// A base implementation of IRecordReader.
	/// </summary>
	/// <typeparam name="T">The type of object that can be read.</typeparam>
	public abstract class RecordReader<T> : IRecordReader<T>
	{
		/// <inheritdoc/>
		public abstract Func<IDataReader, T> GetRecordReader(IDataReader reader);

		/// <inheritdoc/>
		public abstract bool Equals(IRecordReader other);
	}

	/// <summary>
	/// Implements a custom record reader that can read type T.
	/// </summary>
	/// <typeparam name="T">The type to read.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related classes.")]
	public class CustomRecordReader<T> : RecordReader<T>, IChildRecordReader<T>
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
		/// <param name="read">The function to read the record.</param>
		/// <returns>A CustomRecordReader.</returns>
		public static CustomRecordReader<T> Read(Func<IDataReader, T> read)
		{
			return new CustomRecordReader<T>(read);
		}

		/// <inheritdoc/>
		public override Func<IDataReader, T> GetRecordReader(IDataReader reader)
		{
			return r => _read(r);
		}

		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			var c = other as CustomRecordReader<T>;

			if (c == null)
				return false;

			return c._read == this._read;
		}

		/// <inheritdoc/>
		public IRecordReader<Guardian<T, TId>> GetGuardianReader<TId>()
		{
			return (IRecordReader<Guardian<T, TId>>)new CustomChildRecordReader<T>(_read);
		}
	}

	/// <summary>
	/// Implements a custom record reader that can read a child record of type T.
	/// </summary>
	/// <typeparam name="T">The type to read.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related classes.")]
	class CustomChildRecordReader<T> : RecordReader<Guardian<T, object>>, IChildRecordReader<T>
	{
		/// <summary>
		/// The reader function.
		/// </summary>
		private Func<IDataReader, T> _read;

		/// <summary>
		/// Initializes a new instance of the CustomChildRecordReader class.
		/// </summary>
		/// <param name="read">The function used to read the object.</param>
		public CustomChildRecordReader(Func<IDataReader, T> read)
		{
			_read = read;
		}

		/// <inheritdoc/>
		public override Func<IDataReader, Guardian<T, object>> GetRecordReader(IDataReader reader)
		{
			return r =>
			{
				return new Guardian<T, object>()
				{
					ParentId = r[0],
					Object = _read(r)
				};
			};
		}

		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			var c = other as CustomChildRecordReader<T>;

			if (c == null)
				return false;

			return c._read == this._read;
		}

		/// <inheritdoc/>
		public IRecordReader<Guardian<T, TId>> GetGuardianReader<TId>()
		{
			return (IRecordReader<Guardian<T, TId>>)this;
		}
	}
}
