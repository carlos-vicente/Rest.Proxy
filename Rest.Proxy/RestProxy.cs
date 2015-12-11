using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;
using Rest.Proxy.Attributes;
using RestSharp;

namespace Rest.Proxy
{
    public class RestProxy : IRestProxy
    {
        private const string InvalidOperationFormat = "The method {0} has invalid usage of MethodRouteAttribute";

        public void Intercept(IInvocation invocation)
        {
            var attributes = invocation
                .Method
                .GetCustomAttributes(typeof (MethodRouteAttribute))
                .ToList();

            if (!attributes.Any() || attributes.Count > 1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        InvalidOperationFormat,
                        invocation.Method.Name));
            }

            var methodRoute = attributes.Single() as MethodRouteAttribute;

            object response = null;

            switch (methodRoute.Method)
            {
                case Method.GET:
                    response = Get(invocation
                        .Arguments
                        .SingleOrDefault());
                    break;

                case Method.POST:
                    response = Post(invocation
                        .Arguments
                        .SingleOrDefault());
                    break;

                case Method.PUT:
                    response = Put(invocation
                        .Arguments
                        .SingleOrDefault());
                    break;

                case Method.DELETE:
                    response = Delete(invocation
                        .Arguments
                        .SingleOrDefault());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            invocation.ReturnValue = response;
        }

        public object Get(object request)
        {
            throw new NotImplementedException();
        }

        public object Post(object request)
        {
            throw new NotImplementedException();
        }

        public object Put(object request)
        {
            throw new NotImplementedException();
        }

        public object Delete(object request)
        {
            throw new NotImplementedException();
        }
    }
}
