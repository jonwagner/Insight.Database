using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Specifies the format to serialize objects in the database.
	/// </summary>
	public enum SerializationMode
	{
		/// <summary>
		/// Use the default serializer.
		/// </summary>
		Default,

		/// <summary>
		/// Serialize the object as XML, using the DataContractSerializer.
		/// </summary>
		Xml,

		/// <summary>
		/// Serialize the object as JSON.
		/// </summary>
		Json,

		/// <summary>
		/// Serialize the object by calling the ToString method.
		/// </summary>
		ToString,

		/// <summary>
		/// Use a custom serializer for the column.
		/// </summary>
		Custom
	}
}
