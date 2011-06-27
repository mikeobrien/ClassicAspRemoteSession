using System;
using System.Reflection;

namespace RemoteSessionState.Interop
{
    public abstract class RemoteSessionBase : IRemoteSessionState
    {
        private readonly SessionState _session;

        protected RemoteSessionBase(ISessionStateProvider sessionProvider)
        {
            _session = new SessionState(sessionProvider);
        }

        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void Load(dynamic request, dynamic response, dynamic session)
        {            
            try
            {
                _session.Load(CreateContext(request, response, session));
            }
            catch (Exception ex)
            {
                throw new SessionException("loading", ex);
            }
        }

        public void Save(dynamic request, dynamic response, dynamic session)
        {            
            try
            {
                _session.Save(CreateContext(request, response, session));
            }
            catch (Exception ex)
            {
                throw new SessionException("saving", ex);
            }
        }

        public void Abandon(dynamic request, dynamic response, dynamic session)
        {            
            try
            {
                _session.Abandon(CreateContext(request, response, session));
            }
            catch (Exception ex)
            {
                throw new SessionException("abandoning", ex);
            }
        }

        private static HttpContext CreateContext(dynamic request, dynamic response, dynamic session)
        {
            return new HttpContext(new ServerVariableAdapter(request.ServerVariables),
                                   new CookieAdapter(request.Cookies, response),
                                   new CookieAdapter(response.Cookies, response),
                                   new SessionStateAdapter(session));
        }
    }
}