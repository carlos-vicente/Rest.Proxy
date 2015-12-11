using Autofac.Extras.FakeItEasy;
using NUnit.Framework;

namespace Rest.Proxy.UnitTests
{
    [TestFixture]
    public abstract class BaseAutoFakerTestFixture
    {
        protected AutoFake Fake;

        [SetUp]
        public void Setup()
        {
            Fake = new AutoFake();
        }

        [TearDown]
        public void TearDown()
        {
            Fake.Dispose();
        }
    }
}
