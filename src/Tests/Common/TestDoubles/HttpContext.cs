using System.Collections.Generic;
using RemoteSessionState;
using RemoteSessionState.Interop;

namespace Tests.Common.TestDoubles
{
    public class HttpContext : IHttpContext
    {
        public const string DefaultMetabasePath = Constants.MetabasePath;
        public const string DefaultSessionId = Constants.SessionId;

        public HttpContext(string metabasePath, string sessionId, IDictionary<string, object> sessionState)
        {
            ServerVariables = new ServerVariables(metabasePath);
            RequestCookies = new Cookies(sessionId);
            ResponseCookies = new Cookies();
            SessionState = new SessionState(sessionState);
        }

        public static HttpContext CreateWithSession()
        {
            return CreateWithSession(null);
        }

        public static HttpContext CreateWithSession(IDictionary<string, object> sessionState)
        {
            return new HttpContext(DefaultMetabasePath, DefaultSessionId, sessionState);
        }

        public static HttpContext CreateWithoutSession()
        {
            return new HttpContext(DefaultMetabasePath, null, null);
        }

        public IServerVariables ServerVariables { get; private set; }
        public ICookies RequestCookies { get; private set; }
        public ICookies ResponseCookies { get; private set; }
        public ISessionState SessionState { get; private set; }

        public string RequestSessionId { get { return RequestCookies[SessionStateContext.AspNetSessionCookieName]; } }
        public string ResponseSessionId { get { return ResponseCookies[SessionStateContext.AspNetSessionCookieName]; } }
    }
}