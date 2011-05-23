using NSubstitute;
using NUnit.Framework;
using RemoteSessionState;
using RemoteSessionState.Interop;
using Should;
using Tests.Common;

namespace Tests.Unit
{
    [TestFixture]
    public class SessionStateContextTests
    {
        private static HttpContext CreateMockableContext()
        {
            return new HttpContext(
                Substitute.For<IServerVariables>(),
                Substitute.For<ICookies>(),
                Substitute.For<ICookies>(),
                Substitute.For<ISessionState>());
        }

        [Test]
        public void Should_Not_Have_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionContext = new SessionStateContext(context);
            sessionContext.HasActiveSession().ShouldBeFalse();
        }

        [Test]
        public void Should_Have_An_Active_Session()
        {
            var context = CreateMockableContext();
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            var sessionContext = new SessionStateContext(context);
            sessionContext.HasActiveSession().ShouldBeTrue();
        }

        [Test]
        public void Should_Create_A_New_Session()
        {
            var context = CreateMockableContext();
            var sessionContext = new SessionStateContext(context);
            sessionContext.CreateNewSession();

            sessionContext.SessionId.ShouldNotBeEmpty();
            context.ResponseCookies.Received()[SessionStateContext.AspNetSessionCookieName] = Arg.Any<string>();
        }
    }
}
