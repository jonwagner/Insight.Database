using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Insight.Database.Providers.Oracle
{
	/// <summary>
	/// Implements an array of Oracle objects.
	/// </summary>
	/// <typeparam name="T">The type of object contained in the array.</typeparam>
	public class OracleArray<T> : IOracleCustomType, INullable where T : new()
	{
		/// <summary>
		/// Gets or sets the value of the array.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public T[] Value { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is null.
		/// </summary>
		public bool IsNull
		{
			get { return Value == null; }
		}

		/// <summary>
		/// Creates an internal Oracle structure from the array.
		/// </summary>
		/// <param name="con">The connection.</param>
		/// <param name="pUdt">The internal UDT.</param>
		public void FromCustomObject(OracleConnection con, IntPtr pUdt)
		{
			OracleUdt.SetValue(con, pUdt, 0, Value);
		}

		/// <summary>
		/// Creates an array from an internal Oracle structure.
		/// </summary>
		/// <param name="con">The connection.</param>
		/// <param name="pUdt">The internal UDT.</param>
		public void ToCustomObject(OracleConnection con, IntPtr pUdt)
		{
			Value = (T[])OracleUdt.GetValue(con, pUdt, 0);
		}
	}
}
