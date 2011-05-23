using System;
using System.Web.SessionState;
using RemoteSessionState.Interop;

namespace RemoteSessionState
{
    public class SessionStateContext : ISessionStateContext
    {
        public const string AspNetSessionCookieName = "ASP.NET_SessionId";
        public const string MetadataPathServerVariable = "APPL_MD_PATH";
        public const string VirtualDirectoryPathServerVariable = "APPL_PHYSICAL_PATH";

        private readonly IHttpContext _httpContext;

        public SessionStateContext(IHttpContext httpContext)
        {
            _httpContext = httpContext;
            SessionId = _httpContext.RequestCookies[AspNetSessionCookieName];
            MetabasePath = (string)_httpContext.ServerVariables[MetadataPathServerVariable];
            VirtualDirectoryPath = (string)_httpContext.ServerVariables[VirtualDirectoryPathServerVariable];
        }

        public static SessionStateContext Create(IHttpContext httpContext)
        {
            return new SessionStateContext(httpContext);
        }

        public string SessionId { get; private set; }
        public string MetabasePath { get; private set; }
        public string VirtualDirectoryPath { get; private set; }

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