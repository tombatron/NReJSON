using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NReJSON
{
    public class IndexedCollection<TResult> : IEnumerable<TResult>
    {
        private readonly IDictionary<string, IEnumerable<TResult>> _wrappedResult;

        public IndexedCollection(IDictionary<string, IEnumerable<TResult>> wrappedResult) =>
            _wrappedResult = wrappedResult;

        public IEnumerable<TResult> this[string key] => _wrappedResult[key];

        IEnumerator IEnumerable.GetEnumerator() =>
            InnerIterator().GetEnumerator();

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() =>
            InnerIterator().GetEnumerator();

        private IEnumerable<TResult> InnerIterator() =>
            _wrappedResult.Values.SelectMany(x => x);
    }
}