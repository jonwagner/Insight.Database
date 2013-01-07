using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// Represents an accessor for a field/property of a class.
	/// </summary>
	class ClassPropInfo
	{
		#region Private Members
		/// <summary>
		/// The default binding flags to use when looking up properties.
		/// </summary>
		private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;

		/// <summary>
		/// The cache of mappings.
		/// </summary>
		private static ConcurrentDictionary<Type, Dictionary<string, ClassPropInfo>> _mappingCache = new ConcurrentDictionary<Type, Dictionary<string, ClassPropInfo>>();

		/// <summary>
		/// The cache of members.
		/// </summary>
		private static ConcurrentDictionary<Type, ReadOnlyCollection<ClassPropInfo>> _memberCache = new ConcurrentDictionary<Type, ReadOnlyCollection<ClassPropInfo>>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ClassPropInfo class.
		/// </summary>
		/// <param name="memberInfo">The member to represent.</param>
		private ClassPropInfo(MemberInfo memberInfo)
		{
			Type = memberInfo.ReflectedType;
			Name = memberInfo.Name.ToUpperInvariant();

			// try to look up a field with the name
			FieldInfo = memberInfo as FieldInfo;
			if (FieldInfo != null)
			{
				MemberType = FieldInfo.FieldType;
			}
			else
			{
				// get the property
				PropertyInfo p = memberInfo as PropertyInfo;

				// get the getter and setter
				GetMethodInfo = p.GetGetMethod(true);
				SetMethodInfo = p.GetSetMethod(true);

				MemberType = p.PropertyType;
			}

			// see if there is a column attribute defined on the field
			var attribute = memberInfo.GetCustomAttributes(typeof(ColumnAttribute), true).OfType<ColumnAttribute>().FirstOrDefault();
			ColumnName = (attribute != null) ? attribute.ColumnName.ToUpperInvariant() : Name;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the database column associated with the property.
		/// </summary>
		public string ColumnName { get; private set; }

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
		/// Gets a value indicating whether the member can be set.
		/// </summary>
		public bool CanSetMember { get { return FieldInfo != null || SetMethodInfo != null; } }

		/// <summary>
		/// Gets a value indicating whether the member can be gotten.
		/// </summary>
		public bool CanGetMember { get { return FieldInfo != null || GetMethodInfo != null; } }

		/// <summary>
		/// Gets or sets the type that this is bound to.
		/// </summary>
		private Type Type { get; set; }
		#endregion

		#region Method List Members
		/// <summary>
		/// Returns the mapping between columns and set members.
		/// </summary>
		/// <remarks>
		/// The methods are normalized with an uppercase name.
		/// </remarks>
		/// <param name="type">The type to analyze.</param>
		/// <returns>A dictionary of set methods for a type.</returns>
		public static Dictionary<string, ClassPropInfo> GetMappingForType(Type type)
		{
			// always return a clone so nobody can modify this
			// NOTE: you could switch this to ReadOnlyDictionary, but that isn't supported in .NET4.0
			return new Dictionary<string, ClassPropInfo>(_mappingCache.GetOrAdd(
				type,
				t => GetMembersForType(type).ToDictionary(p => p.ColumnName)));
		}

		/// <summary>
		/// Returns the set of set methods (field or property) for a type.
		/// </summary>
		/// <remarks>
		/// The methods are normalized with an uppercase name and are ordered with properties before fields.
		/// </remarks>
		/// <param name="type">The type to analyze.</param>
		/// <returns>A ReadOnlyCollection of set methods for a type.</returns>
		public static ReadOnlyCollection<ClassPropInfo> GetMembersForType(Type type)
		{
			return _memberCache.GetOrAdd(
				type,
				t =>
				{
					List<ClassPropInfo> members = new List<ClassPropInfo>();

					// if this is a structured type get the get properties for the types that we pass in
					// exception are the Xml/XDocument classes that we already have special handlers for
					if (!TypeHelper.IsAtomicType(t)
						&& t != typeof(XmlDocument)
						&& t != typeof(XDocument))
					{
						// get properties first
						foreach (var p in t.GetProperties(DefaultBindingFlags).Select(m => new ClassPropInfo(m)))
							if (!members.Any(m => m.ColumnName == p.ColumnName))
								members.Add(p);

						// then get fields
						foreach (var p in t.GetFields(DefaultBindingFlags).Select(m => new ClassPropInfo(m)))
							if (!members.Any(m => m.ColumnName == p.ColumnName))
								members.Add(p);
					}

					return new ReadOnlyCollection<ClassPropInfo>(members);
				});
		}
		#endregion

		#region Emit Members
		/// <summary>
		/// Emit a call to get the value.
		/// </summary>
		/// <param name="il">The IL generator to use.</param>
		public void EmitGetValue(ILGenerator il)
		{
			if (FieldInfo != null)
				il.Emit(OpCodes.Ldfld, FieldInfo);
			else if (GetMethodInfo != null)
			{
				if (Type.IsValueType)
					il.Emit(OpCodes.Call, GetMethodInfo);
				else
					il.Emit(OpCodes.Callvirt, GetMethodInfo);
			}
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
			{
				if (Type.IsValueType)
					il.Emit(OpCodes.Call, SetMethodInfo);
				else
					il.Emit(OpCodes.Callvirt, SetMethodInfo);
			}
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
		#endregion
	}
}
