using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Insight.Database
{
#if NO_CLASSIC_REFLECTION
    /// <summary>
    /// Compatibility methods for reflection methods that are not implemented in .NET standard.
    /// </summary>
    static class ReflectionCompatibility
	{
		/// <summary>
        /// Creates a type from a TypeBuilder.
        /// </summary>
        /// <param name="builder">The TypeBuilder.</param>
        /// <returns>The created type.</returns>
		public static Type CreateType(this TypeBuilder builder)
		{
			return builder.CreateTypeInfo().AsType();
		}
	}
#endif
}
