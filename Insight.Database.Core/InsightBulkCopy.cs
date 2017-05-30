using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Fired when rows have been copied to the server.
	/// </summary>
	/// <param name="sender">The sender of the event.</param>
	/// <param name="e">The event arguments.</param>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e")]
	public delegate void InsightRowsCopiedEventHandler(object sender, InsightRowsCopiedEventArgs e);

	/// <summary>
	/// Specifies the options for bulk copy. Not all values are supported by all providers.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [Flags]
	public enum InsightBulkCopyOptions
	{
		/// <summary>
		/// Default option.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Keep identity values when inserting.
		/// </summary>
		KeepIdentity = 1,

		/// <summary>
		/// Check constraints when inserting.
		/// </summary>
		CheckConstraints = 2,

		/// <summary>
		/// Lock the table while inserting.
		/// </summary>
		TableLock = 4,

		/// <summary>
		/// Preserve null values instead of applying defaults.
		/// </summary>
		KeepNulls = 8,

		/// <summary>
		/// Fire triggers when inserting rows.
		/// </summary>
		FireTriggers = 0x10,

		/// <summary>
		/// Use an internal transaction to roll back rows upon errors.
		/// </summary>
		UseInternalTransaction = 0x20,

		/// <summary>
		/// Truncate the table prior to loading.
		/// </summary>
		Truncate = 0x40,
	}

	/// <summary>
	/// Abstracts the different Bulk Copy settings for the various providers.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public abstract class InsightBulkCopy
	{
		/// <summary>
		/// Fired when a number of rows has been copied to the server.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public abstract event InsightRowsCopiedEventHandler RowsCopied;

		/// <summary>
		/// Gets or sets the number of rows to write to the server in a batch.
		/// </summary>
		public abstract int BatchSize { get; set; }

		/// <summary>
		/// Gets or sets the number of seconds to wait before timing out.
		/// </summary>
		public abstract int BulkCopyTimeout { get; set; }

		/// <summary>
		/// Gets or sets the destination table name.
		/// </summary>
		public abstract string DestinationTableName { get; set; }

		/// <summary>
		/// Gets or sets the number of rows to copy before firing the RowsCopied event.
		/// </summary>
		public abstract int NotifyAfter { get; set; }

		/// <summary>
		/// Gets the provider-specific BulkCopy object.
		/// </summary>
		public abstract object InnerBulkCopy { get; }
	}

	/// <summary>
	/// Defines the information associated with a rows copied event.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public class InsightRowsCopiedEventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether the bulk copy should be aborted.
		/// </summary>
		public virtual bool Abort { get; set; }

		/// <summary>
		/// Gets or sets the number of rows copied so far.
		/// </summary>
		public virtual long RowsCopied { get; set; }
	}
}
