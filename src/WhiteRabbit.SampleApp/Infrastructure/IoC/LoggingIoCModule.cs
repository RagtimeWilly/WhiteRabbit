using System;
using Autofac;

namespace WhiteRabbit.SampleApp.Infrastructure.IoC
{
    public class LoggingIoCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Action<Exception, string> onError =
                (ex, msg) => Console.WriteLine($"Exception: {msg}{Environment.NewLine}{ex}");

            builder
                .Register(c => onError)
                .AsSelf();
        }
    }
}
