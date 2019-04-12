using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
    /// <summary>
    /// Marks a constructor as the preferred constructor to use when deserializing objects from the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class SqlConstructorAttribute : Attribute
    {
    }
}
