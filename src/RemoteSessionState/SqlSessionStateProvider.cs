using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace RemoteSessionState
{
    public class SqlSessionStateProvider : ISessionStateProvider
    {
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
            var systemWebConfiguration = new SystemWebConfiguration(context.VirtualDirectoryPath);
            var connectionString = systemWebConfiguration.SessionState.SqlConnectionString;
            _timeout = (int)systemWebConfiguration.SessionState.Timeout.TotalMinutes;
            _sessionStore = new SqlSessionStateStore(connectionString, _timeout);
        }
    }
}