using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for object mapping.
	/// </summary>
	public static class DBReaderExtensions
	{
		#region ToList Methods
		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<FastExpando> ToList(this IDataReader reader)
		{
			return reader.AsEnumerable().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T>(this IDataReader reader)
		{
			return reader.AsEnumerable<T>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="withGraph">The graph to use to deserialize the object.</param>
        /// <returns>A list of objects.</returns>
        public static IList<T> ToList<T>(this IDataReader reader, Type withGraph)
        {
            return reader.AsEnumerable<T>(withGraph).ToList();
        }

        /// <summary>
        /// Converts an IDataReader to a list of objects.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T, TSub1>(this IDataReader reader)
		{
			return reader.AsEnumerable<T, TSub1>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T, TSub1, TSub2>(this IDataReader reader)
		{
			return reader.AsEnumerable<T, TSub1, TSub2>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T, TSub1, TSub2, TSub3>(this IDataReader reader)
		{
			return reader.AsEnumerable<T, TSub1, TSub2, TSub3>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T, TSub1, TSub2, TSub3, TSub4>(this IDataReader reader)
		{
			return reader.AsEnumerable<T, TSub1, TSub2, TSub3, TSub4>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <typeparam name="TSub5">The expected type of sub object 5.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T> ToList<T, TSub1, TSub2, TSub3, TSub4, TSub5>(this IDataReader reader)
		{
			return reader.AsEnumerable<T, TSub1, TSub2, TSub3, TSub4, TSub5>().ToList();
		}
		#endregion

		#region AsEnumerable Methods
		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<FastExpando> AsEnumerable(this IDataReader reader)
		{
            var mapper = DbReaderDeserializer.GetDeserializer<FastExpando>(reader);
			return reader.AsEnumerable(mapper);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T>(this IDataReader reader)
		{
			var mapper = DbReaderDeserializer.GetDeserializer<T>(reader);
			return reader.AsEnumerable(mapper);
		}

        /// <summary>
        /// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="withGraph">An optional type to specify the types of objects in the returned graph.</param>
        /// <param name="callback">An optional callback that allows you to assemble the object graph.</param>
        /// <param name="idColumns">An optional dictionary of the names of the ID fields of the types in the graph.</param>
        /// <returns>An enumerable over the return results.</returns>
        /// <remarks>
        /// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
        /// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
        /// </remarks>
        public static IEnumerable<T> AsEnumerable<T>(
            this IDataReader reader,
            Type withGraph = null,
            Action<object[]> callback = null,
            Dictionary<Type, string> idColumns = null)
        {
            if (callback != null)
            {
                var mapper = DbReaderDeserializer.GetDeserializerWithCallback<T>(reader, withGraph, idColumns);
                return reader.AsEnumerable(r => mapper(r, callback));
            }
            else
            {
                var mapper = DbReaderDeserializer.GetDeserializer<T>(reader, withGraph, idColumns);
                return reader.AsEnumerable(mapper);
            }
        }

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="callback">A callback for custom sub-object mapping.</param>
		/// <param name="idColumns">A list of names of columns that identify the start of a new type.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T, TSub1>(
			this IDataReader reader,
			Action<T, TSub1> callback = null,
			Dictionary<Type, string> idColumns = null)
		{
            Action<object[]> action = null;

            if (callback != null)
            {
                action = (object[] objects) =>
                {
                    callback(
                        (T)objects[0],
                        (TSub1)objects[1]);
                };
            }

            return reader.AsEnumerable<T>(typeof(Graph<T, TSub1>), action, idColumns);
        }

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="callback">A callback for custom sub-object mapping.</param>
		/// <param name="idColumns">A list of names of columns that identify the start of a new type.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T, TSub1, TSub2>(
			this IDataReader reader,
            Action<T, TSub1, TSub2> callback = null,
			Dictionary<Type, string> idColumns = null)
		{
            Action<object[]> action = null;

            if (callback != null)
            {
                action = (object[] objects) =>
                {
                    callback(
                        (T)objects[0],
                        (TSub1)objects[1],
                        (TSub2)objects[2]);
                };
            }

            return reader.AsEnumerable<T>(typeof(Graph<T, TSub1, TSub2>), action, idColumns);
        }

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="callback">A callback for custom sub-object mapping.</param>
		/// <param name="idColumns">A list of names of columns that identify the start of a new type.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T, TSub1, TSub2, TSub3>(
			this IDataReader reader,
            Action<T, TSub1, TSub2, TSub3> callback = null,
			Dictionary<Type, string> idColumns = null)
		{
            Action<object[]> action = null;

            if (callback != null)
            {
                action = (object[] objects) =>
                {
                    callback(
                        (T)objects[0],
                        (TSub1)objects[1],
                        (TSub2)objects[2],
                        (TSub3)objects[3]);
                };
            }

            return reader.AsEnumerable<T>(typeof(Graph<T, TSub1, TSub2, TSub3>), action, idColumns);
        }

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="callback">A callback for custom sub-object mapping.</param>
		/// <param name="idColumns">A list of names of columns that identify the start of a new type.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T, TSub1, TSub2, TSub3, TSub4>(
			this IDataReader reader,
            Action<T, TSub1, TSub2, TSub3, TSub4> callback = null,
			Dictionary<Type, string> idColumns = null)
		{
            Action<object[]> action = null;

            if (callback != null)
            {
                action = (object[] objects) =>
                {
                    callback(
                        (T)objects[0],
                        (TSub1)objects[1],
                        (TSub2)objects[2],
                        (TSub3)objects[3],
                        (TSub4)objects[4]);
                };
            }

            return reader.AsEnumerable<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4>), action, idColumns);
        }

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <typeparam name="TSub5">The expected type of sub object 5.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <param name="callback">A callback for custom sub-object mapping.</param>
		/// <param name="idColumns">A list of names of columns that identify the start of a new type.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T> AsEnumerable<T, TSub1, TSub2, TSub3, TSub4, TSub5>(
			this IDataReader reader,
			Action<T, TSub1, TSub2, TSub3, TSub4, TSub5> callback = null,
			Dictionary<Type, string> idColumns = null)
		{
            Action<object[]> action = null;

            if (callback != null)
            {
                action = (object[] objects) =>
                {
                    callback(
                        (T)objects[0],
                        (TSub1)objects[1],
                        (TSub2)objects[2],
                        (TSub3)objects[3],
                        (TSub4)objects[4],
                        (TSub5)objects[5]);
                 };
            }

            return reader.AsEnumerable<T>(typeof(Graph<T, TSub1, TSub2, TSub3, TSub4, TSub5>), action, idColumns);
        }
		#endregion

		#region First Methods
		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static FastExpando Single(this IDataReader reader)
		{
			FastExpando t = reader.AsEnumerable().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T>().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="withGraph">The type of object graph to use to deserialize the object.</param>
        /// <returns>A list of objects.</returns>
        public static T Single<T>(this IDataReader reader, Type withGraph)
        {
            T t = reader.AsEnumerable<T>(withGraph).FirstOrDefault();
            reader.Advance();
            return t;
        }

        /// <summary>
        /// Converts an IDataReader to a single object.
        /// </summary>
        /// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T, TSub1>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T, TSub1>().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T, TSub1, TSub2>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T, TSub1, TSub2>().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T, TSub1, TSub2, TSub3>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T, TSub1, TSub2, TSub3>().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T, TSub1, TSub2, TSub3, TSub4>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T, TSub1, TSub2, TSub3, TSub4>().FirstOrDefault();
			reader.Advance();
			return t;
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T">The expected type of the object.</typeparam>
		/// <typeparam name="TSub1">The expected type of sub object 1.</typeparam>
		/// <typeparam name="TSub2">The expected type of sub object 2.</typeparam>
		/// <typeparam name="TSub3">The expected type of sub object 3.</typeparam>
		/// <typeparam name="TSub4">The expected type of sub object 4.</typeparam>
		/// <typeparam name="TSub5">The expected type of sub object 5.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T Single<T, TSub1, TSub2, TSub3, TSub4, TSub5>(this IDataReader reader)
		{
			T t = reader.AsEnumerable<T, TSub1, TSub2, TSub3, TSub4, TSub5>().FirstOrDefault();
			reader.Advance();
			return t;
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
            var merger = DbReaderDeserializer.GetMerger<T>(reader);

			// read the identities from the recordset and merge it into the object
			reader.Read();
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
			bool moreResults = false;

			try
			{
				var merger = DbReaderDeserializer.GetMerger<T>(reader);

				// read the identities of each item from the recordset and merge them into the objects
				foreach (T item in items)
				{
					reader.Read();
					merger(reader, item);
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

		#region Private Methods
		/// <summary>
		/// Read a data reader and map the objects as they are read.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="reader">The reader to read.</param>
		/// <param name="mapper">The mapper to use.</param>
		/// <returns>An enumerable for the type.</returns>
		private static IEnumerable<T> AsEnumerable<T>(this IDataReader reader, Func<IDataReader, T> mapper)
		{
			// read in all of the objects from the reader
			while (reader.Read())
				yield return mapper(reader);

			// advance to the next recordset
			reader.Advance();
		}

		/// <summary>
		/// Advance an IDataReader to the next result set or close it if there are no more result sets.
		/// </summary>
		/// <param name="reader">The reader to read.</param>
		private static void Advance(this IDataReader reader)
		{
			// if there are no results left, then clean up the reader
			if (!reader.NextResult())
				reader.Dispose();
		}
		#endregion
	}
}
