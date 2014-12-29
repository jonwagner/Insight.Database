using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Serialization
{
    /// <summary>
    /// Allows serialization of a Boolean to other database representations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class BooleanSerializer : DbObjectSerializer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BooleanSerializer class.
        /// </summary>
        /// <param name="dbType">The type to store the value in the database.</param>
        /// <param name="trueValue">The representation of True.</param>
        /// <param name="falseValue">The representation of False.</param>
        public BooleanSerializer(DbType dbType, object trueValue, object falseValue)
        {
            DbType = dbType;
            TrueValue = trueValue;
            FalseValue = falseValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the DbType to use to store the value in the database.
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Gets the representation of True.
        /// </summary>
        public object TrueValue { get; private set; }

        /// <summary>
        /// Gets the representation of False.
        /// </summary>
        public object FalseValue { get; private set; }
        #endregion

        /// <inheritdoc/>
        public override bool CanDeserialize(Type sourceType, Type targetType)
        {
            return targetType == typeof(bool) || targetType == typeof(bool?);
        }

        /// <inheritdoc/>
        public override bool CanSerialize(Type type, DbType dbType)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        /// <inheritdoc/>
        public override DbType GetSerializedDbType(Type type, DbType dbType)
        {
            return DbType;
        }

        /// <inheritdoc/>
        public override object SerializeObject(Type type, object o)
        {
            bool? b = (bool?)o;
            if (b == null)
                return null;

            return b.Value ? TrueValue : FalseValue;
        }

        /// <inheritdoc/>
        public override object DeserializeObject(Type type, object encoded)
        {
            if (encoded == null)
                return null;

            if (encoded is string)
            {
                var s = encoded.ToString();
                if (String.Compare(s, TrueValue.ToString(), true) == 0)
                    return true;
                if (String.Compare(s, FalseValue.ToString(), true) == 0)
                    return false;
            }
            else
            {
                var value = Convert.ChangeType(encoded, type);
                if (value == TrueValue)
                    return true;
                else if (value == FalseValue)
                    return false;
            }

            throw new InvalidOperationException(String.Format("Value {0} could not be converted to bool.", encoded));
        }
    }

    /// <summary>
    /// Serializes booleans as "Y" or "N" in the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class BooleanYNSerializer : BooleanSerializer
    {
        /// <summary>
        /// Initializes a new instance of the BooleanYNSerializer class.
        /// </summary>
        public BooleanYNSerializer() : base(DbType.String, "Y", "N")
        {
        }
    }

    /// <summary>
    /// Serializes booleans as "T" or "F" in the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class BooleanTFSerializer : BooleanSerializer
    {
        /// <summary>
        /// Initializes a new instance of the BooleanTFSerializer class.
        /// </summary>
        public BooleanTFSerializer() : base(DbType.String, "T", "F")
        {
        }
    }

    /// <summary>
    /// Serializes booleans as "True" or "False" in the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class BooleanTrueFalseSerializer : BooleanSerializer
    {
        /// <summary>
        /// Initializes a new instance of the BooleanTrueFalseSerializer class.
        /// </summary>
        public BooleanTrueFalseSerializer() : base(DbType.String, "True", "False")
        {
        }
    }

    /// <summary>
    /// Serializes booleans as 1 or 0 in the database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
    public class Boolean10Serializer : BooleanSerializer
    {
        /// <summary>
        /// Initializes a new instance of the Boolean10Serializer class.
        /// </summary>
        public Boolean10Serializer() : base(DbType.Int32, 1, 0)
        {
        }
    }
}
