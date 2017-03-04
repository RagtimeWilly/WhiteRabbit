using Autofac;
using Autofac.Core;
using WhiteRabbit.Autofac;
using WhiteRabbit.SampleApp.Infrastructure.Messaging;

namespace WhiteRabbit.SampleApp.Infrastructure.IoC
{
    public class InfrastructureIoCModule : Module
    {
        private const string EventsExchangeName = "WhiteRabbit.SampleApp.Events";
        private const string CommandsQueueName = "WhiteRabbit.SampleApp.Commands";

        protected override void Load(ContainerBuilder builder)
        {
            RegisterTopologyConfiguration(builder);
            RegisterSerializer(builder);
            RegisterPublishers(builder);
            RegisterConsumers(builder);

            builder
                .RegisterType<CommandDispatcher>()
                .As<IDispatcher>();
        }

        private static void RegisterTopologyConfiguration(ContainerBuilder builder)
        {
            var evtsExchange = new ExchangeConfig(EventsExchangeName, "fanout");

            builder
                .Register(c => evtsExchange)
                .As<ExchangeConfig>();

            builder
                .Register(c => new QueueConfig(CommandsQueueName))
                .As<QueueConfig>();

            builder
                .Register(c => new QueueConfig("WhiteRabbit.SampleApp.Events.Testing", new RoutingConfig(string.Empty), evtsExchange))
                .As<QueueConfig>();
        }

        private static void RegisterPublishers(ContainerBuilder builder)
        {
            builder
                .RegisterType<WhiteRabbitPublisher>()
                .WithParameter("exchangeName", string.Empty)
                .Named<IPublisher>("commandsPublisher")
                .SingleInstance();

            builder
                .RegisterType<WhiteRabbitPublisher>()
                .WithParameter("exchangeName", EventsExchangeName)
                .Named<IPublisher>("eventsPublisher")
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventPublisher>()
                .WithParameter(ResolvedParameter.ForNamed<IPublisher>("eventsPublisher"))
                .AsImplementedInterfaces();
        }

        private static void RegisterConsumers(ContainerBuilder builder)
        {
            builder
                .RegisterType<WhiteRabbitConsumer>()
                .WithParameter("queueName", CommandsQueueName)
                .As<IConsumer>();
        }

        private static void RegisterSerializer(ContainerBuilder builder)
        {
            builder
                .RegisterSerializersFrom(new [] { "WhiteRabbit.Json" });

            builder
                .Register(c => new ContentType("application/json"))
                .AsSelf();
        }
    }
}
