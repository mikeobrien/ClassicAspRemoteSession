using System.Collections.Generic;

namespace RemoteSession
{
    public interface ISessionProvider
    {
        IDictionary<string, object> Load(ISessionContext context);
        void Save(ISessionContext context, IDictionary<string, object> values);
        void Abandon(ISessionContext context);
    }
}