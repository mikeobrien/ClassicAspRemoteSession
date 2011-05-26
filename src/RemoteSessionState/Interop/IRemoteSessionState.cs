using System.Runtime.InteropServices;

namespace RemoteSessionState.Interop
{
    [ComVisible(true), GuidAttribute("4B533AF8-EE84-4892-8272-5344FDACAF9E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRemoteSessionState
    {
        string GetVersion();
        void Load(dynamic request, dynamic response, dynamic session);
        void Save(dynamic request, dynamic response, dynamic session);
        void Abandon(dynamic request, dynamic response, dynamic session);
    }
}
