using System.Runtime.InteropServices;

namespace RemoteSession.Interop
{
    public class CookieAdapter : ICookies
    {        
        private readonly ErrorWrapper _key = new ErrorWrapper(unchecked((int)0x80020004));
        private readonly dynamic _cookies;
        private readonly dynamic _response;

        public CookieAdapter(dynamic cookies, dynamic response)
        {
            _cookies = cookies;
            _response = response;
        }

        public string this[string name]
        {
            get { return _cookies.Item(name)[_key]; }
            set { _response.AddHeader("Set-Cookie", string.Format("{0}={1}; path=/", name, value)); }
        }
    }
}
