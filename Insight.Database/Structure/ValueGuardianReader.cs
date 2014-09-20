using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Handles the special case when reading child records that are atomic values.
	/// </summary>
	/// <typeparam name="TValue">The value to read.</typeparam>
	/// <typeparam name="TId">The type of parent ID.</typeparam>
	class ValueGuardianReader<TValue, TId> : IRecordReader<Guardian<TValue, TId>>
	{
		/// <summary>
		/// The default reader for atomic child records.
		/// </summary>
		public static readonly ValueGuardianReader<TValue, TId> Records = new ValueGuardianReader<TValue, TId>();

		/// <inheritdoc/>
		public Func<IDataReader, Guardian<TValue, TId>> GetRecordReader(IDataReader reader)
		{
			return r =>
			{
				return new Guardian<TValue, TId>()
				{
					ParentId = (TId)reader[0],
					Object = (TValue)reader[1]
				};
			};
		}

		/// <inheritdoc/>
		public bool Equals(IRecordReader other)
		{
			return other is ValueGuardianReader<TValue, TId>;
		}
	}
}
