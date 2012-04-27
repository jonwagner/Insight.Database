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
	static class ExpandoGenerator
	{
		/// <summary>
		/// The cache for the methods.
		/// </summary>
        private static ConcurrentDictionary<Type, Func<object, FastExpando>> _converters = new ConcurrentDictionary<Type, Func<object, FastExpando>>();

		/// <summary>
		/// The default constructor for type T.
		/// </summary>
		private static ConstructorInfo _constructor = typeof(FastExpando).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

		/// <summary>
		/// The FastExpando.SetValue method.
		/// </summary>
		private static MethodInfo _fastExpandoSetValue = typeof(FastExpando).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);

        public static FastExpando Convert(object obj)
        {
            // get the converter for the type
            var converter = _converters.GetOrAdd(obj.GetType(), CreateConverter);

            return converter(obj);
        }

        /// <summary>
        /// Uses IL to generate a method that converts an object of a given type to a FastExpando.
        /// </summary>
        /// <param name="type">The type of object to be able to convert.</param>
        /// <returns>A function that can convert that type of object to a FastExpando.</returns>
        private static Func<object, FastExpando> CreateConverter(Type type)
        {
            // create a dynamic method
            var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "ExpandoGenerator-{0}", type.FullName), typeof(FastExpando), new[] { typeof(object) }, typeof(ExpandoGenerator), true);

            var il = dm.GetILGenerator();
            il.DeclareLocal(type);                                          // loc.0 = input type properly cast
            il.DeclareLocal(typeof(FastExpando));                           // loc.1 = return value

            // load the parameter object onto the stack and convert it into the local variable
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_0);

            // new instance of fastexpando                                  // top of stack
            il.Emit(OpCodes.Newobj, _constructor);

            // for each public field or method, get the value
            foreach (ClassPropInfo accessor in GetAccessors(type))
            {
                il.Emit(OpCodes.Dup);										// push expando - so we can call set value
                il.Emit(OpCodes.Ldstr, accessor.Name);						// push name

                // get the value of the field or property
                il.Emit(OpCodes.Ldloc_0);									// push loc.0 (typed object)
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

            return (Func<object, FastExpando>)dm.CreateDelegate(typeof(Func<object, FastExpando>));
        }

		/// <summary>
		/// Gets the public accessors on type T.
		/// </summary>
		/// <returns>A list of the public accessors.</returns>
		private static List<ClassPropInfo> GetAccessors(Type type)
		{
			List<ClassPropInfo> getMethods = new List<ClassPropInfo>();

			// get the get properties for the types that we pass in
			// get all fields in the class
            foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                getMethods.Add(new ClassPropInfo(type, f.Name, getMethod: true));

			// get all properties in the class
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                getMethods.Add(new ClassPropInfo(type, p.Name, getMethod: true));

			return getMethods;
		}
	}
}
