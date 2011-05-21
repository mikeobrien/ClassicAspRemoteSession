using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace RemoteSession
{
    public class SqlSessionProvider : ISessionProvider
    {
        private readonly ISqlSessionStore _sessionStore;
        private readonly int _timeout;

        public SqlSessionProvider()
        {
            // TODO: Get configuration from web.config
        }

        public SqlSessionProvider(ISqlSessionStore sessionStore)
        {
            _sessionStore = sessionStore;
            _timeout = 20;
        }

        public IDictionary<string, object> Load(ISessionContext context)
        {
            var data = _sessionStore.Load(SqlSessionId.Create(context.MetabasePath, context.SessionId));
            return data == null || data.Length == 0 ? null : 
                new SessionStateEncoding().Deserialize(data).Items.ToDictionary();
        }

        public void Save(ISessionContext context, IDictionary<string, object> values)
        {
            var sessionState = new SessionStateStoreData(new SessionStateItemCollection().AddItems(values), new HttpStaticObjectsCollection(), _timeout);
            var data = new SessionStateEncoding().Serialize(sessionState);
            _sessionStore.Save(SqlSessionId.Create(context.MetabasePath, context.SessionId), data);
        }

        public void Abandon(ISessionContext context)
        {
            _sessionStore.Abandon(SqlSessionId.Create(context.MetabasePath, context.SessionId));
        }
    }
}