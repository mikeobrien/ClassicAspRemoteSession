using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    public class CookieAdapter : ICookies
    {        
        private readonly ErrorWrapper _key = new ErrorWrapper(unchecked((int)0x80020004));
        private readonly dynamic _cookies;

        public CookieAdapter(dynamic cookies)
        {
            _cookies = cookies;
        }

        public string this[string name]
        {
            get { return _cookies.Item(name)[_key]; }
            set { _cookies.Item(name)[_key] = value; }
        }
    }
}
