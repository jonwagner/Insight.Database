using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Insight.Database.CodeGenerator;

namespace Insight.Database
{
    /// <summary>
    /// Implements a fast Expando object that does not support INotifyPropertyChanged.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "This is an interface imlpementation")]
    public sealed class FastExpando : DynamicObject, IDictionary<string, object>
    {
        #region Private Fields
        /// <summary>
        /// The internal data structure.
        /// </summary>
        private IDictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an expando from an object.
        /// </summary>
        /// <param name="value">The object to initialize with.</param>
        /// <returns>A FastExpando containing the public properties and fields of obj.</returns>
        public static FastExpando FromObject(object value)
        {
            // don't bother converting expandos
            FastExpando x = value as FastExpando;
            if (x != null)
                return x;

            return ExpandoGenerator.Convert(value);
        }
        #endregion

        #region DynamicObject Implementation
        /// <summary>
        /// Tries to set a member value.
        /// </summary>
        /// <param name="binder">The binder to use.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value was set.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            data[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Attempts to get a member value.
        /// </summary>
        /// <param name="binder">The binder to use.</param>
        /// <param name="result">The output result.</param>
        /// <returns>True if a member was returned.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            return data.TryGetValue(binder.Name, out result);
        }
        #endregion

        #region Combination Methods
        /// <summary>
        /// Expand this expando with the members of another expando.
        /// </summary>
        /// <param name="expando">The other expando.</param>
        /// <returns>This expanded FastExpando.</returns>
        public FastExpando Expand(FastExpando expando)
        {
            if (expando == null) throw new ArgumentNullException("expando");

            foreach (var pair in expando)
                data[pair.Key] = pair.Value;

            return this;
        }

        /// <summary>
        /// Expand this expando with the public members of another object.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>This expanded FastExpando.</returns>
        public FastExpando Expand(object other)
        {
            if (other == null) throw new ArgumentNullException("other");

            return Expand(FastExpando.FromObject(other));
        }
        #endregion

        #region IDictionary<string,object> Members
        /// <summary>
        /// Gets or sets the value for a given key.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <returns>The value.</returns>
        public object this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");

                return data[key];
            }

            set
            {
                if (key == null) throw new ArgumentNullException("key");

                data[key] = value;
            }
        }

        /// <summary>
        /// Add an object to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        void IDictionary<string, object>.Add(string key, object value)
        {
            if (key == null) throw new ArgumentNullException("key");

            data[key] = value;
        }

        /// <summary>
        /// Tests whether the expando contains a key.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>True if the key is in the expando.</returns>
        bool IDictionary<string, object>.ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return data.ContainsKey(key);
        }

        /// <summary>
        /// Gets the keys in the expando.
        /// </summary>
        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return data.Keys; }
        }

        /// <summary>
        /// Removes an item from the expando.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>True if the item existed.</returns>
        bool IDictionary<string, object>.Remove(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return data.Remove(key);
        }

        /// <summary>
        /// Try to get a value in the expando.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if an item was found.</returns>
        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            if (key == null) throw new ArgumentNullException("key");

            return data.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the values in the expando.
        /// </summary>
        ICollection<object> IDictionary<string, object>.Values
        {
            get { return data.Values; }
        }
        #endregion

        #region ICollection<KeyValuePair<string,object>> Members
        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            data.Add(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            data.Clear();
        }

        /// <summary>
        /// Determines if the collection contains an item.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>True if the item is in the collection.</returns>
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return data.Contains(item);
        }

        /// <summary>
        /// Copies a values to an array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to copy to.</param>
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return data.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is readonly.
        /// </summary>
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item from the expando.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item existed.</returns>
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return data.Remove(item);
        }
        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members
        /// <summary>
        /// Returns an enumerator on the fields.
        /// </summary>
        /// <returns>An enumerator on the fields.</returns>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator on the fields.
        /// </summary>
        /// <returns>The enumerator on the fields.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        #endregion

        #region Transforms
        /// <summary>
        /// Modifies the FastExpando by mapping the fields given the map.
        /// </summary>
        /// <param name="map">The map of input fields to output fields.</param>
        public void Mutate(IDictionary<string, string> map)
        {
            if (map == null) throw new ArgumentNullException("map");

            foreach (KeyValuePair<string, string> pair in map)
            {
                string key = pair.Key;

                object value;
                if (data.TryGetValue(key, out value))
                {
                    data.Remove(key);
                    data.Add(pair.Value, value);
                }
            }
        }

        /// <summary>
        /// Copies the FastExpando while mapping the fields given the map.
        /// </summary>
        /// <param name="map">The map of input fields to output fields.</param>
        /// <returns>A modified copy of the expando.</returns>
        public FastExpando Transform(IDictionary<string, string> map)
        {
            FastExpando other = new FastExpando();
            foreach (var pair in data)
                other.data.Add(pair.Key, pair.Value);

            // mutate the results
            other.Mutate(map);

            return other;
        }

        /// <summary>
        /// Drops the specified field from the expando if it is null.
        /// </summary>
        /// <param name="fields">The name of the fields to drop.</param>
        /// <returns>A new expando.</returns>
        public FastExpando WithoutNulls(params string[] fields)
        {
            return WithoutNulls((IEnumerable<string>)fields);
        }

        /// <summary>
        /// Drops the specified field from the expando if it is null.
        /// </summary>
        /// <param name="fields">The name of the field(s) to drop or null to drop all fields.</param>
        /// <returns>A new expando.</returns>
        public FastExpando WithoutNulls(IEnumerable<string> fields = null)
        {
            FastExpando other = new FastExpando();
            foreach (var pair in data)
            {
                if (pair.Value == null && (fields == null || fields.Any(f => String.Compare(pair.Key, f, StringComparison.OrdinalIgnoreCase) == 0)))
                    continue;

                other.data.Add(pair.Key, pair.Value);
            }

            return other;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        private void SetValue(string name, object value)
        {
            data[name] = value;
        }
        #endregion
    }
}
