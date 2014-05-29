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
			Name = memberInfo.Name;
			MemberInfo = memberInfo;

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

			// initialize the rest of the data from a ColumnAttribute from the field
			var attribute = memberInfo.GetCustomAttributes(typeof(ColumnAttribute), true).OfType<ColumnAttribute>().FirstOrDefault() ?? new ColumnAttribute();
			ColumnName = attribute.ColumnName ?? Name;
			SerializationMode = attribute.SerializationMode;
			Serializer = attribute.Serializer;
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
		/// Gets the MemberInfo for this member.
		/// </summary>
		/// <value>The MemberInfo this is bound to.</value>
		public MemberInfo MemberInfo { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the member can be set.
		/// </summary>
		public bool CanSetMember
		{
			get
			{
				return (FieldInfo != null && !FieldInfo.Attributes.HasFlag(FieldAttributes.InitOnly)) || SetMethodInfo != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the member can be gotten.
		/// </summary>
		public bool CanGetMember { get { return FieldInfo != null || GetMethodInfo != null; } }

		/// <summary>
		/// Gets the serialization mode defined for the field.
		/// </summary>
		public SerializationMode SerializationMode { get; private set; }

		/// <summary>
		/// Gets the custom serializer defined for the field.
		/// </summary>
		public Type Serializer { get; private set; }		

		/// <summary>
		/// Gets or sets the type that this is bound to.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Gets a value indicating whether the column name has been overridden.
		/// </summary>
		public bool ColumnNameIsOverridden { get { return ColumnName != Name; } }
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

					// if there is a base type, get those members first
					// derived classes won't have access to the private members of the base class
					if (type.BaseType != null)
						members.AddRange(GetMembersForType(type.BaseType));

					// if this is a structured type get the get properties for the types that we pass in
					// exception are the Xml/XDocument classes that we already have special handlers for
					if (!TypeHelper.IsAtomicType(t)
						&& t != typeof(XmlDocument)
						&& t != typeof(XDocument))
					{
						// get fields and properties. Prioritize explicitly mapped members first, then properties over fields.
						foreach (var p in t.GetProperties(DefaultBindingFlags).Select(m => new ClassPropInfo(m))
							.Union(t.GetFields(DefaultBindingFlags).Select(m => new ClassPropInfo(m)))
							.OrderBy(m => !m.ColumnNameIsOverridden)
							.ThenBy(m => (m.FieldInfo == null) ? 0 : 1))
							if (!members.Any(m => String.Compare(m.ColumnName, p.ColumnName, StringComparison.OrdinalIgnoreCase) == 0))
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
		#endregion

		#region Accessor Members
		/// <summary>
		/// Creates a method that gets the property from the object.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <returns>An accessor function.</returns>
		public Func<TObject, TValue> CreateGetMethod<TObject, TValue>()
		{
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Get-{0}-{1}-{2}", typeof(TObject).FullName, Name, Guid.NewGuid()),
				typeof(TValue),
				new Type[] { typeof(TObject) },
				true);
			var il = dm.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			if (FieldInfo != null)
				il.Emit(OpCodes.Ldfld, FieldInfo);
			else
				il.Emit(OpCodes.Call, GetMethodInfo);

			if (typeof(TValue) == typeof(object) && MemberType.IsValueType)
				il.Emit(OpCodes.Box, MemberType);				

			il.Emit(OpCodes.Ret);

			return (Func<TObject, TValue>)dm.CreateDelegate(typeof(Func<TObject, TValue>));
		}

		/// <summary>
		/// Creates a method that sets the property from the object.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <returns>An accessor function.</returns>
		public Action<TObject, TValue> CreateSetMethod<TObject, TValue>()
		{
			var dm = new DynamicMethod(
				String.Format(CultureInfo.InvariantCulture, "Set-{0}-{1}-{2}", typeof(TObject).FullName, Name, Guid.NewGuid()),
				null,
				new Type[] { typeof(TObject), typeof(TValue) },
				true);
			var il = dm.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			if (FieldInfo != null)
				il.Emit(OpCodes.Stfld, FieldInfo);
			else
				il.Emit(OpCodes.Call, SetMethodInfo);

			il.Emit(OpCodes.Ret);

			return (Action<TObject, TValue>)dm.CreateDelegate(typeof(Action<TObject, TValue>));
		}
		#endregion
	}
}
