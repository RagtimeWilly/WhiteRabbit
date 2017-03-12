using System;
using System.Diagnostics;
using RabbitMQ.Client.Events;

namespace WhiteRabbit.SampleApp.Infrastructure.Messaging
{
    internal class PerformanceLoggingHandler : IMessageHandler
    {
        private readonly IMessageHandler _innerHandler;

        public PerformanceLoggingHandler(IMessageHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public void Handle(BasicDeliverEventArgs args)
        {
            var timer = Stopwatch.StartNew();

            _innerHandler.Handle(args);

            timer.Stop();

            Console.WriteLine($"Finished handling {args.BasicProperties.Type} in {timer.Elapsed}");
        }
    }
}
