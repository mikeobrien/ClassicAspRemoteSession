using System;
using System.Collections.Generic;
using System.Linq;
using RemoteSessionState.Interop;

namespace RemoteSessionState
{
    public class SessionState
    {        
        private static readonly Type[] CompatableTypes = new[]
            {
                typeof (string), typeof (byte), typeof (bool), typeof (short), typeof (int), 
                typeof (double), typeof (float), typeof (DateTime)
            }; 

        private readonly ISessionStateProvider _sessionProvider;

        public SessionState(ISessionStateProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public void Load(IHttpContext httpContext)
        {
            var sessionContext = SessionStateContext.Create(httpContext);
            if (!sessionContext.HasActiveSession()) return;
            var values = _sessionProvider.Load(sessionContext);
            httpContext.SessionState.RemoveAll();
            if (values == null || !values.Any()) return;
            foreach (var value in ItemsWithCompatableDataTypes(values)) httpContext.SessionState[value.Key] = value.Value;
        }

        public void Save(IHttpContext httpContext)
        {
            var sessionContext = SessionStateContext.Create(httpContext);
            var values = ItemsWithCompatableDataTypes(httpContext.SessionState.ToDictionary(x => x.Key, x => x.Value));
            if (!sessionContext.HasActiveSession()) sessionContext.CreateNewSession();
            _sessionProvider.Save(sessionContext, values);
        }

        public void Abandon(IHttpContext httpContext)
        {
            var sessionContext = SessionStateContext.Create(httpContext);
            httpContext.SessionState.Abandon();
            if (!sessionContext.HasActiveSession()) return;
            _sessionProvider.Abandon(sessionContext);
        }

        private static IDictionary<string, object> ItemsWithCompatableDataTypes(IDictionary<string, object> values)
        {
            return values.Where(x => x.Value != null).
                          Join(CompatableTypes, x => x.Value.GetType(), x => x, (x, y) => x).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
