using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Types;

namespace Insight.Database.Providers.Oracle
{
	/// <summary>
	/// Implements a factory for an Oracle Array.
	/// </summary>
	/// <typeparam name="T">The type of object contained in the array.</typeparam>
	public class OracleArrayFactory<T> : IOracleCustomTypeFactory, IOracleArrayTypeFactory where T : IOracleCustomType, new()
	{
		/// <summary>
		/// Creates an instance of the OracleArray.
		/// </summary>
		/// <returns>A new instance.</returns>
		public IOracleCustomType CreateObject()
		{
			return new OracleArray<T>();
		}

		/// <summary>
		/// Creates an instance of the OracleArray with the desired number of elements.
		/// </summary>
		/// <param name="numElems">The number of elements.</param>
		/// <returns>A new instance.</returns>
		public Array CreateArray(int numElems)
		{
			return new OracleArray<T>[numElems];
		}

		/// <summary>
		/// Creates a status array for the array, indicating whether elements are null.
		/// </summary>
		/// <param name="numElems">The number of elements.</param>
		/// <returns>A new status arrays.</returns>
		public Array CreateStatusArray(int numElems)
		{
			return new OracleUdtStatus[numElems];
		}
	}
}
