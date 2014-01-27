using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight.Database
{
	/// <summary>
	/// Defines an override to the standard mapping of database fields to object fields.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnAttribute : Attribute
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ColumnAttribute class.
		/// </summary>
		public ColumnAttribute()
		{
		}

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
		/// Gets or sets the name of the column to map this field to.
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// Gets or sets the serialization mode for the column.
		/// </summary>
		public SerializationMode SerializationMode { get; set; }

		/// <summary>
		/// Gets or sets the type of serializer to use for the column.
		/// </summary>
		public Type Serializer { get; set; }
		#endregion
	}
}
