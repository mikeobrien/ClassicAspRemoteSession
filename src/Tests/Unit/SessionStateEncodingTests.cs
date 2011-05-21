using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using NUnit.Framework;
using RemoteSession;
using Should;

namespace Tests.Unit
{
    [TestFixture]
    public class SessionStateEncodingTests
    {
        private const string SerializedSessionData = "14000000010003000000FFFFFFFF046E616D650361676505707269636504000000090000001200000001024564022C000000090000000000404140FF";
        private readonly byte[] _sessionData = Enumerable.Range(0, SerializedSessionData.Length).
                                                          Where(x => x%2 == 0).
                                                          Select(x => Convert.ToByte(SerializedSessionData.Substring(x, 2), 16)).
                                                          ToArray();

        [Test]
        public void Should_Decode_Session_Data()
        {
            var encoding = new SessionStateEncoding();
            var sessionState = encoding.Deserialize(_sessionData);
            sessionState.Items["name"].ShouldEqual("Ed");
            sessionState.Items["age"].ShouldEqual(44);
            sessionState.Items["price"].ShouldEqual(34.5);
            sessionState.Timeout.ShouldEqual(20);
        }

        [Test]
        public void Should_Encode_Session_Data()
        {
            var encoding = new SessionStateEncoding();
            var sessionState = new SessionStateStoreData(new SessionStateItemCollection(), new HttpStaticObjectsCollection(), 20);
            sessionState.Items["name"] = "Ed";
            sessionState.Items["age"] = 44;
            sessionState.Items["price"] = 34.5;
            BitConverter.ToString(encoding.Serialize(sessionState)).Replace("-", string.Empty).ShouldEqual(SerializedSessionData);
        }
    }
}
