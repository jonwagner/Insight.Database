using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Represents a column mapping that overrides the default rules.
	/// ColumnOverrides are processed after any default rules are processed.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class ColumnOverride
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ColumnOverride class.
		/// </summary>
		/// <param name="columnName">The name of the column to map.</param>
		/// <param name="fieldName">The name of the field to map to.</param>
		public ColumnOverride(string columnName, string fieldName) : this(null, columnName, fieldName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ColumnOverride class.
		/// </summary>
		/// <param name="targetType">The type of object to perform mapping on or null to match all objects.</param>
		/// <param name="columnName">The name of the column to map.</param>
		/// <param name="fieldName">The name of the field to map to.</param>
		public ColumnOverride(Type targetType, string columnName, string fieldName)
		{
			if (columnName == null) throw new ArgumentNullException("columnName");
			if (fieldName == null) throw new ArgumentNullException("fieldName");

			TargetType = targetType;
			ColumnName = columnName;
			FieldName = fieldName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the type of the object being read or null to match all object types.
		/// </summary>
		public Type TargetType { get; private set; }

		/// <summary>
		/// Gets the name of the column to map.
		/// </summary>
		public string ColumnName { get; private set; }

		/// <summary>
		/// Gets the name of the target field or property.
		/// </summary>
		public string FieldName { get; private set; }
		#endregion
	}

	/// <summary>
	/// Represents a column mapping that overrides the default rules.
	/// ColumnOverrides are processed after any default rules are processed.
	/// </summary>
	/// <typeparam name="T">The type of object to apply the mapping to.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class ColumnOverride<T> : ColumnOverride
	{
		/// <summary>
		/// Initializes a new instance of the ColumnOverride class.
		/// </summary>
		/// <param name="columnName">The name of the column to map.</param>
		/// <param name="fieldName">The name of the field to map to.</param>
		public ColumnOverride(string columnName, string fieldName)
			: base(typeof(T), columnName, fieldName)
		{
		}
	}
}
