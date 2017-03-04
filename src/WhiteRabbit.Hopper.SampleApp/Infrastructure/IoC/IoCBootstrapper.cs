using System;
using System.Collections.Generic;
using Autofac;
using Ploeh.AutoFixture;
using WhiteRabbit.Autofac;
using WhiteRabbit.Hopper.SampleApp.Definitions;
using WhiteRabbit.Hopper.SampleApp.Services;

namespace WhiteRabbit.Hopper.SampleApp.Infrastructure.IoC
{
    public static class IoCBootstrapper
    {
        public static IContainer Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new WhiteRabbitIoCModule { EnableContextPerMessage = true });

            RegisterSerializers(builder);
            RegisterLogAction(builder);
            RegisterHopper(builder);

            builder
                .RegisterType<SampleService>()
                .AsSelf();

            return builder.Build();
        }

        private static void RegisterSerializers(ContainerBuilder builder)
        {
            builder
                .RegisterSerializersFrom(new[] { "WhiteRabbit.Json", "WhiteRabbit.Protobuf" });
        }

        private static void RegisterLogAction(ContainerBuilder builder)
        {
            Action<string> logAction = Console.WriteLine;

            builder
                .Register(c => logAction)
                .AsSelf();
        }

        public static void RegisterHopper(ContainerBuilder builder)
        {
            var typeFactory = new Dictionary<Type, Func<object>>();

            var fixture = new Fixture();

            typeFactory.Add(typeof(ExampleTypeA), () => fixture.Create<ExampleTypeA>());
            typeFactory.Add(typeof(ExampleTypeB), () => fixture.Create<ExampleTypeB>());

            builder
                .RegisterType<Hopper>()
                .WithParameter("exchange", ExchangeConfig.Default)
                .WithParameter("typeFactory", typeFactory)
                .WithParameter("delayBetweenRuns", TimeSpan.FromSeconds(10))
                .WithParameter("messageTimeout", TimeSpan.FromSeconds(5))
                .AsSelf();
        }
    }
}
