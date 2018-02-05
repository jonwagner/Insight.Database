using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Insight.Database.Mapping;
using Insight.Database.Providers;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// A code generator to create methods to serialize an object into sql parameters.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
	static class DbParameterGenerator
	{
		#region Private Members
		/// <summary>
		/// Special case for enumerable data types.
		/// </summary>
		internal const DbType DbTypeEnumerable = (DbType)(-1);

		/// <summary>
		/// MethodInfos for methods that we are going to call.
		/// </summary>
		private static readonly FieldInfo _dbNullValue = typeof(DBNull).GetField("Value");
		private static readonly MethodInfo _iDbCommandGetParameters = typeof(IDbCommand).GetProperty("Parameters").GetGetMethod();
		private static readonly MethodInfo _iDataParameterCollectionGetItem = typeof(IDataParameterCollection).GetProperty("Item").GetGetMethod();
		private static readonly MethodInfo _iDataParameterSetValue = typeof(IDataParameter).GetProperty("Value").GetSetMethod();
		private static readonly MethodInfo _iDataParameterGetValue = typeof(IDataParameter).GetProperty("Value").GetGetMethod();

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
			{ typeof(TimeSpan), DbType.Time },
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
			{ typeof(TimeSpan?), DbType.Time },
			{ TypeHelper.LinqBinaryType, DbType.Binary },
		};

		/// <summary>
		/// A map from dbtypes to underlying system types.
		/// </summary>
		private static Dictionary<DbType, Type> _dbTypeToTypeMap = new Dictionary<DbType, Type>()
		{
			{ DbType.AnsiString, typeof(string) },
			{ DbType.AnsiStringFixedLength, typeof(string) },
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
			{ DbType.Date, typeof(DateTime) },
			{ DbType.DateTime, typeof(DateTime) },
			{ DbType.DateTime2, typeof(DateTime) },
			{ DbType.DateTimeOffset, typeof(DateTimeOffset) },
			{ DbType.Time, typeof(TimeSpan) },
			{ DbType.Binary, typeof(byte[]) },
			{ DbType.Object, typeof(object) },
            { DbType.Xml, typeof(string) },
		};

		/// <summary>
		/// The cache for the serializers (input parameters).
		/// </summary>
		private static ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>> _serializers = new ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>>();

		/// <summary>
		/// The cache for the deserializers (output parameters).
		/// </summary>
		private static ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>> _deserializers = new ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>>();
		#endregion

		#region Code Cache Members
		/// <summary>
		/// Get the parameter generator method for a command and the type used for the parameter.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>The command generator.</returns>
		public static Action<IDbCommand, object> GetInputParameterGenerator(IDbCommand command, Type type)
		{
			QueryIdentity identity = new QueryIdentity(command, type);

			// try to get the deserializer. if not found, create one.
			return _serializers.GetOrAdd(
				identity,
				key =>
				{
					if (type.IsSubclassOf(typeof(DynamicObject)))
						return CreateDynamicInputParameterGenerator(command);
					else if (type == typeof(Dictionary<string, object>))
						return CreateDynamicInputParameterGenerator(command);
					else
						return CreateClassInputParameterGenerator(command, type);
				});
		}

		/// <summary>
		/// Get the parameter generator method for a command and the type used for the parameter.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>The command generator.</returns>
		public static Action<IDbCommand, object> GetOutputParameterConverter(IDbCommand command, Type type)
		{
			QueryIdentity identity = new QueryIdentity(command, type);

			// try to get the deserializer. if not found, create one.
			return _deserializers.GetOrAdd(identity, key => CreateClassOutputParameterConverter(command, type));
		}
		#endregion

		/// <summary>
		/// Look up a DbType from a .Net type.
		/// </summary>
		/// <param name="type">The type of object to look up.</param>
		/// <returns>The equivalent DbType.</returns>
		internal static DbType LookupDbType(Type type)
		{
			// look up the type
			DbType sqlType = DbType.String;
			_typeToDbTypeMap.TryGetValue(type, out sqlType);

			return sqlType;
		}

		#region Input Parameter Code Generation Members
		/// <summary>
		/// Create the Parameter generator method.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of object to parameterize.</param>
		/// <returns>A method that serializes parameters to values.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static Action<IDbCommand, object> CreateClassInputParameterGenerator(IDbCommand command, Type type)
		{
			var provider = InsightDbProvider.For(command);
			var parameters = provider.DeriveParameters(command);

			// special case if the parameters object is an IEnumerable or Array
			// look for the parameter that is a Structured object and pass the array to the TVP
			// note that string supports ienumerable, so exclude atomic types
			var enumerable = type.GetInterfaces().FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerable != null && type != typeof(string) && parameters.OfType<IDataParameter>().Where(p => p.Direction.HasFlag(ParameterDirection.Input)).Count() == 1)
			{
				return (IDbCommand cmd, object o) =>
				{
					// don't use the provider above. The command may be unwrapped by the time we get back here
					var tableParameter = InsightDbProvider.For(cmd).CloneParameter(cmd, parameters.OfType<IDataParameter>().Single(p => p.Direction.HasFlag(ParameterDirection.Input)));
					cmd.Parameters.Add(tableParameter);
					ListParameterHelper.ConvertListParameter(tableParameter, o, cmd);
				};
			}

			// get the mapping of the properties for the type
			var mappings = ColumnMapping.MapParameters(type, command, parameters);

			// start creating a dynamic method
			Type typeOwner = type.HasElementType ? type.GetElementType() : type;
			var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "CreateInputParameters-{0}", Guid.NewGuid()), null, new[] { typeof(IDbCommand), typeof(object) }, typeOwner, true);
			var il = dm.GetILGenerator();

			// copy the parameters into the command object
			var parametersLocal = il.DeclareLocal(typeof(IDataParameter[]));
			StaticFieldStorage.EmitLoad(il, provider);
			il.Emit(OpCodes.Ldarg_0);
			StaticFieldStorage.EmitLoad(il, parameters);
			il.Emit(OpCodes.Call, typeof(InsightDbProvider).GetMethod("CopyParameters", BindingFlags.NonPublic | BindingFlags.Instance));
			il.Emit(OpCodes.Stloc, parametersLocal);

			// go through all of the mappings
			for (int i = 0; i < mappings.Count; i++)
			{
				var mapping = mappings[i];
				var dbParameter = parameters[i];

				// if there is no mapping for the parameter
				if (mapping == null)
				{
					// sql will silently eat table parameters that are not specified, and that can be difficult to debug
					if (provider.IsTableValuedParameter(command, dbParameter))
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Table parameter {0} must be specified", dbParameter.ParameterName));

					// unspecified input parameters get skipped
					if (dbParameter.Direction == ParameterDirection.Input)
						parameters[i] = null;

					continue;
				}

				var memberType = mapping.Member.MemberType;
				var serializer = mapping.Serializer;

				// get the parameter
				il.Emit(OpCodes.Ldloc, parametersLocal);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem, typeof(IDataParameter));

				// look up the best type to use for the parameter
				DbType sqlType = LookupDbType(memberType, serializer, dbParameter.DbType);
				// give the provider an opportunity to fix up the template parameter (e.g. set UDT type names)
				provider.FixupParameter(command, dbParameter, sqlType, memberType, mapping.Member.SerializationMode);
				// give a chance to override the best guess parameter
				DbType overriddenSqlType = ColumnMapping.MapParameterDataType(memberType, command, dbParameter, sqlType);

				///////////////////////////////////////////////////////////////
				// We have a parameter, start handling all of the other types
				///////////////////////////////////////////////////////////////
				if (overriddenSqlType != sqlType)
				{
					sqlType = overriddenSqlType;
					dbParameter.DbType = sqlType;
				}

				///////////////////////////////////////////////////////////////
				// Get the value from the object onto the stack
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Ldarg_1);
				if (type.GetTypeInfo().IsValueType)
					il.Emit(OpCodes.Unbox_Any, type);

				///////////////////////////////////////////////////////////////
				// Special case support for enumerables. If the type is -1 (our workaround, then call the list parameter method)
				///////////////////////////////////////////////////////////////
				if (sqlType == DbTypeEnumerable)
				{
					// we have the parameter and the value as object, add the command
					ClassPropInfo.EmitGetValue(type, mapping.PathToMember, il);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Call, typeof(ListParameterHelper).GetMethod("ConvertListParameter", BindingFlags.Static | BindingFlags.NonPublic));
					continue;
				}

				Label readyToSetLabel = il.DefineLabel();
				ClassPropInfo.EmitGetValue(type, mapping.PathToMember, il, readyToSetLabel);

				// special conversions for timespan to datetime
				if ((sqlType == DbType.Time && dbParameter.DbType != DbType.Time) ||
					(dbParameter.DbType == DbType.DateTime || dbParameter.DbType == DbType.DateTime2 || dbParameter.DbType == DbType.DateTimeOffset))
				{
					IlHelper.EmitLdInt32(il, (int)dbParameter.DbType);
					il.Emit(OpCodes.Call, typeof(TypeConverterGenerator).GetMethod("ObjectToSqlDateTime"));
				}

				// if it's class type, boxed value type (in an object), or nullable, then we have to check for null
				if (!memberType.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(memberType) != null)
				{
					Label notNull = il.DefineLabel();

					// check to see if it's not null
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Brtrue, notNull);

					// it's null. replace the value with DbNull
					il.Emit(OpCodes.Pop);
					il.Emit(OpCodes.Ldsfld, _dbNullValue);

					// value is set to null. ready to set the property.
					il.Emit(OpCodes.Br, readyToSetLabel);

					// we know the value is not null
					il.MarkLabel(notNull);
                }

				///////////////////////////////////////////////////////////////
				// if this is a linq binary, convert it to a byte array
				///////////////////////////////////////////////////////////////
				if (memberType == TypeHelper.LinqBinaryType)
				{
					il.Emit(OpCodes.Callvirt, TypeHelper.LinqBinaryToArray);
				}
				else if (memberType == typeof(XmlDocument))
				{
					// we are sending up an XmlDocument. ToString just returns the classname, so use the outerxml.
					il.Emit(OpCodes.Callvirt, memberType.GetProperty("OuterXml").GetGetMethod());
				}
				else if (memberType == typeof(XDocument))
				{
					// we are sending up an XDocument. Use ToString.
					il.Emit(OpCodes.Callvirt, memberType.GetMethod("ToString", new Type[] { }));
				}
                else if (serializer != null && serializer.CanSerialize(memberType, sqlType))
				{
					il.EmitLoadType(memberType);
					StaticFieldStorage.EmitLoad(il, serializer);
					il.Emit(OpCodes.Call, typeof(DbParameterGenerator).GetMethod("SerializeParameterValue", BindingFlags.NonPublic | BindingFlags.Static));
				}

				///////////////////////////////////////////////////////////////
				// p.Value = value
				///////////////////////////////////////////////////////////////
				// push parameter is at top of method
				// value is above
				il.MarkLabel(readyToSetLabel);
				if (memberType == typeof(string))
					il.Emit(OpCodes.Call, typeof(DbParameterGenerator).GetMethod("SetParameterStringValue", BindingFlags.NonPublic | BindingFlags.Static));
                else if ((memberType == typeof(Guid?) || (memberType == typeof(Guid))) && dbParameter.DbType != DbType.Guid && command.CommandType == CommandType.StoredProcedure)
                    il.Emit(OpCodes.Call, typeof(DbParameterGenerator).GetMethod("SetParameterGuidValue", BindingFlags.NonPublic | BindingFlags.Static));
                else
					il.Emit(OpCodes.Callvirt, _iDataParameterSetValue);
			}

			il.Emit(OpCodes.Ret);

			return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
		}

		/// <summary>
		/// Serialize a value into a parameter. This is a stub method that reorders the stack.
		/// </summary>
		/// <param name="value">The value to serialize.</param>
		/// <param name="type">The type of the member.</param>
		/// <param name="serializer">The serializer to use.</param>
		/// <returns>The serialized value.</returns>
		private static object SerializeParameterValue(object value, Type type, IDbObjectSerializer serializer)
		{
			return serializer.SerializeObject(type, value) ?? DBNull.Value;
		}

        /// <summary>
        /// Set a string value on a parameter. .NET will not auto-convert GUIDs.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The string value.</param>
        private static void SetParameterGuidValue(IDbDataParameter parameter, object value)
        {
			parameter.Value = value;

            if (parameter.DbType != DbType.Guid)
            {
				if (value != DBNull.Value)
                    parameter.Value = value.ToString();
                parameter.DbType = DbType.AnsiString;
            }
        }

		/// <summary>
		/// Set a string value on a parameter.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="value">The string value.</param>
		private static void SetParameterStringValue(IDbDataParameter parameter, object value)
		{
			parameter.Value = value;

			// need to set the type to string - required for string->guid conversions under sql server
			var s = value as string;
			if (s != null)
			{
				if (parameter.DbType != DbType.String && parameter.DbType != DbType.AnsiString && parameter.DbType != DbType.Xml && parameter.DbType != DbType.Object)
					parameter.DbType = DbType.String;

				var dbParameter = parameter as IDbDataParameter;
                if (dbParameter != null)
				{
                    if (dbParameter.Direction.HasFlag(ParameterDirection.Output))
                    {
                        // for output parameters, we always want to retrieve as much data as possible
                        dbParameter.Size = -1;
                    }
                    else
                    {
                        // for input parameters, some providers may behave better by specifying the length
                        var length = s.Length;
                        if (length > 4000)
                            dbParameter.Size = -1;
                        else
                            dbParameter.Size = s.Length;
                    }
				}
			}
		}

		/// <summary>
		/// Create a parameter generator for a dynamic object.
		/// </summary>
		/// <param name="command">The command to parse.</param>
		/// <returns>An action that fills in the command parameters from a dynamic object.</returns>
		private static Action<IDbCommand, object> CreateDynamicInputParameterGenerator(IDbCommand command)
		{
			var provider = InsightDbProvider.For(command);
			var parameters = provider.DeriveParameters(command);

			return (cmd, o) =>
			{
				// make sure that we have a dictionary implementation
				IDictionary<string, object> dyn = o as IDictionary<string, object>;
				if (dyn == null)
					throw new InvalidOperationException("Dynamic object must support IDictionary<string, object>.");

				foreach (var template in parameters)
				{
					var p = provider.CloneParameter(cmd, template);

					// get the value from the object, converting null to db null
					// note that if the dictionary does not have the value, we leave the value null and then the parameter gets defaulted
					object value = null;
                    if (dyn.TryGetValue(p.ParameterName, out value))
                    {
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        else
                        {
                            DbType sqlType = LookupDbType(value.GetType(), null, p.DbType);
                            if (sqlType == DbTypeEnumerable)
                            {
								cmd.Parameters.Add(p);
								ListParameterHelper.ConvertListParameter(p, value, cmd);
                                continue;
                            }
                        }
                    }

					p.Value = value;

					// if it's a string, fill in the length
					IDbDataParameter dbDataParameter = p as IDbDataParameter;
					if (dbDataParameter != null)
					{
						string s = value as string;
						if (s != null)
						{
							int length = s.Length;
							if (length > 4000)
								length = -1;
							dbDataParameter.Size = length;
						}
					}

					// explicitly set the type of the parameter
					if (value != null && _typeToDbTypeMap.ContainsKey(value.GetType()))
						dbDataParameter.DbType = _typeToDbTypeMap[value.GetType()];

					cmd.Parameters.Add(p);
				}
			};
		}
		#endregion

		#region Output Parameter Code Generation Members
		/// <summary>
		/// Creates a converter from output parameters to an object of a given type.
		/// </summary>
		/// <param name="command">The command to analyze for the results.</param>
		/// <param name="type">The type to put the values into.</param>
		/// <returns>The converter method.</returns>
		private static Action<IDbCommand, object> CreateClassOutputParameterConverter(IDbCommand command, Type type)
		{
			// get the parameters
			List<IDataParameter> parameters = command.Parameters.Cast<IDataParameter>().ToList();

			// if there are no output parameters, then return an empty method
			if (!parameters.Cast<IDataParameter>().Any(p => p.Direction.HasFlag(ParameterDirection.Output)))
				return (IDbCommand c, object o) => { };

			// create a dynamic method
			Type typeOwner = type.HasElementType ? type.GetElementType() : type;

			// start creating a dynamic method
			var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "CreateOutputParameters-{0}", Guid.NewGuid()), null, new[] { typeof(IDbCommand), typeof(object) }, typeOwner, true);
			var il = dm.GetILGenerator();

			var localParameters = il.DeclareLocal(typeof(IDataParameterCollection));

			// get the parameters collection from the command into loc.0
			il.Emit(OpCodes.Ldarg_0);												// push arg.0 (command), stack => [command]
			il.Emit(OpCodes.Callvirt, _iDbCommandGetParameters);					// call getparams, stack => [parameters]
			il.Emit(OpCodes.Stloc, localParameters);

			// go through all of the mappings
			var mappings = ColumnMapping.MapParameters(type, command, parameters);
			for (int i = 0; i < mappings.Count; i++)
			{
				var finishLabel = il.DefineLabel();

				// if there is no parameter for this property, then skip it
				var mapping = mappings[i];
				if (mapping == null)
					continue;

				// if the property is readonly, then skip it
				var prop = mapping.Member;
				if (!prop.CanSetMember)
					continue;

				// if the parameter is not output, then skip it
				IDataParameter parameter = parameters[i];
				if (parameter == null || !parameter.Direction.HasFlag(ParameterDirection.Output))
					continue;

				// push the object on the stack. we will need it to set the value below
				il.Emit(OpCodes.Ldarg_1);

				// if this is a deep mapping, then get the parent object, and do a null test if its not a value type
                if (mapping.IsDeep)
                {
                    ClassPropInfo.EmitGetValue(type, mapping.Prefix, il);

                    if (!ClassPropInfo.FindMember(type, mapping.Prefix).MemberType.GetTypeInfo().IsValueType)
                    {
                        il.Emit(OpCodes.Dup);
                        var label = il.DefineLabel();
                        il.Emit(OpCodes.Brtrue, label);
                        il.Emit(OpCodes.Pop);               // pop the object before finishing
                        il.Emit(OpCodes.Br, finishLabel);
                        il.MarkLabel(label);
                    }
                }

                // get the parameter out of the collection
                il.Emit(OpCodes.Ldloc, localParameters);
                il.Emit(OpCodes.Ldstr, parameter.ParameterName);                 // push (parametername)
                il.Emit(OpCodes.Callvirt, _iDataParameterCollectionGetItem);

                // get the value out of the parameter
                il.Emit(OpCodes.Callvirt, _iDataParameterGetValue);

				// emit the code to convert the value and set it on the object
				TypeConverterGenerator.EmitConvertAndSetValue(il, _dbTypeToTypeMap[parameter.DbType], mapping);
				il.MarkLabel(finishLabel);
			}

			il.Emit(OpCodes.Ret);

			return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
		}
		#endregion

		#region DbType Mapping
		/// <summary>
		/// Look up a DbType from a .Net type.
		/// </summary>
		/// <param name="type">The type of object to look up.</param>
		/// <param name="serializer">The serializer that has been detected for the field.</param>
		/// <param name="parameterType">The expected sql parameter type. Used as the default.</param>
		/// <returns>The equivalent DbType.</returns>
		private static DbType LookupDbType(Type type, IDbObjectSerializer serializer, DbType parameterType)
		{
			DbType sqlType;

            // if the serializer can serialize it, then it's a string
            if (serializer != null && serializer.CanSerialize(type, parameterType))
                return serializer.GetSerializedDbType(type, parameterType);

			// if the type is nullable, get the underlying type
			var nullUnderlyingType = Nullable.GetUnderlyingType(type);
			if (nullUnderlyingType != null)
				type = nullUnderlyingType;

			// if it's an enum, get the underlying type
			if (type.GetTypeInfo().IsEnum)
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

			// sql udts are udts
			if (TypeHelper.IsSqlUserDefinedType(type))
				return DbType.Object;

			// let's see if the type can be directly converted to the parameter type
			return parameterType;
		}
		#endregion

		#region ListParameterHelper Class
		/// <summary>
		/// Helps pack list parameters into a command.
		/// </summary>
		internal static class ListParameterHelper
		{
			/// <summary>
			/// The regex to detect parameters.
			/// </summary>
			private static string _parameterPrefixRegex = "[?@:]";

			/// <summary>
			/// Converts an IEnumerable to a list of parameters, and updates the SQL command to support them.
			/// </summary>
			/// <param name="parameter">The parameter to modify.</param>
			/// <param name="value">The value of the parameter.</param>
			/// <param name="command">The command to add to.</param>
			internal static void ConvertListParameter(IDataParameter parameter, object value, IDbCommand command)
			{
				// convert the value to an enumerable
				IEnumerable list = (IEnumerable)value;

				Type listType = list.GetType();
                if (listType.IsArray)
                {
                    listType = listType.GetElementType();
                }
                else
                {
                    listType = listType.GetInterfaces().FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (listType.GetTypeInfo().IsGenericType)
                        listType = listType.GetGenericArguments()[0];
                }

				var underlyingType = Nullable.GetUnderlyingType(listType) ?? listType;

				if (command.CommandType == CommandType.Text && (underlyingType.GetTypeInfo().IsValueType || underlyingType == typeof(string)))
					ConvertListParameterByValue(parameter, list, command);
				else
					ConvertListParameterByClass(parameter, list, command, listType);
			}

			/// <summary>
			/// Add strings and value parameters (non-table-types).
			/// </summary>
			/// <param name="parameter">The parameter to modify.</param>
			/// <param name="list">The list of objects to add.</param>
			/// <param name="command">The command to add parameters to.</param>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
			private static void ConvertListParameterByValue(IDataParameter parameter, IEnumerable list, IDbCommand command)
			{
				// we are going to replace the current parameter with a list of new parameters
				command.Parameters.Remove(parameter);

				string parameterName = parameter.ParameterName;
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
					command.CommandText = Regex.Replace(command.CommandText, _parameterPrefixRegex + Regex.Escape(parameterName), "NULL", RegexOptions.IgnoreCase);
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
			/// <param name="parameter">The parameter to modify.</param>
			/// <param name="list">The list to add to the parameter.</param>
			/// <param name="command">The command to add parameters to.</param>
			/// <param name="listType">The type that the list contains.</param>
			private static void ConvertListParameterByClass(IDataParameter parameter, IEnumerable list, IDbCommand command, Type listType)
			{
				InsightDbProvider.For(command).SetupTableValuedParameter(command, parameter, list, listType);
			}
		}
		#endregion
	}
}
