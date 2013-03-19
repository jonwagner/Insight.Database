using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Helper class for generating code.
	/// </summary>
	static class IlHelper
	{
		/// <summary>
		/// Emit an opcode to load an Int32.
		/// </summary>
		/// <param name="il">The generator to use.</param>
		/// <param name="value">The value to emit.</param>
		public static void EmitLdInt32(ILGenerator il, int value)
		{
			switch (value)
			{
				case -1:
					il.Emit(OpCodes.Ldc_I4_M1);
					break;
				case 0:
					il.Emit(OpCodes.Ldc_I4_0);
					break;
				case 1:
					il.Emit(OpCodes.Ldc_I4_1);
					break;
				case 2:
					il.Emit(OpCodes.Ldc_I4_2);
					break;
				case 3:
					il.Emit(OpCodes.Ldc_I4_3);
					break;
				case 4:
					il.Emit(OpCodes.Ldc_I4_4);
					break;
				case 5:
					il.Emit(OpCodes.Ldc_I4_5);
					break;
				case 6:
					il.Emit(OpCodes.Ldc_I4_6);
					break;
				case 7:
					il.Emit(OpCodes.Ldc_I4_7);
					break;
				case 8:
					il.Emit(OpCodes.Ldc_I4_8);
					break;
				default:
					if (value >= -128 && value <= 127)
						il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
					else
						il.Emit(OpCodes.Ldc_I4, value);
					break;
			}
		}

		/// <summary>
		/// Assuming the top of the stack is an object of sourceType,
		/// emits the code required to convert the object ToString.
		/// Null values are converted to null.
		/// </summary>
		/// <param name="il">The generator to use.</param>
		/// <param name="sourceType">The type of the object on the stack.</param>
		public static void EmitToStringOrNull(ILGenerator il, Type sourceType)
		{
			var isNull = il.DefineLabel();

			if (sourceType.IsValueType)
			{
				// convert values to a pointer we can call on
				var local = il.DeclareLocal(sourceType);
				il.Emit(OpCodes.Stloc, local);
				il.Emit(OpCodes.Ldloca, local);
				il.Emit(OpCodes.Constrained, sourceType);
			}
			else
			{
				// null check for references
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Brfalse, isNull);
			}

			il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString", Type.EmptyTypes));
			il.MarkLabel(isNull);
		}
	}
}
