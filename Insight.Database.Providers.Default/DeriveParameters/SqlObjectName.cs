#if NO_COMMAND_BUILDER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.MissingExtensions;

#pragma warning disable 1591

namespace Insight.Database.Providers.Default
{
	/// <summary>
	/// Helper Object that parses a multi-part name, e.g. Mydb.dbo.proc.
	/// Handles more complex cases like [My.db].dbo.proc.
	/// But does nto attempt to do a full validation of legal sql
	/// </summary>
	public class SqlObjectName
	{
		public string Database { get; set; }
		public string Schema { get; set; }
		public string Name { get; set; }

		public SqlObjectName(string database, string schema, string name)
		{
			Database = database;
			Schema = schema;
			Name = name;
		}

		public SqlObjectName(string fullyQualifiedNameText)
		{
			var nameParts = SplitName(fullyQualifiedNameText);

			int numElements = nameParts.Count();

			Name = CleanSqlName(nameParts[numElements - 1]);

			if (numElements >= 2)  // Owner is second to last
				Schema = CleanSqlName(nameParts[numElements - 2]);

			if (numElements >= 3) // Database is third from end
				Database = CleanSqlName(nameParts[numElements - 3]);
		}

		/// <summary>
		/// Split a name, eg [My.db].dbo.proc while respecting things in brackets
		/// </summary>
		private string[] SplitName(string fullyQualifiedNameText)
		{
			bool isInLiteralSection = false;
			char[] fullyQualifiedNameArray = fullyQualifiedNameText.ToCharArray();

			// Use the single quote to represent an dot that should be split on...
			char specialDelimiter = '\'';
			if (fullyQualifiedNameText.Contains("'")) // ..unless a quote is somehow used...
				specialDelimiter = '⨁';  // .. then pick something complete obscure from unicode symbols

			for (int i = 0; i < fullyQualifiedNameArray.Length; i++)
			{
				char c = fullyQualifiedNameArray[i];

				if (IsEnclosingChar(c))
					isInLiteralSection = !isInLiteralSection; // flip the state

				else if ((!isInLiteralSection) && (c == '.'))  // its a valid splitter
				{
					fullyQualifiedNameArray[i] = specialDelimiter;
				}
			}

			string[] nameParts = new string(fullyQualifiedNameArray).Split(specialDelimiter);

			return nameParts;
		}

		private bool IsEnclosingChar(char c)
		{
			return ((c == '[') || (c == ']') || (c == '"'));
		}

		public string GetFullNameAsString()
		{
			string fullName = "";

			if (Database.IsNullOrWhiteSpace() && Schema.IsNullOrWhiteSpace())
				fullName = $"[{Name}]";

			else if ((!Database.IsNullOrWhiteSpace()) && (!Schema.IsNullOrWhiteSpace()))
				fullName = $"[{Database}].[{Schema}].[{Name}]";

			else if ((Database.IsNullOrWhiteSpace()) && (!Schema.IsNullOrWhiteSpace()))
				fullName = $"[{Schema}].[{Name}]";

			else if ((!Database.IsNullOrWhiteSpace()) && (Schema.IsNullOrWhiteSpace()))
				fullName = $"[{Database}]..[{Name}]";

			return fullName;
		}

		private static string CleanSqlName(string name)
		{
			if (name == null) return null;

			return name.Replace("\"", "").Replace("'", "").Replace("[", "").Replace("]", "");
		}

	}
}
#endif
