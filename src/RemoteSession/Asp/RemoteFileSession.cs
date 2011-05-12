using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("F714C8BE-4EC9-470A-82B6-1CB579F3F1B0")]
    [ProgId("UltravioletCatastrophe.RemoteFileSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteFileSession : RemoteSessionBase
    {
        public RemoteFileSession() : base(new FileSessionProvider()) {}
    }
}
