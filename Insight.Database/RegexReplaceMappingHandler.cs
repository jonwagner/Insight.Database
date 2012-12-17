using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Handles mapping a column name to a field name by applying a Regex replace.
    /// </summary>
    /// <typeparam name="T">The type of object to apply to.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class RegexReplaceMappingHandler<T> : IColumnMappingHandler
    {
        /// <summary>
        /// The Regex to use for the mapping.
        /// </summary>
        private Regex _regex;

        /// <summary>
        /// The replacement text to use for the mapping.
        /// </summary>
        private string _replacement;

        /// <summary>
        /// Initializes a new instance of the RegexReplaceMappingHandler class.
        /// </summary>
        /// <remarks>
        /// This method uses the Regex.Replace method to alter the target field name.
        /// It supports the standard Regex replacement syntax.
        /// </remarks>
        /// <param name="regex">The regex to use to parse names.</param>
        /// <param name="replacement">The replacement string to use with the regex.</param>
        public RegexReplaceMappingHandler(string regex, string replacement)
        {
            _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _replacement = replacement;
        }

        /// <summary>
        /// Handles a column mapping event.
        /// </summary>
        /// <param name="sender">The ColumnMapping object that has generated the event.</param>
        /// <param name="e">The ColumnMappingEventArgs to process.</param>
        public void HandleColumnMapping(object sender, ColumnMappingEventArgs e)
        {
            // if we aren't mapping the current type, just continue
            if (e.TargetType != typeof(T) && !e.TargetType.IsSubclassOf(typeof(T)))
                return;

            // perform a replacement on the target field name
            e.TargetFieldName = _regex.Replace(e.TargetFieldName, _replacement);
        }
    }

    /// <summary>
    /// Handles mapping a column name to a field name by applying a Regex replace.
    /// This applies to all type of objects.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class RegexReplaceMappingHandler : RegexReplaceMappingHandler<object>
    {
        /// <summary>
        /// Initializes a new instance of the RegexReplaceMappingHandler class.
        /// </summary>
        /// <remarks>
        /// This method uses the Regex.Replace method to alter the target field name.
        /// It supports the standard Regex replacement syntax.
        /// </remarks>
        /// <param name="regex">The regex to use to parse names.</param>
        /// <param name="replacement">The replacement string to use with the regex.</param>
        public RegexReplaceMappingHandler(string regex, string replacement)
            : base(regex, replacement)
        {
        }
    }
}
