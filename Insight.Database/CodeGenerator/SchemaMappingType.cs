using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.CodeGenerator
{
    /// <summary>
    /// Defines the type of a schema mapping and therefore the signature of the delegate.
    /// </summary>
    [Flags]
    enum SchemaMappingType
    {
        /// <summary>
        /// An existing object is updated.
        /// By itself, ExistingObject creates a method of  Func&lt;IDataReader,T,T&gt;.
        /// </summary>
        ExistingObject = 0,

            /// <summary>
        /// A new object is created.
        /// By itself, NewObject creates a method of type Func&lt;IDataReader,T&gt;.
        /// </summary>
        NewObject = 1 << 0,

        /// <summary>
        /// A callback is used to assemble sub-objects.
        /// This currently cannot be used with ExistingObject.
        /// </summary>
        WithCallback = 1 << 1,

        /// <summary>
        /// A new object is created and a callback is used to assemble sub-objects.
        /// This creates a method of type Func&lt;IDataReader,Action&lt;object[]&gt;T&gt;.
        /// </summary>
        NewObjectWithCallback = NewObject | WithCallback,
    }
}
