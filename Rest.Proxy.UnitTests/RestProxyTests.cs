using System.Net;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;

namespace Rest.Proxy.UnitTests
{
    public class RestProxyTests : BaseAutoFakerTestFixture
    {
        public class Request
        {
            public string Id { get; set; }
        }

        [Test]
        public void Proxy_SendsGet_WithCorrectUrl()
        {
            // arrange
            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var request = Fixture.Create<Request>();

            var baseUrl = "http://localhost";
            var resourceUrl = "/resource/{Id}";
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => response.ResponseStatus)
                .Returns(ResponseStatus.Completed);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET
                    && rr.Resource == expectedUrl)))
                .Returns(response);

            var sut = Fake.Resolve<RestProxy>();

            // act
            sut.Get(baseUrl, resourceUrl, request);

            // assert
            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET
                    && rr.Resource == expectedUrl)))
                .MustHaveHappened(Repeated.Exactly.Once);

            restClient
                .BaseUrl
                .Should()
                .Be(baseUrl);
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
