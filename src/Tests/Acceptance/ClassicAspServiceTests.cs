using NUnit.Framework;
using Should;

namespace Tests.Acceptance
{
    [TestFixture]
    public class ClassicAspServiceTests
    {
        [Test]
        public void Should_Enumerate_Session_Variables()
        {
            var result = Common.GetClassic();
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public void Should_Abandon_Session()
        {
            var result = Common.GetClassic("command=abandon");
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public void Should_Add_Session_Item()
        {
            var result = Common.GetClassic("command=add&key=age&value=5&datatype=int32");
            result.Data.ShouldNotBeEmpty();
            result.Data[0].Key.ShouldEqual("age");
            result.Data[0].Value.ShouldEqual("5");
            result.Data[0].DataType.ShouldEqual("Int32");
        }

        [Test]
        public void Should_Edit_Session_Item()
        {
            var result = Common.GetClassic("command=edit&key=age&value=5.5&datatype=double");
            result.Data.ShouldNotBeEmpty();
            result.Data[0].Key.ShouldEqual("age");
            result.Data[0].Value.ShouldEqual("5.5");
            result.Data[0].DataType.ShouldEqual("Double");
        }

        [Test]
        public void Should_Remove_Session_Item()
        {
            var result = Common.GetClassic("command=remove&key=age");
            result.Data.ShouldBeEmpty();
        }
    }
}
