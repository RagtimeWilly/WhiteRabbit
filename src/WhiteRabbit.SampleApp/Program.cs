using System;
using Autofac;
using WhiteRabbit.SampleApp.Infrastructure.IoC;
using WhiteRabbit.SampleApp.Services;

namespace WhiteRabbit.SampleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = IoCBootstrapper.Init();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<SampleService>();

                service.Start();

                Console.WriteLine("Service started, press enter to quit...");
                Console.ReadLine();

                service.Stop();
            }
        }
    }
}
