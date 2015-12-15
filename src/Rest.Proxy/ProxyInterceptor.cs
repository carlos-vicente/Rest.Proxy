using System;
using System.Linq;
using Castle.Core.Interceptor;
using Rest.Proxy.Attributes;
using RestSharp;
using System.Reflection;

namespace Rest.Proxy
{
    public class ProxyInterceptor : IInterceptor
    {
        private const string InvalidServiceFormat = "The interface {0} has invalid usage of ServiceRouteAttribute";
        private const string InvalidOperationFormat = "The method {0} has invalid usage of MethodRouteAttribute";

        private readonly IRestProxy _restProxy;

        public ProxyInterceptor(IRestProxy restProxy)
        {
            _restProxy = restProxy;
        }

        public void Intercept(IInvocation invocation)
        {
            var typeAttibutes = invocation
                .TargetType
                .GetCustomAttributes(typeof (ServiceRouteAttribute))
                .ToList();

            if (!typeAttibutes.Any() || typeAttibutes.Count > 1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        InvalidServiceFormat,
                        invocation.TargetType.Name));
            }

            var serviceRoute = typeAttibutes
                .Single() as ServiceRouteAttribute;

            var methodAttributes = invocation
                .Method
                .GetCustomAttributes(typeof (MethodRouteAttribute))
                .ToList();

            if (!methodAttributes.Any() || methodAttributes.Count > 1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        InvalidOperationFormat,
                        invocation.Method.Name));
            }

            var methodRoute = methodAttributes
                .Single() as MethodRouteAttribute;

            var request = invocation
                .Arguments
                .SingleOrDefault();

            var baseUrl = serviceRoute.BaseUrl;
            var resourceUrl = methodRoute.Template;

            switch (methodRoute.Method)
            {
                case Method.GET:
                    invocation.ReturnValue = _restProxy
                        .Get(baseUrl, resourceUrl, request, invocation.Method.ReturnType);
                    break;

                case Method.POST:
                    _restProxy
                        .Post(baseUrl, resourceUrl, request);
                    break;

                case Method.PUT:
                    _restProxy
                        .Put(baseUrl, resourceUrl, request);
                    break;

                case Method.DELETE:
                    _restProxy
                        .Delete(baseUrl, resourceUrl, request);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
