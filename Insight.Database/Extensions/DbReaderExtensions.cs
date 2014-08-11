using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database.CodeGenerator;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for object mapping.
	/// </summary>
	public static partial class DBReaderExtensions
	{
		#region ToList Methods
		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<FastExpando> ToList(this IDataReader reader)
		{
			return reader.ToList<FastExpando>();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects, using the specified record reader.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="recordReader">The record reader to use.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T>(this IDataReader reader, IRecordReader<T> recordReader)
		{
			return reader.AsEnumerable(recordReader).ToList();
		}
		#endregion

		#region AsEnumerable Methods
		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<FastExpando> AsEnumerable(this IDataReader reader)
		{
			return reader.AsEnumerable<FastExpando>();
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="recordReader">The record reader to use.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T>(this IDataReader reader, IRecordReader<T> recordReader)
		{
			// if the reader is closed, then we return an empty list, rather than blowing up
			if (reader.IsClosed)
				yield break;

			// get the mapper
			var mapper = recordReader.GetRecordReader(reader);

			// read in all of the objects from the reader
			while (reader.Read())
				yield return mapper(reader);

			// advance to the next recordset
			reader.Advance();
		}
		#endregion

		#region Single Methods
		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static FastExpando Single(this IDataReader reader)
		{
			return reader.Single<FastExpando>();
		}

		/// <summary>
		/// Converts an IDataReader to a single object, using the specified record reader.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="recordReader">The the record reader to use.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T>(this IDataReader reader, IRecordReader<T> recordReader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			if (recordReader == null) throw new ArgumentNullException("recordReader");

			if (reader.IsClosed)
				return default(T);

			// get the mapper
			var mapper = recordReader.GetRecordReader(reader);

			try
			{
				if (reader.Read())
					return mapper(reader);
				else
					return default(T);
			}
			finally
			{
				reader.Advance();
			}
		}
		#endregion

		#region Merge Methods
		/// <summary>
		/// Merges the results of a recordset into an existing object.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The recordset to read.</param>
		/// <param name="item">The item to merge into.</param>
		/// <returns>The item that has been merged.</returns>
		/// <remarks>
		/// This method reads a single record from the reader and overwrites the values of the object.
		/// The reader is then advanced to the next result or disposed.
		/// To merge multiple records from the reader, pass an IEnumerable&lt;T&gt; to the method.
		/// </remarks>
		public static T Merge<T>(this IDataReader reader, T item)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			if (item == null) throw new ArgumentNullException("item");

			var merger = DbReaderDeserializer.GetMerger<T>(reader);

			// read the identities from the recordset and merge it into the object
			if (reader.Read() && merger != null)
				merger(reader, item);

			// we are done with this result set, so move onto the next or clean up the reader
			if (!reader.NextResult())
				reader.Dispose();

			return item;
		}

		/// <summary>
		/// Merges the results of a recordset into a list of existing objects.
		/// </summary>
		/// <typeparam name="T">The type of object to merge into.</typeparam>
		/// <param name="reader">The recordset to read.</param>
		/// <param name="items">The list of items to merge into.</param>
		/// <returns>The list of merged objects.</returns>
		public static IEnumerable<T> Merge<T>(this IDataReader reader, IEnumerable<T> items)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			if (items == null) throw new ArgumentNullException("items");

			bool moreResults = false;

			try
			{
				var merger = DbReaderDeserializer.GetMerger<T>(reader);

				// read the identities of each item from the recordset and merge them into the objects
				if (merger != null)
				{
					foreach (T item in items)
					{
						if (!reader.Read())
							break;

						merger(reader, item);
					}
				}

				// we are done with this result set, so move onto the next or clean up the reader
				moreResults = reader.NextResult();
			}
			finally
			{
				if (!moreResults)
					reader.Dispose();
			}

			return items;
		}
		#endregion

		#region Advance Methods
		/// <summary>
		/// Advance an IDataReader to the next result set or close it if there are no more result sets.
		/// </summary>
		/// <param name="reader">The reader to read.</param>
		public static void Advance(this IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			// if there are no results left, then clean up the reader
			if (!reader.NextResult())
				reader.Dispose();
		}
		#endregion
	}
}
