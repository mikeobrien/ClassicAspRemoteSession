using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;

namespace Tests.Unit
{
    [TestFixture]
    public class SqlSessionIdTests
    {
        [Test]
        public void Should_Create_A_Fully_Qualifed_Session_Id()
        {
            var sessionId = SqlSessionId.Create(Constants.MetabasePath, Constants.SessionId);
            sessionId.ToString().ShouldEqual(Constants.FullSessionId);
        }
    }
}
