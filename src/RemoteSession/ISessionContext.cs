namespace RemoteSession
{
    public interface ISessionContext
    {
        string SessionId { get; }
        string MetabasePath { get; }
        bool HasActiveSession();
        void CreateNewSession();
    }
}