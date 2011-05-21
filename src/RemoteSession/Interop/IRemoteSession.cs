using System.Runtime.InteropServices;

namespace RemoteSession.Interop
{
    [ComVisible(true), GuidAttribute("60D7B1BF-8298-4A1A-B058-A80B0DA88AB3")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRemoteSession
    {
        void Load(dynamic request, dynamic response, dynamic session);
        void Save(dynamic request, dynamic response, dynamic session);
        void Abandon(dynamic request, dynamic response, dynamic session);
    }
}
