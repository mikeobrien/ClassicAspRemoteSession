using System;

namespace RemoteSession
{
    public class Context
    {
        public Context(string metabasePath, string sessionId)
        {
            if (string.IsNullOrEmpty(metabasePath)) throw new ArgumentException("Metabase path cannot be empty.", "metabasePath");
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentException("Session id cannot be empty.", "sessionId");
            MetabasePath = metabasePath;
            SessionId = sessionId;
        }

        public string MetabasePath { get; private set; }
        public string SessionId { get; private set; }
    }
}