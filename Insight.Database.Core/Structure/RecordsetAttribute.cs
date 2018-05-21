using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Defines the structure that an interface method returns from a recordset.
	/// Each instance defines a single or one-to-one relationship.
	/// Adding multiple instances to a method defines a Results output or a parent-child relationship.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recordset")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class RecordsetAttribute : Attribute
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the RecordsetAttribute class.
		/// </summary>
		public RecordsetAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the RecordsetAttribute class.
		/// </summary>
		/// <param name="types">The types of the classes in the record. This defines a OneToOne relationship.</param>
		public RecordsetAttribute(params Type[] types)
		{
			Types = types;
		}

		/// <summary>
		/// Initializes a new instance of the RecordsetAttribute class.
		/// </summary>
		/// <param name="index">The index of the recordset in the query results.</param>
		/// <param name="types">The types of the classes in the record. This defines a OneToOne relationship.</param>
		public RecordsetAttribute(int index, params Type[] types)
		{
			Types = types;
			Index = index;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the types of the classes in the record. This defines a OneToOne relationship.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public Type[] Types { get; private set; }

		/// <summary>
		/// Gets the index of the recordset in the query results.
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the recordset is a child relationship.
		/// </summary>
		public bool IsChild { get; set; }

		/// <summary>
		/// Gets or sets the name of the parent ID field in the parent-child relationship.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the List field to insert the child list into for the parent-child relationship.
		/// </summary>
		public string Into { get; set; }

		/// <summary>
		/// Gets or sets the name of the field that contains the parents in a multi-level parent-child relationship.
		/// </summary>
		public string Parents { get; set; }

		/// <summary>
		/// Gets or sets the name of the child ID field in the parent-child relationship.
		/// </summary>
		public string GroupBy { get; set; }
		#endregion
	}
}
