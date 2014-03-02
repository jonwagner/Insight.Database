using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Insight.Database
{
	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2) => callback(new object[] {  t1, t2 });

			return new OneToOne<T1, T2>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3) => callback(new object[] {  t1, t2, t3 });

			return new OneToOne<T1, T2, T3>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4) => callback(new object[] {  t1, t2, t3, t4 });

			return new OneToOne<T1, T2, T3, T4>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => callback(new object[] {  t1, t2, t3, t4, t5 });

			return new OneToOne<T1, T2, T3, T4, T5>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => callback(new object[] {  t1, t2, t3, t4, t5, t6 });

			return new OneToOne<T1, T2, T3, T4, T5, T6>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the graph.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the graph.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the graph.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the graph.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the graph.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the graph.</typeparam>
	/// <typeparam name="T15">The type of the fifteenth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(handler, null, idColumns);
		}
	}

	/// <summary>
	/// Marker class that defines an object graph.
	/// </summary>
	/// <typeparam name="T1">The type of the first subobject in the graph.</typeparam>
	/// <typeparam name="T2">The type of the second subobject in the graph.</typeparam>
	/// <typeparam name="T3">The type of the third subobject in the graph.</typeparam>
	/// <typeparam name="T4">The type of the fourth subobject in the graph.</typeparam>
	/// <typeparam name="T5">The type of the fifth subobject in the graph.</typeparam>
	/// <typeparam name="T6">The type of the sixth subobject in the graph.</typeparam>
	/// <typeparam name="T7">The type of the seventh subobject in the graph.</typeparam>
	/// <typeparam name="T8">The type of the eighth subobject in the graph.</typeparam>
	/// <typeparam name="T9">The type of the nineth subobject in the graph.</typeparam>
	/// <typeparam name="T10">The type of the tenth subobject in the graph.</typeparam>
	/// <typeparam name="T11">The type of the eleventh subobject in the graph.</typeparam>
	/// <typeparam name="T12">The type of the twelfth subobject in the graph.</typeparam>
	/// <typeparam name="T13">The type of the thirteenth subobject in the graph.</typeparam>
	/// <typeparam name="T14">The type of the fourteenth subobject in the graph.</typeparam>
	/// <typeparam name="T15">The type of the fifteenth subobject in the graph.</typeparam>
	/// <typeparam name="T16">The type of the sixteenth subobject in the graph.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Graph<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : Graph<T1>
	{
		internal override OneToOne<T1> GetOneToOneMapping(Action<object[]> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> handler = null;
			if (callback != null)
				handler = ( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) => callback(new object[] {  t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 });

			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(handler, null, idColumns);
		}
	}

}
