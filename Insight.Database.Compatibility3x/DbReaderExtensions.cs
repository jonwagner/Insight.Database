using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Extends IDataReader with handy extensions.
	/// </summary>
	public static class DbReaderExtensions
	{
		#region AsEnumerable Methods
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
			Action<T> handler = null;
			if (callback != null)
				handler = (T t1) => callback(new object[] { t1 });

			var oneToOne = Graph.GetOneToOne<T>(withGraph, callback, idColumns) ?? new OneToOne<T>(handler, idColumns);

			return reader.AsEnumerable(oneToOne);
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

		#region WithGraph Methods
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
		#endregion
	}
}
