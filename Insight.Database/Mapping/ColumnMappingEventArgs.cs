using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// An event that is generated when the ColumnMapper needs to map a column to a type for the first time.
	/// </summary>
	public class ColumnMappingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the target type of the mapping operation.
		/// </summary>
		public Type TargetType { get; internal set; }

		/// <summary>
		/// Gets the source reader of the data set for a table mapping. At the time of the mapping, the reader will be open and the metadata will be available.
		/// </summary>
		public IDataReader Reader { get; internal set; }

		/// <summary>
		/// Gets the Command Text that is currently being mapped.
		/// </summary>
		public string CommandText { get; internal set; }

		/// <summary>
		/// Gets the Command Type that is currently being mapped.
		/// </summary>
		public CommandType? CommandType { get; internal set; }

		/// <summary>
		/// Gets the source parameter list for a parameter mapping.
		/// </summary>
		public IList<IDataParameter> Parameters { get; internal set; }

		/// <summary>
		/// Gets the index of the field that is being mapped.
		/// </summary>
		public int FieldIndex { get; internal set; }

		/// <summary>
		/// Gets the name of the column that is being mapped.
		/// </summary>
		public string ColumnName { get; internal set; }

		/// <summary>
		/// Gets or sets the name of the target field. This will be pre-set to the field that the mapper believes is the correct field.
		/// The default logic will use the name of the property or the ColumnNameAttribute on the property.
		/// Set this value to the desired target column.
		/// </summary>
		public string TargetFieldName { get; set; }

		/// <summary>
		/// Gets or sets the serialization mode for this column mapping.
		/// </summary>
		public SerializationMode? SerializationMode { get; set; }

		/// <summary>
		/// Gets or sets the custom serializer to use for this column mapping.
		/// </summary>
		public Type Serializer { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the mapping operation should be canceled for this column.
		/// If set to true, then the mapper will not map the current column.
		/// </summary>
		public bool Canceled { get; set; }

		/// <summary>
		/// Gets or sets the field the column is bound to.
		/// </summary>
		internal ClassPropInfo ClassPropInfo { get; set; }

		/// <summary>
		/// Gets or sets the prefix of members to get to the current object.
		/// </summary>
		internal List<ClassPropInfo> Prefix { get; set; }

		/// <summary>
		/// Emits the IL to get the prefixed object.
		/// </summary>
		/// <param name="il">The ILGenerator.</param>
		internal void EmitGetPrefix(ILGenerator il)
		{
			if (Prefix == null || !Prefix.Any())
				return;

			var doneLabel = il.DefineLabel();
			var nullLabel = il.DefineLabel();

			foreach (var p in Prefix)
			{
				p.EmitGetValue(il);

				if (!p.MemberType.IsValueType)
				{
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Brfalse, nullLabel);
				}
			}

			il.Emit(OpCodes.Br, doneLabel);

			// if any of the prefix values are null, then we emit the default of the final type
			il.MarkLabel(nullLabel);
			il.Emit(OpCodes.Pop);
			TypeHelper.EmitDefaultValue(il, Prefix.Last().MemberType);

			il.MarkLabel(doneLabel);
		}

		/// <summary>
		/// Emits the IL to get the value of the binding. This always returns a boxed version of the value.
		/// </summary>
		/// <param name="il">The ILGenerator.</param>
		/// <param name="boxNulls">True to box null values on output.</param>
		/// <param name="readyToSetLabel">If specified, the label to jump to if any object on the prefix path is null.</param>
		internal void EmitGetValue(ILGenerator il, bool boxNulls = false, Label? readyToSetLabel = null)
		{
			EmitGetPrefix(il);

			var readyLabel = il.DefineLabel();
			var doneLabel = il.DefineLabel();

			if (Prefix != null && Prefix.Any() && !Prefix.Last().MemberType.IsValueType)
			{
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Brtrue, readyLabel);

				il.Emit(OpCodes.Pop);
				TypeHelper.EmitDefaultValue(il, ClassPropInfo.MemberType, boxNulls);

				il.Emit(OpCodes.Br, readyToSetLabel ?? doneLabel);
			}

			il.MarkLabel(readyLabel);
			ClassPropInfo.EmitGetValue(il);
			il.MarkLabel(doneLabel);
		}
	}
}
