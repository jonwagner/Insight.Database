using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;

namespace Insight.Database
{
	/// <summary>
	/// Defines a serialization rule.
	/// </summary>
	public class DbSerializationRule : IDbSerializationRule
	{
		#region Static Members
		/// <summary>
		/// The list of serialization handlers.
		/// </summary>
		private static List<IDbSerializationRule> _handlers = new List<IDbSerializationRule>();

		/// <summary>
		/// The cached serializers.
		/// </summary>
		private static ConcurrentDictionary<Type, IDbObjectSerializer> _cachedSerializers = new ConcurrentDictionary<Type, IDbObjectSerializer>();

		/// <summary>
		/// The default serialization handler.
		/// </summary>
		private static IDbSerializationRule _defaultConfig = new DbSerializationRule();
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the record type to match. If null, then all record types are matched.
		/// </summary>
		public Type RecordType { get; set; }

		/// <summary>
		/// Gets or sets the name of the field to match. If null, then fields of any name are matched.
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// Gets or sets the type of the column to match. If null, then fields of any type are matched.
		/// </summary>
		public Type FieldType { get; set; }

		/// <summary>
		/// Gets or sets the serialization mode to use for the any matched columns.
		/// </summary>
		public SerializationMode? Mode { get; set; }

		/// <summary>
		/// Gets or sets the serializer that is used when the rule matches.
		/// </summary>
		public IDbObjectSerializer Serializer { get; set; }
		#endregion

		#region Public Configuration Methods
		/// <summary>
		/// Clears all of the serialization rules. Existing bindings are not reset. Use this primarily for testing.
		/// </summary>
		public static void ResetRules()
		{
			_handlers = new List<IDbSerializationRule>();
		}

		/// <summary>
		/// Tells Insight the SerializationMode to use when serializing all instances of type T.
		/// </summary>
		/// <typeparam name="T">The type that is being serialized.</typeparam>
		/// <param name="mode">The SerializationMode to use.</param>
		public static void Serialize<T>(SerializationMode mode)
		{
			AddRule(new DbSerializationRule() { FieldType = typeof(T), Mode = mode });
		}

		/// <summary>
		/// Tells Insight the custom serializer to use when serializing all instances of type T.
		/// </summary>
		/// <typeparam name="T">The type that is being serialized.</typeparam>
		/// <param name="serializer">The serializer to use.</param>
		public static void Serialize<T>(IDbObjectSerializer serializer)
		{
			AddRule(new DbSerializationRule() { FieldType = typeof(T), Mode = SerializationMode.Custom, Serializer = serializer });
		}

		/// <summary>
		/// Tells Insight the SerializationMode to use when serializing the given member of type T.
		/// </summary>
		/// <typeparam name="T">The type containing the object to be serialized.</typeparam>
		/// <param name="fieldType">The type of the field.</param>
		/// <param name="mode">The SerializationMode to use.</param>
		public static void Serialize<T>(Type fieldType, SerializationMode mode)
		{
			AddRule(new DbSerializationRule() { RecordType = typeof(T), FieldType = fieldType, Mode = mode });
		}

		/// <summary>
		/// Tells Insight the SerializationMode to use when serializing the given member of type T.
		/// </summary>
		/// <typeparam name="T">The type containing the object to be serialized.</typeparam>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="mode">The SerializationMode to use.</param>
		public static void Serialize<T>(string fieldName, SerializationMode mode)
		{
			AddRule(new DbSerializationRule() { RecordType = typeof(T), FieldName = fieldName, Mode = mode });
		}

		/// <summary>
		/// Tells Insight the SerializationMode to use when serializing the given member of type T.
		/// </summary>
		/// <typeparam name="T">The type containing the object to be serialized.</typeparam>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="serializer">The serializer to use.</param>
		public static void Serialize<T>(string fieldName, IDbObjectSerializer serializer)
		{
			AddRule(new DbSerializationRule() { RecordType = typeof(T), FieldName = fieldName, Mode = SerializationMode.Custom, Serializer = serializer });
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adds a serialization rule to the configuration.
		/// </summary>
		/// <param name="rule">The serialization rule.</param>
		public static void AddRule(IDbSerializationRule rule)
		{
			_handlers.Add(rule);
		}

		/// <inheritdoc/>
		IDbObjectSerializer IDbSerializationRule.GetSerializer(Type recordType, Type memberType, string memberName)
		{
			if (FieldName != null && String.Compare(FieldName, memberName, StringComparison.OrdinalIgnoreCase) != 0)
				return null;

			if (RecordType != null && recordType != RecordType)
				return null;

			if (FieldType != null && memberType != FieldType)
				return null;

			var prop = ClassPropInfo.GetMemberByName(recordType, memberName);
			var mode = Mode ?? prop.SerializationMode;

			switch (mode)
			{
				default:
				case SerializationMode.Default:
				case SerializationMode.ToString:
					return ToStringObjectSerializer.Serializer;

				case SerializationMode.Xml:
					return XmlObjectSerializer.Serializer;

				case SerializationMode.Json:
					return JsonObjectSerializer.Serializer;

				case SerializationMode.Custom:
					if (Serializer != null)
						return Serializer;

					if (prop.Serializer == null)
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "No custom serializer was provided for {0} on type {1}", prop.Name, prop.Type));

					return _cachedSerializers.GetOrAdd(prop.Serializer, (serializer) => (IDbObjectSerializer)System.Activator.CreateInstance(serializer));
			}
		}
		#endregion

		#region Private Members
		/// <summary>
		/// Gets the serializer for a specific parameter.
		/// </summary>
		/// <param name="command">The command that is being bound.</param>
		/// <param name="parameter">The parameter that is being bound.</param>
		/// <param name="prop">The property that is being bound.</param>
		/// <returns>The serializer.</returns>
		internal static IDbObjectSerializer GetSerializer(IDbCommand command, IDataParameter parameter, ClassPropInfo prop)
		{
			if (InsightDbProvider.For(parameter).IsXmlParameter(command, parameter))
				return GetCustomSerializer(prop) ?? XmlObjectSerializer.Serializer;

			return EvaluateRules(prop);
		}

		/// <summary>
		/// Gets the serializer for a specific parameter.
		/// </summary>
		/// <param name="reader">The reader that is being bound.</param>
		/// <param name="column">The index of the column that is being bound.</param>
		/// <param name="prop">The property that is being bound.</param>
		/// <returns>The serializer.</returns>
		internal static IDbObjectSerializer GetSerializer(IDataReader reader, int column, ClassPropInfo prop)
		{
			if (InsightDbProvider.For(reader).IsXmlColumn(reader, column))
				return GetCustomSerializer(prop) ?? XmlObjectSerializer.Serializer;

			return EvaluateRules(prop);
		}

		/// <summary>
		/// Gets the serializer for a specific property.
		/// </summary>
		/// <param name="prop">The property that is being bound.</param>
		/// <returns>The serializer.</returns>
		internal static IDbObjectSerializer EvaluateRules(ClassPropInfo prop)
		{
			return GetCustomSerializer(prop) ?? _defaultConfig.GetSerializer(prop.Type, prop.MemberType, prop.Name);
		}

		/// <summary>
		/// Gets the custom serializer.
		/// </summary>
		/// <param name="prop">The property that is being bound.</param>
		/// <returns>The serializer.</returns>
		internal static IDbObjectSerializer GetCustomSerializer(ClassPropInfo prop)
		{
			return _handlers.Select(h => h.GetSerializer(prop.Type, prop.MemberType, prop.Name)).Where(s => s != null).FirstOrDefault();
		}
		#endregion
	}
}
