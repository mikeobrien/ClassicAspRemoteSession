using System.Collections;
using System.Collections.Generic;
using RemoteSessionState.Interop;

namespace Tests.Common.TestDoubles
{
    public class SessionState : ISessionState
    {
        private readonly IDictionary<string, object> _items;

        public SessionState(IDictionary<string, object> sessionState)
        {
            if (sessionState != null) _items = sessionState;
            else _items = new Dictionary<string, object>();
        }

        public object this[string name]
        {
            get { return _items.ContainsKey(name) ? _items[name] : null; }
            set { _items[name] = value; }
        }

        public void Remove(string name)
        {
            _items.Remove(name);
        }

        public void RemoveAll()
        {
            _items.Clear();
        }

        public void Abandon()
        {
            RemoveAll();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}