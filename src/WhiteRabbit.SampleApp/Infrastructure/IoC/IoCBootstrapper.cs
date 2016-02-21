using Autofac;
using Autofac.Core;
using WhiteRabbit.Autofac;
using WhiteRabbit.SampleApp.Services;

namespace WhiteRabbit.SampleApp.Infrastructure.IoC
{
    public static class IoCBootstrapper
    {
        public static IContainer Init()
        {
            var builder = new ContainerBuilder();

            RegisterModules(builder);

            builder
                .RegisterType<SampleService>()
                .WithParameter(ResolvedParameter.ForNamed<IPublisher>("commandsPublisher"))
                .AsSelf();

            return builder.Build();
        }

        public static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule(new WhiteRabbitIoCModule { EnableContextPerMessage = true });
            builder.RegisterModule<ApplicationIoCModule>();
            builder.RegisterModule<InfrastructureIoCModule>();
            builder.RegisterModule<LoggingIoCModule>();
        }
    }
}
