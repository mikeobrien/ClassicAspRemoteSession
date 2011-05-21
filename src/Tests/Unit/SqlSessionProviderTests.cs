using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class SqlSessionProviderTests
    {
        private const string SerializedSessionData = "14000000010003000000FFFFFFFF046E616D650361676505707269636504000000090000001200000001024564022C000000090000000000404140FF";
        private readonly byte[] _sessionData = Enumerable.Range(0, SerializedSessionData.Length).
                                                         Where(x => x % 2 == 0).
                                                         Select(x => Convert.ToByte(SerializedSessionData.Substring(x, 2), 16)).
                                                         ToArray();
        private readonly ISessionContext _context;

        public SqlSessionProviderTests()
        {
            _context = Substitute.For<ISessionContext>();
            _context.MetabasePath.Returns("/lm/w3svc/1/root");
            _context.SessionId.Returns("1sc3aoog5u5zyko2tl3ghnvq");
        }

        [Test]
        public void Should_Not_Load_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStore>();
            sessionStore.Load(null).ReturnsForAnyArgs((byte[])null);
            new SqlSessionProvider(sessionStore).Load(_context).ShouldBeNull();
        }

        [Test]
        public void Should_Load_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStore>();
            sessionStore.Load(null).ReturnsForAnyArgs(_sessionData);
            var sessionState = new SqlSessionProvider(sessionStore).Load(_context);
            sessionState.ShouldNotBeNull();
            sessionState.ShouldNotBeEmpty();
            sessionState["name"].ShouldEqual("Ed");
            sessionState["age"].ShouldEqual(44);
            sessionState["price"].ShouldEqual(34.5);
        }

        [Test] 
        public void Should_Save_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStore>();
            var values = new Dictionary<string, object> { { "name", "Ed" }, { "age", 44 }, { "price", 34.5 } };
            new SqlSessionProvider(sessionStore).Save(_context, values);
            sessionStore.Received().Save(Arg.Any<SqlSessionId>(), Arg.Is<byte[]>(x => x.SequenceEqual(_sessionData)));
        }

        [Test]
        public void Should_Abandon_Session()
        {
            var sessionStore = Substitute.For<ISqlSessionStore>();
            new SqlSessionProvider(sessionStore).Abandon(_context);
            sessionStore.Received().Abandon(Arg.Any<SqlSessionId>());
        }
    }
}
