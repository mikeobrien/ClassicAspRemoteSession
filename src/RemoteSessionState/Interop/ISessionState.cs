using System.Collections.Generic;

namespace RemoteSessionState.Interop
{
    public interface ISessionState : IEnumerable<KeyValuePair<string, object>>
    {
        object this[string name] { get; set; }
        void Remove(string name);
        void RemoveAll();
        void Abandon();
    }
}