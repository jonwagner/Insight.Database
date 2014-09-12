using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// Defines a serialization rule.
	/// </summary>
	interface IDbSerializationRule
	{
		/// <summary>
		/// Given a property, select the proper serializer.
		/// </summary>
		/// <param name="prop">The property.</param>
		/// <returns>The serializer for the object.</returns>
		IDbObjectSerializer GetSerializer(ClassPropInfo prop);
	}
}
