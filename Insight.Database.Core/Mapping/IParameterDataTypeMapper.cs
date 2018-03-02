using Insight.Database.Mapping;
using System;
using System.Data;

namespace Insight.Database.Mapping
{
	/// <summary>
	/// Maps a DbType to a field, overriding.
	/// </summary>
	public interface IParameterDataTypeMapper : IMapper
	{
		/// <summary>
		/// Returns a DbType of the field on the given type that maps to the specified parameter.
		/// </summary>
		/// <param name="type">The type to test.</param>
		/// <param name="command">The command being executed.</param>
		/// <param name="parameter">The parameter being mapped.</param>
		/// <param name="dbType">The best guess of which data type the parameter should be.</param>
		/// <returns>The DbType to map the parameter to. Return the best guess parameter if unhandled.</returns>		
		DbType MapParameterType(Type type, IDbCommand command, IDataParameter parameter, DbType dbType);
	}
}
