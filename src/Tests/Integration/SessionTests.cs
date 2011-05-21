using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using RemoteSession.Interop;

namespace Tests.Integration
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
            var sessionProvider = new SqlSessionProvider();

            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns((string)null);

            new Session(sessionProvider).Load(context);

            sessionProvider.DidNotReceiveWithAnyArgs().Load(null);
            context.Session.DidNotReceive().RemoveAll();
        }

        [Test] 
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received().RemoveAll();
            context.Session.Received()["name1"] = "value1";
            context.Session.Received()["name2"] = "value2";
        }

        [Test]
        public void Open_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> {
                    { "string", "string" }, { "byte", (byte)25 }, { "bool", true }, { "short", (short)500 }, 
                    { "int", 700 }, { "double", 44.5 }, { "float", 36.7F }, { "datetime", DateTime.MinValue }, 
                    { "guid", Guid.Empty }
                };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(values);

            new Session(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received().RemoveAll();
            context.Session.Received()["string"] = "string";
            context.Session.Received()["byte"] = (byte)25;
            context.Session.Received()["bool"] = true;
            context.Session.Received()["short"] = (short)500;
            context.Session.Received()["int"] = 700;
            context.Session.Received()["double"] = 44.5;
            context.Session.Received()["float"] = 36.7F;
            context.Session.Received()["datetime"] = DateTime.MinValue;
            context.Session.DidNotReceive()["guid"] = Guid.Empty;
        }

        [Test]
        public void Open_Should_Clear_Session_If_The_Session_Is_Empty()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs((IDictionary<string,object>)null);

            new Session(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.Session.Received().RemoveAll();
            context.Session.DidNotReceiveWithAnyArgs()[null] = Arg.Any<object>();
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<SessionContext>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x["name1"] == "value1" && (string)x["name2"] == "value2"));
            context.ResponseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> {
                    { "string", "string" }, { "byte", (byte)25 }, { "bool", true }, { "short", (short)500 }, 
                    { "int", 700 }, { "double", 44.5 }, { "float", 36.7F }, { "datetime", DateTime.MinValue }, 
                    { "guid", Guid.Empty }
                };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].Returns(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);
            
            sessionProvider.Received().Save(Arg.Any<SessionContext>(),
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
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<SessionContext>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x["name1"] == "value1" && (string)x["name2"] == "value2"));
            context.ResponseCookies.Received()[SessionContext.AspNetSessionCookieName] = Arg.Any<string>();
        }

        [Test]
        public void Should_Not_Clear_Remote_Session_When_Abandoned_If_There_Is_Not_A_Sesison_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.RequestCookies[SessionContext.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Abandon(context);

            sessionProvider.DidNotReceive().Abandon(Arg.Any<SessionContext>());
            context.Session.Received().Abandon();
        }

        [Test]
        public void Should_Abandon_Session_If_There_Is_A_Session_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = new SqlSessionProvider();
            var values = new Dictionary<string, object> { { "name1", "value1" }, { "name2", "value2" } };

            context.ServerVariables[SessionContext.MetadataPathServerVariable].Returns(MetabasePath);
            context.RequestCookies[SessionContext.AspNetSessionCookieName].ReturnsForAnyArgs(SessionId);
            context.Session.GetEnumerator().Returns(values.GetEnumerator());

            new Session(sessionProvider).Abandon(context);

            sessionProvider.Received().Abandon(Arg.Any<SessionContext>());
            context.Session.Received().Abandon();
        }
    }
}
