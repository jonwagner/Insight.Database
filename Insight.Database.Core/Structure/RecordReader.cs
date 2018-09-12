using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;

namespace Insight.Database.Structure
{
    /// <summary>
    /// A base implementation of IRecordReader.
    /// </summary>
    /// <typeparam name="T">The type of object that can be read.</typeparam>
    public abstract partial class RecordReader<T> : IRecordReader<T>
    {
        /// <summary>
        /// Stores the functions that autogroup the given type.
        /// </summary>
        private static ConcurrentDictionary<Tuple<Type, Type>, object> _autoGroupers = new ConcurrentDictionary<Tuple<Type, Type>, object>();

		/// <inheritdoc/>
		public virtual bool RequiresDeduplication { get { return false; } }

        /// <inheritdoc/>
        public abstract Func<IDataReader, T> GetRecordReader(IDataReader reader);

        /// <inheritdoc/>
        public abstract bool Equals(IRecordReader other);

        /// <summary>
        /// Gets a recordreader that can read the guardian of this object.
        /// </summary>
        /// <typeparam name="TGuardian">The type of the guardian.</typeparam>
        /// <returns>A reader.</returns>
        public virtual IRecordReader<TGuardian> GetGuardianReader<TGuardian>() where TGuardian : Guardian<T>, new()
        {
            return OneToOne<TGuardian, T>.Records;
        }

        /// <summary>
        /// Returns a child record reader that reads this type of record and groups by the given function.
        /// </summary>
        /// <typeparam name="TId">The type of the ID.</typeparam>
        /// <param name="grouping">The function that gets the ID to group by.</param>
        /// <returns>A child record reader.</returns>
        public virtual IChildRecordReader<T, TId> GroupBy<TId>(Func<T, TId> grouping)
        {
            return new ChildRecordReader<T, TId, T>(this, records => records.GroupBy(grouping, r => r));
        }

        /// <summary>
        /// Returns a child record reader that reads this type of record and groups by the first column in the recordset.
        /// </summary>
        /// <typeparam name="TId">The type of the ID in the recordset.</typeparam>
        /// <returns>A child record reader.</returns>
        public virtual IChildRecordReader<T, TId> GroupByColumn<TId>()
        {
            return new ChildRecordReader<Guardian<T, TId>, TId, T>(
									GetGuardianReader<Guardian<T, TId>>(),
									records => records.GroupBy(g => g.ParentId1, g => g.Object));
        }

        /// <summary>
        /// Returns a child record reader that reads this type of record, and automatically groups by columns that match the parent id.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <typeparam name="TId">The type of the ID.</typeparam>
        /// <returns>A child record reader.</returns>
        internal IChildRecordReader<T, TId> GroupByAuto<TParent, TId>()
        {
            var key = Tuple.Create(typeof(TParent), typeof(TId));

            return (IChildRecordReader<T, TId>)_autoGroupers.GetOrAdd(key, t => CreateAutoGroupBy<TId>(t.Item1));
        }

        /// <summary>
        /// Gets the type of a guardian that contains the given number of items.
        /// </summary>
        /// <param name="count">The number of items.</param>
        /// <returns>The guardian type.</returns>
        private static Type GetGuardianType(int count)
        {
            switch (count)
            {
                case 1: return typeof(Guardian<>);
                case 2: return typeof(Guardian<,>);
                case 3: return typeof(Guardian<,,>);
                case 4: return typeof(Guardian<,,,>);
                case 5: return typeof(Guardian<,,,,>);
                case 6: return typeof(Guardian<,,,,,>);
                case 7: return typeof(Guardian<,,,,,,>);
                case 8: return typeof(Guardian<,,,,,,,>);
                default:
                    throw new ArgumentException("Too many child levels.");
            }
        }

        /// <summary>
        /// Creates a child record reader that autogroups the given type, based on the parent's ids.
        /// </summary>
        /// <typeparam name="TId">The type of the ID.</typeparam>
        /// <param name="parentType">The type of the parent.</param>
        /// <returns>A child record reader.</returns>
        private IChildRecordReader<T, TId> CreateAutoGroupBy<TId>(Type parentType)
        {
            // if we can detect the parent ids from the child class, then use that to do the mapping
            var idAccessor = ChildMapperHelper.FindParentIDAccessor(typeof(T), null, parentType);
            if (idAccessor != null)
            {
                var getid = idAccessor.CreateGetMethod<T, TId>();
                return new ChildRecordReader<T, TId, T>(this, records => records.GroupBy(getid, r => r));
            }
            else
            {
                List<Type> guardianTypes = new List<Type>();

                // if a selector was specified, use it, else use the parent's ID accessor to define external columns to use as the key
                if (typeof(TId) != typeof(object))
                {
                    if (typeof(TId).Name.StartsWith("Tuple`", StringComparison.OrdinalIgnoreCase))
                        guardianTypes.AddRange(typeof(TId).GetGenericArguments());
                    else
                        guardianTypes.Add(typeof(TId));
                }
                else
                {
                    guardianTypes.AddRange(ChildMapperHelper.GetIDAccessor(parentType).MemberTypes);
                }

                guardianTypes.Insert(0, typeof(T));
                var guardianType = GetGuardianType(guardianTypes.Count).MakeGenericType(guardianTypes.ToArray());

                var getReader = this.GetType().GetMethod("GetGuardianReader").MakeGenericMethod(guardianType);

                var reader = (IRecordReader<Guardian<T>>)getReader.Invoke(this, Parameters.EmptyArray);

                return new ChildRecordReader<Guardian<T>, TId, T>(reader, records => records.GroupBy(g => (TId)g.GetID(), g => g.Object));
            }
        }
    }
}
