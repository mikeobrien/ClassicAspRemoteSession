using System;
using System.Linq;
using System.Web.SessionState;
using RemoteSession.Asp;

namespace RemoteSession
{
    public class Session
    {        
        public const string AspNetSessionCookieName = "ASP.NET_SessionId";
        public const string MetadataPathServerVariable = "APPL_MD_PATH";

        private readonly ISessionProvider _sessionProvider;

        public Session(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public void Open(HttpContext context)
        {
            string sessionId;
            if (!TryGetSessionId(context.RequestCookies, out sessionId)) return;
            var values = _sessionProvider.Open(GetMetadataPath(context.ServerVariables), sessionId);
            if (values != null) foreach (var value in values) context.Session[value.Key] = value.Value;
            else context.Session.Abandon();
        }

        public void Save(HttpContext context)
        {
            var values = context.Session.ToDictionary(x => x.Key, x => x.Value);
            string sessionId;
            if (!TryGetSessionId(context.RequestCookies, out sessionId))
            {
                sessionId = CreateSessionId();
                context.ResponseCookies[AspNetSessionCookieName] = sessionId;
            }
            _sessionProvider.Save(GetMetadataPath(context.ServerVariables), sessionId, values);
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
