using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for wrapped connections.
	/// </summary>
	/// <remarks>
	/// For connections that wrap commands in other commands, 
	/// this class unwraps the command when Insight needs to access advanced features.
	/// </remarks>
	public abstract class WrappedInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Unwraps the given command and returns the inner command.
		/// </summary>
		/// <param name="command">The outer command.</param>
		/// <returns>The inner command.</returns>
		public abstract IDbCommand GetInnerCommand(IDbCommand command);

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public override IList<IDataParameter> DeriveParameters(IDbCommand command)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).DeriveParameters(command);
		}

		/// <summary>
		/// Clones a parameter so that it can be used with another command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameter">The parameter to clone.</param>
		/// <returns>The clone.</returns>
		public override IDataParameter CloneParameter(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).CloneParameter(command, parameter);
		}

		/// <summary>
		/// Creates a parameter for a table-valued parameter.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="tableTypeName">The name of the table type.</param>
		/// <returns>An initialized parameter for the table.</returns>
		public override IDataParameter CreateTableValuedParameter(IDbCommand command, string parameterName, string tableTypeName)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).CreateTableValuedParameter(command, parameterName, tableTypeName);
		}

		/// <summary>
		/// Gets the schema for a given user-defined table type.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <param name="tableTypeName">The name of the table type.</param>
		/// <returns>An open reader with the schema.</returns>
		/// <remarks>The caller is responsible for closing the reader and the connection.</remarks>
		public override IDataReader GetTableTypeSchema(IDbCommand command, string tableTypeName)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).GetTableTypeSchema(command, tableTypeName);
		}

		/// <summary>
		/// Determines if a parameter is a Table-valued parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>True if the parameter is a table-valued parameter.</returns>
		public override bool IsTableValuedParameter(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).IsTableValuedParameter(command, parameter);
		}

		/// <summary>
		/// Calculates the table type name for a table parameter.
		/// </summary>
		/// <param name="command">The related command object.</param>
		/// <param name="parameter">The parameter to test.</param>
		/// <returns>The name of the table parameter.</returns>
		public override string GetTableParameterTypeName(IDbCommand command, IDataParameter parameter)
		{
			command = GetInnerCommand(command);
			return InsightDbProvider.For(command).GetTableParameterTypeName(command, parameter);
		}
	}
}
