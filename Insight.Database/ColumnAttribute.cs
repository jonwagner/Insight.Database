using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Defines an override to the standard mapping of database fields to object fields.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnAttribute : Attribute
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ColumnAttribute class.
		/// </summary>
		/// <param name="columnName">The name of the column to map this field to.</param>
		public ColumnAttribute(string columnName)
		{
			ColumnName = columnName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the column to map this field to.
		/// </summary>
		public string ColumnName { get; private set; }
		#endregion
	}
}
