using System.Collections.Generic;

namespace RemoteSession
{
    public interface ISessionProvider
    {
        IDictionary<string, object> Load(Context context);
        void Save(Context context, IDictionary<string, object> values);
        void Abandon(Context context);
    }
}