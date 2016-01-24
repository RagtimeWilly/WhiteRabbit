using RabbitMQ.Client;

namespace WhiteRabbit
{
    public class ModelFactory : IModelFactory
    {
        private readonly IConnection _connection;

        public ModelFactory(IConnection connection)
        {
            _connection = connection;
        }

        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
    }
}
