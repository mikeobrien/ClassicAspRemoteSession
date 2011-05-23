using System.Runtime.InteropServices;

namespace RemoteSessionState.Interop
{
    [ComVisible(true), GuidAttribute("051303F1-877D-4438-B33B-81887009E60C")]
    [ProgId("UltravioletCatastrophe.RemoteFileSessionState")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteFileSessionState : RemoteSessionBase
    {
        public RemoteFileSessionState() : base(new FileSessionStateProvider()) {}
    }
}
