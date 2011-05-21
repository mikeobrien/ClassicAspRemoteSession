using System.Collections.Generic;

namespace RemoteSession.Interop
{
    public interface ISession : IEnumerable<KeyValuePair<string, object>>
    {
        object this[string name] { get; set; }
        void Remove(string name);
        void RemoveAll();
        void Abandon();
    }
}