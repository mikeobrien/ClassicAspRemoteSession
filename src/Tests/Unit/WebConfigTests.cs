using System;
using System.Web.Configuration;
using NUnit.Framework;
using RemoteSessionState;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class WebConfigTests
    {
        [Test]
        public void Should_Read_Existing_Value_From_Web_Config()
        {
            var webConfig = new WebConfig(Environment.CurrentDirectory);
            var sessionConfig = webConfig.GetSection<SessionStateSection>("system.web/sessionState");
            sessionConfig.SqlConnectionString.ShouldEqual("server=localhost;database=ASPNetSessionState;Integrated Security=SSPI");
        }

        [Test]
        public void Should_Read_Missing_Value_From_Web_Config()
        {
            var webConfig = new WebConfig(Environment.CurrentDirectory);
            var sessionConfig = webConfig.GetSection<SessionStateSection>("system.web/sessionState");
            sessionConfig.StateNetworkTimeout.ShouldEqual(new TimeSpan(0, 0, 10));
        }
    }
}
