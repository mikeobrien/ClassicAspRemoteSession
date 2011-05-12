using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("68F1BBC6-9123-4129-B5DF-88D5B4F9A99D")]
    [ProgId("UltravioletCatastrophe.RemoteSqlSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteSqlSession : RemoteSessionBase, IRemoteSession
    {
        public RemoteSqlSession() : base(new SqlSessionProvider()) { }
    }
}
