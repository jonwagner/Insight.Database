using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace Insight.Database.Structure
{
	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public abstract class Guardian<TChild>
	{
		/// <summary>
		/// Gets or sets the child object.
		/// </summary>
		public TChild Object { get; set; }

		/// <summary>
		/// Reads the current record from the data reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		public virtual void ReadCurrent(IDataReader reader)
		{
		}

		/// <summary>
		/// Returns the ID portion of the guardian.
		/// </summary>
		/// <returns>The ID of the guardian.</returns>
		public abstract object GetID();
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="TId">The type of the ID.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class Guardian<TChild, TId> : Guardian<TChild>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*1")]
		public TId ParentId1 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			base.ReadCurrent(reader);
			ParentId1 = (TId)reader[0];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return ParentId1;
		}
	}
}
