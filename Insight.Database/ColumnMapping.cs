using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// A singleton class that handles the mapping operations from recordsets to objects.
	/// </summary>
	public class ColumnMapping
	{
		#region Internal Fields
		/// <summary>
		/// An internal lock structure.
		/// </summary>
		private static object _lock = new object();

		/// <summary>
		/// The singleton instance of the ColumnMapping configuration for Tables and Table Valued Parameters.
		/// </summary>
		private static ColumnMapping _tables = new ColumnMapping();

		/// <summary>
		/// The singleton instance of the ColumnMapping configuration for Parameters.
		/// </summary>
		private static ColumnMapping _parameters = new ColumnMapping();

		/// <summary>
		/// The mapping event handler.
		/// </summary>
		private EventHandler<ColumnMappingEventArgs> _mappings;
		#endregion

		#region Constructors
		/// <summary>
		/// Prevents a default instance of the ColumnMapping class from being created outside of the class constructor.
		/// </summary>
		private ColumnMapping()
		{
			ResetHandlers();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets singleton instance of the ColumnMapping configuration for Tables and Table Valued Parameters.
		/// </summary>
		public static ColumnMapping Tables { get { return _tables; } }

		/// <summary>
		/// Gets singleton instance of the ColumnMapping configuration for Parameters.
		/// </summary>
		public static ColumnMapping Parameters { get { return _parameters; } }
		#endregion

		/// <summary>
		/// Adds a column mapping handler to the chain of handlers.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping AddHandler(IColumnMappingHandler handler)
		{
			lock (_lock)
			{
				_mappings += handler.HandleColumnMapping;
			}

			return this;
		}

		#region Common Replacement Functions
		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text of the regex.</param>
		/// <param name="replacement">The text to use as a replacement.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping ReplaceRegex<T>(string pattern, string replacement)
		{
			return AddHandler(new RegexReplaceMappingHandler<T>(pattern, replacement));
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// This applies to all types of objects.
		/// </summary>
		/// <param name="pattern">The text of the regex.</param>
		/// <param name="replacement">The text to use as a replacement.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping ReplaceRegex(string pattern, string replacement)
		{
			return ReplaceRegex<object>(pattern, replacement);
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text of the regex.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveRegex<T>(string pattern)
		{
			return AddHandler(new RegexReplaceMappingHandler<T>(pattern, String.Empty));
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// This applies to all types of objects.
		/// </summary>
		/// <param name="text">The text of the regex.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveRegex(string text)
		{
			return RemoveRegex<object>(text);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveStrings<T>(string pattern)
		{
			return AddHandler(new RegexReplaceMappingHandler<T>(pattern, String.Empty));
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="pattern">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveStrings(string pattern)
		{
			return RemoveStrings<object>(pattern);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the beginning of a column name.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="prefix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemovePrefixes<T>(string prefix)
		{
			return RemoveStrings("^" + prefix);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the beginning of a column name.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="prefix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemovePrefixes(string prefix)
		{
			return RemovePrefixes<object>(prefix);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the end of a column name.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="suffix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveSuffixes<T>(string suffix)
		{
			return RemoveStrings(suffix + "$");
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the end of a column name.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="suffix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping RemoveSuffixes(string suffix)
		{
			return RemoveSuffixes<object>(suffix);
		}
		#endregion

		#region Internals
		/// <summary>
		/// Reset all of the handlers to the default.
		/// </summary>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping ResetHandlers()
		{
			lock (_lock)
			{
				_mappings = null;
				_mappings += DefaultMappingHandler;
			}

			return this;
		}

		/// <summary>
		/// Creates the list of property setters for a reader.
		/// </summary>
		/// <param name="type">The type of object to map to.</param>
		/// <param name="reader">The reader to read.</param>
		/// <param name="commandText">The command text that is currently being mapped.</param>
		/// <param name="commandType">The command type that is currently being mapped.</param>
		/// <param name="parameters">The list of parameters used in the mapping operation.</param>
		/// <param name="startColumn">The index of the first column to map.</param>
		/// <param name="columnCount">The number of columns to map.</param>
		/// <param name="uniqueMatches">True to only return the first match per field, false to return all matches per field.</param>
		/// <returns>An array of setters.</returns>
		internal ClassPropInfo[] CreateMapping(Type type, IDataReader reader, string commandText, CommandType? commandType, IList<IDbDataParameter> parameters, int startColumn, int columnCount, bool uniqueMatches)
		{
			ClassPropInfo[] mapping = new ClassPropInfo[columnCount];

			// convert the list of names into a list of set reflections
			// clone the methods list, since we are only going to use each setter once (i.e. if you return two ID columns, we will only use the first one)
			var setMethods = new Dictionary<string, ClassPropInfo>(ClassPropInfo.GetMappingForType(type));

			List<IDataParameter> readOnlyParameters = null;
			if (parameters != null)
				readOnlyParameters = new List<IDataParameter>(parameters.OfType<IDataParameter>().ToList());

			// find all of the mappings
			for (int i = 0; i < columnCount; i++)
			{
				// generate an event
				var e = new ColumnMappingEventArgs()
				{
					TargetType = type,
					Reader = reader,
					FieldIndex = i + startColumn,
					CommandText = commandText,
					CommandType = commandType,
					Parameters = readOnlyParameters,
				};

				lock (_lock)
				{
					_mappings(null, e);
				}

				// if no mapping was returned, then skip the column
				if (e.Canceled || String.IsNullOrWhiteSpace(e.TargetFieldName))
					continue;

				// get the target property based on the result
				string targetFieldName = e.TargetFieldName.ToUpperInvariant();

				ClassPropInfo setter;
				if (setMethods.TryGetValue(targetFieldName, out setter))
				{
					mapping[i] = setter;

					// remove the name from the list so we can only use it once
					if (uniqueMatches)
						setMethods.Remove(targetFieldName);
				}
			}

			return mapping;
		}

		/// <summary>
		/// Provides the default mapping logic.
		/// </summary>
		/// <param name="sender">The ColumnMapping object that has generated the event.</param>
		/// <param name="e">The ColumnMappingEventArgs to process.</param>
		private void DefaultMappingHandler(object sender, ColumnMappingEventArgs e)
		{
			if (e.Reader != null)
				e.TargetFieldName = e.Reader.GetName(e.FieldIndex);
			else if (e.Parameters != null)
				e.TargetFieldName = e.Parameters[e.FieldIndex].ToString();
			else
				throw new InvalidOperationException("DefaultMappingHandler requires either a Reader or Parameters list.");
		}
		#endregion
	}
}
