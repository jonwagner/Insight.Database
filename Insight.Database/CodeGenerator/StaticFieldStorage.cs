﻿using System;
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
        /// The shared module that stores all of the static variables.
        /// </summary>
        private static ModuleBuilder _dynamicModule;

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
            // following line is just a workaround for VS2015 debugger https://github.com/jonwagner/Insight.Database/issues/224
            an.Name += ".Tmp"; 
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
