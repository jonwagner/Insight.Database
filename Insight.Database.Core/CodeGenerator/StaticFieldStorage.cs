using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Allows storage of a static variable in a way that can be easily emitted into IL.
	/// </summary>
	/// <remarks>
	/// We store the information in the static fields of a class because it is easy to access them
	/// in the IL of a DynamicMethod.
	/// </remarks>
	public static class StaticFieldStorage
	{
		/// <summary>
		/// Stores the static objects to return later.
		/// </summary>
		static List<object> _values = new List<object>();

		/// <summary>
		/// Returns a value from the static field cache.
		/// </summary>
		/// <param name="index">The index into the cache.</param>
		/// <returns>The value from the cache.</returns>
		public static object GetValue(int index)
		{
			return _values[index];
		}

		/// <summary>
		/// Adds a value to the cache.
		/// </summary>
		/// <param name="value">The value to add to the cache.</param>
		/// <returns>The index into the cache.</returns>
		internal static int CacheValue(object value)
		{
			lock (_values)
			{
				_values.Add(value);
				return _values.Count - 1;
			}
		}

		/// <summary>
		/// Emits the value stored in static storage.
		/// </summary>
		/// <param name="il">The ILGenerator to emit to.</param>
		/// <param name="value">The value to emit.</param>
		internal static void EmitLoad(ILGenerator il, object value)
		{
			if (value == null)
			{
				il.Emit(OpCodes.Ldnull);
			}
			else
			{
				il.Emit(OpCodes.Ldc_I4, CacheValue(value));
				il.Emit(OpCodes.Call, typeof(StaticFieldStorage).GetMethod("GetValue"));
				il.Emit(OpCodes.Castclass, value.GetType());
			}
		}
	}
}
