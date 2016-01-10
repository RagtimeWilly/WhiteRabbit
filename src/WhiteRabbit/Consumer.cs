using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public sealed class Consumer : IConsumer
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queue;
        private readonly Action _onDispose;
        private readonly ManualResetEvent _started;

        private IConnection _connection;
        private IModel _channel;

        public Consumer(IConnectionFactory connectionFactory, string queue, bool noAck, Action onDispose)
        {
            _connectionFactory = connectionFactory;
            _queue = queue;
            _onDispose = onDispose;

            _started = new ManualResetEvent(false);

            Start(noAck);

            _started.WaitOne();
        }

        public IObservable<BasicDeliverEventArgs> Stream { get; private set; }

        private Task Start(bool noAck)
        {
            return Task.Run(() =>
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                Stream = Observable.Create<BasicDeliverEventArgs>(o =>
                {
                    var consumer = new EventingBasicConsumer(_channel);

                    consumer.Received += (obj, evtArgs) => { o.OnNext(evtArgs); };

                    _channel.BasicConsume(_queue, noAck, consumer);

                    return Disposable.Create(_onDispose);
                });

                _started.Set();
            });
        }
    }
}
