using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        [Test]
        public void Should_Fail_When_Net_Type_Exists_In_Net_Session()
        {
            var session = Common.GetNet("command=addnettype").
                                 GetNet("command=add&key=zip&value=80012&datatype=Int32").
                                 GetClassic();
            session.Error.ShouldBeTrue();
            session.ErrorMessage.ShouldContain("Error loading session");
            session.ErrorMessage.ShouldContain("Session item '__nettype__' type cannot not be marshaled. Only primitive types (Boolean, Byte, System.DateTime/Date, Double, System.Int16/Integer, System.Int32/Long, System.Float/Single, String) can be marshaled between Classic ASP and ASP.NET.");
        }

        [Test]
        public void Should_Return_Session_State_For_All_Classic_Concurent_Requests()
        {
            var counts = new ConcurrentBag<int>();
            var session = Common.GetClassic("command=add&key=state&value=CO&datatype=String");
            for (var i = 0; i < 20; i++) ThreadPool.QueueUserWorkItem(x => counts.Add(session.GetClassic().Data.Count));
            while (counts.Count < 20) Thread.Sleep(100);
            counts.All(x => x == 1).ShouldBeTrue();
        }

        [Test]
        public void Should_Return_Session_State_For_All_Net_Concurent_Requests()
        {
            var counts = new List<int>();
            var session = Common.GetNet("command=add&key=state&value=CO&datatype=String");
            for (var i = 0; i < 20; i++) ThreadPool.QueueUserWorkItem(x => counts.Add(session.GetNet().Data.Count));
            while (counts.Count < 20) Thread.Sleep(100);
            counts.All(x => x == 1).ShouldBeTrue();
        }
    }
}
