using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Rest.Proxy.UnitTests
{
    [TestFixture]
    public class ProxyFactoryTests : BaseAutoFakerTestFixture
    {
        public interface ITest
        {
            void TestOperation();
        }

        [Test]
        public void CreateProxy_ThrowsInvalidOperationException_WhenTypeIsNotInterface()
        {
            // arrange
            var restProxy = Fake.Resolve<IRestProxy>();
            Action exceptionThrower = () => new ProxyFactory<object>(
                restProxy,
                proxy => new object());

            // act & assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void CreateProxy_ReturnsProxyObjectForInterface()
        {
            // arrange
            var fakeFunc = A.Fake<Func<IRestProxy, ITest>>();
            var expectedProxy = Fake.Resolve<ITest>();

            A.CallTo(
                () => fakeFunc(Fake.Resolve<IRestProxy>()))
                .Returns(expectedProxy);

            Fake.Provide(fakeFunc);

            var sut = Fake.Resolve<ProxyFactory<ITest>>();

            // act
            var actual = sut.CreateProxy();

            // assert
            actual
                .Should()
                .Be(expectedProxy);
        }
    }
}
