using System;

namespace RemoteSessionState.Interop
{
    public class SessionException : Exception
    {
        public SessionException(string action, Exception exception):
            base(string.Format("Error {0} session:\r\n{1}", action, exception), exception) {}
    }
}