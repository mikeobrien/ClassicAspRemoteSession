using System.Reflection;
using NUnit.Framework;
using RemoteSessionState.Interop;
using Should;
using Tests.Common;

namespace Tests.Acceptance
{
    [TestFixture]
    public class BridgeTests
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
        public void Installed_Com_Component_Should_Be_Latest_Build()
        {
            var version = Common.GetRegisteredComVersion();
            version.ShouldEqual(Assembly.GetAssembly(typeof (IRemoteSessionState)).GetName().Version.ToString());
        }

        [Test]
        public void Classic_Asp_Should_Should_Sync_Net_Session()
        {
            var session = Common.GetClassic("command=add&key=state&value=CO&datatype=String").
                                 GetNet("command=add&key=name&value=Bohr&datatype=String").
                                 GetClassic("command=add&key=zip&value=80012&datatype=Int32").
                                 GetNet("command=add&key=age&value=25&datatype=Int16").
                                 GetClassic("command=add&key=city&value=Grand Junction&datatype=String").
                                 GetNet("command=add&key=price&value=6.78&datatype=Double").
                                 GetClassic();

            session.Data.Count.ShouldEqual(6);

            session.Data.Exists(x => x.Key == "state");
            session.Data.Find(x => x.Key == "state").Value.ShouldEqual("CO");
            session.Data.Find(x => x.Key == "state").DataType.ShouldEqual("String");

            session.Data.Exists(x => x.Key == "name");
            session.Data.Find(x => x.Key == "name").Value.ShouldEqual("Bohr");
            session.Data.Find(x => x.Key == "name").DataType.ShouldEqual("String");

            session.Data.Exists(x => x.Key == "zip");
            session.Data.Find(x => x.Key == "zip").Value.ShouldEqual("80012");
            session.Data.Find(x => x.Key == "zip").DataType.ShouldEqual("Int32");

            session.Data.Exists(x => x.Key == "age");
            session.Data.Find(x => x.Key == "age").Value.ShouldEqual("25");
            session.Data.Find(x => x.Key == "age").DataType.ShouldEqual("Int16");

            session.Data.Exists(x => x.Key == "price");
            session.Data.Find(x => x.Key == "price").Value.ShouldEqual("6.78");
            session.Data.Find(x => x.Key == "price").DataType.ShouldEqual("Double");

            session.Data.Exists(x => x.Key == "state");
            session.Data.Find(x => x.Key == "state").Value.ShouldEqual("CO");
            session.Data.Find(x => x.Key == "state").DataType.ShouldEqual("String");
        }

        [Test]
        public void Net_Should_Sync_With_Classic_Asp_Session()
        {
            var session = Common.GetNet("command=add&key=state&value=CO&datatype=String").
                                 GetClassic("command=add&key=name&value=Bohr&datatype=String").
                                 GetNet("command=add&key=zip&value=80012&datatype=Int32").
                                 GetClassic("command=add&key=age&value=25&datatype=Int16").
                                 GetNet("command=add&key=city&value=Grand Junction&datatype=String").
                                 GetClassic("command=add&key=price&value=6.78&datatype=Double").
                                 GetNet();

            session.Data.Count.ShouldEqual(6);

            session.Data.Exists(x => x.Key == "state");
            session.Data.Find(x => x.Key == "state").Value.ShouldEqual("CO");
            session.Data.Find(x => x.Key == "state").DataType.ShouldEqual("String");

            session.Data.Exists(x => x.Key == "name");
            session.Data.Find(x => x.Key == "name").Value.ShouldEqual("Bohr");
            session.Data.Find(x => x.Key == "name").DataType.ShouldEqual("String");

            session.Data.Exists(x => x.Key == "zip");
            session.Data.Find(x => x.Key == "zip").Value.ShouldEqual("80012");
            session.Data.Find(x => x.Key == "zip").DataType.ShouldEqual("Int32");

            session.Data.Exists(x => x.Key == "age");
            session.Data.Find(x => x.Key == "age").Value.ShouldEqual("25");
            session.Data.Find(x => x.Key == "age").DataType.ShouldEqual("Int16");

            session.Data.Exists(x => x.Key == "price");
            session.Data.Find(x => x.Key == "price").Value.ShouldEqual("6.78");
            session.Data.Find(x => x.Key == "price").DataType.ShouldEqual("Double");

            session.Data.Exists(x => x.Key == "state");
            session.Data.Find(x => x.Key == "state").Value.ShouldEqual("CO");
            session.Data.Find(x => x.Key == "state").DataType.ShouldEqual("String");
        }

        [Test]
        public void Classic_Asp_Should_Recognize_Abandoned_Net_Session()
        {
            var session = Common.GetNet("command=add&key=state&value=CO&datatype=String").
                                 GetClassic("command=add&key=zip&value=80012&datatype=Int32").
                                 GetNet("command=abandon").
                                 GetClassic();

            session.Data.ShouldBeEmpty();
        }

        [Test]
        public void Net_Should_Recognize_Abandoned_Classic_Asp_Session()
        {
            var session = Common.GetClassic("command=add&key=state&value=CO&datatype=String").
                                 GetNet("command=add&key=zip&value=80012&datatype=Int32").
                                 GetClassic("command=abandon").
                                 GetNet();

            session.Data.ShouldBeEmpty();
        }
    }
}
