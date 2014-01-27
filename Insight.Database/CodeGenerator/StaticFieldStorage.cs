using System;
using System.Collections.Generic;
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
		/// The field used to store the value.
		/// </summary>
		private FieldInfo _field;

		/// <summary>
		/// Initializes a new instance of the StaticFieldStorage class.
		/// </summary>
		/// <param name="value">The value to store statically.</param>
		public StaticFieldStorage(object value) : this(null, value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StaticFieldStorage class.
		/// </summary>
		/// <param name="moduleBuilder">The ModuleBuilder for the type that needs to access the value.</param>
		/// <param name="value">The value to store statically.</param>
		public StaticFieldStorage(ModuleBuilder moduleBuilder, object value)
		{
			if (moduleBuilder == null)
			{
				// create a new assembly
				AssemblyName an = Assembly.GetExecutingAssembly().GetName();
				AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
				moduleBuilder = ab.DefineDynamicModule(an.Name);
			}

			// create a type based on DbConnectionWrapper and call the default constructor
			TypeBuilder tb = moduleBuilder.DefineType(Guid.NewGuid().ToString());
			tb.DefineField("_storage", value.GetType(), FieldAttributes.Static | FieldAttributes.Public);
			Type t = tb.CreateType();

			_field = t.GetField("_storage", BindingFlags.Static | BindingFlags.Public);
			_field.SetValue(null, value);
		}

		/// <summary>
		/// Emits the value stored in static storage.
		/// </summary>
		/// <param name="il">The ILGenerator to emit to.</param>
		public void EmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldsfld, _field);
		}
	}
}
