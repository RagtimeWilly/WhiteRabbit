using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    public interface IMessageHandler
    {
        void Handle<T>(T msg);
    }
}
