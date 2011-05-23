using System.Runtime.InteropServices;

namespace RemoteSessionState.Interop
{
    [ComVisible(true), GuidAttribute("68F1BBC6-9123-4129-B5DF-88D5B4F9A99D")]
    [ProgId("UltravioletCatastrophe.RemoteSqlSessionState")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteSqlSessionState : RemoteSessionBase
    {
        public RemoteSqlSessionState() : base(new SqlSessionStateProvider()) { }
    }
}
