using System;
using System.Web.SessionState;
using RemoteSession.Interop;

namespace RemoteSession
{
    public class SessionContext : ISessionContext
    {
        public const string AspNetSessionCookieName = "ASP.NET_SessionId";
        public const string MetadataPathServerVariable = "APPL_MD_PATH";

        private readonly HttpContext _httpContext;

        public SessionContext(HttpContext httpContext)
        {
            _httpContext = httpContext;
            SessionId = _httpContext.RequestCookies[AspNetSessionCookieName];
            MetabasePath = (string)_httpContext.ServerVariables[MetadataPathServerVariable];
        }

        public static SessionContext Create(HttpContext httpContext)
        {
            return new SessionContext(httpContext);
        }

        public string SessionId { get; private set; }
        public string MetabasePath { get; private set; }

        public bool HasActiveSession()
        {
            return !String.IsNullOrEmpty(SessionId);
        }

        public void CreateNewSession()
        {
            SessionId = new SessionIDManager().CreateSessionID(null);
            _httpContext.ResponseCookies[AspNetSessionCookieName] = SessionId;
        }
    }
}