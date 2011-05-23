using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.Common
{
    public static class SessionDatabase
    {
        public static void CreateSession(string sessionId, byte[] data)
        {
            Constants.ConnectionString.ExecuteNonQuery(
                "INSERT INTO ASPStateTempSessions (SessionId, Created, Expires, LockDate, LockDateLocal, LockCookie, Timeout, Locked, SessionItemShort, SessionItemLong, Flags) VALUES " +
                "(@SessionId, GETUTCDATE(), DATEADD(n, 20, GETUTCDATE()), GETUTCDATE(), GETDATE(), 0, 20, 0, @ShortSessionData, @LongSessionData, 1)", 
                new SqlParameter("SessionId", sessionId),
                new SqlParameter("ShortSessionData", SqlDbType.VarBinary) { Value = data.Length <= 7000 ? (object)data : DBNull.Value },
                new SqlParameter("LongSessionData", SqlDbType.Image) { Value = data.Length > 7000 ? (object)data : DBNull.Value });
        }

        public static SessionData GetSession(string sessionId)
        {
            var result = Constants.ConnectionString.ExecuteRecord(
                "SELECT TOP 1 * FROM ASPStateTempSessions WHERE SessionId = @SessionId",
                new SqlParameter("SessionId", sessionId));
            return result == null ? null :
                new SessionData
                    {
                        Created = (DateTime)result["Created"],
                        Expires = (DateTime)result["Expires"],
                        LockDate = (DateTime)result["LockDate"],
                        LockDateLocal = (DateTime)result["LockDateLocal"],
                        Locked = (bool)result["Locked"],
                        LockCookie = (int)result["LockCookie"],
                        Timeout = (int)result["Timeout"],
                        SessionItemShort = (byte[])result["SessionItemShort"].ValueOrNull(),
                        SessionItemLong = (byte[])result["SessionItemLong"].ValueOrNull(),
                        Flags = (int)result["Flags"],
                    };
        }

        public static void ClearSessions()
        {
            Constants.ConnectionString.ExecuteNonQuery(
                "DELETE FROM ASPStateTempApplications;" +
                "DELETE FROM ASPStateTempSessions");
        }

        public class SessionData
        {
            public DateTime Created { get; set; }
            public DateTime Expires { get; set; }
            public DateTime LockDate { get; set; }
            public DateTime LockDateLocal { get; set; }
            public bool Locked { get; set; }
            public int LockCookie { get; set; }
            public int Timeout { get; set; }
            public byte[] SessionItemShort { get; set; }
            public byte[] SessionItemLong { get; set; }
            public int Flags { get; set; }
        }
    }
}