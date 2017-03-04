using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using WhiteRabbit.Hopper.SampleApp.Infrastructure.IoC;
using WhiteRabbit.Hopper.SampleApp.Services;

namespace WhiteRabbit.Hopper.SampleApp
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
