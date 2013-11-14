using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// Allows configuration of the serialization of a set of fields.
	/// </summary>
	public class SerializationMappingHandler : IColumnMappingHandler
	{
		/// <summary>
		/// Gets or sets the record type to match. If null, then all record types are matched.
		/// </summary>
		public Type RecordType { get; set; }

		/// <summary>
		/// Gets or sets the name of the field to match. If null, then fields of any name are matched.
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// Gets or sets the type of the column to match. If null, then fields of any type are matched.
		/// </summary>
		public Type FieldType { get; set; }

		/// <summary>
		/// Gets or sets the serialization mode to use for the any matched columns.
		/// </summary>
		public SerializationMode SerializationMode { get; set; }

		/// <inheritdoc/>
		public void HandleColumnMapping(object sender, ColumnMappingEventArgs e)
		{
			if (FieldName != null && String.Compare(FieldName, e.TargetFieldName, StringComparison.OrdinalIgnoreCase) != 0)
				return;

			if (RecordType != null && e.TargetType != RecordType)
				return;

			if (FieldType != null)
			{
				var member = ClassPropInfo.GetMembersForType(e.TargetType)
					.Where(p => String.Compare(p.Name, e.TargetFieldName, StringComparison.OrdinalIgnoreCase) == 0)
					.FirstOrDefault();
				if (member == null)
					return;
			}

			e.SerializationMode = SerializationMode;
		}
	}
}
