using System.Linq;
using System.Web;
using System.Web.SessionState;
using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;

namespace Tests.Unit
{
    [TestFixture]
    public class SessionStateEncodingTests
    {
        [Test]
        public void Should_Decode_Session_Data()
        {
            var encoding = new SessionStateEncoding();
            var sessionState = encoding.Deserialize(Constants.SessionStateSerializedBytes);
            sessionState.Items[Constants.SessionStateKey1].ShouldEqual(Constants.SessionStateValue1);
            sessionState.Items[Constants.SessionStateKey2].ShouldEqual(Constants.SessionStateValue2);
            sessionState.Items[Constants.SessionStateKey3].ShouldEqual(Constants.SessionStateValue3);
            sessionState.Timeout.ShouldEqual(Constants.SessionTimeout);
        }

        [Test]
        public void Should_Encode_Session_Data()
        {
            var encoding = new SessionStateEncoding();
            var sessionState = new SessionStateStoreData(new SessionStateItemCollection(), new HttpStaticObjectsCollection(), 20);
            sessionState.Items[Constants.SessionStateKey1] = Constants.SessionStateValue1;
            sessionState.Items[Constants.SessionStateKey2] = Constants.SessionStateValue2;
            sessionState.Items[Constants.SessionStateKey3] = Constants.SessionStateValue3;
            encoding.Serialize(sessionState).SequenceEqual(Constants.SessionStateSerializedBytes).ShouldBeTrue();
        }
    }
}
