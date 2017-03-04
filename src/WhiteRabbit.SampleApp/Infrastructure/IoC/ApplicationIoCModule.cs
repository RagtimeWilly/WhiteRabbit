using Autofac;
using WhiteRabbit.Autofac;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Infrastructure.IoC
{
    public class ApplicationIoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterHandlersFrom<ICommandHandler>(new [] { "WhiteRabbit.SampleApp" });
        }
    }
}
