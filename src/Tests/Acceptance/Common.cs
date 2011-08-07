using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RestSharp;

namespace Tests.Acceptance
{
    public static class Common
    {
        private static readonly Func<RestClient> Client = () => new RestClient("http://localhost/RemoteSessionStateTestService/");

        public static string GetRegisteredComVersion()
        {
            var response = Client().Execute(new RestRequest("session.asp?command=version"));
            if (!string.IsNullOrEmpty(response.ErrorMessage)) Debug.WriteLine(response.Content);
            return response.Content;
        }

        public static SessionResponse<List<SessionVariable>> GetClassic()
        {
            return GetClassic((string)null, null);
        }

        public static SessionResponse<List<SessionVariable>> GetClassic(string querystring)
        {
            return GetClassic((string)null, querystring);
        }

        public static SessionResponse<List<SessionVariable>> GetNet()
        {
            return GetNet((string)null, null);
        }

        public static SessionResponse<List<SessionVariable>> GetNet(string querystring)
        {
            return GetNet((string)null, querystring);
        }

        public static SessionResponse<List<SessionVariable>> GetClassic(this SessionResponse<List<SessionVariable>> response)
        {
            return GetClassic(response.SessionId, null);
        }

        public static SessionResponse<List<SessionVariable>> GetNet(this SessionResponse<List<SessionVariable>> response)
        {
            return GetNet(response.SessionId, null);
        }

        public static SessionResponse<List<SessionVariable>> GetClassic(this SessionResponse<List<SessionVariable>> response, string querystring)
        {
            return GetClassic(response.SessionId, querystring);
        }

        public static SessionResponse<List<SessionVariable>> GetNet(this SessionResponse<List<SessionVariable>> response, string querystring)
        {
            return GetNet(response.SessionId, querystring);
        }

        private static SessionResponse<List<SessionVariable>> GetClassic(string sessionId, string querystring)
        {
            return Get(sessionId, "session.asp", querystring);
        }

        private static SessionResponse<List<SessionVariable>> GetNet(string sessionId, string querystring)
        {
            return Get(sessionId, "session.aspx", querystring);
        }

        private static SessionResponse<List<SessionVariable>> Get(string sessionId, string page, string querystring)
        {
            const string sessionIdCookie = "ASP.NET_SessionId";
            var request = new RestRequest(page + (querystring != null ? "?" + querystring : string.Empty));
            if (sessionId != null) request.AddParameter(sessionIdCookie, sessionId, ParameterType.Cookie);
            var response = Client().Execute<List<SessionVariable>>(request);
            if (response.ResponseStatus == ResponseStatus.Error) Debug.WriteLine(response.Content);
            return new SessionResponse<List<SessionVariable>>
                       {
                           Response = response,
                           SessionId = sessionId ?? response.Cookies.Where(x => x.Name == sessionIdCookie).Select(x => x.Value).FirstOrDefault(),
                           Error = response.ResponseStatus == ResponseStatus.Error,
                           ErrorMessage = response.Content
                       };
        }

        public class SessionResponse<T>
        {
            public string SessionId { get; set; }
            public RestResponse<T> Response { get; set; }
            public T Data { get { return Response.Data; } }
            public bool Error { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class SessionVariable
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string DataType { get; set; }

            public bool Equals(SessionVariable other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Key, Key) && Equals(other.Value, Value) && Equals(other.DataType, DataType);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var result = (Key != null ? Key.GetHashCode() : 0);
                    result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                    result = (result*397) ^ (DataType != null ? DataType.GetHashCode() : 0);
                    return result;
                }
            }
        }
    }
}