using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using RemoteSession.Asp;

namespace RemoteSession
{
    public class Session
    {        
        public const string AspNetSessionCookieName = "ASP.NET_SessionId";
        public const string MetadataPathServerVariable = "APPL_MD_PATH";

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

        public void Open(HttpContext context)
        {
            string sessionId;
            if (!TryGetSessionId(context.RequestCookies, out sessionId)) return;
            var values = _sessionProvider.Load(new Context(GetMetadataPath(context.ServerVariables), sessionId));
            if (values != null)
            {
                var compatableValues = WhereDataTypesAreCompatable(values);
                foreach (var value in compatableValues) context.Session[value.Key] = value.Value;
                foreach (var value in context.Session.Except(compatableValues)) context.Session.Remove(value.Key);
            }
            else context.Session.Abandon();
        }

        public void Save(HttpContext context)
        {
            var values = WhereDataTypesAreCompatable(context.Session.ToDictionary(x => x.Key, x => x.Value));
            string sessionId;
            if (!TryGetSessionId(context.RequestCookies, out sessionId))
            {
                sessionId = CreateSessionId();
                context.ResponseCookies[AspNetSessionCookieName] = sessionId;
            }
            _sessionProvider.Save(new Context(GetMetadataPath(context.ServerVariables), sessionId), values);
        }

        public void Abandon(HttpContext context)
        {
            context.Session.Abandon();
            string sessionId;
            if (!TryGetSessionId(context.RequestCookies, out sessionId)) return;
            _sessionProvider.Abandon(new Context(GetMetadataPath(context.ServerVariables), sessionId));
        }

        private static IDictionary<string, object> WhereDataTypesAreCompatable(IDictionary<string, object> values)
        {
            return values.Where(x => x.Value != null).
                          Join(CompatableTypes, x => x.Value.GetType(), x => x, (x, y) => x).ToDictionary(x => x.Key, x => x.Value);
        }

        private static string CreateSessionId()
        {
            return new SessionIDManager().CreateSessionID(null);
        }

        private static string GetMetadataPath(IServerVariables serverVariables)
        {
            return (string)serverVariables[MetadataPathServerVariable];
        }

        private static bool TryGetSessionId(ICookies cookies, out string sessionId)
        {
            sessionId = cookies[AspNetSessionCookieName];
            return !String.IsNullOrEmpty(sessionId);
        }
    }
}
