using RabbitMQ.Client.Events;
using System;

namespace WhiteRabbit
{
    public interface IConsumer
    {
        IObservable<BasicDeliverEventArgs> Stream { get; }
    }
}
