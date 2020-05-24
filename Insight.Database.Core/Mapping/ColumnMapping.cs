using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Mapping;
using Insight.Database.Providers;
using Insight.Database.Structure;

namespace Insight.Database
{
    /// <summary>
    /// A singleton class that handles the mapping operations from recordsets to objects.
    /// </summary>
    public static class ColumnMapping
    {
        #region Internal Fields
        /// <summary>
        /// The singleton instance of the ColumnMapping configuration for Tables and Table Valued Parameters.
        /// </summary>
        private static MappingCollection<IColumnMapper> _tables = new MappingCollection<IColumnMapper>(BindChildrenFor.Tables);

        /// <summary>
        /// The singleton instance of the ColumnMapping configuration for Parameters.
        /// </summary>
        private static MappingCollection<IParameterMapper> _parameters = new MappingCollection<IParameterMapper>(BindChildrenFor.Parameters);

        /// <summary>
        /// The singleton instance of the AllColumnMapping configuration for Parameters.
        /// </summary>
        private static DualMappingCollection _all = new DualMappingCollection(_parameters, _tables);

        /// <summary>
        /// The singleton instance of the ColumnMapping configuration for Parameters.
        /// </summary>
        private static MappingCollection<IParameterDataTypeMapper> _parameterDataTypes = new MappingCollection<IParameterDataTypeMapper>(BindChildrenFor.Parameters);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the singleton instance of the ColumnMapping configuration for Tables and Table Valued Parameters.
        /// </summary>
        public static MappingCollection<IColumnMapper> Tables { get { return _tables; } }

        /// <summary>
        /// Gets the singleton instance of the ColumnMapping configuration for Parameters.
        /// </summary>
        public static MappingCollection<IParameterMapper> Parameters { get { return _parameters; } }

        /// <summary>
        /// Gets the singleton instance of the ColumnMapping configuration for both Tables and Parameters.
        /// </summary>
        public static MappingCollection<IDualMapper> All { get { return _all; } }

        /// <summary>
        /// Gets the singleton instance of the ColumnMapping configuration for Parameters and altering their Data Types.
        /// </summary>
        public static MappingCollection<IParameterDataTypeMapper> ParameterDataTypes { get { return _parameterDataTypes; } }
        #endregion

        #region Parameter Mapping Methods
        /// <summary>
        /// Maps the parameters for a command.
        /// </summary>
        /// <param name="type">The type being bound.</param>
        /// <param name="command">The command being executed.</param>
        /// <param name="parameters">The parameters to bind.</param>
        /// <returns>The mapping for the parameters.</returns>
        internal static List<FieldMapping> MapParameters(Type type, IDbCommand command, IEnumerable<IDataParameter> parameters)
        {
            return parameters.Select(p => MapParameter(type, command, p)).ToList();
        }

        /// <summary>
        /// Maps the columns of a reader to a type.
        /// </summary>
        /// <param name="type">The type being bound.</param>
        /// <param name="reader">The reader being bound.</param>
        /// <param name="startColumn">The index of the first column to bind.</param>
        /// <param name="columnCount">The number of columns to bind.</param>
        /// <param name="overrideMapping">An optional column mapper to override the operation</param>
        /// <returns>The mappings for the columns</returns>
        internal static List<FieldMapping> MapColumns(Type type, IDataReader reader, int startColumn = 0, int? columnCount = null, IColumnMapper overrideMapping = null)
        {
            var array = Enumerable.Range(startColumn, columnCount ?? reader.FieldCount)
                .Select(i => MapColumn(type, reader, i, overrideMapping))
                .ToArray();

            // remove duplicates from the list
            var usedFields = new HashSet<ClassPropInfo>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    continue;

                if (usedFields.Contains(array[i].Member))
                    array[i] = null;
                else
                    usedFields.Add(array[i].Member);
            }

            return array.ToList();
        }

        /// <summary>
        /// Maps a parameter data type to a new data type, if no mappings are available, returns the original best guess data type.
        /// </summary>
        /// <param name="type">The type being bound.</param>
        /// <param name="command">The command being bound.</param>
        /// <param name="parameter">The parameter being bound.</param>
        /// <param name="dbType">The best guess data type.</param>
        /// <returns>The data type to assign when sending the parameter to the server.</returns>
        internal static DbType MapParameterDataType(Type type, IDbCommand command, IDataParameter parameter, DbType dbType)
        {
            if (!ParameterDataTypes.Mappers.Any())
                return dbType;

            return ParameterDataTypes
                .Mappers
                .Aggregate(
                    dbType,
                    (previousDbType, parameterTypeMapper) => parameterTypeMapper.MapParameterType(type, command, parameter, previousDbType));
        }

        /// <summary>
        /// Maps a parameters for a command.
        /// </summary>
        /// <param name="type">The type being bound.</param>
        /// <param name="command">The command being executed.</param>
        /// <param name="parameter">The parameter to bind.</param>
        /// <returns>The mapping for the parameters.</returns>
        private static FieldMapping MapParameter(Type type, IDbCommand command, IDataParameter parameter)
        {
            // allow for a custom mapper - the first one that doesn't return null
            var fieldName = Parameters.Mappers
                                        .Select(m => m.MapParameter(type, command, parameter))
                                        .Where(m => m != null)
                                        .FirstOrDefault();

            // by default, let all of the transforms transform the name, then search for it
            if (fieldName == null)
            {
                var parameterName = Parameters.Transform(type, parameter.ParameterName);
                fieldName = ClassPropInfo.SearchForMatchingField(type, parameterName, Parameters);
            }

            if (fieldName == null)
                return null;

            var member = ClassPropInfo.FindMember(type, fieldName);
            return new FieldMapping(fieldName, member, DbSerializationRule.GetSerializer(command, parameter, member));
        }

        /// <summary>
        /// Maps a column of a reader to a type.
        /// </summary>
        /// <param name="type">The type being bound.</param>
        /// <param name="reader">The reader being bound.</param>
        /// <param name="column">The column to bind.</param>
        /// <param name="overrideMapping">An optional column mapper to override the operation</param>
        /// <returns>The mapping for the column</returns>
        private static FieldMapping MapColumn(Type type, IDataReader reader, int column, IColumnMapper overrideMapping)
        {
            string fieldName = null;

            // if an override mapping was used, then allow it the first go at the mapping
            if (overrideMapping != null)
                fieldName = overrideMapping.MapColumn(type, reader, column);

            // allow for custom mappings
            if (fieldName == null)
                fieldName = Tables.Mappers.Select(m => m.MapColumn(type, reader, column)).FirstOrDefault();

            // allow the first column to match the * wildcard on Guardian records
            if (fieldName == null)
            {
                var wildcards = ClassPropInfo.GetMembersForType(type).Where(m => m.ColumnName.StartsWith("*", StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.ColumnName).ToList();
                if (column < wildcards.Count)
                    fieldName = wildcards[column].Name;
            }

            // by default, let all of the transforms transform the name, then search for it
            if (fieldName == null)
            {
                var columnName = reader.GetName(column);
                if (columnName != null)
                    columnName = Tables.Transform(type, columnName);
                fieldName = ClassPropInfo.SearchForMatchingField(type, columnName, Tables);
            }

            if (fieldName == null)
                return null;

            var member = ClassPropInfo.FindMember(type, fieldName);
            if (member == null)
                return null;

            return new FieldMapping(fieldName, member, DbSerializationRule.GetSerializer(reader, column, member));
        }
        #endregion
    }
}
