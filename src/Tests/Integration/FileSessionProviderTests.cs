using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using RemoteSessionState;
using Should;
using Tests.Common;

namespace Tests.Integration
{
    [TestFixture]
    public class FileSessionProviderTests
    {
        private readonly ISessionStateContext _context;
        private readonly IDictionary<string, object> _values = new Dictionary<string, object> { { "Name", "Neils" }, { "Age", 55 } };

        public FileSessionProviderTests()
        {
            _context = Substitute.For<ISessionStateContext>();
            _context.MetabasePath.Returns(Constants.MetabasePath);
            _context.SessionId.Returns(Constants.SessionId);
        }

        [Test]
        public void Should_Save_And_Load_State()
        {
            var provider = new FileSessionStateProvider();

            provider.Save(_context, _values);

            File.Exists(provider.GetSessionFilePath(_context)).ShouldBeTrue();

            var values = provider.Load(_context);

            values["Name"].ShouldEqual("Neils");
            values["Age"].ShouldEqual(55);

            File.Delete(provider.GetSessionFilePath(_context));
        }

        [Test]
        public void Should_Not_Load_State_If_It_Does_Not_Exist()
        {
            var provider = new FileSessionStateProvider();

            var values = provider.Load(_context);

            values.ShouldBeNull();
            File.Exists(provider.GetSessionFilePath(_context)).ShouldBeFalse();
        }

        [Test]
        public void Should_Abandon_State()
        {
            var provider = new FileSessionStateProvider();

            provider.Save(_context, _values);
            provider.Abandon(_context);

            File.Exists(provider.GetSessionFilePath(_context)).ShouldBeFalse();
        }
    }
}