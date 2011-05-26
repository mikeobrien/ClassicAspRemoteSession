using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RemoteSessionState.Interop
{
    [ComVisible(true), GuidAttribute("51D25D7A-75E7-4584-804A-12851D37BB97")]
    [ProgId("UltravioletCatastrophe.RemoteSqlSessionState")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RemoteSqlSessionState : RemoteSessionBase
    {
        public RemoteSqlSessionState() : base(new SqlSessionStateProvider()) { }
    }
}
