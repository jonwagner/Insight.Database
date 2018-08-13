using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
	/// <summary>
	/// Serializes objects to XML using the DataContractSerializer.
	/// </summary>
	public class XmlObjectSerializer : DbObjectSerializer
	{
		/// <summary>
		/// The singleton Serializer.
		/// </summary>
		public static readonly XmlObjectSerializer Serializer = new XmlObjectSerializer();

        /// <inheritdoc/>
        public override bool CanSerialize(Type type, DbType dbType)
        {
            return TypeHelper.IsDbTypeAString(dbType) || dbType == DbType.Xml;
        }

        /// <inheritdoc/>
        public override DbType GetSerializedDbType(Type type, DbType dbType)
        {
            return DbType.Xml;
        }

		/// <inheritdoc/>
		public override object SerializeObject(Type type, object value)
		{
			if (value == null)
				return null;

			if (type == null)
				type = value.GetType();

			// don't double-encode strings. assume the string is xml.
			if (type == typeof(string))
				return (string)value;

			// serialize the parameters
			StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
			StringWriter disposable = sw;
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.OmitXmlDeclaration = true;
				using (XmlWriter xw = XmlWriter.Create(sw, settings))
				{
					disposable = null;
					new DataContractSerializer(type).WriteObject(xw, value);
				}

				return sw.ToString();
			}
			finally
			{
				if (disposable != null)
					disposable.Dispose();
			}
		}

		/// <inheritdoc/>
		public override object DeserializeObject(Type type, object encoded)
		{
			DataContractSerializer serializer = new DataContractSerializer(type);

			StringReader reader = new StringReader((string)encoded);
			try
			{
				using (XmlReader xr = XmlReader.Create(reader))
				{
					reader = null;
					return serializer.ReadObject(xr);
				}
			}
			finally
			{
				if (reader != null)
					reader.Dispose();
			}
		}
	}
}
