using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Types;

namespace Insight.Database.Providers.Oracle
{
	/// <summary>
	/// Implements a factory for an Oracle Custom Type.
	/// </summary>
	/// <typeparam name="T">The type of object.</typeparam>
	public class OracleObjectFactory<T> : IOracleCustomTypeFactory where T : IOracleCustomType, new()
	{
		/// <summary>
		/// Creates a new instance of the object.
		/// </summary>
		/// <returns>A new instance of the object.</returns>
		public virtual IOracleCustomType CreateObject()
		{
			return new T();
		}
	}
}
