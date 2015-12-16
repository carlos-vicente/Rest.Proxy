using Autofac;
using Nancy.Bootstrappers.Autofac;
using Server.Contracts;
using Server.Services;

namespace Server
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        protected override ILifetimeScope GetApplicationContainer()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterType<PortOrderService>()
                .As<IPortOrderService>()
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}