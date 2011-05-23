using System.Collections;
using System.Collections.Generic;

namespace RemoteSessionState.Interop
{
    public class CollectionEnumerator : IEnumerator<KeyValuePair<string, object>>
    {
        private readonly dynamic _collection;
        private int _index;

        public CollectionEnumerator(dynamic collection)
        {
            _collection = collection;
        }

        public bool MoveNext()
        {
            return _index++ < _collection.Contents.Count;
        }

        public void Reset()
        {
            _index = 0;
        }

        public KeyValuePair<string, object> Current
        {
            get { return new KeyValuePair<string, object>(
                            _collection.Contents.Key(_index), 
                            _collection.Contents.Item(_index)); }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose() { }
    }
}