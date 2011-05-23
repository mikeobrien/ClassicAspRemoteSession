using System.Collections.Generic;

namespace RemoteSessionState
{
    public interface ISessionStateProvider
    {
        IDictionary<string, object> Load(ISessionStateContext context);
        void Save(ISessionStateContext context, IDictionary<string, object> values);
        void Abandon(ISessionStateContext context);
    }
}