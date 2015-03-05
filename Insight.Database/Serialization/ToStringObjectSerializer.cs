using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Providers;

namespace Insight.Database
{
	/// <summary>
	/// Serializes objects to strings. Does not deserialize.
	/// </summary>
	class ToStringObjectSerializer : DbObjectSerializer
	{
		/// <summary>
		/// The singleton Serializer.
		/// </summary>
		public static readonly ToStringObjectSerializer Serializer = new ToStringObjectSerializer();

		/// <inheritdoc/>
		public override bool CanSerialize(Type type, DbType dbType)
		{
            // no point in converting convertibles or enumerables
            if (type.GetInterfaces().Contains(typeof(IConvertible)) || type == typeof(object))
                return false;
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return false;
            if (type == TypeHelper.LinqBinaryType)
                return false;

            // never try to convert an atomic type
            if (TypeHelper.IsAtomicType(type))
                return false;

			if (typeof(IEnumerable).IsAssignableFrom(type))
				return false;

			return true;
		}

		/// <inheritdoc/>
		public override bool CanDeserialize(Type sourceType, Type targetType)
		{
			return false;
		}

        /// <inheritdoc/>
        public override DbType GetSerializedDbType(Type type, DbType dbType)
        {
            return DbType.String;
        }

		/// <inheritdoc/>
		public override object SerializeObject(Type type, object o)
		{
			return o.ToString();
		}

		/// <inheritdoc/>
		public override object DeserializeObject(Type type, object o)
		{
			return o;
		}
	}
}
