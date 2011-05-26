using System;
using NUnit.Framework;
using RemoteSessionState;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class SystemWebConfigurationTests
    {
        [Test]
        public void Should_Read_Existing_Value_From_Session_Config()
        {
            var webConfiguration = new SystemWebConfiguration(Environment.CurrentDirectory);
            webConfiguration.SessionState.SqlConnectionString.ShouldEqual("server=localhost;database=ASPNetSessionState;Integrated Security=SSPI");
        }

        [Test]
        public void Should_Read_Missing_Value_From_Sessionb_Config()
        {
            var webConfiguration = new SystemWebConfiguration(Environment.CurrentDirectory);
            webConfiguration.SessionState.StateNetworkTimeout.ShouldEqual(new TimeSpan(0, 0, 10));
        }
    }
}
