using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;
using Insight.Database.Structure;

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
		/// The singleton instance of the AllColumnMapping configuration for Parameters.
		/// </summary>
		private static AllColumnMapping _all = new AllColumnMapping();

		/// <summary>
		/// The default object serializer.
		/// </summary>
		private static Type _defaultObjectSerializer = typeof(ToStringObjectSerializer);

		/// <summary>
		/// The mapping event handler.
		/// </summary>
		private EventHandler<ColumnMappingEventArgs> _mappings;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the ColumnMapping class.
		/// </summary>
		protected ColumnMapping()
		{
			_mappings += DefaultMappingHandler;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the singleton instance of the ColumnMapping configuration for Tables and Table Valued Parameters.
		/// </summary>
		public static ColumnMapping Tables { get { return _tables; } }

		/// <summary>
		/// Gets the singleton instance of the ColumnMapping configuration for Parameters.
		/// </summary>
		public static ColumnMapping Parameters { get { return _parameters; } }

		/// <summary>
		/// Gets the singleton instance of the ColumnMapping configuration for both Tables and Parameters.
		/// </summary>
		public static ColumnMapping All { get { return _all; } }

		/// <summary>
		/// Gets the type to use to serialize objects.
		/// </summary>
		public static Type DefaultObjectSerializer { get { return _defaultObjectSerializer; } }
		#endregion

		/// <summary>
		/// Adds a column mapping handler to the chain of handlers.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public virtual ColumnMapping AddHandler(IColumnMappingHandler handler)
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

		#region Common Serialization Functions
		/// <summary>
		/// Adds a serialization handler that serializes the named field of the given type as the given mode.
		/// </summary>
		/// <param name="recordType">The type to handle.</param>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="mode">The serialization mode to use for the field of the type.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping SerializeAs(Type recordType, string fieldName, SerializationMode mode)
		{
			return AddHandler(new SerializationMappingHandler() { RecordType = recordType, FieldName = fieldName, SerializationMode = mode });
		}

		/// <summary>
		/// Adds a serialization handler that serializes the given field of the given type as the given mode.
		/// </summary>
		/// <param name="recordType">The type to handle.</param>
		/// <param name="fieldType">The type of the field.</param>
		/// <param name="mode">The serialization mode to use for the field of the type.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping SerializeAs(Type recordType, Type fieldType, SerializationMode mode)
		{
			return AddHandler(new SerializationMappingHandler() { RecordType = recordType, FieldType = fieldType, SerializationMode = mode });
		}

		/// <summary>
		/// Adds a serialization handler that serializes any field of a given type.
		/// </summary>
		/// <param name="fieldType">The type of the field.</param>
		/// <param name="mode">The serialization mode to use for the field of the type.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public ColumnMapping SerializeAs(Type fieldType, SerializationMode mode)
		{
			return AddHandler(new SerializationMappingHandler() { FieldType = fieldType, SerializationMode = mode });
		}
		#endregion

		#region Internals
		/// <summary>
		/// Reset all of the handlers to the default.
		/// </summary>
		/// <returns>The current ColumnMapping configuration.</returns>
		public virtual ColumnMapping ResetHandlers()
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
		/// <param name="command">The command that is currently being mapped.</param>
		/// <param name="parameters">The list of parameters used in the mapping operation.</param>
		/// <param name="structure">The structure of the record being read.</param>
		/// <param name="startColumn">The index of the first column to map.</param>
		/// <param name="columnCount">The number of columns to map.</param>
		/// <param name="uniqueMatches">True to only return the first match per field, false to return all matches per field.</param>
		/// <returns>An array of setters.</returns>
		internal ColumnMappingEventArgs[] CreateMapping(
			Type type,
			IDataReader reader,
			IDbCommand command,
			IList<IDataParameter> parameters,
			IRecordStructure structure,
			int startColumn,
			int columnCount,
			bool uniqueMatches)
		{
			ColumnMappingEventArgs[] mapping = new ColumnMappingEventArgs[columnCount];

			// convert the list of names into a list of set reflections
			// clone the methods list, since we are only going to use each setter once (i.e. if you return two ID columns, we will only use the first one)
			// Also, we want to do a case-insensitive lookup of the property, so convert the dictionary to an uppercase dictionary
			var setMethods = new Dictionary<string, ClassPropInfo>(ClassPropInfo.GetMappingForType(type), StringComparer.OrdinalIgnoreCase);

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
					Parameters = readOnlyParameters,
				};

				if (command != null)
				{
					e.CommandText = command.CommandText;
					e.CommandType = command.CommandType;
				}

				if (reader != null)
					e.ColumnName = reader.GetSchemaTable().Rows[i]["ColumnName"].ToString();

				lock (_lock)
				{
					_mappings(null, e);
				}

				// if no mapping was returned, then skip the column
				if (e.Canceled || String.IsNullOrEmpty(e.TargetFieldName.Trim()))
					continue;

				// if a column mapping override was specified, then attempt an override
				if (structure != null)
					structure.MapColumn(e);

				// get the target property based on the result
				string targetFieldName = e.TargetFieldName;

				// first see if there is a wildcard column, if not, then look up the field name
				ClassPropInfo setter;
				if (setMethods.TryGetValue("*", out setter) ||
					setMethods.TryGetValue(targetFieldName, out setter))
				{
					mapping[i] = e;
					e.ClassPropInfo = setter;

					InitializeMappingSerializer(e, reader, command, parameters, i);

					// remove the name from the list so we can only use it once
					if (uniqueMatches)
						setMethods.Remove(setter.ColumnName);
				}
			}

			return mapping;
		}

		/// <summary>
		/// Initialize the serializer on the given mapping.
		/// </summary>
		/// <param name="mapping">The mapping being evaluated.</param>
		/// <param name="reader">The reader being evaluated.</param>
		/// <param name="command">The command being evaluated.</param>
		/// <param name="parameters">The parameters being evaluated.</param>
		/// <param name="i">The index of the parameter being evaluated.</param>
		private static void InitializeMappingSerializer(ColumnMappingEventArgs mapping, IDataReader reader, IDbCommand command, IList<IDataParameter> parameters, int i)
		{
			// if the provider knows that this is an xml field, then automatically use that
			if ((command != null && InsightDbProvider.For(command).IsXmlParameter(command, parameters[i])) ||
				(reader != null && InsightDbProvider.For(reader).IsXmlColumn(command, reader.GetSchemaTable(), i)))
			{
				mapping.SerializationMode = SerializationMode.Xml;
				mapping.Serializer = typeof(XmlObjectSerializer);
			}
			else
			{
				mapping.SerializationMode = mapping.SerializationMode ?? mapping.ClassPropInfo.SerializationMode;

				switch (mapping.SerializationMode)
				{
					default:
					case SerializationMode.Default:
						mapping.Serializer = ColumnMapping.DefaultObjectSerializer;
						break;
					case SerializationMode.Xml:
						mapping.Serializer = typeof(XmlObjectSerializer);
						break;
					case SerializationMode.Json:
						mapping.Serializer = JsonObjectSerializer.SerializerType;
						break;
					case SerializationMode.ToString:
						mapping.Serializer = typeof(ToStringObjectSerializer);
						break;
					case SerializationMode.Custom:
						mapping.Serializer = mapping.Serializer ?? mapping.ClassPropInfo.Serializer ?? ColumnMapping.DefaultObjectSerializer;
						break;
				}
			}

			// if we allow atomic types to get a different serializer, then there are certain situations where we can't figure out the right thing to do
			if (mapping.SerializationMode != SerializationMode.Default && TypeHelper.IsAtomicType(mapping.ClassPropInfo.MemberType) && mapping.ClassPropInfo.MemberType != typeof(string))
				throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Atomic types cannot have a column serializer: {0}.{1}", mapping.ClassPropInfo.Type.Name, mapping.ClassPropInfo.Name));
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
				e.TargetFieldName = e.Parameters[e.FieldIndex].ParameterName;
			else
				throw new InvalidOperationException("DefaultMappingHandler requires either a Reader or Parameters list.");
		}
		#endregion
	}

	/// <summary>
	/// Allows for configuration of both the Parameters and Tables mappings at the same time.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	class AllColumnMapping : ColumnMapping
	{
		/// <inheritdoc/>
		public override ColumnMapping AddHandler(IColumnMappingHandler handler)
		{
			ColumnMapping.Parameters.AddHandler(handler);
			ColumnMapping.Tables.AddHandler(handler);
			return this;
		}

		/// <inheritdoc/>
		public override ColumnMapping ResetHandlers()
		{
			ColumnMapping.Parameters.ResetHandlers();
			ColumnMapping.Tables.ResetHandlers();
			return this;
		}
	}
}
