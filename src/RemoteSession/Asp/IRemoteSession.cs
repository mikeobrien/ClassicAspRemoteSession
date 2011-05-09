using System.Runtime.InteropServices;

namespace RemoteSession.Asp
{
    [ComVisible(true), GuidAttribute("B43A2878-A130-4AC3-BFBC-B7EFC4940723")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRemoteSession
    {
        void Load(dynamic request, dynamic session);
        void Save(dynamic request, dynamic response, dynamic session);
    }
}
