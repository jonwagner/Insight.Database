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
			return new OneToOne<T1, T2>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(callback, idColumns);
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
			return new OneToOne<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(callback, idColumns);
		}
	}

}
