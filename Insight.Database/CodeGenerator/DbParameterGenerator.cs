using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using Insight.Database.Providers;

namespace Insight.Database.CodeGenerator
{
	/// <summary>
	/// A code generator to create methods to serialize an object into sql parameters.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the internal properties reduces readability without adding additional information.")]
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
		private static readonly MethodInfo _iDataParameterCollectionGetItem = typeof(IDataParameterCollection).GetProperty("Item").GetGetMethod();
		private static readonly MethodInfo _iDataParameterSetDbType = typeof(IDataParameter).GetProperty("DbType").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetValue = typeof(IDataParameter).GetProperty("Value").GetSetMethod();
		private static readonly MethodInfo _iDataParameterSetSize = typeof(IDbDataParameter).GetProperty("Size").GetSetMethod();
		private static readonly MethodInfo _stringGetLength = typeof(string).GetProperty("Length").GetGetMethod();
		private static readonly MethodInfo _linqBinaryToArray = typeof(System.Data.Linq.Binary).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance);
		private static readonly MethodInfo _listParameterHelperAddEnumerableParameters = typeof(ListParameterHelper).GetMethod("AddEnumerableParameters");
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
			{ typeof(System.Data.Linq.Binary), DbType.Binary },
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
		};

		/// <summary>
		/// The cache for the serializers (input parameters).
		/// </summary>
		private static ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>> _serializers = new ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>>();

		/// <summary>
		/// The cache for the deserializers (output parameters).
		/// </summary>
		private static ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>> _deserializers = new ConcurrentDictionary<QueryIdentity, Action<IDbCommand, object>>();

		/// <summary>
		/// Regex to detect parameters in sql text.
		/// </summary>
		private static Regex _parameterRegex = new Regex("[?@]([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

		#region Parameter Detection Members
		/// <summary>
		/// Derive the parameters for a command.
		/// </summary>
		/// <param name="command">The command to derive.</param>
		/// <returns>The list of parameter names.</returns>
		internal static IList<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			// for stored procs, can call the server to derive them
			if (command.CommandType == CommandType.StoredProcedure)
				return InsightDbProvider.For(command).DeriveParameters(command);

			// otherwise we have to look at the text
			// the text can even contain the parameters in a comment (e.g. "myproc -- @p, @q")
			return DeriveParametersFromSqlText(command);
		}

		/// <summary>
		/// Get a list of parameters from Sql text.
		/// </summary>
		/// <param name="command">The command to scan.</param>
		/// <returns>A list of the detected parameters.</returns>
		private static IList<IDbDataParameter> DeriveParametersFromSqlText(IDbCommand command)
		{
			// TODO: move this into providers
			return _parameterRegex.Matches(command.CommandText)
				.Cast<Match>()
				.Select(m => m.Groups[1].Value.ToUpperInvariant())
				.Distinct()
				.Select(p =>
					{
						var dbParameter = command.CreateParameter();
						dbParameter.ParameterName = p;
						return dbParameter; 
					})
				.ToList();
		}
		#endregion

		#region Input Parameter Code Generation Members
		/// <summary>
		/// Create the Parameter generator method.
		/// </summary>
		/// <param name="command">The command to analyze.</param>
		/// <param name="type">The type of object to parameterize.</param>
		/// <returns>A method that serializes parameters to values.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		static Action<IDbCommand, object> CreateClassInputParameterGenerator(IDbCommand command, Type type)
		{
			var provider = InsightDbProvider.For(command);
			var parameters = DeriveParameters(command);

			// special case if the parameters object is an IEnumerable or Array
			// look for the parameter that is a Structured object and pass the array to the TVP
			var enumerable = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerable != null)
			{
				var tableParameter = parameters.OfType<IDbDataParameter>().FirstOrDefault(p => provider.IsTableValuedParameter(command, p));
				return (IDbCommand cmd, object o) => 
				{ 
					ListParameterHelper.AddEnumerableParameters(
						cmd,
						tableParameter.ParameterName,
						provider.GetTableParameterTypeName(command, tableParameter),
						enumerable.GetGenericArguments()[0],
						o);
				};
			}

			// get the mapping of the properties for the type
			var mapping = ColumnMapping.Parameters.CreateMapping(type, null, command.CommandText, command.CommandType, parameters, 0, parameters.Count, true);

			// start creating a dynamic method
			Type typeOwner = type.HasElementType ? type.GetElementType() : type;
			var dm = new DynamicMethod(String.Format(CultureInfo.InvariantCulture, "CreateInputParameters-{0}", Guid.NewGuid()), null, new[] { typeof(IDbCommand), typeof(object) }, typeOwner, true);
			var il = dm.GetILGenerator();

			var stringLength = il.DeclareLocal(typeof(Int32));

			// copy the parameters into the command object, returning the parameters list
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, typeof(DbParameterGenerator).GetMethod("CopyParameters", BindingFlags.Static | BindingFlags.NonPublic));
			// stack => [parameters]

			// go through all of the mappings
			for (int i = 0; i < mapping.Length; i++)
			{
				var prop = mapping[i];
				var dbParameter = parameters[i];

				// if there is no mapping for that parameter, then continue
				if (prop == null)
				{
					// sql will silently eat table parameters that are not specified, and that can be difficult to debug
					if (provider.IsTableValuedParameter(command, dbParameter))
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Table parameter {0} must be specified", dbParameter.ParameterName));

					continue;
				}

				// look up the best type to use for the parameter
				DbType sqlType = LookupDbType(prop.MemberType, dbParameter.DbType);

				///////////////////////////////////////////////////////////////
				// Special case support for enumerables. If the type is -1 (our workaround, then call the list parameter method)
				///////////////////////////////////////////////////////////////
				if (sqlType == DbTypeEnumerable)
				{
					il.Emit(OpCodes.Ldarg_0);											// push command
					il.Emit(OpCodes.Ldstr, dbParameter.ParameterName);					// push parameter name

					// get the type of the contents of the list
					Type listType = prop.MemberType;
					if (listType.IsArray)
						listType = listType.GetElementType();
					else if (listType.IsGenericType)
						listType = listType.GetGenericArguments()[0];

					// emit the table type name and list type
					il.Emit(OpCodes.Ldstr, provider.GetTableParameterTypeName(command, dbParameter));
					il.EmitLoadType(listType);

					// get the value from the object
					il.Emit(OpCodes.Ldarg_1);
					prop.EmitGetValue(il);

					if (prop.MemberType.IsValueType)								// box value types before calling object parameter
						il.Emit(OpCodes.Box, prop.MemberType);

					// TODO: the table type parameter is already on the stack, so eliminate the above code
					il.Emit(OpCodes.Call, _listParameterHelperAddEnumerableParameters);
					continue;
				}

				///////////////////////////////////////////////////////////////
				// Start handling all of the other types
				///////////////////////////////////////////////////////////////

				// convert the parameters list to the parameter at the current index
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Call, typeof(IList).GetProperty("Item").GetGetMethod());
				il.Emit(OpCodes.Castclass, typeof(IDataParameter));

				// duplicate the parameter so we can call setvalue
				il.Emit(OpCodes.Dup);

				// for non-stored procedures, we don't know the desired type, so we have to set the type of the parameter to the type of the value
				if (command.CommandType != CommandType.StoredProcedure)
				{
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldc_I4, (int)sqlType);
					il.Emit(OpCodes.Call, _iDataParameterSetDbType);
				}

				///////////////////////////////////////////////////////////////
				// Get the value from the object onto the stack
				///////////////////////////////////////////////////////////////
				il.Emit(OpCodes.Ldarg_1);
				prop.EmitGetValue(il);

				// if this is a value type, then box the value so the compiler can check the type and we can call methods on it
				if (prop.MemberType.IsValueType)
					il.Emit(OpCodes.Box, prop.MemberType);

				// special conversions for timespan to datetime
				if ((sqlType == DbType.Time && dbParameter.DbType != DbType.Time) ||
					(dbParameter.DbType == DbType.DateTime || dbParameter.DbType == DbType.DateTime2 || dbParameter.DbType == DbType.DateTimeOffset))
				{
					IlHelper.EmitLdInt32(il, (int)dbParameter.DbType);
					il.Emit(OpCodes.Call, typeof(TypeConverterGenerator).GetMethod("ObjectToSqlDateTime"));
				}

				// if it's class type, boxed value type (in an object), or nullable, then we have to check for null
				Label readyToSetLabel = il.DefineLabel();
				if (!prop.MemberType.IsValueType || Nullable.GetUnderlyingType(prop.MemberType) != null)
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
					if (IsDbTypeAString(sqlType))
					{
						IlHelper.EmitLdInt32(il, 0);
						il.Emit(OpCodes.Stloc, stringLength);
					}

					// value is set to null. ready to set the property.
					il.Emit(OpCodes.Br_S, readyToSetLabel);

					// we know the value is not null
					il.MarkLabel(notNull);
				}

				///////////////////////////////////////////////////////////////
				// if this is a linq binary, convert it to a byte array
				///////////////////////////////////////////////////////////////
				if (prop.MemberType == typeof(System.Data.Linq.Binary))
				{
					il.Emit(OpCodes.Callvirt, _linqBinaryToArray);
				}
				else if (prop.MemberType == typeof(XmlDocument))
				{
					// we are sending up an XmlDocument. ToString just returns the classname, so use the outerxml.
					il.Emit(OpCodes.Callvirt, prop.MemberType.GetProperty("OuterXml").GetGetMethod());
				}
				else if (prop.MemberType == typeof(XDocument))
				{
					// we are sending up an XDocument. Use ToString.
					il.Emit(OpCodes.Callvirt, prop.MemberType.GetMethod("ToString", new Type[] { }));
				}
				else if (prop.MemberType == typeof(string))
				{
					// if this is a string, set the length property
					Label isLong = il.DefineLabel();
					Label lenDone = il.DefineLabel();

					// get the string length
					il.Emit(OpCodes.Dup);									// dup string
					il.Emit(OpCodes.Callvirt, _stringGetLength);			// call get length

					// compare to 4000
					IlHelper.EmitLdInt32(il, 4000);							// [string] [length] [4000]
					il.Emit(OpCodes.Cgt);									// [string] [0 or 1]
					il.Emit(OpCodes.Brtrue_S, isLong);

					// it is not longer than 4000, so store the value
					IlHelper.EmitLdInt32(il, 4000);							// [string] [4000]
					il.Emit(OpCodes.Br_S, lenDone);

					// it is longer than 4000, so store -1
					il.MarkLabel(isLong);
					IlHelper.EmitLdInt32(il, -1);							// [string] [-1]

					il.MarkLabel(lenDone);

					// store the length of the string in local so we can stick it in the parameter
					il.Emit(OpCodes.Stloc, stringLength);					// [string]
				}
				else if (prop.MemberType.GetInterfaces().Contains(typeof(IConvertible)) || prop.MemberType == typeof(object))
				{
					// if the type supports IConvertible, then let SQL convert it
					// if the type is object, we can't do anything, so let SQL attempt to convert it
				}
				else if (!TypeHelper.IsAtomicType(prop.MemberType))
				{
					// NOTE: at this point we know it's an object and the object reference is not null
					if (dbParameter.DbType == DbType.Xml)
					{
						// we have an object and it is expecting an xml parameter. let's serialize the object.
						il.EmitLoadType(prop.MemberType);
						il.Emit(OpCodes.Call, typeof(TypeHelper).GetMethod("SerializeObjectToXml", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(object), typeof(Type) }, null));
					}
					else if (dbParameter.DbType != DbType.Object)
					{
						// it's not a system type and it's not IConvertible, so let's add it as a string and let the data engine convert it.
						il.Emit(OpCodes.Callvirt, prop.MemberType.GetMethod("ToString", new Type[] { }));

						// it's possible that ToString returns a null
						Label internalValueIsNotNull = il.DefineLabel();

						// check to see if it's not null
						il.Emit(OpCodes.Dup);
						il.Emit(OpCodes.Brtrue_S, internalValueIsNotNull);

						// it's null. replace the value with DbNull
						il.Emit(OpCodes.Pop);
						il.Emit(OpCodes.Ldsfld, _dbNullValue);

						il.MarkLabel(internalValueIsNotNull);
					}
				}

				///////////////////////////////////////////////////////////////
				// p.Value = value
				///////////////////////////////////////////////////////////////
				// push parameter is at top of method
				// value is above
				il.MarkLabel(readyToSetLabel);
				il.Emit(OpCodes.Callvirt, _iDataParameterSetValue);

				///////////////////////////////////////////////////////////////
				// p.Size = string.length
				///////////////////////////////////////////////////////////////
				if (IsDbTypeAString(sqlType))
				{
					var endOfSize = il.DefineLabel();

					// don't set if 0
					il.Emit(OpCodes.Ldloc, stringLength);
					il.Emit(OpCodes.Brfalse_S, endOfSize);

					// parameter.setsize = string.length
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldloc, stringLength);
					il.Emit(OpCodes.Callvirt, _iDataParameterSetSize);

					il.MarkLabel(endOfSize);
				}

				il.Emit(OpCodes.Pop);					// pop (parameter)
			}

			// clean up any parameters that were not set
			il.Emit(OpCodes.Call, typeof(DbParameterGenerator).GetMethod("CleanParameters", BindingFlags.Static | BindingFlags.NonPublic));

			il.Emit(OpCodes.Ret);

			return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
		}

		/// <summary>
		/// Create a parameter generator for a dynamic object.
		/// </summary>
		/// <param name="command">The command to parse.</param>
		/// <returns>An action that fills in the command parameters from a dynamic object.</returns>
		static Action<IDbCommand, object> CreateDynamicInputParameterGenerator(IDbCommand command)
		{
			var provider = InsightDbProvider.For(command);

			// figure out what the parameters are
			var parameters = DeriveParameters(command);

			return (cmd, o) =>
			{
				// make sure that we have a dictionary implementation
				IDictionary<string, object> dyn = o as IDictionary<string, object>;
				if (dyn == null)
					throw new InvalidOperationException("Dynamic object must support IDictionary<string, object>.");

				foreach (var template in parameters)
				{
					var p = provider.CloneParameter(command, template);

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

		static IDataParameterCollection CopyParameters(IDbCommand command)
		{
			var provider = InsightDbProvider.For(command);

			// TODO: cache this
			var parameters = DbParameterGenerator.DeriveParameters(command);

			var commandParameters = command.Parameters;

			foreach (var p in parameters)
				commandParameters.Add(provider.CloneParameter(command, p));

			return commandParameters;
		}

		static void CleanParameters(IDataParameterCollection parameters)
		{
			// remove any input parameters that were not set
			// and set the size on any unset string parameters to -1 to make them valid
			for (int i = parameters.Count - 1; i >= 0; i--)
			{
				var p = (IDbDataParameter)parameters[i];

				if (p.Direction == ParameterDirection.Input)
					parameters.RemoveAt(i);
				else if (IsDbTypeAString(p.DbType))
					p.Size = -1;
			}
		}
		#endregion

		#region Output Parameter Code Generation Members
		/// <summary>
		/// Creates a converter from output parameters to an object of a given type.
		/// </summary>
		/// <param name="command">The command to analyze for the results.</param>
		/// <param name="type">The type to put the values into.</param>
		/// <returns>The converter method.</returns>
		static Action<IDbCommand, object> CreateClassOutputParameterConverter(IDbCommand command, Type type)
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

			foreach (var prop in ClassPropInfo.GetMappingForType(type))
			{
				if (!prop.Value.CanSetMember)
					continue;

				// if there is no parameter for this property, then skip it
				// if the parameter is not output, then skip it
				IDataParameter parameter = parameters.Find(p => p.ParameterName == prop.Key);
				if (parameter == null || !parameter.Direction.HasFlag(ParameterDirection.Output))
					continue;

				// push the object on the stack. we will need it to set the value below
				il.Emit(OpCodes.Ldarg_1);

				// get the parameter out of the collection
				il.Emit(OpCodes.Ldloc, localParameters);
				il.Emit(OpCodes.Ldstr, parameter.ParameterName);                 // push (parametername)
				il.Emit(OpCodes.Callvirt, _iDataParameterCollectionGetItem);

				// get the value out of the parameter
				il.Emit(OpCodes.Callvirt, _iDataParameterGetValue);

				// emit the code to convert the value and set it on the object
				Label finishLabel = TypeConverterGenerator.EmitConvertAndSetValue(il, _dbTypeToTypeMap[parameter.DbType], prop.Value);
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

		/// <summary>
		/// Determines if a given DbType represents a string.
		/// </summary>
		/// <param name="dbType">The dbType to test.</param>
		/// <returns>True if the type is a string type.</returns>
		internal static bool IsDbTypeAString(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return true;

				default:
					return false;
			}
		}
		#endregion

		#region ListParameterHelper Class
		/// <summary>
		/// Helps pack list parameters into a command.
		/// </summary>
		internal static class ListParameterHelper
		{
			/// <summary>
			/// Cache for Table-Valued Parameter schemas.
			/// </summary>
			private static ConcurrentDictionary<Tuple<string, Type>, ObjectReader> _tvpReaders = new ConcurrentDictionary<Tuple<string, Type>, ObjectReader>();

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
				// trim any prefixes
				if (tableTypeName.Count(c => c == '.') > 1)
					tableTypeName = tableTypeName.Split(new char[] { '.' }, 2)[1];

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
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "A use case of the library is to execute SQL.")]
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
				// TODO: make this method accept a parameter instead of variables

				// convert any nullable types to their underlying type
				listType = Nullable.GetUnderlyingType(listType) ?? listType;

				// if the table type name is null, then default to the name of the class
				if (String.IsNullOrWhiteSpace(tableTypeName))
					tableTypeName = String.Format(CultureInfo.InstalledUICulture, "[{0}Table]", listType.Name);

				// see if we already have a reader for the given type and table type name
				// we can't use the schema cache because we don't have a schema yet
				var key = Tuple.Create<string, Type>(tableTypeName, listType);
				ObjectReader objectReader = _tvpReaders.GetOrAdd(
					key,
					k => command.Connection.ExecuteAndAutoClose(
						_ => null,
						(_, __) =>
						{
							using (var reader = InsightDbProvider.For(command).GetTableTypeSchema(command, tableTypeName))
								return ObjectReader.GetObjectReader(reader, listType);
						},
						CommandBehavior.Default));

				// create the structured parameter
				var param = InsightDbProvider.For(command).CreateTableValuedParameter(command, parameterName, tableTypeName);
				if (param == null)
					throw new InvalidOperationException("Database provider does not support table-valued parameters");
				param.Value = new ObjectListDbDataReader(objectReader, list);
				command.Parameters.Add(param);
			}
		}
		#endregion
	}
}
