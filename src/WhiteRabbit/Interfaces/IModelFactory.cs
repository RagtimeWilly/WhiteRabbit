using RabbitMQ.Client;

namespace WhiteRabbit
{
    public interface IModelFactory
    {
        IModel CreateModel();
    }
}
