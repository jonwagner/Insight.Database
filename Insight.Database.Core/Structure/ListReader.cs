using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
    /// <summary>
    /// Reads a list of objects from a data reader.
    /// </summary>
    /// <typeparam name="T">The type of object to read from each record in the reader.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class ListReader<T> : QueryReader<T>, IQueryReader<IList<T>>
    {
        #region Fields
        /// <summary>
        /// The default reader to read a list of type T.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly IQueryReader<IList<T>> Default = new ListReader<T>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ListReader class.
        /// </summary>
        public ListReader() : this(OneToOne<T>.Records)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ListReader class.
        /// </summary>
        /// <param name="recordReader">The one-to-one mapping to use to read objects from each record.</param>
        public ListReader(IRecordReader<T> recordReader) : base(recordReader)
        {
        }
        #endregion

        #region Implementation
        /// <inheritdoc/>
        public virtual Type ReturnType { get { return typeof(IList<T>); } }

        /// <inheritdoc/>
        public virtual IList<T> Read(IDbCommand command, IDataReader reader)
        {
            IList<T> results = reader.ToList(RecordReader);
			results = MergeChildren(results);

            ReadChildren(reader, results);

            return results;
        }

        /// <inheritdoc/>
        public virtual async Task<IList<T>> ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
        {
            IList<T> results = await reader.ToListAsync(RecordReader, cancellationToken, firstRecordOnly: false);
			results = MergeChildren(results);

            await ReadChildrenAsync(reader, results, cancellationToken);

			return results;
        }

        /// <summary>
        /// Adds a child reader to this reader.
        /// </summary>
        /// <param name="child">The child reader to add.</param>
        /// <returns>A list reader that also reads the specified child.</returns>
        internal new ListReader<T> AddChild(Children<T> child)
        {
            return (ListReader<T>)base.AddChild(child);
        }
        #endregion

		#region Methods for Parent and Child Together
		private IList<T> MergeChildren(IList<T> records)
		{
			if (RecordReader.RequiresDeduplication)
				return records.Distinct().ToList();
			else
				return records;
		}
		#endregion
    }

    /// <summary>
    /// Allows ListReader to return any of the interfaces implemented by List.
    /// Used by InterfaceGenerator. Not intended to be used by user code.
    /// </summary>
    /// <typeparam name="TList">The type of list to return.</typeparam>
    /// <typeparam name="T1">The type of object to return.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class ListReaderAdapter<TList, T1> : ListReader<T1>, IQueryReader<TList>
    {
        #region Fields
        /// <summary>
        /// The default reader to read a list of type T.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly new IQueryReader<TList> Default = new ListReaderAdapter<TList, T1>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ListReaderAdapter class.
        /// </summary>
        public ListReaderAdapter() : this(OneToOne<T1>.Records)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ListReaderAdapter class.
        /// </summary>
        /// <param name="recordReader">The one-to-one mapping to use to read objects from each record.</param>
        public ListReaderAdapter(IRecordReader<T1> recordReader) : base(recordReader)
        {
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        Type IQueryReader.ReturnType { get { return typeof(TList); } }
        #endregion

        #region Methods
        /// <inheritdoc/>
        TList IQueryReader<TList>.Read(IDbCommand command, IDataReader reader)
        {
            return (TList)Read(command, reader);
        }

        /// <inheritdoc/>
        async Task<TList> IQueryReader<TList>.ReadAsync(IDbCommand command, IDataReader reader, CancellationToken ct)
        {
            return (TList)await ReadAsync(command, reader, ct);
        }
        #endregion
    }
}
