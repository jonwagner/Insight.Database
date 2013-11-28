using System;

namespace Insight.Database
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class SchemaAttribute : Attribute
    {
        public string Name { get; private set; }

        public SchemaAttribute(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
        }
    }
}