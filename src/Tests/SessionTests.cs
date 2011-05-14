using System;
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
        private const string MetabasePath = "/lm/w3svc/1/root";
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

            sessionProvider.DidNotReceiveWithAnyArgs().Load(null);
            context.Session.DidNotReceive().Abandon();
        }

        [Test] 
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received()["name1"] = "value1";
            context.Session.Received()["name2"] = "value2";
            context.Session.DidNotReceive().Abandon();
        }

        [Test]
        public void Open_Should_Remove_Extra_Session_Variables()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };
            var extraValues = new Dictionary<string, object> { { "name3", "value3" }, { "name4", "value4" } };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(extraValues.GetEnumerator());
            sessionProvider.Load(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received()["name1"] = "value1";
            context.Session.Received()["name2"] = "value2";
            context.Session.Received().Remove("name3");
            context.Session.Received().Remove("name4");
            context.Session.DidNotReceive().Abandon();
        }

        [Test]
        public void Open_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> {
                    { "string", "string" }, { "byte", (byte)25 }, { "bool", true }, { "short", (short)500 }, 
                    { "int", 700 }, { "double", 44.5 }, { "float", 36.7F }, { "datetime", DateTime.MinValue }, 
                    { "guid", Guid.Empty }
                };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received()["string"] = "string";
            context.Session.Received()["byte"] = (byte)25;
            context.Session.Received()["bool"] = true;
            context.Session.Received()["short"] = (short)500;
            context.Session.Received()["int"] = 700;
            context.Session.Received()["double"] = 44.5;
            context.Session.Received()["float"] = 36.7F;
            context.Session.Received()["datetime"] = DateTime.MinValue;
            context.Session.DidNotReceive()["guid"] = Guid.Empty;
            context.Session.DidNotReceive().Abandon();
        }

        [Test]
        public void Open_Should_Abandon_Session_If_The_Session_Has_Expired()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs((IDictionary<string,object>)null);

            new Session(sessionProvider).Open(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.DidNotReceiveWithAnyArgs()[null] = Arg.Any<object>();
            context.Session.Received().Abandon();
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<Context>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x["name1"] == "value1" && (string)x["name2"] == "value2"));
            context.ResponseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> {
                    { "string", "string" }, { "byte", (byte)25 }, { "bool", true }, { "short", (short)500 }, 
                    { "int", 700 }, { "double", 44.5 }, { "float", 36.7F }, { "datetime", DateTime.MinValue }, 
                    { "guid", Guid.Empty }
                };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);
            
            sessionProvider.Received().Save(Arg.Any<Context>(),
                Arg.Is<Dictionary<string, object>>(x => 
                        (string)x["string"] == "string" && 
                        (byte)x["byte"] == 25 && 
                        (bool)x["bool"] &&
                        (short)x["short"] == 500 && 
                        (int)x["int"] == 700 && 
                        (double)x["double"] == 44.5 && 
                        (float)x["float"] == 36.7F && 
                        (DateTime)x["datetime"] == DateTime.MinValue && 
                        !x.ContainsKey("guid")));
        }

        [Test]
        public void Save_Should_Create_A_New_Session_And_Save_The_Varaibles_To_It()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<Context>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x["name1"] == "value1" && (string)x["name2"] == "value2"));
            context.ResponseCookies.Received()[Session.AspNetSessionCookieName] = Arg.Any<string>();
        }

        [Test]
        public void Should_Not_Abandon_Remote_Session_If_There_Is_Not_A_Sesison_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.RequestCookies[Session.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Abandon(context);

            sessionProvider.DidNotReceive().Abandon(Arg.Any<Context>());
            context.Session.Received().Abandon();
        }

        [Test]
        public void Should_Abandon_Session_If_There_Is_A_Session_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionProvider>();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[Session.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[Session.AspNetSessionCookieName].ReturnsForAnyArgs(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Abandon(context);

            sessionProvider.Received().Abandon(Arg.Any<Context>());
            context.Session.Received().Abandon();
        }
    }
}
