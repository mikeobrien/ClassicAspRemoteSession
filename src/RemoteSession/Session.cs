using System;
using System.Collections.Generic;
using System.Linq;
using RemoteSession.Interop;

namespace RemoteSession
{
    public class Session
    {        
        private static readonly Type[] CompatableTypes = new[]
            {
                typeof (string), typeof (byte), typeof (bool), typeof (short), typeof (int), 
                typeof (double), typeof (float), typeof (DateTime)
            }; 

        private readonly ISessionProvider _sessionProvider;

        public Session(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public void Load(HttpContext httpContext)
        {
            var sessionContext = SessionContext.Create(httpContext);
            if (!sessionContext.HasActiveSession()) return;
            var values = _sessionProvider.Load(sessionContext);
            httpContext.Session.RemoveAll();
            if (values == null || !values.Any()) return;
            foreach (var value in ItemsWithCompatableDataTypes(values)) httpContext.Session[value.Key] = value.Value;
        }

        public void Save(HttpContext httpContext)
        {
            var sessionContext = SessionContext.Create(httpContext);
            var values = ItemsWithCompatableDataTypes(httpContext.Session.ToDictionary(x => x.Key, x => x.Value));
            if (!sessionContext.HasActiveSession()) sessionContext.CreateNewSession();
            _sessionProvider.Save(sessionContext, values);
        }

        public void Abandon(HttpContext httpContext)
        {
            var sessionContext = SessionContext.Create(httpContext);
            httpContext.Session.Abandon();
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
