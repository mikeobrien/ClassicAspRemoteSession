using System;
using System.Reflection;

namespace RemoteSessionState.Interop
{
    public class SessionException : Exception
    {
        public SessionException(string action, Exception exception):
            base(string.Format("RemoteSessionState v{0}: Error {1} session:\r\n{2}", Assembly.GetExecutingAssembly().GetName().Version, action, exception), exception) {}
    }
}