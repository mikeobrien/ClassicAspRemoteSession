using System;
using System.Collections.Generic;
using System.IO;

namespace SharedSession
{
    public class SqlSessionProvider : ISessionProvider
    {
        public bool Open(string sessionId, out IDictionary<string, object> values)
        {
            File.AppendAllText(@"d:\temp\session.txt", string.Format("Open: {0}\r\n", sessionId));
            values = new Dictionary<string, object>
                       {
                           {"name", "mob"},
                           {"age", 32}
                       };
            return true;
        }

        public string Save(IDictionary<string, object> values)
        {
            var id = Guid.NewGuid().ToString();
            Save(id, values);
            return id;
        }

        public void Save(string sessionId, IDictionary<string, object> values)
        {
            foreach (var value in values)
            {
                File.AppendAllText(@"d:\temp\session.txt", string.Format("Save: {0}, {1}={2}\r\n", value.Key, value.Value, sessionId));
            }
        }
    }
}