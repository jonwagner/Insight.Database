using System;
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
	/// Converts objects to/from a serialized form (usually string, but binary would work too).
	/// </summary>
	public abstract class DbObjectSerializer : IDbObjectSerializer
	{
		#region IDbObjectSerializer Methods
		/// <inheritdoc/>
        public virtual bool CanSerialize(Type type, DbType dbType)
        {
            if (!TypeHelper.IsDbTypeAString(dbType))
                return false;

            return true;
        }

		/// <inheritdoc/>
		public virtual bool CanDeserialize(Type sourceType, Type targetType)
		{
            return sourceType == typeof(string) && !TypeHelper.IsAtomicType(targetType) && targetType != typeof(object);
		}

		/// <inheritdoc/>
		public virtual DbType GetSerializedDbType(Type type, DbType dbType)
		{
			return DbType.String;
		}

		/// <inheritdoc/>
		public abstract object SerializeObject(Type type, object value);

		/// <inheritdoc/>
		public abstract object DeserializeObject(Type type, object encoded);
		#endregion
	}
}
