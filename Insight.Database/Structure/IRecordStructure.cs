using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Tells the serialization engine the structure of a record.
	/// </summary>
	interface IRecordStructure : IRecordReader
	{
		/// <summary>
		/// Gets a list of the subtypes in the record.
		/// </summary>
		/// <returns>The list of the subtypes in the record.</returns>
		Type[] GetObjectTypes();

		/// <summary>
		/// Gets a mapping of the ID columns.
		/// </summary>
		/// <returns>A mapping of the ID columns or null to use the default.</returns>
		Dictionary<Type, string> GetIDColumns();
	}
}
