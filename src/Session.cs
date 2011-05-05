using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharedSession
{
    [ComVisible(true), GuidAttribute("4564790F-85A8-4F8D-86FF-5C3FD7B12255")]
    [ProgId("UltravioletCatastrophe.SharedSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Session : ISession
    {
        private const string AspNetSessionCookieName = "ASP.NET_SessionId";

        private readonly ISessionProvider _sessionProvider;

        public Session()
        {
            _sessionProvider = new SqlSessionProvider();
        }

        public dynamic GetCookie(dynamic request, string name)
        {
            return request.Cookies.Item(AspNetSessionCookieName);
        }

        public void Open(dynamic request, dynamic session)
        {
            if (!HasActiveSession(request)) return;
            IDictionary<string, object> values;
            if (_sessionProvider.Open(GetSessionId(request), out values))
            {
                //foreach (var value in values) session.Contents[value.Key] = value.Value;
            }
            else session.Abandon();
        }

        public void Save(dynamic request, dynamic response, dynamic session)
        {
            var values = new Dictionary<string, object>();
            foreach (var key in session) values.Add(key, session.Contents(key));
            if (HasActiveSession(request)) _sessionProvider.Save(GetSessionId(request), values);
            else response.Cookies[AspNetSessionCookieName] = _sessionProvider.Save(values);
        }

        private bool HasActiveSession(dynamic request)
        {
            return !String.IsNullOrEmpty(GetSessionId(request));
        }

        private static string GetSessionId(dynamic request)
        {
            var key = new ErrorWrapper(unchecked((int)0x80020004));
            return request.Cookies.Item(AspNetSessionCookieName)[key];
        }
    }
}
