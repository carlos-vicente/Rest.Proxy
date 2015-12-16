using System;
using Castle.DynamicProxy;
using CV.Common.Serialization.Json;
using Jil;
using Rest.Proxy;
using Rest.Proxy.Settings;
using RestSharp;
using Server.Contracts;
using Server.Contracts.Requests;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Func<string, Method, IRestRequest> restRequestBuilderFunc =
                (resourceUrl, method) =>
                    new RestRequest(resourceUrl, method);

            Func<IRestProxy, IPortOrderService> portOrderServiceProxy = p =>
            {
                var gen = new ProxyGenerator();
                var interceptor = new ProxyInterceptor(p, new AppSettings());
                return gen.CreateInterfaceProxyWithoutTarget<IPortOrderService>(interceptor);
            };

            IRestProxy proxy = new RestProxy(
                new RestClient(),
                new JsonSerializer(Options.ISO8601IncludeInherited),
                restRequestBuilderFunc);

            IProxyFactory<IPortOrderService> proxyFactory = new ProxyFactory<IPortOrderService>(
                proxy,
                portOrderServiceProxy);

            var serviceProxy = proxyFactory.CreateProxy();

            var getAllResponse = serviceProxy.GetAll(new GetAllRequest());

            Console.WriteLine("GetAllRequest");
            Console.WriteLine(JSON.Serialize(getAllResponse));

            var getByIdResponse = serviceProxy.GetById(new GetByIdRequest
            {
                Id = "23"
            });

            Console.WriteLine("GetByIdRequest");
            Console.WriteLine(JSON.Serialize(getByIdResponse));

            serviceProxy.CreateNewPortOrder(new CreateNewPortOrderRequest
            {
                Msisdn = "0035198789654"
            });
        }
    }
}
