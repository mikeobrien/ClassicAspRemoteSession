using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using RemoteSession.Interop;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class SessionContextTests
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
        public void Should_Not_Have_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionContext = new SessionContext(context);
            sessionContext.HasActiveSession().ShouldBeFalse();
        }

        [Test]
        public void Should_Have_An_Active_Session()
        {
            var context = CreateMockableContext();
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            var sessionContext = new SessionContext(context);
            sessionContext.HasActiveSession().ShouldBeTrue();
        }

        [Test]
        public void Should_Create_A_New_Session()
        {
            var context = CreateMockableContext();
            var sessionContext = new SessionContext(context);
            sessionContext.CreateNewSession();

            sessionContext.SessionId.ShouldNotBeEmpty();
            context.ResponseCookies.Received()[SessionContext.AspNetSessionCookieName] = Arg.Any<string>();
        }
    }
}
