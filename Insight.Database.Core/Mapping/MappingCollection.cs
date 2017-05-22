using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Mapping
{
	/// <summary>
	/// Allows for configuration of mapping rules.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public abstract class MappingCollection
	{
		/// <summary>
		/// Determines if the configuration allows child fields to be bound on the given type
		/// </summary>
		/// <param name="type">The type to test.</param>
		/// <returns>True if child fields can be bound.</returns>
		internal abstract bool CanBindChild(Type type);
	}

	/// <summary>
	/// Allows for configuration of mapping rules and translations.
	/// </summary>
	/// <typeparam name="TMapper">The type of mapping in the collection.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class MappingCollection<TMapper> : MappingCollection where TMapper : IMapper
	{
		#region Private Members
		/// <summary>
		/// The child binding mode that this is bound to.
		/// </summary>
		private BindChildrenFor _bindChildFor;

		/// <summary>
		/// The mapping event handler.
		/// </summary>
		private List<IMappingTransform> _transforms = new List<IMappingTransform>();

		/// <summary>
		/// The custom mappers.
		/// </summary>
		private List<TMapper> _mappers = new List<TMapper>();

		/// <summary>
		/// Tracks if child binding is enabled.
		/// </summary>
		private Dictionary<Type, bool> _childBindingEnabled = new Dictionary<Type, bool>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the MappingCollection class.
		/// </summary>
		/// <param name="bindChildFor">The context for binding child members.</param>
		internal MappingCollection(BindChildrenFor bindChildFor)
		{
			_bindChildFor = bindChildFor;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the mappers in the configuration.
		/// </summary>
		internal IEnumerable<TMapper> Mappers { get { return _mappers; } }
		#endregion

		#region Public Methods
		/// <summary>
		/// Enables binding to child fields for all objects.
		/// </summary>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public MappingCollection<TMapper> EnableChildBinding()
		{
			EnableChildBinding<object>();
			return this;
		}

		/// <summary>
		/// Disables binding to child fields for all objects.
		/// </summary>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public MappingCollection<TMapper> DisableChildBinding()
		{
			DisableChildBinding<object>();
			return this;
		}

		/// <summary>
		/// Enables binding to child fields objects that derive from the given type.
		/// </summary>
		/// <typeparam name="T">The type to enable.</typeparam>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> EnableChildBinding<T>()
		{
			_childBindingEnabled[typeof(T)] = true;
			return this;
		}

		/// <summary>
		/// Disables binding to child fields objects that derive from the given type.
		/// </summary>
		/// <typeparam name="T">The type to disable.</typeparam>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> DisableChildBinding<T>()
		{
			_childBindingEnabled[typeof(T)] = false;
			return this;
		}

		/// <summary>
		/// Resets the child binding configuration. Intended to be used for test purposes.
		/// </summary>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> ResetChildBinding()
		{
			_childBindingEnabled.Clear();
			return this;
		}

		/// <summary>
		/// Adds a mapping transformation to the configuration.
		/// </summary>
		/// <param name="transform">The transformation to add.</param>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> AddTransform(IMappingTransform transform)
		{
			_transforms.Add(transform);
			return this;
		}

		/// <summary>
		/// Resets the mapping transformations. Intended to be used for test purposes.
		/// </summary>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> ResetTransforms()
		{
			_transforms.Clear();
			return this;
		}

		/// <summary>
		/// Adds a custom mapper to the configuration.
		/// </summary>
		/// <param name="mapper">The mapper to add.</param>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> AddMapper(TMapper mapper)
		{
			_mappers.Add(mapper);
			return this;
		}

		/// <summary>
		/// Resets the custom mappings. Intended to be used for test purposes.
		/// </summary>
		/// <returns>The current mapping collection, for fluent configuration.</returns>
		public virtual MappingCollection<TMapper> ResetMappers()
		{
			_mappers.Clear();
			return this;
		}
		#endregion

		#region Common Replacement Functions
		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text of the regex.</param>
		/// <param name="replacement">The text to use as a replacement.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> ReplaceRegex<T>(string pattern, string replacement)
		{
			return AddTransform(new RegexReplaceTransform<T>(pattern, replacement));
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// This applies to all types of objects.
		/// </summary>
		/// <param name="pattern">The text of the regex.</param>
		/// <param name="replacement">The text to use as a replacement.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> ReplaceRegex(string pattern, string replacement)
		{
			return ReplaceRegex<object>(pattern, replacement);
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text of the regex.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveRegex<T>(string pattern)
		{
			return AddTransform(new RegexReplaceTransform<T>(pattern, String.Empty));
		}

		/// <summary>
		/// Adds a removal operation that uses a Regex to determine the text to remove.
		/// This applies to all types of objects.
		/// </summary>
		/// <param name="text">The text of the regex.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveRegex(string text)
		{
			return RemoveRegex<object>(text);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="pattern">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveStrings<T>(string pattern)
		{
			return AddTransform(new RegexReplaceTransform<T>(pattern, String.Empty));
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="pattern">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveStrings(string pattern)
		{
			return RemoveStrings<object>(pattern);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the beginning of a column name.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="prefix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemovePrefixes<T>(string prefix)
		{
			return RemoveStrings("^" + prefix);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the beginning of a column name.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="prefix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemovePrefixes(string prefix)
		{
			return RemovePrefixes<object>(prefix);
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the end of a column name.
		/// </summary>
		/// <typeparam name="T">The type of objects to apply this operation to.</typeparam>
		/// <param name="suffix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveSuffixes<T>(string suffix)
		{
			return RemoveStrings(suffix + "$");
		}

		/// <summary>
		/// Adds a removal operation that replaces any occurence of a specified string at the end of a column name.
		/// </summary>
		/// This applies to all types of objects.
		/// <param name="suffix">The text to remove from the column names.</param>
		/// <returns>The current ColumnMapping configuration.</returns>
		public MappingCollection<TMapper> RemoveSuffixes(string suffix)
		{
			return RemoveSuffixes<object>(suffix);
		}
		#endregion

		/// <inheritdoc/>
		internal override bool CanBindChild(Type type)
		{
			for (; type != null; type = type.BaseType)
			{
				// attributes on the type override any other configuration
				var bindAttribute = type.GetCustomAttributes(typeof(BindChildrenAttribute), true).OfType<BindChildrenAttribute>().FirstOrDefault();
				if (bindAttribute != null)
					return bindAttribute.For.HasFlag(_bindChildFor);

				bool enabled;
				if (_childBindingEnabled.TryGetValue(type, out enabled))
					return enabled;
			}

			// default to deep binding off unless someone has enabled it
			return false;
		}

		/// <inheritdoc/>
		internal string Transform(Type type, string parameterName)
		{
			return _transforms.Aggregate(parameterName, (n, t) => t.TransformDatabaseName(type, n));
		}
	}
}
