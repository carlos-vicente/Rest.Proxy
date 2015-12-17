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
        public void Get_UsesWithCorrectUrl_WhenSendsHttpGetRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.GET))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
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
                .Execute(restRequest))
                .MustHaveHappened(Repeated.Exactly.Once);

            restClient
                .BaseUrl
                .Should()
                .Be(baseUrl);
        }

        [Test]
        public void Get_ReturnsResponse_WhenSendsHttpGetRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var restResponse = Fake.Resolve<IRestResponse>();
            var serializer = Fake.Resolve<ISerializer>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedResponse = Fixture.Create<Response>();
            var expectedResponseSerialized = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.GET))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(null);

            A.CallTo(() => restResponse.StatusCode)
                .Returns(HttpStatusCode.OK);

            A.CallTo(() => restResponse.Content)
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
            var restResponse = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.GET))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(null);

            A.CallTo(() => restResponse.StatusCode)
                .Returns(HttpStatusCode.BadRequest);

            A.CallTo(() => restResponse.StatusDescription)
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
            var restResponse = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            var exception = new Exception("BOOM!!");

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.GET))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(exception);

            A.CallTo(() => restResponse.ErrorMessage)
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

        [Test]
        public void Post_UsesWithCorrectUrl_WhenSendsHttpPostRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.POST))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            var sut = Fake.Resolve<RestProxy>();

            // act
            sut.Post(baseUrl, resourceUrl, request, typeof(Response));

            // assert
            A.CallTo(() => restRequest
                .AddJsonBody(request))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .MustHaveHappened(Repeated.Exactly.Once);

            restClient
                .BaseUrl
                .Should()
                .Be(baseUrl);
        }

        [Test]
        public void Post_ReturnsResponse_WhenSendsHttpGetRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var restResponse = Fake.Resolve<IRestResponse>();
            var serializer = Fake.Resolve<ISerializer>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedResponse = Fixture.Create<Response>();
            var expectedResponseSerialized = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.POST))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(null);

            A.CallTo(() => restResponse.StatusCode)
                .Returns(HttpStatusCode.OK);

            A.CallTo(() => restResponse.Content)
                .Returns(expectedResponseSerialized);

            A.CallTo(() => serializer
                .Deserialize(typeof(Response), expectedResponseSerialized))
                .Returns(expectedResponse);

            var sut = Fake.Resolve<RestProxy>();

            // act
            var actualResponse = sut.Post(baseUrl, resourceUrl, request, typeof(Response));

            // assert
            actualResponse
                .ShouldBeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Post_ThrowsException_WhenBaseUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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
        public void Post_ThrowsException_WhenResourceUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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
        public void Post_ThrowsException_WhenRequestIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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
        public void Post_ThrowsException_WhenResponseTypeIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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
        public void Post_ThrowsException_WhenServerReturnsErrorHttpCode()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.POST))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.BadRequest);

            A.CallTo(() => response.StatusDescription)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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
        public void Post_ThrowsException_WhenHttpRequestFailsOverTCPErrors()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            var exception = new Exception("BOOM!!");

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.POST))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(exception);

            A.CallTo(() => response.ErrorMessage)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Post(
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

        [Test]
        public void Put_UsesWithCorrectUrl_WhenSendsHttpPostRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.PUT))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            var sut = Fake.Resolve<RestProxy>();

            // act
            sut.Put(baseUrl, resourceUrl, request, typeof(Response));

            // assert
            A.CallTo(() => restRequest
                .AddJsonBody(request))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .MustHaveHappened(Repeated.Exactly.Once);

            restClient
                .BaseUrl
                .Should()
                .Be(baseUrl);
        }

        [Test]
        public void Put_ReturnsResponse_WhenSendsHttpGetRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var restResponse = Fake.Resolve<IRestResponse>();
            var serializer = Fake.Resolve<ISerializer>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedResponse = Fixture.Create<Response>();
            var expectedResponseSerialized = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.PUT))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(null);

            A.CallTo(() => restResponse.StatusCode)
                .Returns(HttpStatusCode.OK);

            A.CallTo(() => restResponse.Content)
                .Returns(expectedResponseSerialized);

            A.CallTo(() => serializer
                .Deserialize(typeof(Response), expectedResponseSerialized))
                .Returns(expectedResponse);

            var sut = Fake.Resolve<RestProxy>();

            // act
            var actualResponse = sut.Put(baseUrl, resourceUrl, request, typeof(Response));

            // assert
            actualResponse
                .ShouldBeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Put_ThrowsException_WhenBaseUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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
        public void Put_ThrowsException_WhenResourceUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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
        public void Put_ThrowsException_WhenRequestIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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
        public void Put_ThrowsException_WhenResponseTypeIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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
        public void Put_ThrowsException_WhenServerReturnsErrorHttpCode()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.PUT))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.BadRequest);

            A.CallTo(() => response.StatusDescription)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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
        public void Put_ThrowsException_WhenHttpRequestFailsOverTCPErrors()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            var exception = new Exception("BOOM!!");

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.PUT))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(exception);

            A.CallTo(() => response.ErrorMessage)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Put(
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

        [Test]
        public void Delete_UsesWithCorrectUrl_WhenSendsHttpGetRequest()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var response = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.DELETE))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(response);

            A.CallTo(() => response.ErrorException)
                .Returns(null);

            A.CallTo(() => response.StatusCode)
                .Returns(HttpStatusCode.OK);

            var sut = Fake.Resolve<RestProxy>();

            // act
            sut.Delete(baseUrl, resourceUrl, request);

            // assert
            A.CallTo(() => restClient
                .Execute(restRequest))
                .MustHaveHappened(Repeated.Exactly.Once);

            restClient
                .BaseUrl
                .Should()
                .Be(baseUrl);
        }

        [Test]
        public void Delete_ThrowsException_WhenBaseUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Delete(
                null,
                Fixture.Create<string>(),
                Fixture.Create<object>());

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("baseUrl");
        }

        [Test]
        public void Delete_ThrowsException_WhenResourceUrlIsNullOrWhitespace()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Delete(
                Fixture.Create<string>(),
                null,
                Fixture.Create<object>());

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("resourceUrl");
        }

        [Test]
        public void Delete_ThrowsException_WhenRequestIsNull()
        {
            // arrange
            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Delete(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                null);

            // act/assert
            throwable
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("request");
        }

        [Test]
        public void Delete_ThrowsException_WhenServerReturnsErrorHttpCode()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var restResponse = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.DELETE))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(null);

            A.CallTo(() => restResponse.StatusCode)
                .Returns(HttpStatusCode.BadRequest);

            A.CallTo(() => restResponse.StatusDescription)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Delete(
                baseUrl,
                resourceUrl,
                request);

            // act/assert
            throwable
                .ShouldThrow<HttpException>()
                .And
                .Message
                .Should()
                .Contain(description);
        }

        [Test]
        public void Delete_ThrowsException_WhenHttpRequestFailsOverTCPErrors()
        {
            // arrange
            const string baseUrl = "http://localhost";
            const string resourceUrl = "/resource/{Id}";

            var restClient = Fake.Resolve<IRestClient>();
            var restResponse = Fake.Resolve<IRestResponse>();
            var fakeRequestFactoryFunc = A.Fake<Func<string, Method, IRestRequest>>();
            var restRequest = Fake.Resolve<IRestRequest>();
            var request = Fixture.Create<Request>();
            var description = Fixture.Create<string>();
            var expectedUrl = $"/resource/{request.Id}";

            var exception = new Exception("BOOM!!");

            A.CallTo(() => fakeRequestFactoryFunc(expectedUrl, Method.DELETE))
                .Returns(restRequest);

            A.CallTo(() => restClient
                .Execute(restRequest))
                .Returns(restResponse);

            A.CallTo(() => restResponse.ErrorException)
                .Returns(exception);

            A.CallTo(() => restResponse.ErrorMessage)
                .Returns(description);

            var sut = Fake.Resolve<RestProxy>();

            Action throwable = () => sut.Delete(
                baseUrl,
                resourceUrl,
                request);

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
