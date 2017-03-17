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

        public void Handle<T>(T msg)
        {
            var timer = Stopwatch.StartNew();

            _innerHandler.Handle(msg);

            timer.Stop();

            Console.WriteLine($"Finished handling {(msg as BasicDeliverEventArgs).BasicProperties.Type} in {timer.Elapsed}");
        }
    }
}
