using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using RemoteSession.Asp;
using RemoteSession.Sql;
using Should;

namespace Tests
{
    [TestFixture]
    public class SqlSessionStoreTests
    {
        private const string ConnectionString = "server=localhost;database=ASPNetSessionState;Integrated Security=SSPI";
        private const string ApplicationName = "/lm/w3svc/1/root";
        private const int ApplicationId = 538231025;
        private const string SessionId = "4m4irghotazmxblpyakfaw1d2014c0f1";
        private static readonly byte[] ShortData = GetByteArray(500);
        private static readonly byte[] LongData = GetByteArray(9000);
        
        [SetUp]
        public void Setup()
        {
            ClearSessionData();
        }

        [TearDown]
        public void TearDown()
        {
            //ClearSessionData();
        }

        [Test]
        public void Should_Return_An_App_Id()
        {
            var store = new SqlSessionStore(ConnectionString);
            store.GetApplicationId(ApplicationName).ShouldEqual(ApplicationId);
        }

        [Test]
        public void Should_Return_An_Empty_Session()
        {
            var store = new SqlSessionStore(ConnectionString);
            var session = store.LoadSession(SessionId);
            session.Empty.ShouldBeTrue();
        }

        [Test]
        public void Should_Load_A_Session_And_Unlock_It_Afterwards()
        {
            CreateSession();
            var store = new SqlSessionStore(ConnectionString);
            var session = store.LoadSession(SessionId);

            session.Empty.ShouldBeFalse();
            session.Data.ShouldEqual(ShortData);
            session.ActionFlags.ShouldEqual(1);
            session.LockAge.ShouldEqual(0);
            session.LockCookie.ShouldEqual(1);
            session.Locked.ShouldBeFalse();
            session.SessionId.ShouldEqual(SessionId);
            SessionExists(SessionId).ShouldBeTrue();
            IsLocked(SessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Abandon_Session()
        {
            CreateSession();
            var store = new SqlSessionStore(ConnectionString);
            store.AbandonSession(SessionId);

            SessionExists(SessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Write_New_Session_With_Short_Data()
        {
            var store = new SqlSessionStore(ConnectionString);
            store.SaveSession(SessionId, ShortData);
            SessionExists(SessionId).ShouldBeTrue();
            IsLocked(SessionId).ShouldBeFalse();
            GetValue<DateTime>(SessionId, "Created").ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            GetValue<DateTime>(SessionId, "Expires").ShouldBeInRange(DateTime.Now.AddMinutes(20).AddSeconds(-5), DateTime.Now.AddMinutes(20).AddSeconds(5));
        }

        [Test]
        public void Should_Write_New_Session_With_Long_Data()
        {
            var store = new SqlSessionStore(ConnectionString);
            store.SaveSession(SessionId, LongData);
            SessionExists(SessionId).ShouldBeTrue();
            IsLocked(SessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Write_Existing_Session_With_Short_Data()
        {
            CreateSession();
            var store = new SqlSessionStore(ConnectionString);
            store.SaveSession(SessionId, ShortData);
            SessionExists(SessionId).ShouldBeTrue();
            IsLocked(SessionId).ShouldBeFalse();
        }

        [Test]
        public void Should_Write_Existing_Session_With_Long_Data()
        {
            CreateSession();
            var store = new SqlSessionStore(ConnectionString);
            store.SaveSession(SessionId, LongData);
            SessionExists(SessionId).ShouldBeTrue();
            IsLocked(SessionId).ShouldBeFalse();
        }

        private static void CreateSession()
        {
            ConnectionString.ExecuteNonQuery(
                "INSERT INTO ASPStateTempSessions (SessionId, Created, Expires, LockDate, LockDateLocal, LockCookie, Timeout, Locked, SessionItemShort, SessionItemLong, Flags) VALUES " +
                                                 "('{0}', GETDATE(), DATEADD(n, 20, GETDATE()), GETDATE(), GETDATE(), 0, 20, 0, {1}, NULL, 1)", SessionId, ShortData);
            
        }

        private static T GetValue<T>(string sessionId, string name)
        {
            var value = ConnectionString.ExecuteScalar<T>("SELECT TOP 1 {0} FROM ASPStateTempSessions WHERE SessionId='{1}'", name, sessionId);
            return Convert.IsDBNull(value) ? null : value;
        }

        private static bool IsLocked(string sessionId)
        {
            return ConnectionString.ExecuteScalar<bool>("SELECT TOP 1 Locked FROM ASPStateTempSessions WHERE SessionId='{0}'", sessionId);
        }

        private static bool SessionExists(string sessionId)
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
