using Autofac;
using Nancy.Bootstrappers.Autofac;
using Server.Contracts;
using Server.Nancy.Services;

namespace Server.Nancy
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