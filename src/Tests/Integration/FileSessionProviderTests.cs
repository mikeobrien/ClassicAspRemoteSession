using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using RemoteSession;
using Should;

namespace Tests.Integration
{
    [TestFixture]
    public class FileSessionProviderTests
    {
        private readonly ISessionContext _context;
        private readonly IDictionary<string, object> _values = new Dictionary<string, object> { { "Name", "Neils" }, { "Age", 55 } };

        public FileSessionProviderTests()
        {
            _context = Substitute.For<ISessionContext>();
            _context.MetabasePath.Returns("/lm/w3svc/1/root");
            _context.SessionId.Returns("1sc3aoog5u5zyko2tl3ghnvq");
        }

        [Test]
        public void Should_Save_And_Load_State()
        {
            var provider = new FileSessionProvider();

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
            var provider = new FileSessionProvider();

            var values = provider.Load(_context);

            values.ShouldBeNull();
            File.Exists(provider.GetSessionFilePath(_context)).ShouldBeFalse();
        }

        [Test]
        public void Should_Abandon_State()
        {
            var provider = new FileSessionProvider();

            provider.Save(_context, _values);
            provider.Abandon(_context);

            File.Exists(provider.GetSessionFilePath(_context)).ShouldBeFalse();
        }
    }
}