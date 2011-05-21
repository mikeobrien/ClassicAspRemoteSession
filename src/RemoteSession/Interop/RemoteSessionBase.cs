using System;
using System.IO;

namespace RemoteSession.Interop
{
    public abstract class RemoteSessionBase : IRemoteSession
    {
        private static readonly string LogFile = Path.Combine(Path.GetTempPath(), "RemoteSession.log");
        private readonly Session _session;

        protected RemoteSessionBase(ISessionProvider sessionProvider)
        {
            _session = new Session(sessionProvider);
        }

        public void Load(dynamic request, dynamic response, dynamic session)
        {            
            try
            {
                _session.Load(CreateContext(request, response, session));
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
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
                LogException(ex);
                throw;
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
                LogException(ex);
                throw;
            }
        }

        private static HttpContext CreateContext(dynamic request, dynamic response, dynamic session)
        {
            return new HttpContext(new ServerVariableAdapter(request.ServerVariables),
                                   new CookieAdapter(request.Cookies, response),
                                   new CookieAdapter(response.Cookies, response),
                                   new SessionAdapter(session));
        }

        private static void LogException(Exception exception)
        {
            File.AppendAllText(LogFile, string.Format("{0}: {1}\r\n\r\n", DateTime.Now, exception));
        }
    }
}