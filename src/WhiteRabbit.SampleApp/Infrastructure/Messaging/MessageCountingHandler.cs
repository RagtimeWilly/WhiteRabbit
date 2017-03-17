using System;
using RabbitMQ.Client.Events;

namespace WhiteRabbit.SampleApp.Infrastructure.Messaging
{
    internal class MessageCountingHandler : IMessageHandler
    {
        private readonly IMessageHandler _innerHandler;
        private readonly MessageCounter _counter;

        public MessageCountingHandler(IMessageHandler innerHandler, MessageCounter counter)
        {
            _innerHandler = innerHandler;
            _counter = counter;
        }

        public void Handle<T>(T args)
        {
            _counter.Increment();

            Console.WriteLine($"Total messages received = {_counter.Count}");

            _innerHandler.Handle(args);
        }
    }
}
