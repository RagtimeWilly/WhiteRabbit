using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public interface IMessageHandler
    {
        void Handle(BasicDeliverEventArgs args);
    }
}
