using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Defines an interface that allows column mappings to be processed and altered.
    /// </summary>
    public interface IColumnMappingHandler
    {
        /// <summary>
        /// Handles a column mapping event.
        /// </summary>
        /// <param name="sender">The ColumnMapping object that has generated the event.</param>
        /// <param name="e">The ColumnMappingEventArgs to process.</param>
        void HandleColumnMapping(object sender, ColumnMappingEventArgs e);
    }
}
