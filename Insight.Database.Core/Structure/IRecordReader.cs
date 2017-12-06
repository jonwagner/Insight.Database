using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
    /// <summary>
    /// An object that can read a record.
    /// It needs to be equatable so serializer caching works properly.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IRecordReader : IEquatable<IRecordReader>
    {
    }

    /// <summary>
    /// An object that can read a record of a given type.
    /// </summary>
    /// <typeparam name="T">The type of record that can be read.</typeparam>
    public interface IRecordReader<out T> : IRecordReader
    {
        /// <summary>
        /// Gets a function that can read a record from the given data reader.
        /// </summary>
        /// <param name="reader">The reader to be read from.</param>
        /// <returns>A function that can read a record.</returns>
        /// <remarks>This returns a function because each reader may have a different schema.</remarks>
        Func<IDataReader, T> GetRecordReader(IDataReader reader);
    }
}
