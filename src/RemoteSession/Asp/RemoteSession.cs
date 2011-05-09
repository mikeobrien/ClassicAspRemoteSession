using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("863F7F83-0BA2-4FDD-BC78-5B6B4170D754")]
    [ProgId("UltravioletCatastrophe.RemoteSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteSession : IRemoteSession
    {
        private readonly Session _session = new Session(new SqlSessionProvider());

        public void Load(dynamic request, dynamic session)
        {
            _session.Open(new CookieAdapter(request.Cookies), new SessionAdapter(session));
        }

        public void Save(dynamic request, dynamic response, dynamic session)
        {
            _session.Save(new CookieAdapter(request.Cookies), new CookieAdapter(response.Cookies), new SessionAdapter(session));
        }
    }
}
