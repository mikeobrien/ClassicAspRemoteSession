using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("A8F5228B-199E-41F2-91B0-0C60C85B69AB")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRemoteSession
    {
        void Load(dynamic request, dynamic response, dynamic session);
        void Save(dynamic request, dynamic response, dynamic session);
    }
}
