using System;
using NUnit.Framework;
using RemoteSession;
using Should;

namespace Tests.Integration
{
    [TestFixture]
    public class SqlSessionStoreTests
    {
        private const string ConnectionString = "server=localhost;database=ASPNetSessionState;Integrated Security=SSPI";
        private readonly SqlSessionId _sessionId = SqlSessionId.Create("/lm/w3svc/1/root", "4m4irghotazmxblpyakfaw1d2014c0f1");
        private static readonly byte[] ShortData = GetByteArray(500);
        private static readonly byte[] LongData = GetByteArray(9000);
        private const int Timeout = 20; 
        
        [SetUp]
        public void Setup()
        {
            ClearSessionData();
        }

        [TearDown]
        public void TearDown()
        {
            ClearSessionData();
        }

        [Test]
        public void Should_Return_A_Null_Session()
        {
            var store = new SqlSessionStore(ConnectionString, Timeout);
            var session = store.Load(_sessionId);
            session.ShouldBeNull();
        }

        [Test]
        public void Should_Load_A_Session_And_Unlock_It_Afterwards()
        {
            CreateSession(_sessionId);
            var store = new SqlSessionStore(ConnectionString, Timeout);
            var session = store.Load(_sessionId);

            session.ShouldEqual(ShortData);
            SessionExists(_sessionId).ShouldBeTrue();
            IsLocked(_sessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Abandon_Session()
        {
            CreateSession(_sessionId);
            var store = new SqlSessionStore(ConnectionString, Timeout);
            store.Abandon(_sessionId);

            SessionExists(_sessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Write_New_Session_With_Short_Data()
        {
            var store = new SqlSessionStore(ConnectionString, Timeout);
            store.Save(_sessionId, ShortData);

            SessionExists(_sessionId).ShouldBeTrue();
            IsLocked(_sessionId).ShouldBeFalse();
            GetValue<DateTime?>(_sessionId, "Created").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "Expires").ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDate").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDateLocal").ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            GetValue<int?>(_sessionId, "LockCookie").ShouldEqual(1);
            GetValue<int?>(_sessionId, "Timeout").ShouldEqual(20);
            GetValue<byte[]>(_sessionId, "SessionItemShort").ShouldEqual(ShortData);
            GetValue<object>(_sessionId, "SessionItemLong").ShouldBeNull();
            GetValue<int?>(_sessionId, "Flags").ShouldEqual(0);
        }

        [Test]
        public void Should_Write_New_Session_With_Long_Data()
        {
            var store = new SqlSessionStore(ConnectionString, Timeout);
            store.Save(_sessionId, LongData);

            SessionExists(_sessionId).ShouldBeTrue();
            IsLocked(_sessionId).ShouldBeFalse();
            GetValue<DateTime?>(_sessionId, "Created").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "Expires").ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDate").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDateLocal").ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            GetValue<int?>(_sessionId, "LockCookie").ShouldEqual(1);
            GetValue<int?>(_sessionId, "Timeout").ShouldEqual(20);
            GetValue<object>(_sessionId, "SessionItemShort").ShouldBeNull();
            GetValue<byte[]>(_sessionId, "SessionItemLong").ShouldEqual(LongData);
            GetValue<int?>(_sessionId, "Flags").ShouldEqual(0);
        }

        [Test]
        public void Should_Write_Existing_Session_With_Short_Data()
        {
            CreateSession(_sessionId);
            var store = new SqlSessionStore(ConnectionString, Timeout);
            store.Save(_sessionId, ShortData);

            SessionExists(_sessionId).ShouldBeTrue();
            IsLocked(_sessionId).ShouldBeFalse();
            GetValue<DateTime?>(_sessionId, "Created").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "Expires").ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDate").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDateLocal").ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            GetValue<int?>(_sessionId, "LockCookie").ShouldEqual(1);
            GetValue<int?>(_sessionId, "Timeout").ShouldEqual(20);
            GetValue<byte[]>(_sessionId, "SessionItemShort").ShouldEqual(ShortData);
            GetValue<object>(_sessionId, "SessionItemLong").ShouldBeNull();
            GetValue<int?>(_sessionId, "Flags").ShouldEqual(0);
        }

        [Test]
        public void Should_Write_Existing_Session_With_Long_Data()
        {
            CreateSession(_sessionId);
            var store = new SqlSessionStore(ConnectionString, Timeout);
            store.Save(_sessionId, LongData);

            SessionExists(_sessionId).ShouldBeTrue();
            IsLocked(_sessionId).ShouldBeFalse();
            GetValue<DateTime?>(_sessionId, "Created").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "Expires").ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDate").ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            GetValue<DateTime?>(_sessionId, "LockDateLocal").ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            GetValue<int?>(_sessionId, "LockCookie").ShouldEqual(1);
            GetValue<int?>(_sessionId, "Timeout").ShouldEqual(20);
            GetValue<object>(_sessionId, "SessionItemShort").ShouldBeNull();
            GetValue<byte[]>(_sessionId, "SessionItemLong").ShouldEqual(LongData);
            GetValue<int?>(_sessionId, "Flags").ShouldEqual(0);
        }

        private static void CreateSession(SqlSessionId sessionId)
        {
            ConnectionString.ExecuteNonQuery(
                "INSERT INTO ASPStateTempSessions (SessionId, Created, Expires, LockDate, LockDateLocal, LockCookie, Timeout, Locked, SessionItemShort, SessionItemLong, Flags) VALUES " +
                                                 "('{0}', GETUTCDATE(), DATEADD(n, 20, GETUTCDATE()), GETUTCDATE(), GETDATE(), 0, 20, 0, {1}, NULL, 1)", sessionId, ConvertToHex(ShortData));
            
        }

        private static T GetValue<T>(SqlSessionId sessionId, string name)
        {
            var value = ConnectionString.ExecuteScalar<T>("SELECT TOP 1 {0} FROM ASPStateTempSessions WHERE SessionId='{1}'", name, sessionId);
            return (T)(Convert.IsDBNull(value) ? null : (object)value);
        }

        private static bool IsLocked(SqlSessionId sessionId)
        {
            return ConnectionString.ExecuteScalar<bool>("SELECT TOP 1 Locked FROM ASPStateTempSessions WHERE SessionId='{0}'", sessionId);
        }

        private static bool SessionExists(SqlSessionId sessionId)
        {
            return ConnectionString.ExecuteScalar<bool>("SELECT TOP 1 CAST(COUNT(*) AS bit) FROM ASPStateTempSessions WHERE SessionId='{0}'", sessionId);
        }

        private static void ClearSessionData()
        {
            ConnectionString.ExecuteNonQuery(
                "DELETE FROM ASPStateTempApplications;" +
                "DELETE FROM ASPStateTempSessions");
        }

        private static byte[] GetByteArray(int length)
        {
            var array = new byte[length];
            for (var x = 0; x < array.Length; x++) array[x] = 1;
            return array;
        }

        private static string ConvertToHex(byte[] data)
        {
            return "0x" + BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
