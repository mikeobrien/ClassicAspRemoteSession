using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("5EAFB353-4299-4275-B36F-05EB8F1EC6D4")]
    [ProgId("UltravioletCatastrophe.RemoteSqlSession")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteSqlSession : RemoteSessionBase
    {
        public RemoteSqlSession() : base(new SqlSessionProvider()) { }
    }
}
