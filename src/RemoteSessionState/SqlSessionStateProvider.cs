using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace RemoteSessionState
{
    public class SqlSessionStateProvider : ISessionStateProvider
    {
        private const int DefaultTimeout = 20;
        private ISqlSessionStateStore _sessionStore;
        private int _timeout;

        public SqlSessionStateProvider() { }

        public SqlSessionStateProvider(ISqlSessionStateStore sessionStore)
        {
            _sessionStore = sessionStore;
            _timeout = 20;
        }

        public IDictionary<string, object> Load(ISessionStateContext context)
        {
            EnsureInitialized(context);
            var data = _sessionStore.Load(SqlSessionId.Create(context.MetabasePath, context.SessionId));
            return data == null || data.Length == 0 ? null : 
                new SessionStateEncoding().Deserialize(data).Items.ToDictionary();
        }

        public void Save(ISessionStateContext context, IDictionary<string, object> values)
        {
            EnsureInitialized(context);
            var sessionState = new SessionStateStoreData(new SessionStateItemCollection().AddItems(values), new HttpStaticObjectsCollection(), _timeout);
            var data = new SessionStateEncoding().Serialize(sessionState);
            _sessionStore.Save(SqlSessionId.Create(context.MetabasePath, context.SessionId), data);
        }

        public void Abandon(ISessionStateContext context)
        {
            EnsureInitialized(context);
            _sessionStore.Abandon(SqlSessionId.Create(context.MetabasePath, context.SessionId));
        }

        private void EnsureInitialized(ISessionStateContext context)
        {
            if (_sessionStore == null) InitializeFromWebConfig(context);
        }

        private void InitializeFromWebConfig(ISessionStateContext context)
        {
            var webConfig = new WebConfig(context.VirtualDirectoryPath);
            var connectionString = webConfig.GetValue<string>("configuration/system.web/sessionState/@sqlConnectionString");
            var timeout = webConfig.GetValue<int>("configuration/system.web/sessionState/@timeout");
            _timeout = timeout> 0 ? timeout : DefaultTimeout;
            _sessionStore = new SqlSessionStateStore(connectionString, _timeout);
        }
    }
}