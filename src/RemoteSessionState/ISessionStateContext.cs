namespace RemoteSessionState
{
    public interface ISessionStateContext
    {
        string SessionId { get; }
        string MetabasePath { get; }
        string VirtualDirectoryPath { get; }
        bool HasActiveSession();
        void CreateNewSession();
    }
}