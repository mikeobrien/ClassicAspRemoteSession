using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using RemoteSession.Asp;
using Should;

namespace Tests
{
    [TestFixture]
    public class SqlSessionStoreTests
    {
        private const string ConnectionString = "server=localhost;database=ASPNetSessionState;Integrated Security=SSPI";
        private const string ApplicationName = "/lm/w3svc/1/root";
        private const int ApplicationId = 538231025;
        
        [SetUp]
        public void Setup()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Should_Return_An_App_Id()
        {
            var store = new SqlSessionStore(ConnectionString);
            store.GetApplicationId(ApplicationName).ShouldEqual(ApplicationId);
        }
    }
}
