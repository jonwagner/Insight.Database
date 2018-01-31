using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.CodeGenerator;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
    /// <summary>
    /// A list reader that postprocesses the records and implements self-references.
    /// </summary>
    /// <typeparam name="T">The type of object to read from each record in the reader.</typeparam>
    public class SelfReferencingListReader<T> : ListReader<T>
    {
        /// <summary>
        /// The default function that selects IDs from the parent object.
        /// </summary>
        private static Lazy<Func<T, Object>> _defaultIDSelector = new Lazy<Func<T, Object>>(GetIDSelector);

        /// <summary>
        /// The default function that selects IDs from the parent object.
        /// </summary>
        private static Lazy<Func<T, Object>> _defaultParentIDSelector = new Lazy<Func<T, Object>>(GetParentIDSelector);

        /// <summary>
        /// The default action that sets the children into the proper parent.
        /// </summary>
        private Lazy<Action<T, T>> _defaultAssigner = new Lazy<Action<T, T>>(GetAssigner);

        private ListReader<T> previous;
        private Func<T, Object> idSelector;
        private Func<T, Object> parentIdSelector;
        private Action<T, T> assigner;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ListReader class.
        /// </summary>
        public SelfReferencingListReader(
            ListReader<T> previous,
            Func<T, Object> idSelector,
            Func<T, Object> parentIdSelector,
            Action<T, T> assigner)
        {
            this.previous = previous;
            this.idSelector = idSelector ?? _defaultIDSelector.Value;
            this.parentIdSelector = parentIdSelector ?? _defaultParentIDSelector.Value;
            this.assigner = assigner ?? _defaultAssigner.Value;
        }
        #endregion

        #region Implementation
        /// <inheritdoc/>
        public override IList<T> Read(IDbCommand command, IDataReader reader)
        {
            var results = previous.Read(command, reader);
            PostProcess(results);
            ReadChildren(reader, results);
            return results;
        }

        /// <inheritdoc/>
        public override async Task<IList<T>> ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
        {
            var results = await previous.ReadAsync(command, reader, cancellationToken);
            PostProcess(results);
            await ReadChildrenAsync(reader, results, cancellationToken);
            return results;
        }

        private void PostProcess(IList<T> results)
        {
            Dictionary<Object, T> map = results.ToDictionary<T, Object>(idSelector);

            foreach (T t in results)
            {
                T parent;
                if (map.TryGetValue(parentIdSelector(t), out parent))
                    assigner(t, parent);
            }
        }
        #endregion

        #region Automatic Methods
        /// <summary>
        /// Gets the ID selector from the class, looking for ID, classID, and then anything with xxxID.
        /// </summary>
        /// <returns>An accessor for the ID field.</returns>
        private static Func<T, Object> GetIDSelector()
        {
            return ChildMapperHelper.GetIDAccessor(typeof(T)).CreateGetMethod<T, Object>();
        }

        /// <summary>
        /// Gets the ID selector from the class, looking for ID, classID, and then anything with xxxID.
        /// </summary>
        /// <returns>An accessor for the ID field.</returns>
        private static Func<T, Object> GetParentIDSelector()
        {
            return ChildMapperHelper.FindParentIDAccessor(typeof(T), null, typeof(T)).CreateGetMethod<T, Object>();
        }

        /// <summary>
        /// Gets the list setter for the class, looking for an IList that matches the type.
        /// </summary>
        /// <returns>An accessor for the ID field.</returns>
        private static Action<T, T> GetAssigner()
        {
            var members = ClassPropInfo.GetMembersForType(typeof(T)).Where(mi => mi.CanSetMember);
            var childTypeMembers = members.Where(m => m.MemberType == typeof(T));
            var member = childTypeMembers.SingleOrDefault(m => m.SetMethodInfo != null) ?? childTypeMembers.SingleOrDefault(m => m.FieldInfo != null);

            return member.CreateSetMethod<T, T>();
        }
        #endregion
    }
}
