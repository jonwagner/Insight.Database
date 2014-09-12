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
		public virtual bool CanSerialize(Type type)
		{
			return CanDeserialize(type);
		}

		/// <inheritdoc/>
		public virtual bool CanDeserialize(Type type)
		{
			return !TypeHelper.IsAtomicType(type) && type != typeof(object);
		}

		/// <inheritdoc/>
		public virtual DbType GetSerializedDbType(Type type)
		{
			return DbType.String;
		}

		/// <inheritdoc/>
		public abstract object SerializeObject(Type type, object o);

		/// <inheritdoc/>
		public abstract object DeserializeObject(Type type, object encoded);
		#endregion
	}
}
