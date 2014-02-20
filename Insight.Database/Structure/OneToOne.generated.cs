using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2> Records = new OneToOne<T1, T2>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2>)Callback)(
				(T1)objects[0],
				(T2)objects[1]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3> Records = new OneToOne<T1, T2, T3>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4> Records = new OneToOne<T1, T2, T3, T4>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5> Records = new OneToOne<T1, T2, T3, T4, T5>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6> Records = new OneToOne<T1, T2, T3, T4, T5, T6>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
			typeof(T12),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10],
				(T12)objects[11]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the record.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
			typeof(T12),
			typeof(T13),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10],
				(T12)objects[11],
				(T13)objects[12]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the record.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the record.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
			typeof(T12),
			typeof(T13),
			typeof(T14),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10],
				(T12)objects[11],
				(T13)objects[12],
				(T14)objects[13]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the record.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the record.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the record.</typeparam>
	/// <typeparam name="T15">The type of the fifteenth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
			typeof(T12),
			typeof(T13),
			typeof(T14),
			typeof(T15),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			return OneToOne<Guardian<T1, TID>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Records;
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10],
				(T12)objects[11],
				(T13)objects[12],
				(T14)objects[13],
				(T15)objects[14]
			);
		}
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the record.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the record.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the record.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the record.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the record.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the record.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the record.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the record.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the record.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the record.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the record.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the record.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the record.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the record.</typeparam>
	/// <typeparam name="T15">The type of the fifteenth subobject in the record.</typeparam>
	/// <typeparam name="T16">The type of the sixteenth subobject in the record.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : OneToOne<T1>, IRecordStructure
	{
		/// <summary>
		/// The static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Records = new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>();

		/// <summary>
		/// The types of objects returned by this record type.
		/// </summary>
		private static Type[] _objectTypes = new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(T4),
			typeof(T5),
			typeof(T6),
			typeof(T7),
			typeof(T8),
			typeof(T9),
			typeof(T10),
			typeof(T11),
			typeof(T12),
			typeof(T13),
			typeof(T14),
			typeof(T15),
			typeof(T16),
		};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		public override IRecordReader<Guardian<T1, TID>> GetGuardianReader<TID>()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		protected override void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)Callback)(
				(T1)objects[0],
				(T2)objects[1],
				(T3)objects[2],
				(T4)objects[3],
				(T5)objects[4],
				(T6)objects[5],
				(T7)objects[6],
				(T8)objects[7],
				(T9)objects[8],
				(T10)objects[9],
				(T11)objects[10],
				(T12)objects[11],
				(T13)objects[12],
				(T14)objects[13],
				(T15)objects[14],
				(T16)objects[15]
			);
		}
	}

}