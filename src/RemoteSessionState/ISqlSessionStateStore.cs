namespace RemoteSessionState
{
    public interface ISqlSessionStateStore
    {
        byte[] Load(SqlSessionId sessionId);
        void Save(SqlSessionId sessionId, byte[] data);
        void Abandon(SqlSessionId sessionId);
    }
}