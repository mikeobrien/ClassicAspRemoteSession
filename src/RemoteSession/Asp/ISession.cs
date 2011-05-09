using System.Collections.Generic;

namespace RemoteSession.Asp
{
    public interface ISession : IEnumerable<KeyValuePair<string, object>>
    {
        object this[string name] { get; set; }
        void Abandon();
    }
}