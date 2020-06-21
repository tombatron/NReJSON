using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NReJSON
{
    /// <summary>
    /// This type was introduced specifically for the result of the JsonIndexGet(Async) methods.
    /// 
    /// Since those methods return a dictionary of IEnumerables I figured it'd be nice if you 
    /// had the option to iterate over all of the values without having to directly interact with
    /// the dictionary, or access the underlying values by specifying the keys.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class IndexedCollection<TResult> : IEnumerable<TResult>
    {
        private readonly IDictionary<string, IEnumerable<TResult>> _wrappedResult;

        internal IndexedCollection(IDictionary<string, IEnumerable<TResult>> wrappedResult) =>
            _wrappedResult = wrappedResult;

        /// <summary>
        /// Provides a way to access the underlying values by key (just like a regular dictionary).
        /// </summary>
        public IEnumerable<TResult> this[string key] => _wrappedResult[key];

        /// <summary>
        /// Provides a way to access the underlying keys in the wrapped dictionary.
        /// </summary>
        public IEnumerable<string> Keys => _wrappedResult.Keys;

        IEnumerator IEnumerable.GetEnumerator() =>
            InnerIterator().GetEnumerator();

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() =>
            InnerIterator().GetEnumerator();

        private IEnumerable<TResult> InnerIterator() =>
            _wrappedResult.Values.SelectMany(x => x);
    }
}