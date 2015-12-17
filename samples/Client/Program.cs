using System;
using Autofac;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using CV.Common.Serialization;
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
            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterType<AppSettings>()
                .As<ISettings>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<ProxyInterceptor>()
                .As<IInterceptor>()
                .InstancePerLifetimeScope();

            containerBuilder
                .Register<Func<string, Method, IRestRequest>>(ctx =>
                {
                    return (resourceUrl, method) => new RestRequest(resourceUrl, method);
                });

            containerBuilder
                .Register<Func<IRestProxy, IPortOrderService>>(ctx =>
                {
                    return p =>
                    {
                        var gen = new ProxyGenerator();
                        var interceptor = ctx
                            .Resolve<IInterceptor>(
                                new TypedParameter(typeof (IRestProxy), p));
                        return gen
                            .CreateInterfaceProxyWithoutTarget<IPortOrderService>(interceptor);
                    };
                });

            containerBuilder
                .RegisterType<RestClient>()
                .As<IRestClient>()
                .InstancePerLifetimeScope();

            containerBuilder
                .Register(ctx => new JsonSerializer(Options.ISO8601IncludeInherited))
                .As<ISerializer>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<RestProxy>()
                .As<IRestProxy>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<ProxyFactory<IPortOrderService>>()
                .As<IProxyFactory<IPortOrderService>>()
                .InstancePerLifetimeScope();

            containerBuilder
                .Register(ctx => ctx
                    .Resolve<IProxyFactory<IPortOrderService>>()
                    .CreateProxy())
                .As<IPortOrderService>()
                .InstancePerLifetimeScope();

            var container = containerBuilder.Build();

            // Testing
            var serviceProxy = container.Resolve<IPortOrderService>();

            var getAllResponse = serviceProxy.GetAll(new GetAllRequest());

            Console.WriteLine("GetAllRequest");
            Console.WriteLine(JSON.Serialize(getAllResponse));

            var getByIdResponse = serviceProxy.GetById(new GetByIdRequest
            {
                Id = "23"
            });

            Console.WriteLine("GetByIdRequest");
            Console.WriteLine(JSON.Serialize(getByIdResponse));

            var createNewPortResponse = serviceProxy.CreateNewPortOrder(new CreateNewPortOrderRequest
            {
                Msisdn = "0035198789654"
            });

            Console.WriteLine("CreateNewPortOrder");
            Console.WriteLine(JSON.Serialize(createNewPortResponse));

            serviceProxy.SchedulePortOrder(new SchedulePortOrderRequest
            {
                Id = "23",
                ToDate = DateTime.Today.AddDays(2),
                DonorNetworkOperator = "Vodafone",
                RecipientNetworkOperator = "Meo"
            });

            Console.WriteLine("SchedulePortOrder");
            Console.WriteLine("No response");


            Console.ReadKey();
        }
    }
}
