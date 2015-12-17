using Castle.Core.Interceptor;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rest.Proxy.Settings;
using Rest.Proxy.UnitTests.Support;

namespace Rest.Proxy.UnitTests
{
    public class ProxyInterceptorTests : BaseAutoFakerTestFixture
    {
        [Test]
        public void Intercept_InvokesGetWithCorrectValues_WhenMethodIsDefinedAsGet()
        {
            // arrange
            const string baseUrlSettingName = "SomeUrlSetting";     // As defined on ITest interface
            const string resourceUrl = "/resource/{Id}";            // As defined on ITest interface
            var baseUrl = Fixture.Create<string>();
            var invocation = Fake.Resolve<IInvocation>();
            var request = Fixture.Create<object>();

            var methodInfo = typeof (ITest)
                .GetMethod("TestGet");

            A.CallTo(() => invocation
                .Method)
                .Returns(methodInfo);

            A.CallTo(() => invocation
                .Arguments)
                .Returns(new[] { request });

            A.CallTo(() => Fake.Resolve<ISettings>()
                .GetBaseUrl(baseUrlSettingName))
                .Returns(baseUrl);

            var sut = Fake.Resolve<ProxyInterceptor>();

            // act
            sut.Intercept(invocation);

            // assert
            A.CallTo(() => Fake.Resolve<IRestProxy>()
                .Get(baseUrl, resourceUrl, request, methodInfo.ReturnType))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Intercept_SetsReturnValueWithGetResponse_WhenMethodIsDefinedAsGet()
        {
            // arrange
            const string baseUrlSettingName = "SomeUrlSetting";     // As defined on ITest interface
            const string resourceUrl = "/resource/{Id}";            // As defined on ITest interface
            var baseUrl = Fixture.Create<string>();
            var invocation = Fake.Resolve<IInvocation>();
            var request = Fixture.Create<object>();
            var response = Fixture.Create<object>();

            var methodInfo = typeof(ITest)
                .GetMethod("TestGet");

            A.CallTo(() => invocation
                .Method)
                .Returns(methodInfo);

            A.CallTo(() => invocation
                .Arguments)
                .Returns(new[] { request });

            A.CallTo(() => Fake.Resolve<ISettings>()
                .GetBaseUrl(baseUrlSettingName))
                .Returns(baseUrl);

            A.CallTo(() => Fake.Resolve<IRestProxy>()
                .Get(baseUrl, resourceUrl, request, methodInfo.ReturnType))
                .Returns(response);

            var sut = Fake.Resolve<ProxyInterceptor>();

            // act
            sut.Intercept(invocation);

            // assert
            invocation
                .ReturnValue
                .Should()
                .Be(response);
        }

        [Test]
        public void Intercept_InvokesPostWithCorrectValues_WhenMethodIsDefinedAsPost()
        {
            // arrange
            const string baseUrlSettingName = "SomeUrlSetting";     // As defined on ITest interface
            const string resourceUrl = "/resources";                // As defined on ITest interface
            var baseUrl = Fixture.Create<string>();
            var invocation = Fake.Resolve<IInvocation>();
            var request = Fixture.Create<object>();

            var methodInfo = typeof(ITest)
                .GetMethod("TestPost");

            A.CallTo(() => invocation
                .Method)
                .Returns(methodInfo);

            A.CallTo(() => invocation
                .Arguments)
                .Returns(new[] { request });

            A.CallTo(() => Fake.Resolve<ISettings>()
                .GetBaseUrl(baseUrlSettingName))
                .Returns(baseUrl);

            var sut = Fake.Resolve<ProxyInterceptor>();

            // act
            sut.Intercept(invocation);

            // assert
            A.CallTo(() => Fake.Resolve<IRestProxy>()
                .Post(baseUrl, resourceUrl, request, methodInfo.ReturnType))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Intercept_SetsReturnValueWithPostResponse_WhenMethodIsDefinedAsPost()
        {
            // arrange
            const string baseUrlSettingName = "SomeUrlSetting";     // As defined on ITest interface
            const string resourceUrl = "/resources";                // As defined on ITest interface
            var baseUrl = Fixture.Create<string>();
            var invocation = Fake.Resolve<IInvocation>();
            var request = Fixture.Create<object>();
            var response = Fixture.Create<object>();

            var methodInfo = typeof(ITest)
                .GetMethod("TestPost");

            A.CallTo(() => invocation
                .Method)
                .Returns(methodInfo);

            A.CallTo(() => invocation
                .Arguments)
                .Returns(new[] { request });

            A.CallTo(() => Fake.Resolve<ISettings>()
                .GetBaseUrl(baseUrlSettingName))
                .Returns(baseUrl);

            A.CallTo(() => Fake.Resolve<IRestProxy>()
                .Post(baseUrl, resourceUrl, request, methodInfo.ReturnType))
                .Returns(response);

            var sut = Fake.Resolve<ProxyInterceptor>();

            // act
            sut.Intercept(invocation);

            // assert
            invocation
                .ReturnValue
                .Should()
                .Be(response);
        }
    }
}
