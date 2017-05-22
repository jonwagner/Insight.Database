using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Specifies under which circumstances database objects should be bound to child objects.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), Flags]
	public enum BindChildrenFor
	{
		/// <summary>
		/// Do not bind child fields.
		/// </summary>
		None = 0,

		/// <summary>
		/// Bind child fields when mapping input and output parameters.
		/// </summary>
		Parameters = 1,

		/// <summary>
		/// Bind child fields when mapping TVPs and Insert/Update/Upsert results.
		/// </summary>
		Tables = 2,

		/// <summary>
		/// Always bind child fields.
		/// </summary>
		All = Parameters | Tables
	}
}
