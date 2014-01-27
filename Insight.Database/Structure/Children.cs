using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// An instance of Children knows how to read children into a parent.
	/// This is a one-to-many relationship.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent to read into.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	abstract class Children<TParent>
	{
		/// <summary>
		/// Read a list of children into the given list of parents.
		/// </summary>
		/// <param name="parents">The list of parents.</param>
		/// <param name="reader">The reader to read from.</param>
		public abstract void Read(IEnumerable<TParent> parents, IDataReader reader);

		/// <summary>
		/// Read a list of children into the given list of parents.
		/// </summary>
		/// <param name="parents">The list of parents.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task representing the completion of the read.</returns>
		public abstract Task ReadAsync(IEnumerable<TParent> parents, IDataReader reader, CancellationToken cancellationToken);
	}

	/// <summary>
	/// An instance of Children knows how to read children into a parent.
	/// This is a one-to-many relationship.
	/// </summary>
	/// <typeparam name="TParent">The type of the parent to read into.</typeparam>
	/// <typeparam name="TChild">The type of the child object.</typeparam>
	/// <typeparam name="TID">The type of the ID value.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	class Children<TParent, TChild, TID> : Children<TParent>
	{
		/// <summary>
		/// Can read the child records from the stream.
		/// </summary>
		private IRecordReader<Guardian<TChild, TID>> _recordReader;

		/// <summary>
		/// Can map the children into the parent in the right place.
		/// </summary>
		private IChildMapper<TParent, TChild, TID> _mapper;

		/// <summary>
		/// Initializes a new instance of the Children class.
		/// </summary>
		/// <param name="recordReader">The recordReader that can read the children from the data stream.</param>
		/// <param name="mapper">The mapper that puts the children into the parent.</param>
		public Children(IRecordReader<Guardian<TChild, TID>> recordReader, IChildMapper<TParent, TChild, TID> mapper)
		{
			_recordReader = recordReader;
			_mapper = mapper;
		}

		/// <inheritdoc/>
		public override void Read(IEnumerable<TParent> parents, IDataReader reader)
		{
			_mapper.MapChildren(parents, reader.AsEnumerable(_recordReader));
		}

		/// <inheritdoc/>
		public override Task ReadAsync(IEnumerable<TParent> parents, IDataReader reader, CancellationToken ct)
		{
#if NET35
			Read(parents, reader);
			return Helpers.FalseTask;
#else
			return reader.ToListAsync(_recordReader)
				.ContinueWith(t => _mapper.MapChildren(parents, t.Result), TaskContinuationOptions.ExecuteSynchronously);
#endif
		}
	}
}
