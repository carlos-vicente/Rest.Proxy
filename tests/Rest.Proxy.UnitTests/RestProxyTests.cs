using System;
using System.Net;
using CV.Common.Serialization;
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

        public class Response
        {
            public string Id { get; set; }

            public string Result { get; set; }
        }

        [Test]
        public void Get_SendsHttpGetRequest_WithCorrectUrl()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var request = Fixture.Create<Request>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET
                    && rr.Resource == expectedUrl)))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            var sut = Fake.Resolve<RestProxy>();

            // act
            sut.Get(baseUrl, resourceUrl, request, typeof(Response));

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
        public void Get_SendsHttpGetRequest_WillReturnResponse()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var serializer = Fake.Resolve<ISerializer>();
            var request = Fixture.Create<Request>();
            var expectedResponse = Fixture.Create<Response>();
            var expectedResponseSerialized = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET
                    && rr.Resource == expectedUrl)))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            A.CallTo(() => response.Content)
                .Returns(expectedResponseSerialized);

            A.CallTo(() => serializer
                .Deserialize(typeof(Response), expectedResponseSerialized))
                .Returns(expectedResponse);

            var sut = Fake.Resolve<RestProxy>();

            // act
            var actualResponse = sut.Get(baseUrl, resourceUrl, request, typeof(Response));

            // assert
            actualResponse
                .ShouldBeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Get_ThrowsException_WhenBaseUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                null,
                Fixture.Create<string>(),
                Fixture.Create<object>(),
                Fixture.Create<Type>());

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("baseUrl");
        }

        [Test]
        public void Get_ThrowsException_WhenResourceUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                Fixture.Create<string>(),
                null,
                Fixture.Create<object>(),
                Fixture.Create<Type>());

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("resourceUrl");
        }

        [Test]
        public void Get_ThrowsException_WhenRequestIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                null,
                Fixture.Create<Type>());

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("request");
        }

        [Test]
        public void Get_ThrowsException_WhenResponseTypeIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<object>(),
                null);

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("responseType");
        }

        [Test]
        public void Get_ThrowsException_WhenServerReturnsErrorHttpCode()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();

            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET)))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.BadRequest);

            A.CallTo(() => response.StatusDescription)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                baseUrl,
                resourceUrl,
                request,
                typeof(Response));

            // act/assert
            throwable
                .ShouldThrow<HttpException>()
                .And
                .Message
                .Should()
                .Contain(description);
        }

        [Test]
        public void Get_ThrowsException_WhenHttpRequestFailsOverTCPErrors()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();

            var exception = new Exception("BOOM!!");

            A.CallTo(() => restClient
                .Execute(A<IRestRequest>.That.Matches(rr =>
                    rr.Method == Method.GET)))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(exception);

            A.CallTo(() => response.ErrorMessage)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Get(
                baseUrl,
                resourceUrl,
                request,
                typeof(Response));

            // act/assert
            var exceptionThrown = throwable
                .ShouldThrow<HttpException>();

            exceptionThrown
                .And
                .Message
                .Should()
                .Contain(description);

            exceptionThrown
                .And
                .InnerException
                .Should()
                .Be(exception);
        }
    }
}
