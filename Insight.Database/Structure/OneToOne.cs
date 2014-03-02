using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T">The type of root object that is being returned.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class OneToOne<T> : RecordReader<T>, IRecordStructure, IChildRecordReader<T>
	{
		#region Properties
		/// <summary>
		/// Gets the static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly OneToOne<T> Records = new OneToOne<T>();

		/// <summary>
		/// The type of objects returned by this mapping.
		/// </summary>
		private static Type[] _objectTypes;

		/// <summary>
		/// The hash code for this mapping.
		/// </summary>
		private int _hashCode;

		/// <summary>
		/// An optional mapping of the name of ID columns that is used to split the recordset.
		/// </summary>
		private Dictionary<Type, string> _idColumns;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes static members of the OneToOne class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static OneToOne()
		{
			var recordset = typeof(T).GetCustomAttributes(true).OfType<RecordsetAttribute>().SingleOrDefault();
			if (recordset != null)
				_objectTypes = recordset.Types.ToArray();
			else
				_objectTypes = new Type[] { typeof(T) };
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		public OneToOne() : this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		public OneToOne(Action<T> callback = null, Dictionary<Type, string> idColumns = null)
		{
			Initialize(callback, idColumns);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets an optional callback that can be used to assemble the records.
		/// </summary>
		internal Delegate Callback { get; private set; }
		#endregion

		#region Methods
		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			if (other == null)
				return false;

			// they need to be the same type
			if (other.GetType() != GetType())
				return false;

			var o = (OneToOne<T>)other;

			if (o.Callback != Callback)
				return false;

			// validate that the columns are the same object
			if (_idColumns != o._idColumns)
			{
				// different objects, so we have to check the contents.
				// this is a performance hit, so you should pass in the same id mapping each time!
				var otherIdColumns = o._idColumns;

				// check the count first as a short-circuit
				if (_idColumns.Count != otherIdColumns.Count)
					return false;

				// check the id mappings individually
				foreach (var pair in _idColumns)
				{
					string otherID;
					if (!otherIdColumns.TryGetValue(pair.Key, out otherID))
						return false;

					if (pair.Value != otherID)
						return false;
				}

				return false;
			}

			return true;
		}

		/// <inheritdoc/>
		Type[] IRecordStructure.GetObjectTypes()
		{
			return _objectTypes;
		}

		/// <inheritdoc/>
		Dictionary<Type, string> IRecordStructure.GetSplitColumns() { return _idColumns; }

		/// <summary>
		/// Returns a function that can read a single record from a given data reader.
		/// </summary>
		/// <param name="reader">The reader that will be read.</param>
		/// <returns>A function that can read a single record.</returns>
		public override Func<IDataReader, T> GetRecordReader(IDataReader reader)
		{
			if (Callback != null)
			{
				var mapper = DbReaderDeserializer.GetDeserializerWithCallback<T>(reader, this);
				return r => mapper(r, HandleCallback);
			}
			else
				return DbReaderDeserializer.GetDeserializer<T>(reader, this);
		}

		/// <summary>
		/// Returns the definition for a guardian record containing a ParentID and a child object.
		/// </summary>
		/// <typeparam name="TId">The type of the ID field.</typeparam>
		/// <returns>The record definition.</returns>
		public virtual IRecordReader<Guardian<T, TId>> GetGuardianReader<TId>()
		{
			return OneToOne<Guardian<T, TId>, T>.Records;
		}

		/// <summary>
		/// Initializes a new instance of the OneToOne class.
		/// </summary>
		/// <param name="callback">An optional callback that can be used to assemble the records.</param>
		/// <param name="idColumns">An optional map of the names of ID columns that can be used to split the recordset.</param>
		protected void Initialize(Delegate callback, Dictionary<Type, string> idColumns)
		{
			Callback = callback;
			_idColumns = idColumns;

			unchecked
			{
				_hashCode = GetType().GetHashCode();
				_hashCode *= 13;
				if (callback != null)
					_hashCode += callback.GetHashCode();
			}
		}

		/// <summary>
		/// Handles a callback to assemble the objects.
		/// </summary>
		/// <param name="objects">The objects read from the record.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "objects")]
		protected virtual void HandleCallback(object[] objects)
		{
			if (objects == null) throw new ArgumentNullException("objects");

			((Action<T>)Callback)((T)objects[0]);
		}
		#endregion
	}

	/// <summary>
	/// Represents a one-to-one object mapping that is returned in a single recordset.
	/// </summary>
	/// <typeparam name="T">The type of root object that is being returned.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "These are related generic classes.")]
	public class Some<T> : OneToOne<T>
	{
	}
}