using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("051303F1-877D-4438-B33B-81887009E60C")]
    [ProgId("UltravioletCatastrophe.RemoteFileSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteFileSession : RemoteSessionBase, IRemoteSession
    {
        public RemoteFileSession() : base(new FileSessionProvider()) {}
    }
}
