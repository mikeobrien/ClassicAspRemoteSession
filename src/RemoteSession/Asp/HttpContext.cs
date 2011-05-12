namespace RemoteSession.Asp
{
    public class HttpContext
    {
        public HttpContext(IServerVariables serverVariables,
                           ICookies requestCookies,
                           ICookies responseCookies,
                           ISession session)
        {
            ServerVariables = serverVariables;
            RequestCookies = requestCookies;
            ResponseCookies = responseCookies;
            Session = session;
        }

        public IServerVariables ServerVariables { get; private set; }
        public ICookies RequestCookies { get; private set; }
        public ICookies ResponseCookies { get; private set; }
        public ISession Session { get; private set; }
    }
}
