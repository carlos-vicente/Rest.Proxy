using NUnit.Framework;

namespace Rest.Proxy.UnitTests
{
    public class RestProxyTests : BaseAutoFakerTestFixture
    {
        [Test]
        public void Proxy_SendsGet_WithCorrectUrl()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>()

            // act

            // assert
        }

        [Test]
        public void Proxy_SendsGet_WillReturnResponse()
        {
            // arrange

            // act

            // assert
        }
    }
}
