using System;
using System.Globalization;

namespace RemoteSession
{
    public class SqlSessionId
    {
        private readonly string _id;

        private SqlSessionId(string metabasePath, string sessionId)
        {
            _id = sessionId + GetApplicationId(metabasePath);
        }

        public static SqlSessionId Create(string metabasePath, string sessionId)
        {
            return new SqlSessionId(metabasePath, sessionId);
        }

        public override string ToString()
        {
            return _id;
        }

        private static string GetApplicationId(string metabasePath)
        {
            var hash = 5381;
            var length = metabasePath.Length;

            for (var i = 0; i < length; i++)
            {
                var c = Convert.ToInt32(metabasePath[i]);
                hash = ((hash << 5) + hash) ^ c;
            }

            return hash.ToString("x8", CultureInfo.InvariantCulture);
        }
    }
}