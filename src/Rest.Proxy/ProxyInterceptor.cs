using System;
using System.Linq;
using Castle.Core.Interceptor;
using Rest.Proxy.Attributes;
using System.Reflection;
using Rest.Proxy.Settings;

namespace Rest.Proxy
{
    public class ProxyInterceptor : IInterceptor
    {
        private const string InvalidServiceFormat = "The interface {0} has invalid usage of ServiceRouteAttribute";
        private const string InvalidOperationFormat = "The method {0} has invalid usage of MethodRouteAttribute";

        private readonly IRestProxy _restProxy;
        private readonly ISettings _settings;

        public ProxyInterceptor(
            IRestProxy restProxy,
            ISettings settings)
        {
            _restProxy = restProxy;
            _settings = settings;
        }

        public void Intercept(IInvocation invocation)
        {
            var typeAttibutes = invocation
                .Method
                .DeclaringType
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


            var baseUrl = _settings.GetBaseUrl(serviceRoute.SettingBaseUrlName);
            var resourceUrl = methodRoute.Template;

            switch (methodRoute.Method)
            {
                case HttpMethod.Get:
                    invocation.ReturnValue = _restProxy
                        .Get(baseUrl, resourceUrl, request, invocation.Method.ReturnType);
                    break;

                case HttpMethod.Post:
                    invocation.ReturnValue = _restProxy
                        .Post(baseUrl, resourceUrl, request, invocation.Method.ReturnType);
                    break;

                case HttpMethod.Put:
                    invocation.ReturnValue = _restProxy
                        .Put(baseUrl, resourceUrl, request, invocation.Method.ReturnType);
                    break;

                case HttpMethod.Delete:
                    _restProxy
                        .Delete(baseUrl, resourceUrl, request);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
