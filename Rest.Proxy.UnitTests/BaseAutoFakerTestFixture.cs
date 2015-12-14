using Autofac.Extras.FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Rest.Proxy.UnitTests
{
    [TestFixture]
    public abstract class BaseAutoFakerTestFixture
    {
        protected AutoFake Fake;
        protected Fixture Fixture;

        [SetUp]
        public void Setup()
        {
            Fake = new AutoFake();
            Fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            Fake.Dispose();
        }
    }
}
