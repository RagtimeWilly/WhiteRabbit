using Autofac;
using WhiteRabbit.SampleApp.Application;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Infrastructure.IoC
{
    public class ApplicationIoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AddMessageHandler>()
                .As<ICommandHandler>();

            builder
                .RegisterType<UpdateMessageHandler>()
                .As<ICommandHandler>();
        }
    }
}
