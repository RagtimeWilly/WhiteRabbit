using Autofac;
using Autofac.Core;
using WhiteRabbit.Json;
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

            builder
                .RegisterType<JsonSerializer>()
                .As<ISerializer>();

            builder
                .RegisterType<CommandDispatcher>()
                .As<IDispatcher>();

            builder
                .RegisterType<Publisher>()
                .WithParameter("exchange", string.Empty)
                .Named<IPublisher>("commandsPublisher")
                .SingleInstance();

            builder
                .RegisterType<Publisher>()
                .WithParameter("exchange", EventsExchangeName)
                .Named<IPublisher>("eventsPublisher")
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventPublisher>()
                .WithParameter(ResolvedParameter.ForNamed<IPublisher>("eventsPublisher"))
                .AsImplementedInterfaces();

            builder
                .RegisterType<BasicConsumer>()
                .WithParameter("queueName", CommandsQueueName)
                .As<IConsumer>();
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
                .Register(c => new QueueConfig("WhiteRabbit.SampleApp.Events.Testing", string.Empty, evtsExchange))
                .As<QueueConfig>();
        }
    }
}
