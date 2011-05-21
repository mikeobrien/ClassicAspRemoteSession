namespace RemoteSession
{
    public interface ISqlSessionStore
    {
        byte[] Load(SqlSessionId sessionId);
        void Save(SqlSessionId sessionId, byte[] data);
        void Abandon(SqlSessionId sessionId);
    }
}