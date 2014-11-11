using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
	/// <summary>
	/// A base implementation of IRecordReader.
	/// </summary>
	public abstract partial class RecordReader<T> : IRecordReader<T>
	{
		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2>> GroupByColumns<T1, T2>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2>, Tuple<T1, T2>, T>(
				GetGuardianReader<Guardian<T, T1, T2>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2), g => g.Object));
		}

		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <typeparam name="T3">The type of the third ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2, T3>> GroupByColumns<T1, T2, T3>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2, T3>, Tuple<T1, T2, T3>, T>(
				GetGuardianReader<Guardian<T, T1, T2, T3>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2, g.ParentId3), g => g.Object));
		}

		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <typeparam name="T3">The type of the third ID.</typeparam>
		/// <typeparam name="T4">The type of the fourth ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2, T3, T4>> GroupByColumns<T1, T2, T3, T4>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2, T3, T4>, Tuple<T1, T2, T3, T4>, T>(
				GetGuardianReader<Guardian<T, T1, T2, T3, T4>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2, g.ParentId3, g.ParentId4), g => g.Object));
		}

		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <typeparam name="T3">The type of the third ID.</typeparam>
		/// <typeparam name="T4">The type of the fourth ID.</typeparam>
		/// <typeparam name="T5">The type of the fifth ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2, T3, T4, T5>> GroupByColumns<T1, T2, T3, T4, T5>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2, T3, T4, T5>, Tuple<T1, T2, T3, T4, T5>, T>(
				GetGuardianReader<Guardian<T, T1, T2, T3, T4, T5>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2, g.ParentId3, g.ParentId4, g.ParentId5), g => g.Object));
		}

		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <typeparam name="T3">The type of the third ID.</typeparam>
		/// <typeparam name="T4">The type of the fourth ID.</typeparam>
		/// <typeparam name="T5">The type of the fifth ID.</typeparam>
		/// <typeparam name="T6">The type of the sixth ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2, T3, T4, T5, T6>> GroupByColumns<T1, T2, T3, T4, T5, T6>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2, T3, T4, T5, T6>, Tuple<T1, T2, T3, T4, T5, T6>, T>(
				GetGuardianReader<Guardian<T, T1, T2, T3, T4, T5, T6>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2, g.ParentId3, g.ParentId4, g.ParentId5, g.ParentId6), g => g.Object));
		}

		/// <summary>
		/// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
		/// </summary>
		/// <typeparam name="T1">The type of the first ID.</typeparam>
		/// <typeparam name="T2">The type of the second ID.</typeparam>
		/// <typeparam name="T3">The type of the third ID.</typeparam>
		/// <typeparam name="T4">The type of the fourth ID.</typeparam>
		/// <typeparam name="T5">The type of the fifth ID.</typeparam>
		/// <typeparam name="T6">The type of the sixth ID.</typeparam>
		/// <typeparam name="T7">The type of the seventh ID.</typeparam>
		/// <returns>A child record reader.</returns>
		public IChildRecordReader<T, Tuple<T1, T2, T3, T4, T5, T6, T7>> GroupByColumns<T1, T2, T3, T4, T5, T6, T7>()
		{
			return new ChildRecordReader<Guardian<T, T1, T2, T3, T4, T5, T6, T7>, Tuple<T1, T2, T3, T4, T5, T6, T7>, T>(
				GetGuardianReader<Guardian<T, T1, T2, T3, T4, T5, T6, T7>>(),
				records => records.GroupBy(g => Tuple.Create(g.ParentId1, g.ParentId2, g.ParentId3, g.ParentId4, g.ParentId5, g.ParentId6, g.ParentId7), g => g.Object));
		}

	}
}
