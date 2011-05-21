using NUnit.Framework;
using RemoteSession;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class SqlSessionIdTests
    {
        [Test]
        public void Should_Create_A_Fully_Qualifed_Session_Id()
        {
            var sessionId = SqlSessionId.Create("/lm/w3svc/1/root", "4m4irghotazmxblpyakfaw1d2014c0f1");
            sessionId.ToString().ShouldEqual("4m4irghotazmxblpyakfaw1d2014c0f12014c0f1");
        }
    }
}
