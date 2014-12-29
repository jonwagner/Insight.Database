using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Represents a dynamic method that extracts the ID portion of an object.
	/// When multiple fields are part of the ID, they are combined into a Tuple.
	/// </summary>
	class IDAccessor
	{
		/// <summary>
		/// The list of properties to extract.
		/// </summary>
		private List<ClassPropInfo> _propInfo;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the IDAccessor class.
		/// </summary>
		/// <param name="propInfo">The property to access.</param>
		public IDAccessor(ClassPropInfo propInfo)
		{
			_propInfo = new List<ClassPropInfo>();
			_propInfo.Add(propInfo);

			MemberType = propInfo.MemberType;
		}

		/// <summary>
		/// Initializes a new instance of the IDAccessor class.
		/// </summary>
		/// <param name="propInfo">The properties to access.</param>
		public IDAccessor(IEnumerable<ClassPropInfo> propInfo)
		{
			_propInfo = propInfo.ToList();

			if (_propInfo.Count == 1)
				MemberType = _propInfo.First().MemberType;
			else
				MemberType = GetTupleType(_propInfo.Count).MakeGenericType(MemberTypes.ToArray());
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the type of the ID.
		/// </summary>
		public Type MemberType { get; private set; }

		/// <summary>
		/// Gets the list of types contained in the ID.
		/// </summary>
		public IEnumerable<Type> MemberTypes { get { return _propInfo.Select(p => p.MemberType); } }

        /// <summary>
        /// Gets the list of names of properties contained in the ID.
        /// </summary>
        public IEnumerable<string> MemberNames { get { return _propInfo.Select(p => p.Name); } }
		#endregion

		/// <summary>
		/// Creates a dynamic method that gets the IDs from the object.
		/// </summary>
		/// <typeparam name="TObject">The type of object.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <returns>A method that can extract the IDs.</returns>
		public Func<TObject, TValue> CreateGetMethod<TObject, TValue>()
		{
			if (_propInfo.Count == 1)
			{
				return _propInfo.First().CreateGetMethod<TObject, TValue>();
			}
			else
			{
				var dm = new DynamicMethod(
					String.Format(CultureInfo.InvariantCulture, "Get-{0}-{1}-{2}", typeof(TObject).FullName, "IDs", Guid.NewGuid()),
					MemberType,
					new Type[] { typeof(TObject) },
					true);
				var il = dm.GetILGenerator();

				// get all of the values
				foreach (var p in _propInfo)
				{
					il.Emit(OpCodes.Ldarg_0);
					p.EmitGetValue(il);
				}

				var createTupleMethod = typeof(Tuple).GetMethods(BindingFlags.Static | BindingFlags.Public)
						.Single(m => m.Name == "Create" && m.GetGenericArguments().Length == _propInfo.Count)
						.MakeGenericMethod(MemberType.GetGenericArguments());

				// create the tuple
				il.Emit(OpCodes.Call, createTupleMethod);
				il.Emit(OpCodes.Ret);

				return (Func<TObject, TValue>)dm.CreateDelegate(typeof(Func<TObject, TValue>));
			}
		}

		/// <summary>
		/// Gets the generic type of a tuple that can store the given number of parameters.
		/// </summary>
		/// <param name="count">The number of parameters.</param>
		/// <returns>The tuple type.</returns>
		private Type GetTupleType(int count)
		{
			switch (count)
			{
				case 1: return typeof(Tuple<>);
				case 2: return typeof(Tuple<,>);
				case 3: return typeof(Tuple<,,>);
				case 4: return typeof(Tuple<,,,>);
				case 5: return typeof(Tuple<,,,,>);
				case 6: return typeof(Tuple<,,,,,>);
				case 7: return typeof(Tuple<,,,,,,>);
				default:
					throw new ArgumentException("count");
			}
		}
	}
}
