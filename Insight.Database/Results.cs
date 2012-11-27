using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Defines the read operation that is required to be able to read multiple recordsets from the database.
    /// </summary>
    /// <remarks>This interface is used just to allow DynamicConnection to call into the Read method.</remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    interface IDatabaseResults
    {
        /// <summary>
        /// Reads the contents from an IDataReader. Multiple recordsets are consumed from the reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        void Read(IDataReader reader);
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results : IDatabaseResults
    {
        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public virtual void Read(IDataReader reader)
        {
        }

        /// <summary>
        /// Implements the IDatabaseResult.Read operation.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        void IDatabaseResults.Read(IDataReader reader)
        {
            Read(reader);
        }
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    /// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results<T1> : Results
    {
        /// <summary>
        /// Gets the first set of data returned from the database.
        /// </summary>
        public IList<T1> Set1 { get; private set; }

        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public override void Read(IDataReader reader)
        {
            base.Read(reader);
            Set1 = reader.ToList<T1>();
        }
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    /// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
    /// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results<T1, T2> : Results<T1>
    {
        /// <summary>
        /// Gets the second set of data returned from the database.
        /// </summary>
        public IList<T2> Set2 { get; private set; }

        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public override void Read(IDataReader reader)
        {
            base.Read(reader);
            Set2 = reader.ToList<T2>();
        }
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    /// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
    /// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
    /// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results<T1, T2, T3> : Results<T1, T2>
    {
        /// <summary>
        /// Gets the third set of data returned from the database.
        /// </summary>
        public IList<T3> Set3 { get; private set; }

        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public override void Read(IDataReader reader)
        {
            base.Read(reader);
            Set3 = reader.ToList<T3>();
        }
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    /// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
    /// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
    /// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
    /// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results<T1, T2, T3, T4> : Results<T1, T2, T3>
    {
        /// <summary>
        /// Gets the fourth set of data returned from the database.
        /// </summary>
        public IList<T4> Set4 { get; private set; }

        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public override void Read(IDataReader reader)
        {
            base.Read(reader);
            Set4 = reader.ToList<T4>();
        }
    }

    /// <summary>
    /// Encapsulates multiple sets of data returned from the database.
    /// </summary>
    /// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
    /// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
    /// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
    /// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
    /// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
    public class Results<T1, T2, T3, T4, T5> : Results<T1, T2, T3, T4>
    {
        /// <summary>
        /// Gets the fifth set of data returned from the database.
        /// </summary>
        public IList<T5> Set5 { get; private set; }

        /// <summary>
        /// Reads the contents from an IDataReader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public override void Read(IDataReader reader)
        {
            base.Read(reader);
            Set5 = reader.ToList<T5>();
        }
    }
}