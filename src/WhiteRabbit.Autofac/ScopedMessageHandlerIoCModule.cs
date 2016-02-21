using Autofac;

namespace WhiteRabbit.Autofac
{
    internal class ScopedMessageHandlerIoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<DispatchingMessageHandler>()
                .Named<IMessageHandler>("innerDispatchingHandler");

            builder
                .RegisterType<ScopedMessageHandler>()
                .WithParameter("innerHandlerName", "innerDispatchingHandler")
                .As<IMessageHandler>();

            builder
                .RegisterType<MessageContextProvider>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .Register(c =>
                {
                    var contextProvider = c.Resolve<MessageContextProvider>();
                    return contextProvider.GetContext();
                })
                .As<IMessageContext>()
                .InstancePerLifetimeScope();
        }
    }
}
