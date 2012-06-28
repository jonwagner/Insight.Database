using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Insight.Database.CodeGenerator;
using Insight.Database.Reliable;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// A code generator to create methods to serialize an object into sql parameters.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class DbParameterGenerator
	{
		#region Private Members
		/// <summary>
		/// Special case for enumerable data types.
		/// </summary>
		private const DbType DbTypeEnumerable = (DbType)(-1);

		/// <summary>
		/// MethodInfos for methods that we are going to call.
		/// </summary>
		private static readonly FieldInfo _dbNullValue = typeof(DBNull).GetField("Value");
		private static readonly MethodInfo _iDbCommandGetParameters = typeof(IDbCommand).GetProperty("Parameters").GetGetMethod();
		private static readonly MethodInfo _iDbCommandCreateParameter = typeof(IDbCommand).GetMethod("CreateParameter");
		private static readonly MethodInfo _iDataParameterSetParameterName = typeof(IDataParameter).GetProperty("ParameterName").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetDbType = typeof(IDataParameter).GetProperty("DbType").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetValue = typeof(IDataParameter).GetProperty("Value").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetSize = typeof(IDbDataParameter).GetProperty("Size").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetDirection = typeof(IDataParameter).GetProperty("Direction").GetSetMethod();
		private static readonly MethodInfo _iListAdd = typeof(IList).GetMethod("Add");
		private static readonly MethodInfo _stringGetLength = typeof(string).GetProperty("Length").GetGetMethod();
		private static readonly MethodInfo _linqBinaryToArray = typeof(System.Data.Linq.Binary).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _listParameterHelperAddEnumerableParameters = typeof(ListParameterHelper).GetMethod("AddEnumerableParameters");
		private static readonly MethodInfo _typeGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

		/// <summary>
		/// Mapping from object types to DbTypes.
		/// </summary>
		private static Dictionary<Type, DbType> _typeToDbTypeMap = new Dictionary<Type, DbType>()
		{
			{ typeof(byte), DbType.Byte },
			{ typeof(sbyte), DbType.SByte },
			{ typeof(short), DbType.Int16 },
			{ typeof(ushort), DbType.UInt16 },
			{ typeof(int), DbType.Int32 },
			{ typeof(uint), DbType.UInt32 },
			{ typeof(long), DbType.Int64 },
			{ typeof(ulong), DbType.UInt64 },
			{ typeof(float), DbType.Single },
			{ typeof(double), DbType.Double },
			{ typeof(decimal), DbType.Decimal },
			{ typeof(bool), DbType.Boolean },
			{ typeof(string), DbType.String },
			{ typeof(char), DbType.StringFixedLength },
			{ typeof(Guid), DbType.Guid },
			{ typeof(DateTime), DbType.DateTime },
			{ typeof(DateTimeOffset), DbType.DateTimeOffset },
			{ typeof(byte[]), DbType.Binary },
			{ typeof(byte?), DbType.Byte }, 
			{ typeof(sbyte?), DbType.SByte },
			{ typeof(short?), DbType.Int16 },
			{ typeof(ushort?), DbType.UInt16 },
			{ typeof(int?), DbType.Int32 },
			{ typeof(uint?), DbType.UInt32 },
			{ typeof(long?), DbType.Int64 },
			{ typeof(ulong?), DbType.UInt64 },
			{ typeof(float?), DbType.Single },
			{ typeof(double?), DbType.Double },
			{ typeof(decimal?), DbType.Decimal },
			{ typeof(bool?), DbType.Boolean }, 
			{ typeof(char?), DbType.StringFixedLength }, 
			{ typeof(Guid?), DbType.Guid },
			{ typeof(DateTime?), DbType.DateTime },
			{ typeof(DateTimeOffset?), DbType.DateTimeOffset },
			{ typeof(System.Data.Linq.Binary), DbType.Binary },
		};

		/// <summary>
		/// A map from dbtypes to underlying system types.
		/// </summary>
		private static Dictionary<DbType, Type> _dbTypeToTypeMap = new Dictionary<DbType, Type>()
		{
			{ DbType.Byte, typeof(byte) },
			{ DbType.SByte, typeof(sbyte) },
			{ DbType.Int16, typeof(short) },
			{ DbType.UInt16, typeof(ushort) },
			{ DbType.Int32, typeof(int) },
			{ DbType.UInt32, typeof(uint) },
			{ DbType.Int64, typeof(long) },
			{ DbType.UInt64, typeof(ulong) },
			{ DbType.Single, typeof(float) },
			{ DbType.Double, typeof(double) },
			{ DbType.Decimal, typeof(decimal) },
			{ DbType.Boolean, typeof(bool) },
			{ DbType.String, typeof(string) },
			{ DbType.StringFixedLength, typeof(char) },
			{ DbType.Guid, typeof(Guid) },
			{ DbType.DateTime, typeof(DateTime) },
			{ DbType.DateTimeOffset, typeof(DateTimeOffset) },
			{ DbType.Binary, typeof(byte[]) },
		};

		/// <summary>
		/// The cache for the serializers.
		/// </summary>
		private static ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>> _serializers = new ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>>();

		/// <summary>
		/// The cache for reflection.
		/// </summary>
		private static ConcurrentDictionary<Type, Dictionary<string, ClassPropInfo>> _getMethods = new ConcurrentDictionary<Type, Dictionary<string, ClassPropInfo>>();

		/// <summary>
		/// Regex to detect parameters in sql text.
		/// </summary>
		private static Regex _parameterRegex = new Regex("[?@:]([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static Regex _parameterPrefixRegex = new Regex("^[?@:]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		#endregion

		#region Code Cache Members
		/// <summary>
		/// Get the parameter generator method for a command and the type used for the parameter.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>The command generator.</returns>
		public static Action<IDbCommand, object> GetParameterGenerator(IDbCommand command, Type type)
		{
			QueryIdentity identity = new QueryIdentity(command, type);

			// try to get the deserializer. if not found, create one.
			if (type.IsSubclassOf(typeof(DynamicObject)))
				return _serializers.GetOrAdd(identity, key => CreateDynamicParameterGenerator(command));
			else
				return _serializers.GetOrAdd(identity, key => CreateClassParameterGenerator(command, type));
		}
		#endregion

		#region Parameter Detection Members
		/// <summary>
		/// Derive the parameters for a command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		/// <returns>The list of parameter names.</returns>
		private static List<SqlParameter> DeriveParameters(IDbCommand command)
		{
			string sql = command.CommandText;

			// for SqlServer stored procs, we can call the server to derive them
			if (command.CommandType == CommandType.StoredProcedure)
			{
				// if we have a SqlCommand, use it
				SqlCommand sqlCommand = command.UnwrapSqlCommand();
				if (sqlCommand != null)
					return DeriveParametersFromSqlProcedure(sqlCommand);
			}

			// otherwise we have to look at the text
			// the text can even contain the parameters in a comment (e.g. "myproc -- @p, @q")
			return DeriveParametersFromSqlText(sql);
		}

		/// <summary>
		/// Derive the list of parameters from the stored procedure.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		/// <returns>The list of stored procedures.</returns>
		private static List<SqlParameter> DeriveParametersFromSqlProcedure(SqlCommand command)
		{
			// call the server to get the parameters
			SqlCommandBuilder.DeriveParameters(command);

			// make the list of parameters
			List<SqlParameter> parameters = command.Parameters.Cast<SqlParameter>().ToList();
			parameters.ForEach(p => p.ParameterName = _parameterPrefixRegex.Replace(p.ParameterName, String.Empty).ToUpperInvariant());

			// clear the list so we can re-add them
			command.Parameters.Clear();

			return parameters;
		}

		/// <summary>
		/// Get a list of parameters from Sql text.
		/// </summary>
		/// <param name="sql">The sql to scan.</param>
		/// <returns>A list of the detected parameters.</returns>
		private static List<SqlParameter> DeriveParametersFromSqlText(string sql)
		{
			return _parameterRegex.Matches(sql).Cast<Match>().Select(m => new SqlParameter(m.Groups[1].Value.ToUpperInvariant(), null)).ToList();
		}
		#endregion

		#region Property Detection Members
		/// <summary>
		/// Get the gettable fields and properties of the type.
		/// </summary>
		/// <param name="type">The type that we are working on.</param>
		/// <returns>The list of gettable properties.</returns>
		private static Dictionary<string, ClassPropInfo> GetProperties(Type type)
		{
			return _getMethods.GetOrAdd(
				type,
				key =>
				{
					Dictionary<string, ClassPropInfo> getMethods = new Dictionary<string, ClassPropInfo>();

					// get the get properties for the types that we pass in
					// get all fields in the class
					foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						getMethods.Add(f.Name.ToUpperInvariant(), new ClassPropInfo(type, f.Name, getMethod: true));

					// get all properties in the class
					foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						getMethods.Add(p.Name.ToUpperInvariant(), new ClassPropInfo(type, p.Name, getMethod: true));

					return getMethods;
				});
		}
		#endregion

		#region Code Generation Members
		/// <summary>
		/// Create the Parameter generator method.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of object to parameterize.</param>
		/// <returns>A method that serializes parameters to values.</returns>
		static Action<IDbCommand, object> CreateClassParameterGenerator(IDbCommand command, Type type)
		{
			// get the parameters
			List<SqlParameter> parameters = DeriveParameters(command);

			// create a dynamic method
			Type typeOwner = type.HasElementType ? type.GetElementType() : type;

			// special case if the parameters object is an IEnumerable or Array
			// look for the parameter that is a Structured object and pass the array to the TVP
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				// If this is an IEnumerable<T>, then grab the contents type from the type argument
				Type[] typeArgs = type.GetGenericArguments();
				if (typeArgs != null && typeArgs.Length == 1)
					typeOwner = typeArgs[0];

				SqlParameter sqlParameter = parameters.Find(p => p.SqlDbType == SqlDbType.Structured);
				string parameterName = sqlParameter.ParameterName;
				string tableTypeName = sqlParameter.TypeName;
				if (tableTypeName.Count(c => c == '.') > 1)
					tableTypeName = tableTypeName.Split(new char[] { '.' }, 2)[1];

				return (IDbCommand cmd, object o) => { ListParameterHelper.AddEnumerableParameters(cmd, parameterName, tableTypeName, typeOwner, o); };
			}

			// start creating a dynamic method
			var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "CreateParameters-{0}", Guid.NewGuid()), null, new[] { typeof(IDbCommand), typeof(object) }, typeOwner, true);
			var il = dm.GetILGenerator();

			// start the standard serialization method
			il.DeclareLocal(typeof(Int32));											// loc.0 = string.length
			il.Emit(OpCodes.Ldarg_0);												// push arg.0 (command), stack => [command]
			il.EmitCall(OpCodes.Callvirt, _iDbCommandGetParameters, null);			// call getparams, stack => [parameters]

			foreach (var prop in GetProperties(type))
			{
				// if there is no parameter for this property, then skip it
				SqlParameter sqlParameter = parameters.Find(p => p.ParameterName == prop.Key);
				if (sqlParameter == null)
					continue;

				// look up the type of the parameters
				DbType sqlType = LookupDbType(prop.Value.MemberType, sqlParameter.DbType);

				// stack => [parameters]

				///////////////////////////////////////////////////////////////
				// Special case support for enumerables. If the type is -1 (our workaround, then call the list parameter method)
				///////////////////////////////////////////////////////////////
				if (sqlType == DbTypeEnumerable)
				{
					il.Emit(OpCodes.Ldarg_0);											// push command
					il.Emit(OpCodes.Ldstr, prop.Key);									// push parameter name

					// get the type of the contents of the list
					Type listType = prop.Value.MemberType;
					if (listType.IsArray)
						listType = listType.GetElementType();
					else if (listType.IsGenericType)
						listType = listType.GetGenericArguments()[0];

					// emit the table type name, but not too many prefixes
					string tableTypeName = sqlParameter.TypeName;
					if (tableTypeName.Count(c => c == '.') > 1)
						tableTypeName = tableTypeName.Split(new char[] { '.' }, 2)[1];
					il.Emit(OpCodes.Ldstr, tableTypeName);

					il.Emit(OpCodes.Ldtoken, listType);									// push listType
					il.EmitCall(OpCodes.Call, _typeGetTypeFromHandle, null);

					il.Emit(OpCodes.Ldarg_1);											// push object							
					if (prop.Value.MethodInfo != null)									// get value
						il.Emit(OpCodes.Callvirt, prop.Value.MethodInfo);
					else
						il.Emit(OpCodes.Ldfld, prop.Value.FieldInfo);

					if (prop.Value.MemberType.IsValueType)								// box value types before calling object parameter
						il.Emit(OpCodes.Box, prop.Value.MemberType);

					il.EmitCall(OpCodes.Call, _listParameterHelperAddEnumerableParameters, null);
					continue;
				}

				///////////////////////////////////////////////////////////////
				// Start handling all of the other types
				///////////////////////////////////////////////////////////////

				// duplicate parameters so we can call add later
				il.Emit(OpCodes.Dup);													// dup parameter

				///////////////////////////////////////////////////////////////
				// Create a new parameter
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Ldarg_0);												// push dbcommand						
				il.EmitCall(OpCodes.Callvirt, _iDbCommandCreateParameter, null);		// call createparameter

				// duplicate the parameter so we can call setvalue later
				il.Emit(OpCodes.Dup);													// dup parameter

				///////////////////////////////////////////////////////////////
				// p.ParameterName = name
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Dup);													// dup parameter
				il.Emit(OpCodes.Ldstr, prop.Key);										// push string
				il.EmitCall(OpCodes.Callvirt, _iDataParameterSetParameterName, null);	// call setname

				///////////////////////////////////////////////////////////////
				// p.Direction = Direction
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Dup);
				IlHelper.EmitLdInt32(il, (int)ParameterDirection.Input);
				il.EmitCall(OpCodes.Callvirt, _iDataParameterSetDirection, null);

				///////////////////////////////////////////////////////////////
				// Get the value from the object onto the stack
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Ldarg_1);												// push object							
				if (prop.Value.MethodInfo != null)										// get value
					il.Emit(OpCodes.Callvirt, prop.Value.MethodInfo);
				else
					il.Emit(OpCodes.Ldfld, prop.Value.FieldInfo);

				if (prop.Value.MemberType.IsValueType)
				{
					// if this is a value type, then box the value so the compiler can check the type
					il.Emit(OpCodes.Box, prop.Value.MemberType);
				}

				// jump right to the spot where we are ready to set the value
				Label readyToSetLabel = il.DefineLabel();

				// if it's class type or nullable, then we have to check for null
				if (!prop.Value.MemberType.IsValueType || Nullable.GetUnderlyingType(prop.Value.MemberType) != null)
				{
					Label notNull = il.DefineLabel();

					// check to see if it's not null
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Brtrue_S, notNull);

					// it's null. replace the value with DbNull
					il.Emit(OpCodes.Pop);
					il.Emit(OpCodes.Ldsfld, _dbNullValue);

					///////////////////////////////////////////////////////////////
					// if this is a string, set the local length variable to 0
					///////////////////////////////////////////////////////////////
					if (sqlType == DbType.String)
					{
						IlHelper.EmitLdInt32(il, 0);
						il.Emit(OpCodes.Stloc_0);
					}

					// value is set to null. ready to set the property.
					il.Emit(OpCodes.Br_S, readyToSetLabel);

					// we know the value is not null
					il.MarkLabel(notNull);

					// support converting simple object types to values
					if (!TypeHelper.IsSystemType(prop.Value.MemberType))
					{
						// if the type supports IConvertible, then we can just cast it by boxing
						if (prop.Value.MemberType.GetInterfaces().Contains(typeof(IConvertible)))
						{
							// switch the sql type to the type of the parameter
							sqlType = sqlParameter.DbType;

							// use IConvertible to convert to the expected type
							Type expectedType = _dbTypeToTypeMap[sqlParameter.DbType];
							il.Emit(OpCodes.Call, typeof(Convert).GetMethod("To" + expectedType.Name, new Type[] { typeof(Object) }));

							// we need to box the object as the expected type before loading into the field
							il.Emit(OpCodes.Box, expectedType);
						}
						else if (prop.Value.MemberType == typeof(XmlDocument))
						{
							// we are sending up an XmlDocument. ToString just returns the classname, so use the outerxml.
							il.Emit(OpCodes.Callvirt, prop.Value.MemberType.GetProperty("OuterXml").GetGetMethod());
						}
						else if (prop.Value.MemberType == typeof(XDocument))
						{
							// we are sending up an XDocument. Use ToString.
							il.Emit(OpCodes.Callvirt, prop.Value.MemberType.GetMethod("ToString", new Type[] { }));
						}
						else if (sqlParameter.DbType == DbType.Xml)
						{
							// we have an object and it is expecting an xml parameter. let's serialize the object.
							il.Emit(OpCodes.Ldtoken, prop.Value.MemberType);
							il.EmitCall(OpCodes.Call, _typeGetTypeFromHandle, null);
							il.Emit(OpCodes.Call, typeof(TypeHelper).GetMethod("SerializeObjectToXml", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(object), typeof(Type) }, null));
						}
						else
						{
							// it's not a system type and it's not IConvertible, so let's add it as a string and let the data engine convert it.
							il.Emit(OpCodes.Callvirt, prop.Value.MemberType.GetMethod("ToString", new Type[] { }));
						}

						// handle null internal values
						Label internalValueIsNotNull = il.DefineLabel();

						// check to see if it's not null
						il.Emit(OpCodes.Dup);
						il.Emit(OpCodes.Brtrue_S, internalValueIsNotNull);

						// it's null. replace the value with DbNull
						il.Emit(OpCodes.Pop);
						il.Emit(OpCodes.Ldsfld, _dbNullValue);

						il.MarkLabel(internalValueIsNotNull);
					}

					///////////////////////////////////////////////////////////////
					// if this is a linq binary, convert it to a byte array
					///////////////////////////////////////////////////////////////
					if (prop.Value.MemberType == typeof(System.Data.Linq.Binary))
						il.EmitCall(OpCodes.Callvirt, _linqBinaryToArray, null);

					///////////////////////////////////////////////////////////////
					// if this is a string, set the length property
					///////////////////////////////////////////////////////////////
					if (prop.Value.MemberType == typeof(string))
					{
						Label isLong = il.DefineLabel();
						Label lenDone = il.DefineLabel();

						// get the string length
						il.Emit(OpCodes.Dup);									// dup string
						il.EmitCall(OpCodes.Callvirt, _stringGetLength, null);	// call get length

						// compare to 4000
						IlHelper.EmitLdInt32(il, 4000);					// [string] [length] [4000]
						il.Emit(OpCodes.Cgt);									// [string] [0 or 1]
						il.Emit(OpCodes.Brtrue_S, isLong);

						// it is not longer than 4000, so store the value
						IlHelper.EmitLdInt32(il, 4000);					// [string] [4000]
						il.Emit(OpCodes.Br_S, lenDone);

						// it is longer than 4000, so store -1
						il.MarkLabel(isLong);
						IlHelper.EmitLdInt32(il, -1);						// [string] [-1]

						il.MarkLabel(lenDone);

						// store the length of the string in loc.1 so we can stick it in the parameter
						il.Emit(OpCodes.Stloc_0);								// [string]   
					}
				}

				///////////////////////////////////////////////////////////////
				// p.Value = value
				///////////////////////////////////////////////////////////////
				// push parameter is at top of method
				// value is above
				il.MarkLabel(readyToSetLabel);
				il.EmitCall(OpCodes.Callvirt, _iDataParameterSetValue, null);

				///////////////////////////////////////////////////////////////
				// p.Size = string.length
				///////////////////////////////////////////////////////////////
				if (prop.Value.MemberType == typeof(string))
				{
					var endOfSize = il.DefineLabel();

					// don't set if 0
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Brfalse_S, endOfSize);

					// parameter.setsize = string.length
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldloc_0);
					il.EmitCall(OpCodes.Callvirt, _iDataParameterSetSize, null);

					il.MarkLabel(endOfSize);
				}

				///////////////////////////////////////////////////////////////
				// p.DbType = DbType
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Dup);													// dup parameter
				IlHelper.EmitLdInt32(il, (int)sqlType);									// push dbtype
				il.EmitCall(OpCodes.Callvirt, _iDataParameterSetDbType, null);			// call settype

				///////////////////////////////////////////////////////////////
				// parameters.Add (p)
				///////////////////////////////////////////////////////////////
				il.EmitCall(OpCodes.Callvirt, _iListAdd, null);							// stack => [parameters]
				il.Emit(OpCodes.Pop);													// IList.Add returns the new index (int); we don't care
			}

			il.Emit(OpCodes.Pop);						// pop (parameters)
			il.Emit(OpCodes.Ret);

			return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
		}

		/// <summary>
		/// Create a parameter generator for a dynamic object.
		/// </summary>
		/// <param name="command">The command to parse.</param>
		/// <returns>An action that fills in the command parameters from a dynamic object.</returns>
		static Action<IDbCommand, object> CreateDynamicParameterGenerator(IDbCommand command)
		{
			// figure out what the parameters are
			List<SqlParameter> parameters = DeriveParameters(command);

			return (cmd, o) =>
			{
				foreach (SqlParameter template in parameters)
				{
					// create the parameter
					IDbDataParameter p = command.CreateParameter();
					p.ParameterName = template.ParameterName;
					p.Direction = template.Direction;
					p.DbType = template.DbType;

					// make sure that we have a dictionary implementation
					IDictionary<string, object> dyn = o as IDictionary<string, object>;
					if (dyn == null)
						throw new InvalidOperationException("Dynamic object must support IDictionary<string, object>.");

					// get the value from the object, converting null to db null
					// note that if the dictionary does not have the value, we leave the value null and then the parameter gets defaulted
					object value = null;
					if (dyn.TryGetValue(p.ParameterName, out value) && value == null)
						value = DBNull.Value;
					p.Value = value;

					// if it's a string, fill in the length
					string s = value as string;
					if (s != null)
					{
						int length = s.Length;
						if (length > 4000)
							length = -1;
						p.Size = length;
					}

					cmd.Parameters.Add(p);
				}
			};
		}
		#endregion

		#region DbType Mapping
		/// <summary>
		/// Look up a DbType from a .Net type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <param name="parameterType">The expected sql parameter type. Used as the default.</param>
		/// <returns>The equivalend DbType.</returns>
		private static DbType LookupDbType(Type type, DbType parameterType)
		{
			DbType sqlType;

			// if the type is nullable, get the underlying type
			var nullUnderlyingType = Nullable.GetUnderlyingType(type);
			if (nullUnderlyingType != null)
				type = nullUnderlyingType;

			// if it's an enum, get the underlying type
			if (type.IsEnum)
				type = Enum.GetUnderlyingType(type);

			// look up the type
			if (_typeToDbTypeMap.TryGetValue(type, out sqlType))
				return sqlType;

			// special cases for XmlDocument and XDocument
			if (type == typeof(XmlDocument))
				return DbType.Xml;
			if (type == typeof(XDocument))
				return DbType.Xml;

			// support for enumerables
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				// use -1 to denote its a list, hacky but will work on any DB
				return DbTypeEnumerable;
			}

			// let's see if the type can be directly converted to the parameter type
			return parameterType;
		}
		#endregion

		#region ListParameterHelper Class
		/// <summary>
		/// Helps pack list parameters into a command.
		/// </summary>
		static class ListParameterHelper
		{
			/// <summary>
			/// Cache for Table-Valued Parameter schemas.
			/// </summary>
			private static ConcurrentDictionary<Type, DataTable> _tvpSchemas = new ConcurrentDictionary<Type, DataTable>();

			/// <summary>
			/// The regex to detect parameters.
			/// </summary>
			private static string _parameterPrefixRegex = "[?@:]";

			/// <summary>
			/// Converts an IEnumerable to a list of parameters, and updates the SQL command to support them.
			/// </summary>
			/// <param name="command">The command to add to.</param>
			/// <param name="parameterName">The name of the parameter.</param>
			/// <param name="tableTypeName">The name of the table type or null to assume TypeTable.</param>
			/// <param name="listType">The type of the list to parameterize.</param>
			/// <param name="value">The value of the parameter.</param>
			public static void AddEnumerableParameters(IDbCommand command, string parameterName, string tableTypeName, Type listType, object value)
			{
				// convert the value to an enumerable
				IEnumerable list = (IEnumerable)value;

				if (command.CommandType == CommandType.Text && (listType.IsValueType || listType == typeof(string)))
					AddEnumerableValueParameters(command, parameterName, list);
				else
					AddEnumerableClassParameters(command, parameterName, tableTypeName, listType, list);
			}

			/// <summary>
			/// Add strings and value parameters (non-table-types).
			/// </summary>
			/// <param name="command">The command to add parameters to.</param>
			/// <param name="parameterName">The name of the parameter.</param>
			/// <param name="list">The list of objects to add.</param>
			private static void AddEnumerableValueParameters(IDbCommand command, string parameterName, IEnumerable list)
			{
				var count = 0;
				bool isString = list is IEnumerable<string>;

				// add a parameter for each item
				foreach (var item in list)
				{
					count++;

					// create the parameter for the item
					var listParam = command.CreateParameter();
					listParam.ParameterName = parameterName + count;
					listParam.Value = item ?? DBNull.Value;

					// if we are dealing with strings, add the length of the string
					if (isString && item != null)
					{
						int length = ((string)item).Length;
						if (length > 4000)
							length = -1;
						listParam.Size = length;
					}

					command.Parameters.Add(listParam);
				}

				if (count == 0)
				{
					command.CommandText = Regex.Replace(command.CommandText, _parameterPrefixRegex + Regex.Escape(parameterName), "SELECT NULL WHERE 1 = 0", RegexOptions.IgnoreCase);
				}
				else
				{
					command.CommandText = Regex.Replace(
						command.CommandText,
						_parameterPrefixRegex + Regex.Escape(parameterName),
						match =>
						{
							var grp = match.Value;
							var sb = new StringBuilder();

							// append the parameters
							sb.Append(grp).Append(1);
							for (int i = 2; i <= count; i++)
								sb.Append(',').Append(grp).Append(i);

							return sb.ToString();
						},
						RegexOptions.IgnoreCase);
				}
			}

			/// <summary>
			/// Add a list of objects as a table-valued parameter.
			/// </summary>
			/// <param name="command">The command to add parameters to.</param>
			/// <param name="parameterName">The name of the parameter.</param>
			/// <param name="tableTypeName">The name of the table type or null to assume TypeTable.</param>
			/// <param name="listType">The type that the list contains.</param>
			/// <param name="list">The list of objects.</param>
			private static void AddEnumerableClassParameters(IDbCommand command, string parameterName, string tableTypeName, Type listType, IEnumerable list)
			{
				// table-valued parameters only work with sql
				SqlCommand cmd = (SqlCommand)command;

				// convert any nullable types to their underlying type
				listType = Nullable.GetUnderlyingType(listType) ?? listType;

				// if the table type name is null, then default to the name of the class
				if (String.IsNullOrWhiteSpace(tableTypeName))
					tableTypeName = String.Format(CultureInfo.InstalledUICulture, "[{0}Table]", listType.Name);

				// let's see if we can get the schema table from the server
				DataTable schema = _tvpSchemas.GetOrAdd(
					listType,
					type =>
					{
						SqlCommand schemaCommand = new SqlCommand(String.Format(CultureInfo.InvariantCulture, "DECLARE @schema {0} SELECT TOP 0 * FROM @schema", tableTypeName));
						schemaCommand.Connection = cmd.Connection;
						schemaCommand.Transaction = cmd.Transaction;
						using (var reader = schemaCommand.ExecuteReader())
							return reader.GetSchemaTable();
					});

				// create the structured parameter
				SqlParameter p = new SqlParameter();
				p.ParameterName = parameterName;
				p.SqlDbType = SqlDbType.Structured;
				p.TypeName = tableTypeName;
				p.Value = new ObjectListDbDataReader(schema, listType, list);
				cmd.Parameters.Add(p);
			}
		}
		#endregion
	}
}
