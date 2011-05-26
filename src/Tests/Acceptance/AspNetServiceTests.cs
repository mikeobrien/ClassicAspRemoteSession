using NUnit.Framework;
using Should;

namespace Tests.Acceptance
{
    [TestFixture]
    public class AspNetServiceTests
    {
        [Test]
        public void Should_Enumerate_Session_Variables()
        {
            var result = Common.GetNet();
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public void Should_Abandon_Session()
        {
            var result = Common.GetNet("command=abandon");
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public void Should_Add_Session_Item()
        {
            var result = Common.GetNet("command=add&key=age&value=5&datatype=Int32");
            result.Data.ShouldNotBeEmpty();
            result.Data[0].Key.ShouldEqual("age");
            result.Data[0].Value.ShouldEqual("5");
            result.Data[0].DataType.ShouldEqual("Int32");
        }

        [Test]
        public void Should_Edit_Session_Item()
        {
            var result = Common.GetNet("command=edit&key=age&value=5.5&datatype=Double");
            result.Data.ShouldNotBeEmpty();
            result.Data[0].Key.ShouldEqual("age");
            result.Data[0].Value.ShouldEqual("5.5");
            result.Data[0].DataType.ShouldEqual("Double");
        }

        [Test]
        public void Should_Remove_Session_Item()
        {
            var result = Common.GetNet("command=remove&key=age");
            result.Data.ShouldBeEmpty();
        }
    }
}
