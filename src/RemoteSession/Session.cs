using System;
using System.Collections.Generic;
using System.Linq;
using RemoteSession.Asp;

namespace RemoteSession
{
    public class Session
    {        
        private const string AspNetSessionCookieName = "ASP.NET_SessionId";

        private readonly ISessionProvider _sessionProvider;

        public Session(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public void Open(ICookies cookies, ISession session)
        {
            if (!HasActiveSession(cookies)) return;
            IDictionary<string, object> values;
            if (_sessionProvider.Open(GetSessionId(cookies), out values))
            {
                foreach (var value in values) session[value.Key] = value.Value;
            }
            else session.Abandon();
        }

        public void Save(ICookies requestCookies, ICookies responseCookies, ISession session)
        {
            var values = session.ToDictionary(x => x.Key, x => x.Value);
            if (HasActiveSession(requestCookies)) _sessionProvider.Save(GetSessionId(requestCookies), values);
            else responseCookies[AspNetSessionCookieName] = _sessionProvider.Save(values);
        }

        private static bool HasActiveSession(ICookies cookies)
        {
            return !String.IsNullOrEmpty(GetSessionId(cookies));
        }

        private static string GetSessionId(ICookies cookies)
        {
            return cookies[AspNetSessionCookieName];
        }
    }
}
