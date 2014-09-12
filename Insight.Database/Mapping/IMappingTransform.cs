using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Mapping
{
	/// <summary>
	/// Transforms database names (e.g. parameters, columns), into object names.
	/// </summary>
	public interface IMappingTransform
	{
		/// <summary>
		/// Transforms the database name (parameter or column) prior to attempting to map the field.
		/// </summary>
		/// <param name="type">The type being bound.</param>
		/// <param name="databaseName">The name from the database.</param>
		/// <returns>The database name, transformed. For example, by removing underscores.</returns>
		string TransformDatabaseName(Type type, string databaseName);
	}
}
