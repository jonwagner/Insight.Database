using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	public static partial class Query
	{
		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2> Then<T1, T2>(
			this IQueryReader<Results<T1>> previous,
			IRecordReader<T2> recordReader)
		{
			return new ResultsReader<T1, T2>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3> Then<T1, T2, T3>(
			this IQueryReader<Results<T1, T2>> previous,
			IRecordReader<T3> recordReader)
		{
			return new ResultsReader<T1, T2, T3>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4> Then<T1, T2, T3, T4>(
			this IQueryReader<Results<T1, T2, T3>> previous,
			IRecordReader<T4> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5> Then<T1, T2, T3, T4, T5>(
			this IQueryReader<Results<T1, T2, T3, T4>> previous,
			IRecordReader<T5> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6> Then<T1, T2, T3, T4, T5, T6>(
			this IQueryReader<Results<T1, T2, T3, T4, T5>> previous,
			IRecordReader<T6> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7> Then<T1, T2, T3, T4, T5, T6, T7>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6>> previous,
			IRecordReader<T7> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8> Then<T1, T2, T3, T4, T5, T6, T7, T8>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7>> previous,
			IRecordReader<T8> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8>> previous,
			IRecordReader<T9> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>> previous,
			IRecordReader<T10> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> previous,
			IRecordReader<T11> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <typeparam name="T12">The type of objects in the twelfth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> previous,
			IRecordReader<T12> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <typeparam name="T12">The type of objects in the twelfth set of results.</typeparam>
		/// <typeparam name="T13">The type of objects in the thirteenth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> previous,
			IRecordReader<T13> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <typeparam name="T12">The type of objects in the twelfth set of results.</typeparam>
		/// <typeparam name="T13">The type of objects in the thirteenth set of results.</typeparam>
		/// <typeparam name="T14">The type of objects in the fourteenth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> previous,
			IRecordReader<T14> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <typeparam name="T12">The type of objects in the twelfth set of results.</typeparam>
		/// <typeparam name="T13">The type of objects in the thirteenth set of results.</typeparam>
		/// <typeparam name="T14">The type of objects in the fourteenth set of results.</typeparam>
		/// <typeparam name="T15">The type of objects in the fifteenth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> previous,
			IRecordReader<T15> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(previous, recordReader);
		}

		/// <summary>
		/// Extends the reader by reading another set of records.
		/// </summary>
		/// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
		/// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
		/// <typeparam name="T3">The type of objects in the third set of results.</typeparam>
		/// <typeparam name="T4">The type of objects in the fourth set of results.</typeparam>
		/// <typeparam name="T5">The type of objects in the fifth set of results.</typeparam>
		/// <typeparam name="T6">The type of objects in the sixth set of results.</typeparam>
		/// <typeparam name="T7">The type of objects in the seventh set of results.</typeparam>
		/// <typeparam name="T8">The type of objects in the eighth set of results.</typeparam>
		/// <typeparam name="T9">The type of objects in the nineth set of results.</typeparam>
		/// <typeparam name="T10">The type of objects in the tenth set of results.</typeparam>
		/// <typeparam name="T11">The type of objects in the eleventh set of results.</typeparam>
		/// <typeparam name="T12">The type of objects in the twelfth set of results.</typeparam>
		/// <typeparam name="T13">The type of objects in the thirteenth set of results.</typeparam>
		/// <typeparam name="T14">The type of objects in the fourteenth set of results.</typeparam>
		/// <typeparam name="T15">The type of objects in the fifteenth set of results.</typeparam>
		/// <typeparam name="T16">The type of objects in the sixteenth set of results.</typeparam>
		/// <param name="previous">The previous reader.</param>
		/// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
		/// <returns>A reader that reads a Results object with multiple results.</returns>
		public static ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			this IQueryReader<Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> previous,
			IRecordReader<T16> recordReader)
		{
			return new ResultsReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(previous, recordReader);
		}

	}
}
