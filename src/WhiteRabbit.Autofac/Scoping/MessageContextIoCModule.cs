using Autofac;

namespace WhiteRabbit.Autofac.Scoping
{
    internal class MessageContextIoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MessageContextProvider>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .Register(c =>
                {
                    var contextProvider = c.Resolve<MessageContextProvider>();
                    return contextProvider.Get();
                })
                .As<IMessageContext>()
                .InstancePerLifetimeScope();
        }
    }
}
