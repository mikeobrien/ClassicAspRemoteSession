using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using RemoteSession.Asp;

namespace Tests
{
    [TestFixture]
    public class SessionTests
    {
        private const string SessionId = "3D97A1F743CD44BE987FD3974CFA9ED8";

        [Test]
        public void Open_Should_Not_Do_Anything_If_There_Is_No_Session()
        {
            var cookies = Substitute.For<ICookies>();
            var session = Substitute.For<ISession>();
            var sessionProvider = Substitute.For<ISessionProvider>();

            cookies[Session.AspNetSessionCookieName].Returns((string)null);

            new Session(sessionProvider).Open(cookies, session);

            sessionProvider.DidNotReceiveWithAnyArgs().Open(null);
            session.DidNotReceive().Abandon();
        }

        [Test] 
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            var cookies = Substitute.For<ICookies>();
            var session = Substitute.For<ISession>();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            cookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Open(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Open(cookies, session);

            sessionProvider.ReceivedWithAnyArgs().Open(SessionId);
            session.Received()["name1"] = Arg.Is("value1");
            session.Received()["name2"] = Arg.Is("value2");
            session.DidNotReceive().Abandon();
        }

        [Test]
        public void Open_Should_Abandon_Session_If_The_Session_Has_Expired()
        {
            var cookies = Substitute.For<ICookies>();
            var session = Substitute.For<ISession>();
            var sessionProvider = Substitute.For<ISessionProvider>();

            cookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Open(null).ReturnsForAnyArgs((IDictionary<string,object>)null);

            new Session(sessionProvider).Open(cookies, session);

            sessionProvider.ReceivedWithAnyArgs().Open(SessionId);
            session.DidNotReceiveWithAnyArgs()[null] = Arg.Any<object>();
            session.Received().Abandon();
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            var requestCookies = Substitute.For<ICookies>();
            var responseCookies = Substitute.For<ICookies>();
            var session = Substitute.For<ISession>();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            requestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(requestCookies, responseCookies, session);

            sessionProvider.Received().Save(SessionId, 
                Arg.Is<Dictionary<string, object>>(x => x["name1"] == "value1" && x["name2"] == "value2"));
            responseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Create_A_New_Session_And_Save_The_Varaibles_To_It()
        {
            var requestCookies = Substitute.For<ICookies>();
            var responseCookies = Substitute.For<ICookies>();
            var session = Substitute.For<ISession>();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            requestCookies[Session.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            session.GetEnumerator().Returns(values.GetEnumerator());
            sessionProvider.Save(null).ReturnsForAnyArgs(SessionId);

            new Session(sessionProvider).Save(requestCookies, responseCookies, session);

            sessionProvider.Received().Save(Arg.Is<Dictionary<string, object>>(x => x["name1"] == "value1" && x["name2"] == "value2"));
            responseCookies.Received()[Session.AspNetSessionCookieName] = Arg.Is(SessionId);
        }
    }
}
