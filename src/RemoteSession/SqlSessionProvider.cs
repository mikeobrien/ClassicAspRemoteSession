using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.SessionState;

namespace RemoteSession
{
    /*
     * ASP.NET_SessionId = 1sc3aoog5u5zyko2tl3ghnvq
     * 
     * ASPStateTempApplications
     * ------------------------------
     * AppId        AppName
     * 538231025    /lm/w3svc/1/root
     * 685293685    /lm/w3svc/5/root
     * 
     * ASPStateTempSessions
     * ------------------------------
     * SessionId = 1sc3aoog5u5zyko2tl3ghnvq28d8c075
     * Created = 2011-05-12 00:12:19.980
     * Expires = 2011-05-12 00:33:52.437
     * LockDate = 2011-05-12 00:13:52.430
     * LockDateLocal = 2011-05-11 20:13:52.430
     * LockCookie = 2
     * Timeout = 20
     * Locked = 0
     * SessionItemShort = 0x14000000010001000000FFFFFFFF046E616D6507000000010576616C7565FF
     * SessionItemLong = NULL
     * Flags = 0
     * 
     */
    public class SqlSessionProvider : ISessionProvider
    {
        public IDictionary<string, object> Open(string metabasePath, string sessionId)
        {
            File.AppendAllText(@"d:\temp\session.txt", string.Format("Open: {0}\r\n", sessionId));
            return new Dictionary<string, object>
                       {
                           {"name", "mob"},
                           {"age", 32}
                       };
        }

        public string Save(string metabasePath, IDictionary<string, object> values)
        {
            var id = CreateSessionId();
            Save(metabasePath, id, values);
            return id;
        }

        public void Save(string metabasePath, string sessionId, IDictionary<string, object> values)
        {
            foreach (var value in values)
            {
                File.AppendAllText(@"d:\temp\session.txt", string.Format("Save: {0}, {1}={2}\r\n", value.Key, value.Value, sessionId));
            }
        }

        private static string CreateSessionId()
        {
            return new SessionIDManager().CreateSessionID(null);
        }

        private string GetFullId(string id, string metabasePath)
        {
            return id + GetAppSuffix(HashMetabasePath(metabasePath));
        }

        private static int HashMetabasePath(string metabasePath)
        {
            var hash = 5381;
            var len = metabasePath.Length;

            for (var i = 0; i < len; i++)
            {
                var c = Convert.ToInt32(metabasePath[i]);
                hash = ((hash << 5) + hash) ^ c;
            }

            return hash;
        }

        private static string GetAppSuffix(int appId)
        {
            return appId.ToString("x8", CultureInfo.InvariantCulture);
        }
    }
}