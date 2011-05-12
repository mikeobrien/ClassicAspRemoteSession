using System.Collections.Generic;

namespace RemoteSession
{
    public interface ISessionProvider
    {
        IDictionary<string, object> Open(string metabasePath, string sessionId);
        void Save(string metabasePath, string sessionId, IDictionary<string, object> values);
    }
}