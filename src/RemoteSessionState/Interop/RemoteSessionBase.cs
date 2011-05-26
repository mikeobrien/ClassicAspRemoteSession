using System;
using System.IO;
using System.Reflection;

namespace RemoteSessionState.Interop
{
    public abstract class RemoteSessionBase : IRemoteSessionState
    {
        private static readonly string LogFile = Path.Combine(Path.GetTempPath(), "RemoteSession.log");
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
                                   new SessionStateAdapter(session));
        }

        private static void LogException(Exception exception)
        {
            File.AppendAllText(LogFile, string.Format("{0}: {1}\r\n\r\n", DateTime.Now, exception));
        }
    }
}