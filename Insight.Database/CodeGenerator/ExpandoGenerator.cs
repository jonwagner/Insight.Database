using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Generates a dynamic method to convert an object to a FastExpando.
	/// </summary>
	/// <typeparam name="T">The type to convert to a FastExpando.</typeparam>
	static class ExpandoGenerator<T>
	{
		/// <summary>
		/// The cache for the methods.
		/// </summary>
		internal static readonly Func<T, FastExpando> Converter = null;

		/// <summary>
		/// The default constructor for type T.
		/// </summary>
		private static ConstructorInfo _constructor = typeof(FastExpando).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

		/// <summary>
		/// The FastExpando.SetValue method.
		/// </summary>
		private static MethodInfo _fastExpandoSetValue = typeof(FastExpando).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);

		/// <summary>
		/// Initializes static members of the ExpandoGenerator class.
		/// </summary>
		static ExpandoGenerator()
		{
			// create a dynamic method
			var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "ExpandoGenerator-{0}", typeof(T)), typeof(FastExpando), new[] { typeof(T) }, typeof(T), true);

			var il = dm.GetILGenerator();
			il.DeclareLocal(typeof(FastExpando));

			// new instance of fastexpando
			il.Emit(OpCodes.Newobj, _constructor);

			// for each public field or method, get the value
			foreach (ClassPropInfo accessor in GetAccessors())
			{
				il.Emit(OpCodes.Dup);										// push expando - so we can call set value
				il.Emit(OpCodes.Ldstr, accessor.Name);						// push name

				// get the value of the field or property
				il.Emit(OpCodes.Ldarg_0);									// push arg.0 (object)
				accessor.EmitGetValue(il);									// get the value
				// stack: [expando] [name] [value]

				// value types need to be boxed
				if (accessor.MemberType.IsValueType)
					il.Emit(OpCodes.Box, accessor.MemberType);

				// call expando.setvalue
				il.Emit(OpCodes.Callvirt, _fastExpandoSetValue);
			}

			// return expando - it should be left on the stack
			il.Emit(OpCodes.Ret);

			Converter = (Func<T, FastExpando>)dm.CreateDelegate(typeof(Func<T, FastExpando>));
		}

		/// <summary>
		/// Gets the public accessors on type T.
		/// </summary>
		/// <returns>A list of the public accessors.</returns>
		private static List<ClassPropInfo> GetAccessors()
		{
			List<ClassPropInfo> getMethods = new List<ClassPropInfo>();

			// get the get properties for the types that we pass in
			// get all fields in the class
			foreach (var f in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				getMethods.Add(new ClassPropInfo(typeof(T), f.Name, getMethod: true));

			// get all properties in the class
			foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				getMethods.Add(new ClassPropInfo(typeof(T), p.Name, getMethod: true));

			return getMethods;
		}
	}
}
