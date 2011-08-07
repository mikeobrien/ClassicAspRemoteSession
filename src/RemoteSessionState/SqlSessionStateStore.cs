using System.Data;

namespace RemoteSessionState
{
    public class SqlSessionStateStore : ISqlSessionStateStore
    {
        private readonly string _connectionString;
        private readonly int _timeout;

        public SqlSessionStateStore(string connectionString, int timeout)
        {
            _connectionString = connectionString;
            _timeout = timeout;
        }

        public byte[] Load(SqlSessionId sessionId)
        {
            var session = GetSessionData(sessionId);
            if (!session.Empty) ReleaseSessionLock(session);
            return session.Data;
        }

        public void Save(SqlSessionId sessionId, byte[] data)
        {
            var session = GetSessionData(sessionId);
            if (session.Empty) InsertItem(sessionId, data);
            else UpdateItem(sessionId, data, session.LockCookie);
        }

        public void Abandon(SqlSessionId sessionId)
        {
            var session = GetSessionData(sessionId);
            if (!session.Empty) AbandonSession(session);
        }

        private void InsertItem(SqlSessionId sessionId, byte[] data)
        {
            SqlProcedure.Create(_connectionString, SqlSession.IsShortData(data) ? 
                                                   "TempInsertStateItemShort" : 
                                                   "TempInsertStateItemLong").
                         Execute(x => x.In("id", sessionId.ToString()),
                                 x => x.In(SqlSession.IsShortData(data) ? "itemShort" : "itemLong", data),
                                 x => x.In("timeout", _timeout));
        }

        private void UpdateItem(SqlSessionId sessionId, byte[] data, int lockCookie)
        {
            SqlProcedure.Create(_connectionString, SqlSession.IsShortData(data) ? 
                                                   "TempUpdateStateItemShortNullLong" : 
                                                   "TempUpdateStateItemLongNullShort").
                         Execute(x => x.In("id", sessionId.ToString()),
                                 x => x.In(SqlSession.IsShortData(data) ? "itemShort" : "itemLong", data),
                                 x => x.In("timeout", _timeout),
                                 x => x.In("lockCookie", lockCookie));
        }

        private void AbandonSession(SqlSession session)
        {
            SqlProcedure.Create(_connectionString, "TempRemoveStateItem").
                         Execute(x => x.In("id", session.Id.ToString()),
                                 x => x.In("lockCookie", session.LockCookie));
        }

        private void ReleaseSessionLock(SqlSession session)
        {
            SqlProcedure.Create(_connectionString, "TempReleaseStateItemExclusive").
                         Execute(x => x.In("id", session.Id.ToString()),
                                 x => x.In("lockCookie", session.LockCookie));   
        }

        private SqlSession GetSessionData(SqlSessionId sessionId)
        {
            var session = new SqlSession { Id = sessionId };
            SqlProcedure.Create(_connectionString, "TempGetStateItemExclusive3").
                         Execute(x => session.Data = x.Read() ? (byte[])x[0] : session.Data,
                                 x => x.In("id", sessionId.ToString()),
                                 x => x.Out<byte[]>("itemShort", SqlDbType.VarBinary, 7000, y => session.Data = y),
                                 x => x.Out<bool?>("locked", SqlDbType.Bit, y => session.Locked = y.GetValueOrDefault()),
                                 x => x.Out<int?>("lockAge", SqlDbType.Int, y => session.LockAge = y.GetValueOrDefault()),
                                 x => x.Out<int?>("lockCookie", SqlDbType.Int, y => session.LockCookie = y.GetValueOrDefault()),
                                 x => x.Out<int?>("actionFlags", SqlDbType.Int, y => session.ActionFlags = y.GetValueOrDefault()));
            return session;
        }
    }
}