using System.Linq;
using NSubstitute;
using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;

namespace Tests.Unit
{
    [TestFixture]
    public class SqlSessionStateProviderTests
    {
        private readonly ISessionStateContext _context;

        public SqlSessionStateProviderTests()
        {
            _context = Substitute.For<ISessionStateContext>();
            _context.MetabasePath.Returns(Constants.MetabasePath);
            _context.SessionId.Returns(Constants.SessionId);
        }

        [Test]
        public void Should_Not_Load_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStateStore>();
            sessionStore.Load(null).ReturnsForAnyArgs((byte[])null);
            new SqlSessionStateProvider(sessionStore).Load(_context).ShouldBeNull();
        }

        [Test]
        public void Should_Load_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStateStore>();
            sessionStore.Load(null).ReturnsForAnyArgs(Constants.SessionStateSerializedBytes);
            var sessionState = new SqlSessionStateProvider(sessionStore).Load(_context);
            sessionState.ShouldNotBeNull();
            sessionState.ShouldNotBeEmpty();
            sessionState[Constants.SessionStateKey1].ShouldEqual(Constants.SessionStateValue1);
            sessionState[Constants.SessionStateKey2].ShouldEqual(Constants.SessionStateValue2);
            sessionState[Constants.SessionStateKey3].ShouldEqual(Constants.SessionStateValue3);
        }

        [Test] 
        public void Should_Save_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStateStore>();
            new SqlSessionStateProvider(sessionStore).Save(_context, Constants.SessionState);
            sessionStore.Received().Save(Arg.Any<SqlSessionId>(), Arg.Is<byte[]>(x => x.SequenceEqual(Constants.SessionStateSerializedBytes)));
        }

        [Test]
        public void Should_Abandon_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStateStore>();
            new SqlSessionStateProvider(sessionStore).Abandon(_context);
            sessionStore.Received().Abandon(Arg.Any<SqlSessionId>());
        }
    }
}
