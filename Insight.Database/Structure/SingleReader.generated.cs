using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	public class SingleReader<T1, T2> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2> Default = new SingleReader<T1, T2>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	public class SingleReader<T1, T2, T3> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3> Default = new SingleReader<T1, T2, T3>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4> Default = new SingleReader<T1, T2, T3, T4>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5> Default = new SingleReader<T1, T2, T3, T4, T5>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6> Default = new SingleReader<T1, T2, T3, T4, T5, T6>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
	/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Records)
		{
		}
		#endregion
	}	
	/// <summary>
	/// Reads a single record from a data stream.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth subobject.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth subobject.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth subobject.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh subobject.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth subobject.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth subobject.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth subobject.</typeparam>
	/// <typeparam name="T15">The type of the data in the fifteenth subobject.</typeparam>
	/// <typeparam name="T16">The type of the data in the sixteenth subobject.</typeparam>
	public class SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : SingleReader<T1>
	{
		#region Fields
		/// <summary>
		/// The default reader to read a list of type T.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Default = new SingleReader<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the SingleReader class.
		/// </summary>
		public SingleReader() : base(OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Records)
		{
		}
		#endregion
	}	
}
