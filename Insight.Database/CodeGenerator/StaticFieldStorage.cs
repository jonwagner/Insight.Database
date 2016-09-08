using System;
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
	class StaticFieldStorage
	{
		/// <summary>
		/// The shared module that stores all of the static variables.
		/// </summary>
		private static ModuleBuilder _dynamicModule;

		/// <summary>
		/// Temporary variable to cache whether a debugger is attached. Remove in v6.
		/// </summary>
		private static bool? _isDebuggerAttached;

		/// <summary>
		/// The cache of the static fields.
		/// </summary>
		private static Dictionary<Tuple<ModuleBuilder, object>, FieldInfo> _fields = new Dictionary<Tuple<ModuleBuilder, object>, FieldInfo>();

		/// <summary>
		/// Initializes static members of the StaticFieldStorage class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static StaticFieldStorage()
		{
			// create a shared assembly for all of the static fields to live in
			AssemblyName an = Assembly.GetExecutingAssembly().GetName();

			// TODO remove debugger condition for v6
			if (DebuggerIsAttached())  // Make the dynamic assembly have a unique name.  Fixes debugger issue #224.  
				an.Name = an.Name + ".DynamicAssembly";

			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
			_dynamicModule = ab.DefineDynamicModule(an.Name);
		}

		/// <summary>
		/// Emits the value stored in static storage.
		/// </summary>
		/// <param name="il">The ILGenerator to emit to.</param>
		/// <param name="value">The value to emit.</param>
		/// <param name="moduleBuilder">The module to write to or null to use the default module.</param>
		public static void EmitLoad(ILGenerator il, object value, ModuleBuilder moduleBuilder = null)
		{
			FieldInfo field;

			var key = Tuple.Create(moduleBuilder ?? _dynamicModule, value);

			if (!_fields.TryGetValue(key, out field))
			{
				field = CreateField(key.Item1, value);
				_fields[key] = field;
			}

			il.Emit(OpCodes.Ldsfld, field);
		}

		/// <summary>
		/// Indicates if the debugger is attached.  Only evaluated once so that the answer is stable
		/// Temporary method, remove in v6
		/// </summary>
		/// <returns>True if there is a debugger attached.</returns>
		internal static bool DebuggerIsAttached()
		{
			if (!_isDebuggerAttached.HasValue)
				_isDebuggerAttached = Debugger.IsAttached;

			return _isDebuggerAttached.Value;
		}

		/// <summary>
		/// Creates a static field that contains the given value.
		/// </summary>
		/// <param name="moduleBuilder">The modulebuilder to write to.</param>
		/// <param name="value">The value to store.</param>
		/// <returns>A static field containing the value.</returns>
		private static FieldInfo CreateField(ModuleBuilder moduleBuilder, object value)
		{
			// create a type based on DbConnectionWrapper and call the default constructor
			TypeBuilder tb = moduleBuilder.DefineType(Guid.NewGuid().ToString());
			tb.DefineField("_storage", value.GetType(), FieldAttributes.Static | FieldAttributes.Public);
			Type t = tb.CreateType();

			var field = t.GetField("_storage", BindingFlags.Static | BindingFlags.Public);
			field.SetValue(null, value);

			return field;
		}
	}
}
