using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public abstract class ConsumerBase
    {
        private readonly IModelFactory _modelFactory;
        private readonly string _queueName;
        private readonly Action<Exception, string> _onError;

        private IModel _channel;
        private bool _isRunning;
        private ManualResetEventSlim _stopConsumerEvent;

        protected ConsumerBase(IModelFactory modelFactory, string queueName, Action<Exception, string> onError)
        {
            _modelFactory = modelFactory;
            _queueName = queueName;
            _onError = onError;
            _isRunning = true;
        }

        protected async Task Start(bool noAck, Action<object, BasicDeliverEventArgs> onReceived)
        {
            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    try
                    {
                        _stopConsumerEvent = new ManualResetEventSlim(false);

                        _channel = _modelFactory.CreateModel();

                        var consumer = new EventingBasicConsumer(_channel);

                        consumer.Received += (obj, evtArgs) =>
                        {
                            try
                            {
                                onReceived(obj, evtArgs);
                            }
                            catch (Exception ex)
                            {
                                _onError(ex, $"Error processing message from {_queueName}");
                            }
                        };

                        consumer.Shutdown += (obj, evtArgs) =>
                        {
                            _onError(new Exception(evtArgs.ReplyText), $"{_queueName}: Consumer shutdown: {evtArgs.ReplyText}");
                            _stopConsumerEvent.Set();
                        };

                        _channel.BasicConsume(_queueName, noAck, consumer);

                        _stopConsumerEvent.Wait();
                    }
                    catch (Exception ex)
                    {
                        _onError(ex, $"Error while trying to consume from {_queueName}: {ex.Message}");
                    }

                    Task.Delay(1000).Wait();
                }

                _channel.Dispose();
            });
        }

        public void Dispose()
        {
            _isRunning = false;
            _stopConsumerEvent.Set();
        }
    }
}
