using System;
using System.IO;
using NUnit.Framework;
using RemoteSessionState;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class WebConfigTests
    {
        private const string WebConfigContents = 
                @"<configuration>
                  <system.web>
                    <compilation debug=""true"" targetFramework=""4.0"" />
                    <sessionState mode=""SQLServer"" allowCustomSqlDatabase=""true"" 
                                  sqlConnectionString=""server=localhost;database=ASPNetSessionState;Integrated Security=SSPI"" />
                  </system.web>
                </configuration>";

        private readonly string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(_path);
            File.WriteAllText(Path.Combine(_path, WebConfig.Filename), WebConfigContents);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_path, true);
        }

        [Test]
        public void Should_Read_String_Attribute_From_Web_Config()
        {
            var webConfig = new WebConfig(_path);
            webConfig.GetValue<string>("configuration/system.web/sessionState/@sqlConnectionString").ShouldEqual("server=localhost;database=ASPNetSessionState;Integrated Security=SSPI");
        }

        [Test]
        public void Should_Read_Boolean_Attribute_From_Web_Config()
        {
            var webConfig = new WebConfig(_path);
            webConfig.GetValue<bool>("configuration/system.web/sessionState/@allowCustomSqlDatabase").ShouldBeTrue();
        }

        [Test]
        public void Should_Return_Null_When_Attribute_Is_Missing()
        {
            var webConfig = new WebConfig(_path);
            webConfig.GetValue<int>("configuration/system.web/sessionState/@timeout").ShouldEqual(0);
        }
    }
}
