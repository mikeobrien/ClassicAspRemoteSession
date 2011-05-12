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

        private static HttpContext CreateMockableContext()
        {
            return new HttpContext(
                Substitute.For<IServerVariables>(),
                Substitute.For<ICookies>(),
                Substitute.For<ICookies>(),
                Substitute.For<ISession>());
        }

        [Test]
        public void Open_Should_Not_Do_Anything_If_There_Is_No_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();

            context.RequestCookies[Session.AspNetSessionCookieName].Returns((string)null);

            new Session(sessionProvider).Open(context);

            sessionProvider.DidNotReceiveWithAnyArgs().Open(null, null);
            context.Session.DidNotReceive().Abandon();
        }

        [Test] 
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Open(null, null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Open(null, null);
            context.Session.Received()["name1"] = Arg.Is("value1");
            context.Session.Received()["name2"] = Arg.Is("value2");
            context.Session.DidNotReceive().Abandon();
        }

        [Test]
        public void Open_Should_Abandon_Session_If_The_Session_Has_Expired()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();

            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Open(null, null).ReturnsForAnyArgs((IDictionary<string,object>)null);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Open(null, null);
            context.Session.DidNotReceiveWithAnyArgs()[null] = Arg.Any<object>();
            context.Session.Received().Abandon();
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<string>(), SessionId,
                Arg.Is<Dictionary<string, object>>(x => x["name1"] == "value1" && x["name2"] == "value2"));
            context.ResponseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Create_A_New_Session_And_Save_The_Varaibles_To_It()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.RequestCookies[Session.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<string>(), Arg.Any<string>(), 
                Arg.Is<Dictionary<string, object>>(x => x["name1"] == "value1" && x["name2"] == "value2"));
            context.ResponseCookies.Received()[Session.AspNetSessionCookieName] = Arg.Any<string>();
        }
    }
}
