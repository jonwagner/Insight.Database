using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database.Mapping;

namespace Insight.Database
{
	/// <summary>
	/// Specifes when the fields of child objects can be bound on a class or on the parameters of an interface method.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class BindChildrenAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the BindChildrenAttribute class.
		/// </summary>
		public BindChildrenAttribute()
		{
			For = BindChildrenFor.All;
		}

		/// <summary>
		/// Initializes a new instance of the BindChildrenAttribute class.
		/// </summary>
		/// <param name="bindFor">Specifies the times when child fields can be bound.</param>
		public BindChildrenAttribute(BindChildrenFor bindFor)
		{
			For = bindFor;
		}

		/// <summary>
		/// Gets the valid times when child fields can be bound.
		/// </summary>
		public BindChildrenFor For { get; private set; }
	}
}
