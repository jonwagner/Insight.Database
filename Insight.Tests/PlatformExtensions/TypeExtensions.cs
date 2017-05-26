using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight.Tests
{
	public static class TypeExtensions
	{
#if NO_TYPE_INFO
		public static Type GetTypeInfo(this Type type) { return type; }
#endif
	}
}
