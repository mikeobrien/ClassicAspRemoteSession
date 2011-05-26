using System;
using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;

namespace Tests.Integration
{
    [TestFixture]
    public class SqlSessionStoreTests
    {
        private readonly SqlSessionId _sessionId = SqlSessionId.Create(Constants.MetabasePath, Constants.SessionId);
        private static readonly byte[] ShortData = GetByteArray(500);
        private static readonly byte[] LongData = GetByteArray(9000);
        
        [SetUp]
        public void Setup()
        {
            SessionDatabase.ClearSessions();
        }

        [TearDown]
        public void TearDown()
        {
            SessionDatabase.ClearSessions();
        }

        [Test]
        public void Should_Return_A_Null_Session()
        {
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            var session = store.Load(_sessionId);
            session.ShouldBeNull();
        }

        [Test]
        public void Should_Load_A_Session_And_Unlock_It_Afterwards()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, ShortData);
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            var sessionState = store.Load(_sessionId);

            sessionState.ShouldEqual(ShortData);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
        }

        [Test]
        public void Should_Load_An_Empty_Session_And_Unlock_It_Afterwards()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, Constants.SessionStateEmptySerializedBytes);
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            var sessionState = store.Load(_sessionId);

            sessionState.ShouldEqual(Constants.SessionStateEmptySerializedBytes);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
        }

        [Test]
        public void Should_Abandon_Session()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, ShortData);
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            store.Abandon(_sessionId);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldBeNull();
        }

        [Test]
        public void Should_Write_New_Session_With_Short_Data()
        {
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            store.Save(_sessionId, ShortData);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
            session.Created.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.Expires.ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            session.LockDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.LockDateLocal.ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            session.LockCookie.ShouldEqual(1);
            session.Timeout.ShouldEqual(Constants.SessionTimeout);
            session.SessionItemShort.ShouldEqual(ShortData);
            session.SessionItemLong.ShouldBeNull();
            session.Flags.ShouldEqual(0);
        }

        [Test]
        public void Should_Write_New_Session_With_Long_Data()
        {
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            store.Save(_sessionId, LongData);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
            session.Created.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.Expires.ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            session.LockDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.LockDateLocal.ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            session.LockCookie.ShouldEqual(1);
            session.Timeout.ShouldEqual(20);
            session.SessionItemShort.ShouldBeNull();
            session.SessionItemLong.ShouldEqual(LongData);
            session.Flags.ShouldEqual(0);
        }

        [Test]
        public void Should_Write_Existing_Session_With_Short_Data()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, ShortData);
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            store.Save(_sessionId, ShortData);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
            session.Created.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.Expires.ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            session.LockDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.LockDateLocal.ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            session.LockCookie.ShouldEqual(1);
            session.Timeout.ShouldEqual(20);
            session.SessionItemShort.ShouldEqual(ShortData);
            session.SessionItemLong.ShouldBeNull();
            session.Flags.ShouldEqual(0);
        }

        [Test]
        public void Should_Write_Existing_Session_With_Long_Data()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, ShortData);
            var store = new SqlSessionStateStore(Constants.ConnectionString, Constants.SessionTimeout);
            store.Save(_sessionId, LongData);

            var session = SessionDatabase.GetSession(Constants.FullSessionId);

            session.ShouldNotBeNull("No session found");
            session.Locked.ShouldBeFalse();
            session.Created.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.Expires.ShouldBeInRange(DateTime.UtcNow.AddMinutes(20).AddSeconds(-5), DateTime.UtcNow.AddMinutes(20).AddSeconds(5));
            session.LockDate.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
            session.LockDateLocal.ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
            session.LockCookie.ShouldEqual(1);
            session.Timeout.ShouldEqual(20);
            session.SessionItemShort.ShouldBeNull();
            session.SessionItemLong.ShouldEqual(LongData);
            session.Flags.ShouldEqual(0);
        }

        private static byte[] GetByteArray(int length)
        { 
            var array = new byte[length];
            for (var x = 0; x < array.Length; x++) array[x] = 1;
            return array;
        }
    }
}
