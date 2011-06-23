using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            if (data == null || data.Length == 0) return null; 
            return ValidateItems(new SessionStateEncoding().Deserialize(data).Items).ToDictionary();
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

        // This needs to be done because there is no other way to get information on the 
        // exact session item that failed to deserialize.
        private static ISessionStateItemCollection ValidateItems(ISessionStateItemCollection items)
        {
            var failedItems = new List<Tuple<int, string>>();

            for (var i = 0; i < items.Count; i++)
            {
                try
                {
                    var item = items[i];
                }
                catch (SerializationException e)
                {
                    items[i] = null;
                    failedItems.Add(Tuple.Create(i, e.Message));
                }
            }

            if (failedItems.Any())
                throw new SerializationException(string.Format("Session item '{0}' type cannot not be marshaled. Only primitive types " +
                                                               "(Boolean, Byte, System.DateTime/Date, Double, System.Int16/Integer, " +
                                                               "System.Int32/Long, System.Float/Single, String) can be marshaled between " +
                                                               "Classic ASP and ASP.NET. {1}", items.Keys[failedItems.First().Item1], failedItems.First().Item2));
            return items;
        }
    }
}