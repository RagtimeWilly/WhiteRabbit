using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace WhiteRabbit
{
    public sealed class Consumer : IConsumer
    {
        private readonly IModelFactory _modelFactory;
        private readonly Action _onDispose;

        private IModel _channel;

        public Consumer(IModelFactory modelFactory, Action onDispose)
        {
            _modelFactory = modelFactory;
            _onDispose = onDispose;
        }

        public IObservable<BasicDeliverEventArgs> Start(string queue, bool noAck)
        {
            _channel = _modelFactory.CreateModel();

            return Observable.Create<BasicDeliverEventArgs>(o =>
            {
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (obj, evtArgs) => { o.OnNext(evtArgs); };

                _channel.BasicConsume(queue, noAck, consumer);

                return Disposable.Create(() =>
                {
                    _channel.Dispose();
                    _onDispose();
                });
            });
        }
    }
}
