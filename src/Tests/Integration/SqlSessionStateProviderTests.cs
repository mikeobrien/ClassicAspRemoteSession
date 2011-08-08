using System.Linq;
using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;
using Tests.Common.TestDoubles;
using SessionState = RemoteSessionState.SessionState;

namespace Tests.Integration
{
    [TestFixture]
    public class SqlSessionStateProviderTests
    {
        [SetUp]
        public void Setup()
        {
            SessionDatabase.ClearSessions();
        }

        [TearDown]
        public void TearDown()
        {
            SessionDatabase.ClearSessions();
        }

        [Test]
        public void Open_Should_Not_Do_Anything_If_There_Is_No_Session()
        {
            var context = HttpContext.CreateWithoutSession();
            new SessionState(new SqlSessionStateProvider()).Load(context);

            context.SessionState.ShouldBeEmpty();
        }

        [Test]
        public void Open_Should_Sync_Session_Variables_If_There_Is_An_Active_Session()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, Constants.SessionStateSerializedBytes);
            var context = HttpContext.CreateWithSession();

            context.SessionState["test"] = "test";

            new SessionState(new SqlSessionStateProvider()).Load(context);

            context.SessionState.Count().ShouldEqual(Constants.SessionStateItemCount);
            context.SessionState[Constants.SessionStateKey1].ShouldEqual(Constants.SessionStateValue1);
            context.SessionState[Constants.SessionStateKey2].ShouldEqual(Constants.SessionStateValue2);
            context.SessionState[Constants.SessionStateKey3].ShouldEqual(Constants.SessionStateValue3);
        }

        [Test]
        public void Open_Should_Not_Sync_Incompatable_Data_Types()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, Constants.SessionStateMultiDataTypeSerializedBytes);
            var context = HttpContext.CreateWithSession();

            new SessionState(new SqlSessionStateProvider()).Load(context);

            context.SessionState.Count().ShouldEqual(Constants.SessionStateMultiDataTypeValidItemCount);
            context.SessionState[Constants.SessionStateStringKey].ShouldEqual(Constants.SessionStateStringValue);
            context.SessionState[Constants.SessionStateByteKey].ShouldEqual(Constants.SessionStateByteValue);
            context.SessionState[Constants.SessionStateBoolKey].ShouldEqual(Constants.SessionStateBoolValue);
            context.SessionState[Constants.SessionStateShortKey].ShouldEqual(Constants.SessionStateShortValue);
            context.SessionState[Constants.SessionStateIntKey].ShouldEqual(Constants.SessionStateIntValue);
            context.SessionState[Constants.SessionStateDoubleKey].ShouldEqual(Constants.SessionStateDoubleValue);
            context.SessionState[Constants.SessionStateFloatKey].ShouldEqual(Constants.SessionStateFloatValue);
            context.SessionState[Constants.SessionStateDateTimeKey].ShouldEqual(Constants.SessionStateDateTimeValue);
        }

        [Test]
        public void Open_Should_Clear_Session_If_The_Session_Is_Empty()
        {
            var context = HttpContext.CreateWithSession();
            var sessionProvider = new SqlSessionStateProvider();

            context.SessionState["test"] = "test";

            new SessionState(sessionProvider).Load(context);

            context.SessionState.Count().ShouldEqual(0);
        }

        [Test]
        public void Save_Should_Save_Variables_To_The_Active_Session()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, Constants.SessionStateMultiDataTypeSerializedBytes);
            var context = HttpContext.CreateWithSession(Constants.SessionState);
            
            var session = new SessionState(new SqlSessionStateProvider());
            session.Save(context);
            session.Load(context);

            context.SessionState.Count().ShouldEqual(Constants.SessionStateItemCount);
            context.SessionState[Constants.SessionStateKey1].ShouldEqual(Constants.SessionStateValue1);
            context.SessionState[Constants.SessionStateKey2].ShouldEqual(Constants.SessionStateValue2);
            context.SessionState[Constants.SessionStateKey3].ShouldEqual(Constants.SessionStateValue3);
        }

        [Test]
        public void Save_Should_Not_Sync_Incompatable_Data_Types()
        {
            var context = HttpContext.CreateWithSession(Constants.SessionStateMultiDataType);

            var session = new SessionState(new SqlSessionStateProvider());
            session.Save(context);
            session.Load(context);

            context.SessionState.Count().ShouldEqual(Constants.SessionStateMultiDataTypeValidItemCount);
            context.SessionState[Constants.SessionStateStringKey].ShouldEqual(Constants.SessionStateStringValue);
            context.SessionState[Constants.SessionStateByteKey].ShouldEqual(Constants.SessionStateByteValue);
            context.SessionState[Constants.SessionStateBoolKey].ShouldEqual(Constants.SessionStateBoolValue);
            context.SessionState[Constants.SessionStateShortKey].ShouldEqual(Constants.SessionStateShortValue);
            context.SessionState[Constants.SessionStateIntKey].ShouldEqual(Constants.SessionStateIntValue);
            context.SessionState[Constants.SessionStateDoubleKey].ShouldEqual(Constants.SessionStateDoubleValue);
            context.SessionState[Constants.SessionStateFloatKey].ShouldEqual(Constants.SessionStateFloatValue);
            context.SessionState[Constants.SessionStateDateTimeKey].ShouldEqual(Constants.SessionStateDateTimeValue);
        }

        [Test]
        public void Save_Should_Create_A_New_Session_And_Save_The_Varaibles_To_It()
        {
            var context = HttpContext.CreateWithSession(Constants.SessionState);

            var session = new SessionState(new SqlSessionStateProvider());
            session.Save(context);
            session.Load(context);

            context.SessionState.Count().ShouldEqual(Constants.SessionStateItemCount);
            context.SessionState[Constants.SessionStateKey1].ShouldEqual(Constants.SessionStateValue1);
            context.SessionState[Constants.SessionStateKey2].ShouldEqual(Constants.SessionStateValue2);
            context.SessionState[Constants.SessionStateKey3].ShouldEqual(Constants.SessionStateValue3);
        }

        [Test]
        public void Should_Not_Clear_Remote_Session_When_Abandoned_If_There_Is_Not_A_Sesison_Id()
        {
            var context = HttpContext.CreateWithoutSession();
            new SessionState(new SqlSessionStateProvider()).Abandon(context);
        }

        [Test]
        public void Should_Abandon_Session_If_There_Is_A_Session_Id()
        {
            SessionDatabase.CreateSession(Constants.FullSessionId, Constants.SessionStateSerializedBytes);
            var context = HttpContext.CreateWithSession();

            var session = new SessionState(new SqlSessionStateProvider());
            session.Abandon(context);
            session.Load(context);

            context.SessionState.Count().ShouldEqual(0);
            SessionDatabase.GetSession(Constants.FullSessionId).ShouldBeNull();
        }
    }
}
