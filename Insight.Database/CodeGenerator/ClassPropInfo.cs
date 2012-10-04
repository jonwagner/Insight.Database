using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Class representing get/set methods on a class.
	/// </summary>
	class ClassPropInfo
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ClassPropInfo class.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="propertyName">The name of the field.</param>
		public ClassPropInfo(Type type, string propertyName)
		{
			Type = type;
			Name = propertyName;

			// try to look up a field with the name
			FieldInfo = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (FieldInfo != null)
			{
				MemberType = FieldInfo.FieldType;
			}
			else
			{
				// get the property
				PropertyInfo p = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Field/Property {0} does not exist on type {1}", propertyName, type));
				
				// if p was on a base class, then switch to that type
				if (p.DeclaringType != type)
					p = p.DeclaringType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

				// get the getter and setter
				GetMethodInfo = p.GetGetMethod(true);
				SetMethodInfo = p.GetSetMethod(true);

				MemberType = p.PropertyType;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the type of the member this is bound to.
		/// </summary>
		/// <value>The type this member is bound to.</value>
		public Type MemberType { get; private set; }

		/// <summary>
		/// Gets the MethodInfo this is bound to, if a property.
		/// </summary>
		/// <value>The MethodInfo this is bound to.</value>
		public MethodInfo GetMethodInfo { get; private set; }

		/// <summary>
		/// Gets the SetMethodInfo this is bound to, if a property.
		/// </summary>
		/// <value>The MethodInfo this is bound to.</value>
		public MethodInfo SetMethodInfo { get; private set; }

		/// <summary>
		/// Gets the FieldInfo this is bound to, if a field.
		/// </summary>
		/// <value>The FieldInfo this is bound to.</value>
		public FieldInfo FieldInfo { get; private set; }

		/// <summary>
		/// Gets or sets the type that this is bound to.
		/// </summary>
		private Type Type { get; set; }
		#endregion

		/// <summary>
		/// Emit a call to get the value.
		/// </summary>
		/// <param name="il">The IL generator to use.</param>
		public void EmitGetValue(ILGenerator il)
		{
			if (FieldInfo != null)
				il.Emit(OpCodes.Ldfld, FieldInfo);
			else if (GetMethodInfo != null)
				il.Emit(OpCodes.Callvirt, GetMethodInfo);
			else
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a GetProperty method for {1} on class {0}.", Type.FullName, Name));
		}

		/// <summary>
		/// Emit a call to get the value.
		/// </summary>
		/// <param name="il">The IL generator to use.</param>
		public void EmitSetValue(ILGenerator il)
		{
			if (FieldInfo != null)
				il.Emit(OpCodes.Stfld, FieldInfo);
			else if (SetMethodInfo != null)
				il.Emit(OpCodes.Callvirt, SetMethodInfo);
			else
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot find a SetProperty method for {1} on class {0}.", Type.FullName, Name));
		}

		/// <summary>
		/// Emit a box operation if the type is a value type.
		/// </summary>
		/// <param name="il">The ILGenerator to use.</param>
		public void EmitBox(ILGenerator il)
		{
			// these should all be value types, so we need to box them
			if (MemberType.IsValueType)
				il.Emit(OpCodes.Box, MemberType);
		}
	}
}
