using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Specifies that Insight should generate the specified SQL for the interface method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class SqlAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the SqlAttribute class.
		/// </summary>
		/// <param name="sql">The SQL to use for the interface method.</param>
		public SqlAttribute(string sql)
		{
			Sql = sql;
			CommandType = CommandType.Text;
		}

		/// <summary>
		/// Gets the SQL to use for the interface method.
		/// </summary>
		public string Sql { get; private set; }

		/// <summary>
		/// Gets or sets the CommandType to use for the interface method.
		/// </summary>
		public CommandType CommandType { get; set; }
	}
}
