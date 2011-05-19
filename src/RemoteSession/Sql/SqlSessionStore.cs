using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RemoteSession.Sql
{
    public class SqlSessionStore
    {
        private readonly string _connectionString;

        public SqlSessionStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetApplicationId(string applicationName)
        {
            var applicationId = 0;
            SqlProcedure.Create(_connectionString, "TempGetAppID").
                         Execute(x => x.In("appName", applicationName),
                                 x => x.Out<int>("appId", SqlDbType.Int, y => applicationId = y));
            return applicationId;
        }

        public SqlSession LoadSession(string sessionId)
        {
            var session = GetSessionData(sessionId);
            if (!session.Empty) ReleaseSessionLock(session);
            return session;
        }

        public void SaveSession(string sessionId, byte[] shortData)
        {
            var session = GetSessionData(sessionId);

        }

        public void AbandonSession(string sessionId)
        {
            var session = GetSessionData(sessionId);
            if (!session.Empty) AbandonSession(session);
        }

        private void AbandonSession(SqlSession session)
        {
            SqlProcedure.Create(_connectionString, "TempRemoveStateItem").
                         Execute(x => x.In("id", session.SessionId),
                                 x => x.In("lockCookie", session.LockCookie));
        }

        private void ReleaseSessionLock(SqlSession session)
        {
            SqlProcedure.Create(_connectionString, "TempReleaseStateItemExclusive").
                         Execute(x => x.In("id", session.SessionId),
                                 x => x.In("lockCookie", session.LockCookie));   
        }

        private SqlSession GetSessionData(string sessionId)
        {
            var session = new SqlSession { SessionId = sessionId };
            SqlProcedure.Create(_connectionString, "TempGetStateItemExclusive3").
                         Execute(x => session.Data = x.Read() ? (byte[])x[0] : session.Data,
                                 x => x.In("id", sessionId),
                                 x => x.Out<byte[]>("itemShort", SqlDbType.VarBinary, 7000, y => session.Data = y),
                                 x => x.Out<bool?>("locked", SqlDbType.Bit, y => session.Locked = y.GetValueOrDefault()),
                                 x => x.Out<int?>("lockAge", SqlDbType.Int, y => session.LockAge = y.GetValueOrDefault()),
                                 x => x.Out<int?>("lockCookie", SqlDbType.Int, y => session.LockCookie = y.GetValueOrDefault()),
                                 x => x.Out<int?>("actionFlags", SqlDbType.Int, y => session.ActionFlags = y.GetValueOrDefault()));
            return session;
        }
    }
}