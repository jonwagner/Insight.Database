using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Insight.Database
{
	/// <summary>
	/// Extension methods for object mapping.
	/// </summary>
	public static partial class DBReaderExtensions
	{
		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to an enumerable. The reader is closed after all records are read.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>An enumerable over the return results.</returns>
		/// <remarks>
		/// If you use this method and are relying on CommandBehavior.CloseConnection to close the connection, note that if all of the records are not read
		/// (due to an exception or otherwise), then the connection will leak until GC is run. Your code is responsible for closing the connection.
		/// </remarks>
		public static IEnumerable<T1> AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IDataReader reader)
		{
			return reader.AsEnumerable(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Records);
		}

		/// <summary>
		/// Converts an IDataReader to a list of objects.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static IList<T1> ToList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IDataReader reader)
		{
			return reader.AsEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>().ToList();
		}

		/// <summary>
		/// Converts an IDataReader to a single object.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
		/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
		/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
		/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
		/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
		/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
		/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
		/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
		/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
		/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
		/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
		/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
		/// <param name="reader">The data reader.</param>
		/// <returns>A list of objects.</returns>
		public static T1 Single<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IDataReader reader)
		{
			return reader.Single(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Records);
		}

	}
}