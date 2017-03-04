using System;
using System.Configuration;
using Autofac;
using RabbitMQ.Client;

namespace WhiteRabbit.Autofac
{
    public class WhiteRabbitIoCModule : Module
    {
        public bool EnableContextPerMessage { get; set; }

        public bool RegisterDefaultSerializerFactory { get; set; } = true;
         
        protected override void Load(ContainerBuilder builder)
        {
            RegisterModelFactory(builder);

            if (EnableContextPerMessage)
            {
                builder.RegisterModule<ScopedMessageHandlerIoCModule>();
            }

            if (RegisterDefaultSerializerFactory)
            {
                builder
                    .RegisterType<SerializerFactory>()
                    .As<ISerializerFactory>();
            }

            builder
                .RegisterType<TopologyBootstrapper>()
                .AsSelf();
        }

        private static void RegisterModelFactory(ContainerBuilder builder)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings["WhiteRabbit:HostName"],
                VirtualHost = ConfigurationManager.AppSettings["WhiteRabbit:VirtualHost"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["WhiteRabbit:Port"]),
            };

            var user = ConfigurationManager.AppSettings["WhiteRabbit:User"];
            var password = ConfigurationManager.AppSettings["WhiteRabbit:Password"];

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                connectionFactory.UserName = user;
                connectionFactory.Password = password;
            }

            builder
                .Register(c => connectionFactory)
                .As<IConnectionFactory>()
                .SingleInstance();

            builder
                .RegisterType<ModelFactory>()
                .As<IModelFactory>()
                .SingleInstance();
        }
    }
}
