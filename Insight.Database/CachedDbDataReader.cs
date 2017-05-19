using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Reads a data stream, caching the fields for a single record at a time.
	/// This allows you to convert a sequential read into a random read over a single record.
	/// </summary>
	/// <remarks>Disposing this reader does not dispose the inner reader.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
#if HAS_COLUMN_SCHEMA
	public class CachedDbDataReader : DbDataReaderWrapper, IDbColumnSchemaGenerator
#else
	public class CachedDbDataReader : DbDataReaderWrapper
#endif
	{
		/// <summary>
		/// The inner reader.
		/// </summary>
		private IDataReader _inner;

		/// <summary>
		/// The cache of values for the current record.
		/// </summary>
		private object[] _cache;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the CachedDbDataReader class.
		/// </summary>
		/// <param name="innerReader">The reader to wrap.</param>
		public CachedDbDataReader(IDataReader innerReader)
		{
			_inner = innerReader;
		}
		#endregion

		/// <inheritdoc/>
		public override int Depth
		{
			get { return _inner.Depth; }
		}

		/// <inheritdoc/>
		public override int FieldCount
		{
			get { return _inner.FieldCount; }
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override bool HasRows
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc/>
		public override bool IsClosed
		{
			get { return _inner == null || _inner.IsClosed; }
		}

		/// <inheritdoc/>
		public override int RecordsAffected
		{
			get { return _inner.RecordsAffected; }
		}

		/// <inheritdoc/>
		public override string GetDataTypeName(int i)
		{
			return _inner.GetDataTypeName(i);
		}

		/// <inheritdoc/>
		public override Type GetFieldType(int i)
		{
			return _inner.GetFieldType(i);
		}

		/// <inheritdoc/>
		public override string GetName(int i)
		{
			return _inner.GetName(i);
		}

		/// <inheritdoc/>
		public override int GetOrdinal(string name)
		{
			return _inner.GetOrdinal(name);
		}

		/// <inheritdoc/>
		public override object GetValue(int i)
		{
			GetValues();
			return _cache[i];
		}

		/// <inheritdoc/>
		public override int GetValues(object[] values)
		{
			if (values == null) throw new ArgumentNullException("values");

			GetValues();

			int length = Math.Min(values.Length, FieldCount);
			Array.Copy(_cache, values, length);
			return length;
		}

		/// <inheritdoc/>
		public override void Close()
		{
			if (_inner != null)
				_inner.Close();
		}

		/// <inheritdoc/>
		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public override DataTable GetSchemaTable()
		{
			return _inner.GetSchemaTable();
		}

		/// <inheritdoc/>
		public override bool NextResult()
		{
			return _inner.NextResult();
		}

		/// <inheritdoc/>
		public override bool Read()
		{
			return _inner.Read();
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			// we don't own the stream, so do nothing
			_inner = null;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Read all of the values from the underlying reader.
		/// </summary>
		private void GetValues()
		{
			if (_cache == null)
			{
				_cache = new object[FieldCount];
				_inner.GetValues(_cache);
			}
		}

#if HAS_COLUMN_SCHEMA
		ReadOnlyCollection<DbColumn> IDbColumnSchemaGenerator.GetColumnSchema()
		{
			return ((IDbColumnSchemaGenerator)_inner).GetColumnSchema();
		}
#endif
	}
}
