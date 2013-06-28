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
	/// Implements an Oracle Custom Type.
	/// </summary>
	public abstract class OracleCustomType : INullable, IOracleCustomType
	{
		/// <summary>
		/// Gets a value indicating whether this value is null.
		/// </summary>
		public bool IsNull
		{
			get { return false; }
		}

		/// <summary>
		/// Creates an internal Oracle structure from the object.
		/// </summary>
		/// <param name="con">The connection.</param>
		/// <param name="pUdt">The internal UDT.</param>
		public abstract void FromCustomObject(OracleConnection con, IntPtr pUdt);

		/// <summary>
		/// Creates an object from an internal Oracle structure.
		/// </summary>
		/// <param name="con">The connection.</param>
		/// <param name="pUdt">The internal UDT.</param>
		public virtual void ToCustomObject(OracleConnection con, IntPtr pUdt)
		{
			throw new NotImplementedException();
		}
	}
}
