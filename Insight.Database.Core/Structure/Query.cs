using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
    /// <summary>
    /// Extension methods that build an object that can read results from a data reader.
    /// </summary>
    public static partial class Query
    {
        #region Returns Methods
        /// <summary>
        /// Defines a reader that returns a list of records.
        /// </summary>
        /// <typeparam name="T">The type of object in the list of records.</typeparam>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <returns>A reader that reads a list of objects.</returns>
        public static ListReader<T> Returns<T>(IRecordReader<T> recordReader = null)
        {
            return new ListReader<T>(recordReader ?? OneToOne<T>.Records);
        }

        /// <summary>
        /// Defines a reader that returns a Results object.
        /// </summary>
        /// <typeparam name="T">The type of object in the first set of results.</typeparam>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <returns>A reader that reads a Results object.</returns>
        public static ResultsReader<T> ReturnsResults<T>(IRecordReader<T> recordReader = null)
        {
            return new ResultsReader<T>(recordReader ?? OneToOne<T>.Records);
        }

        /// <summary>
        /// Defines a reader that returns a single record.
        /// </summary>
        /// <typeparam name="T">The type of object in the list of records.</typeparam>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <returns>A reader that reads a list of objects.</returns>
        public static SingleReader<T> ReturnsSingle<T>(IRecordReader<T> recordReader = null)
        {
            return new SingleReader<T>(recordReader ?? OneToOne<T>.Records);
        }
        #endregion

        #region Then Methods
        /// <summary>
        /// Extends the reader by reading another set of records.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <returns>A reader that reads a Results object with multiple results.</returns>
        public static ResultsReader<T1, T2> Then<T1, T2>(this ListReader<T1> previous, IRecordReader<T2> recordReader = null)
        {
            return new ResultsReader<T1>(previous).Then(recordReader);
        }
        #endregion

        #region ThenChildren Methods
        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ListReader<T1> ThenChildren<T1, T2>(
            this ListReader<T1> previous,
            RecordReader<T2> recordReader,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<T1, object>(), null, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        public static ListReader<T1> ThenChildren<T1, T2, TId>(
            this ListReader<T1> previous,
            RecordReader<T2> recordReader,
            Func<T1, TId> id,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<T1, TId>(), id, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ListReader<T1> ThenChildren<T1, T2, TId>(
            this ListReader<T1> previous,
            IChildRecordReader<T2, TId> recordReader,
            Func<T1, TId> id = null,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<T1, T2, TId>(recordReader, new ChildMapper<T1, T2, TId>(id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<T1> ThenChildren<T1, T2>(
            this SingleReader<T1> previous,
            IRecordReader<T2> recordReader,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new SingleChildren<T1, T2>(recordReader, new SingleChildMapper<T1, T2>(into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<T1> ThenChildren<T1, T2, TId>(
            this SingleReader<T1> previous,
            RecordReader<T2> recordReader,
            Func<T1, TId> id,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<T1, TId>(), id, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects and their children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<T1> ThenChildren<T1, T2, TId>(
            this SingleReader<T1> previous,
            IChildRecordReader<T2, TId> recordReader,
            Func<T1, TId> id = null,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<T1, T2, TId>(recordReader, new ChildMapper<T1, T2, TId>(id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results object with child records.</returns>
        public static ResultsReader<T1> ThenChildren<T1, T2>(
            this ResultsReader<T1> previous,
            RecordReader<T2> recordReader,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<T1, object>(), null, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results object with child records.</returns>
        public static ResultsReader<T1> ThenChildren<T1, T2, TId>(
            this ResultsReader<T1> previous,
            RecordReader<T2> recordReader,
            Func<T1, TId> id,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<T1, TId>(), id, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of the previous results.
        /// </summary>
        /// <typeparam name="T1">The type of objects in the first set of results.</typeparam>
        /// <typeparam name="T2">The type of objects in the second set of results.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="id">An optional function that extracts an ID from the object. Use when this row is a parent in a parent-child relationship.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results object with child records.</returns>
        public static ResultsReader<T1> ThenChildren<T1, T2, TId>(
            this ResultsReader<T1> previous,
            IChildRecordReader<T2, TId> recordReader,
            Func<T1, TId> id = null,
            Action<T1, List<T2>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<T1, T2, TId>(recordReader, new ChildMapper<T1, T2, TId>(id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ListReader<TRoot> ThenChildren<TRoot, TParent, TChild>(
            this ListReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<TParent, object>(), parents, null, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ListReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this ListReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<TParent, TId>(), parents, id, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ListReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this ListReader<TRoot> previous,
            IChildRecordReader<TChild, TId> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id = null,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<TRoot, TChild, TId>(recordReader, new ChildMapper<TRoot, TParent, TChild, TId>(parents, id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results objects with children.</returns>
        public static ResultsReader<TRoot> ThenChildren<TRoot, TParent, TChild>(
            this ResultsReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<TParent, object>(), parents, null, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results objects with children.</returns>
        public static ResultsReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this ResultsReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<TParent, TId>(), parents, id, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a Results objects with children.</returns>
        public static ResultsReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this ResultsReader<TRoot> previous,
            IChildRecordReader<TChild, TId> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id = null,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<TRoot, TChild, TId>(recordReader, new ChildMapper<TRoot, TParent, TChild, TId>(parents, id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<TRoot> ThenChildren<TRoot, TParent, TChild>(
            this SingleReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.ThenChildren(recordReader.GroupByAuto<TParent, object>(), parents, null, into);
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this SingleReader<TRoot> previous,
            RecordReader<TChild> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<TRoot, TChild, TId>(recordReader.GroupByAuto<TParent, TId>(), new ChildMapper<TRoot, TParent, TChild, TId>(parents, id, into)));
        }

        /// <summary>
        /// Extends the reader by reading another set of records that are children of a subobject of the previous results.
        /// </summary>
        /// <typeparam name="TRoot">The type of the root object that is returned.</typeparam>
        /// <typeparam name="TParent">The type of the parent object in the parent-child relationship.</typeparam>
        /// <typeparam name="TChild">The type of the child in the parent-child relationship.</typeparam>
        /// <typeparam name="TId">The type of the ID value.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="recordReader">The mapping that defines the layout of the records in each row.</param>
        /// <param name="parents">A function that selects the list of parents from the root object.</param>
        /// <param name="id">A function that selects the ID from a parent.</param>
        /// <param name="into">A function that assigns the children to their parent.</param>
        /// <returns>A reader that reads a list of objects with children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SingleReader<TRoot> ThenChildren<TRoot, TParent, TChild, TId>(
            this SingleReader<TRoot> previous,
            IChildRecordReader<TChild, TId> recordReader,
            Func<TRoot, IEnumerable<TParent>> parents,
            Func<TParent, TId> id = null,
            Action<TParent, List<TChild>> into = null)
        {
            if (previous == null) throw new ArgumentNullException("previous");
            if (recordReader == null) throw new ArgumentNullException("recordReader");

            return previous.AddChild(new Children<TRoot, TChild, TId>(recordReader, new ChildMapper<TRoot, TParent, TChild, TId>(parents, id, into)));
        }
        #endregion

        #region SelfReferencing Methods
        /// <summary>
        /// Evaluates parent/child relationships in a top-level result set.
        /// </summary>
        /// <typeparam name="T">The type of object that is returned.</typeparam>
        /// <param name="previous">The previous reader.</param>
        /// <param name="id">An optional function that selects the ID from the object.</param>
        /// <param name="parentId">An optional function that selects the ParentID from the object.</param>
        /// <param name="into">A function that assigns the parent to the child.</param>
        /// <returns>A reader that reads a Results objects with children.</returns>
        public static ListReader<T> SelfReferencing<T>(
			this ListReader<T> previous,
            Func<T, Object> id = null,
            Func<T, Object> parentId = null,
            Action<T, T> into = null)
        {
            return new SelfReferencingListReader<T>(previous, id, parentId, into);
        }
        #endregion

        #region OneToOne Methods
        /// <summary>
        /// Gets the onetoone mapping type for a list of types.
        /// This method is not intended to be used by user code.
        /// </summary>
        /// <param name="types">The list of types to convert.</param>
        /// <returns>The OneToOne mapping.</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine")]
        internal static Type GetOneToOneType(Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");

            Type oneToOne = null;
            switch (types.Length)
            {
                case 1: oneToOne = typeof(OneToOne<>); break;
                case 2: oneToOne = typeof(OneToOne<,>); break;
                case 3: oneToOne = typeof(OneToOne<,,>); break;
                case 4: oneToOne = typeof(OneToOne<,,,>); break;
                case 5: oneToOne = typeof(OneToOne<,,,,>); break;
                case 6: oneToOne = typeof(OneToOne<,,,,,>); break;
                case 7: oneToOne = typeof(OneToOne<,,,,,,>); break;
                case 8: oneToOne = typeof(OneToOne<,,,,,,,>); break;
                case 9: oneToOne = typeof(OneToOne<,,,,,,,,>); break;
                case 10: oneToOne = typeof(OneToOne<,,,,,,,,,>); break;
                case 11: oneToOne = typeof(OneToOne<,,,,,,,,,,>); break;
                case 12: oneToOne = typeof(OneToOne<,,,,,,,,,,,>); break;
                case 13: oneToOne = typeof(OneToOne<,,,,,,,,,,,,>); break;
                case 14: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,>); break;
                case 15: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,,>); break;
                case 16: oneToOne = typeof(OneToOne<,,,,,,,,,,,,,,,>); break;
            }

            return oneToOne.MakeGenericType(types);
        }
        #endregion
    }
}
