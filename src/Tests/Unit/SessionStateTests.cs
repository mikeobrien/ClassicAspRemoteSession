using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RemoteSessionState;
using RemoteSessionState.Interop;
using Tests.Common;

namespace Tests.Unit
{
    [TestFixture]
    public class SessionStateTests
    {
        private const string SessionStateKey1 = "name1", SessionStateValue1 = "value1";
        private const string SessionStateKey2 = "name2", SessionStateValue2 = "value2";

        private readonly IDictionary<string, object> _sessionState = 
            new Dictionary<string, object> { { SessionStateKey1, SessionStateValue1 }, { SessionStateKey2, SessionStateValue2 } };

        private static HttpContext CreateMockableContext()
        {
            return new HttpContext(
                Substitute.For<IServerVariables>(),
                Substitute.For<ICookies>(),
                Substitute.For<ICookies>(),
                Substitute.For<ISessionState>());
        }

        [Test]
        public void Open_Should_Not_Do_Anything_If_There_Is_No_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns((string)null);

            new SessionState(sessionProvider).Load(context);

            sessionProvider.DidNotReceiveWithAnyArgs().Load(null);
            context.SessionState.DidNotReceive().RemoveAll();
        }

        [Test] 
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(_sessionState);

            new SessionState(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.SessionState.Received()[SessionStateKey1] = SessionStateValue1;
            context.SessionState.Received()[SessionStateKey2] = SessionStateValue2;
            context.SessionState.DidNotReceiveWithAnyArgs().Remove(null);
        }

        [Test]
        public void Open_Should_Should_Remove_Extra_Variables_If_There_Is_An_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();
            const string TestVariable = "test";

            context.SessionState.GetEnumerator().Returns(x => (new Dictionary<string, object> { { TestVariable, new object()}}).GetEnumerator());
            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(_sessionState);

            new SessionState(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.SessionState.Received()[SessionStateKey1] = SessionStateValue1;
            context.SessionState.Received()[SessionStateKey2] = SessionStateValue2;
            context.SessionState.Received().Remove(TestVariable);
        }

        [Test]
        public void Open_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs(Constants.SessionStateMultiDataType);

            new SessionState(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.SessionState.Received()[Constants.SessionStateStringKey] = Constants.SessionStateStringValue;
            context.SessionState.Received()[Constants.SessionStateByteKey] = Constants.SessionStateByteValue;
            context.SessionState.Received()[Constants.SessionStateBoolKey] = Constants.SessionStateBoolValue;
            context.SessionState.Received()[Constants.SessionStateShortKey] = Constants.SessionStateShortValue;
            context.SessionState.Received()[Constants.SessionStateIntKey] = Constants.SessionStateIntValue;
            context.SessionState.Received()[Constants.SessionStateDoubleKey] = Constants.SessionStateDoubleValue;
            context.SessionState.Received()[Constants.SessionStateFloatKey] = Constants.SessionStateFloatValue;
            context.SessionState.Received()[Constants.SessionStateDateTimeKey] = Constants.SessionStateDateTimeValue;
            context.SessionState.DidNotReceive()[Constants.SessionStateGuidKey] = Constants.SessionStateGuidValue;
            context.SessionState.DidNotReceiveWithAnyArgs().Remove(null);
        }

        [Test]
        public void Open_Should_Clear_Session_If_The_Session_Is_Empty()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            sessionProvider.Load(null).ReturnsForAnyArgs((IDictionary<string,object>)null);

            new SessionState(sessionProvider).Load(context);

            sessionProvider.ReceivedWithAnyArgs().Load(null);
            context.SessionState.Received().RemoveAll();
            context.SessionState.DidNotReceiveWithAnyArgs()[null] = Arg.Any<object>();
        }

        [Test]
        public void Save_Should_Save_Empty_Variables_To_The_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            context.SessionState.GetEnumerator().Returns((new Dictionary<string, object>()).GetEnumerator());

            new SessionState(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<SessionStateContext>(), Arg.Is<Dictionary<string, object>>(x => x.Count == 0));
            context.ResponseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            context.SessionState.GetEnumerator().Returns(_sessionState.GetEnumerator());

            new SessionState(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<SessionStateContext>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x[SessionStateKey1] == SessionStateValue1 && (string)x[SessionStateKey2] == SessionStateValue2));
            context.ResponseCookies.DidNotReceiveWithAnyArgs()[null] = Arg.Any<string>();
        }

        [Test]
        public void Save_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].Returns(Constants.SessionId);
            context.SessionState.GetEnumerator().Returns(Constants.SessionStateMultiDataType.GetEnumerator());

            new SessionState(sessionProvider).Save(context);
            
            sessionProvider.Received().Save(Arg.Any<SessionStateContext>(),
                Arg.Is<Dictionary<string, object>>(x =>
                        (string)x[Constants.SessionStateStringKey] == Constants.SessionStateStringValue &&
                        (byte)x[Constants.SessionStateByteKey] == Constants.SessionStateByteValue &&
                        (bool)x[Constants.SessionStateBoolKey] == Constants.SessionStateBoolValue &&
                        (short)x[Constants.SessionStateShortKey] == Constants.SessionStateShortValue &&
                        (int)x[Constants.SessionStateIntKey] == Constants.SessionStateIntValue &&
                        (double)x[Constants.SessionStateDoubleKey] == Constants.SessionStateDoubleValue &&
                        (float)x[Constants.SessionStateFloatKey] == Constants.SessionStateFloatValue &&
                        (DateTime)x[Constants.SessionStateDateTimeKey] == Constants.SessionStateDateTimeValue &&
                        !x.ContainsKey(Constants.SessionStateGuidKey)));
        }

        [Test]
        public void Save_Should_Create_A_New_Session_And_Save_The_Varaibles_To_It()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.SessionState.GetEnumerator().Returns(_sessionState.GetEnumerator());

            new SessionState(sessionProvider).Save(context);

            sessionProvider.Received().Save(Arg.Any<SessionStateContext>(),
                Arg.Is<Dictionary<string, object>>(x => (string)x[SessionStateKey1] == SessionStateValue1 && (string)x[SessionStateKey2] == SessionStateValue2));
            context.ResponseCookies.Received()[SessionStateContext.AspNetSessionCookieName] = Arg.Any<string>();
        }

        [Test]
        public void Should_Not_Clear_Remote_Session_When_Abandoned_If_There_Is_Not_A_Sesison_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].ReturnsForAnyArgs((string)null);
            context.SessionState.GetEnumerator().Returns(_sessionState.GetEnumerator());

            new SessionState(sessionProvider).Abandon(context);

            sessionProvider.DidNotReceive().Abandon(Arg.Any<SessionStateContext>());
            context.SessionState.Received().Abandon();
        }

        [Test]
        public void Should_Abandon_Session_If_There_Is_A_Session_Id()
        {
            var context = CreateMockableContext();
            var sessionProvider = Substitute.For<ISessionStateProvider>();

            context.ServerVariables[SessionStateContext.MetadataPathServerVariable].Returns(Constants.MetabasePath);
            context.RequestCookies[SessionStateContext.AspNetSessionCookieName].ReturnsForAnyArgs(Constants.SessionId);
            context.SessionState.GetEnumerator().Returns(_sessionState.GetEnumerator());

            new SessionState(sessionProvider).Abandon(context);

            sessionProvider.Received().Abandon(Arg.Any<SessionStateContext>());
            context.SessionState.Received().Abandon();
        }
    }
}
