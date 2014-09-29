using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Common functions of the RecordIdAttribute classes.
	/// </summary>
	interface IRecordIdAttribute
	{
		/// <summary>
		/// Gets the order of the record ids.
		/// </summary>
		int Order { get; }
	}
}
