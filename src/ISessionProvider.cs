using System.Collections.Generic;

namespace SharedSession
{
    public interface ISessionProvider
    {
        bool Open(string sessionId, out IDictionary<string, object> values);
        string Save(IDictionary<string, object> values);
        void Save(string sessionId, IDictionary<string, object> values);
    }
}