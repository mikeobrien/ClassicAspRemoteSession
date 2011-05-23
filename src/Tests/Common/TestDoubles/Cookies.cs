using System.Collections.Generic;
using RemoteSessionState;
using RemoteSessionState.Interop;

namespace Tests.Common.TestDoubles
{
    public class Cookies : ICookies
    {
        private readonly IDictionary<string, string> _items = new Dictionary<string, string>();

        public Cookies() {}

        public Cookies(string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId)) _items[SessionStateContext.AspNetSessionCookieName] = sessionId;
        }

        public string this[string name]
        {
            get { return _items.ContainsKey(name) ? _items[name] : null; }
            set { _items[name] = value; }
        }
    }
}