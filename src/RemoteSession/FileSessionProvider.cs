using System;
using System.Collections.Generic;

namespace RemoteSession
{
    public class FileSessionProvider : ISessionProvider
    {
        public IDictionary<string, object> Open(string metabasePath, string sessionId)
        {
            throw new NotImplementedException();
        }

        public string Save(string metabasePath, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public void Save(string metabasePath, string sessionId, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        //private string GetSessionFilePath()
        //{
            
        //}
    }
}