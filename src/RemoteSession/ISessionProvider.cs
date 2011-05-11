using System.Collections.Generic;

namespace RemoteSession
{
    public interface ISessionProvider
    {
        IDictionary<string, object> Open(string sessionId);
        string Save(IDictionary<string, object> values);
        void Save(string sessionId, IDictionary<string, object> values);
    }
}