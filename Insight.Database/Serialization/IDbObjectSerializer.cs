using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Serializes objects in the database.
	/// </summary>
	public interface IDbObjectSerializer
	{
		/// <summary>
		/// Returns true if the serializer can convert the type of object to serialized form.
		/// </summary>
		/// <param name="type">The type of object.</param>
        /// <param name="dbType">The type that the database expects.</param>
		/// <returns>True if the serializer can convert the type of object to serialized form.</returns>
        bool CanSerialize(Type type, DbType dbType);

		/// <summary>
		/// Returns true if the serializer can convert the type of object from serialized form.
		/// </summary>
		/// <param name="sourceType">The type of object in the database.</param>
        /// <param name="targetType">The type of the object in the target object.</param>
		/// <returns>True if the serializer can convert the type of object from serialized form.</returns>
		bool CanDeserialize(Type sourceType, Type targetType);

		/// <summary>
		/// Returns the DbType used to serialize the given type in the database.
		/// Usually DbType.String, but could also be binary.
		/// </summary>
		/// <param name="type">The type of object.</param>
        /// <param name="dbType">The type that the database expects.</param>
		/// <returns>The DbType used to store the serialized representation.</returns>
		DbType GetSerializedDbType(Type type, DbType dbType);

		/// <summary>
		/// Serializes an object, using the type as the root type.
		/// </summary>
		/// <param name="type">The root type of object.</param>
		/// <param name="o">The object to serialize.</param>
		/// <returns>The serialized form of the object.</returns>
		object SerializeObject(Type type, object o);

		/// <summary>
		/// Deserializes an object, using the type as the root type.
		/// </summary>
		/// <param name="type">The root type of object.</param>
		/// <param name="encoded">The object to serialize.</param>
		/// <returns>The object extracted from the serialized form.</returns>
		object DeserializeObject(Type type, object encoded);
	}
}
