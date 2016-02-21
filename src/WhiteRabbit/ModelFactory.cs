using RabbitMQ.Client;

namespace WhiteRabbit
{
    public class ModelFactory : IModelFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _syncLock = new object();

        private IConnection _connection;

        public ModelFactory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IModel CreateModel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                lock (_syncLock)
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = _connectionFactory.CreateConnection();
                    }
                }   
            }

            return _connection.CreateModel();
        }
    }
}
