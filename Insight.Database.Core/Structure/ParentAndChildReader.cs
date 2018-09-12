using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Holds parent and child data so they can be read together.
	/// </summary>
	/// <typeparam name="T">The type of root object that is being returned.</typeparam>
	/// <typeparam name="TChild">The type of child object that is being returned.</typeparam>
	class ParentAndChild<T, TChild>
	{
		/// <summary>
		/// Gets or sets the parent object.
		/// </summary>
		public T Parent { get; set; }

		/// <summary>
		/// Gets or sets the child object.
		/// </summary>
		public TChild Child { get; set; }
	}

	/// <summary>
	/// Holds parent and child data so they can be read together with a guardian ID.
	/// </summary>
	/// <typeparam name="T">The type of root object that is being returned.</typeparam>
	/// <typeparam name="TChild">The type of child object that is being returned.</typeparam>
	/// <typeparam name="TId">The type of linking guardian ID.</typeparam>
	class ParentAndChildWithGuardian<T, TChild, TId>
	{
		/// <summary>
		/// Gets or sets the ID for linking.
		/// </summary>
		[Column("*1")]
		public TId ID { get; set; }

		/// <summary>
		/// Gets or sets the parent object.
		/// </summary>
		public T Parent { get; set; }

		/// <summary>
		/// Gets or sets the child object.
		/// </summary>
		public TChild Child { get; set; }
	}

	/// <summary>
	/// Represents a record mapping that combines parent and child records together.
	/// </summary>
	/// <typeparam name="T">The type of root object that is being returned.</typeparam>
	/// <typeparam name="TChild">The type of child object that is being returned.</typeparam>
	public class Together<T, TChild> : RecordReader<T>
	{
		#region Static Members
		/// <summary>
		/// Gets the static definition of this record type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly RecordReader<T> Records = new Together<T, TChild>();
		#endregion

		#region Private Members
		private Func<T, Object> _parentID;
		private Action<T, List<TChild>> _listSetter;
		private Func<T, ICollection<TChild>> _listGetter;
		private string _parentIDName;
		private string _into;
		private static RecordReader<ParentAndChild<T, TChild>> _parentAndChildRecords = OneToOne<ParentAndChild<T, TChild>, T, TChild>.Records;
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes static members of the Together class.
		/// </summary>
		/// <param name="parentIDName">The name of the field containing the ID in the parent class. Defaults to autodetect.</param>
		/// <param name="into">The name of the field list of the child objects in the parent class. Defaults to autodetect.</param>
		public Together(string parentIDName = null, string into = null)
		{
			_parentIDName = parentIDName;
			_into = into;

			_parentID = ChildMapperHelper.GetIDAccessor(typeof(T), parentIDName).CreateGetMethod<T, Object>();
			_listSetter = ChildMapperHelper.GetListAccessor(typeof(T), typeof(TChild), into, setter: true).CreateSetMethod<T, List<TChild>>();
			_listGetter = ChildMapperHelper.GetListAccessor(typeof(T), typeof(TChild), into, setter: false).CreateGetMethod<T, ICollection<TChild>>();
		}
		#endregion

		#region Properties
		/// <inheritdoc/>
		public override bool RequiresDeduplication { get { return true; } }
		#endregion

		#region Implementation
		/// <inheritdoc/>
        public override Func<IDataReader, T> GetRecordReader(IDataReader reader)
		{
			var oneToOneReader = _parentAndChildRecords.GetRecordReader(reader);

			// this parents keeps track of the unique parents as we go. the closure, and therefore, the dictionary, will last only while reading the query.
			var parents = new Dictionary<Object, T>();

			return r =>
			{
				var record = oneToOneReader(r);
				var parent = record.Parent;
				var child = record.Child;

				// see if we have seen the parent before
				var key = _parentID(parent);
				if (parents.ContainsKey(key))
					parent = parents[key];
				else
					parents.Add(key, parent);

				// get the collection of children
				var childList = _listGetter(parent);
				if (childList == null)
				{
					var list = new List<TChild>();
					childList = list;
					_listSetter(parent, list);
				}

				// stick the child into the parent
				childList.Add(child);

				return parent;
			};
		}

		/// <inheritdoc/>
		public override IChildRecordReader<T, TId> GroupByColumn<TId>()
        {
            return new ChildRecordReader<ParentAndChildWithGuardian<T, TChild, TId>, TId, T>(
				OneToOne<ParentAndChildWithGuardian<T, TChild, TId>, T, TChild>.Records,
				records =>
				{
					var parents = new Dictionary<Object, ParentAndChildWithGuardian<T, TChild, TId>>();

					foreach (var record in records)
					{
						var r = record;
						var parent = record.Parent;
						var child = record.Child;

						var key = _parentID(parent);
						if (parents.ContainsKey(key))
						{
							// we've seen this ID before. use the previous parent.
							r = parents[key];
							parent = r.Parent;
						}
						else
						{
							// this is the first time we've seen the parent. make a new list and save the parent.
							_listSetter(parent, new List<TChild>());
							parents.Add(key, r);
						}

						// stick the child into the parent
						var childList = _listGetter(parent);
						childList.Add(child);
					}

					return parents.Values.GroupBy(g => g.ID, g => g.Parent);
				});
        }

		/// <inheritdoc/>
		public override bool Equals(IRecordReader other)
		{
			var o = other as Together<T, TChild>;
			if (o == null)
				return false;

			return this._parentIDName == o._parentIDName && this._into == o._into;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int hashCode = 17;

			unchecked
			{
				hashCode += _parentIDName?.GetHashCode() ?? 0;
				hashCode *= 23;
				hashCode += _into?.GetHashCode() ?? 0;
			}

			return hashCode;
		}
		#endregion
	}
}
