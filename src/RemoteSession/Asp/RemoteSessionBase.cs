namespace RemoteSession.Asp
{
    public abstract class RemoteSessionBase
    {
        private readonly Session _session;

        protected RemoteSessionBase(ISessionProvider sessionProvider)
        {
            _session = new Session(sessionProvider);
        }

        public void Load(dynamic request, dynamic response, dynamic session)
        {
            _session.Open(CreateContext(request, response, session));
        }

        public void Save(dynamic request, dynamic response, dynamic session)
        {
            _session.Save(CreateContext(request, response, session));
        }

        private static HttpContext CreateContext(dynamic request, dynamic response, dynamic session)
        {
            return new HttpContext(new ServerVariableAdapter(request.ServerVariables),
                                   new CookieAdapter(request.Cookies),
                                   new CookieAdapter(response.Cookies),
                                   new SessionAdapter(session));
        }
    }
}