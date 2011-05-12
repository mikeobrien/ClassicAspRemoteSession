using System;
using System.Linq;
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
            if (!HasActiveSession(context.RequestCookies)) return;
            var values = _sessionProvider.Open(GetMetadataPath(context.ServerVariables), 
                                               GetSessionId(context.RequestCookies));
            if (values != null) foreach (var value in values) context.Session[value.Key] = value.Value;
            else context.Session.Abandon();
        }

        public void Save(HttpContext context)
        {
            var values = context.Session.ToDictionary(x => x.Key, x => x.Value);
            if (HasActiveSession(context.RequestCookies)) 
                _sessionProvider.Save(GetMetadataPath(context.ServerVariables), 
                                      GetSessionId(context.RequestCookies), values);
            else context.ResponseCookies[AspNetSessionCookieName] = 
                _sessionProvider.Save(GetMetadataPath(context.ServerVariables), values);
        }

        private static string GetMetadataPath(IServerVariables serverVariables)
        {
            return (string)serverVariables[MetadataPathServerVariable];
        }

        private static bool HasActiveSession(ICookies cookies)
        {
            return !String.IsNullOrEmpty(GetSessionId(cookies));
        }

        private static string GetSessionId(ICookies cookies)
        {
            return cookies[AspNetSessionCookieName];
        }
    }
}
