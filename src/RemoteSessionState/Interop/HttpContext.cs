namespace RemoteSessionState.Interop
{
    public class HttpContext : IHttpContext
    {
        public HttpContext(IServerVariables serverVariables,
                           ICookies requestCookies,
                           ICookies responseCookies,
                           ISessionState sessionState)
        {
            ServerVariables = serverVariables;
            RequestCookies = requestCookies;
            ResponseCookies = responseCookies;
            SessionState = sessionState;
        }

        public IServerVariables ServerVariables { get; private set; }
        public ICookies RequestCookies { get; private set; }
        public ICookies ResponseCookies { get; private set; }
        public ISessionState SessionState { get; private set; }
    }
}
