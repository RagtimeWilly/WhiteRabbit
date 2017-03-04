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
        private readonly Action<Exception, string> _onError;

        private IModel _channel;
        private bool _isRunning;
        private ManualResetEventSlim _stopConsumerEvent;

        protected ConsumerBase(IModelFactory modelFactory, string queueName, Action<Exception, string> onError)
        {
            _modelFactory = modelFactory;
            QueueName = queueName;
            _onError = onError;

            _isRunning = true;
        }

        protected string QueueName { get; }

        protected async Task Start(bool noAck, Action<object, BasicDeliverEventArgs> onReceived)
        {
            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    _stopConsumerEvent = new ManualResetEventSlim(false);

                    try
                    {
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
                                _onError(ex, $"Error processing message from {QueueName}");
                            }
                        };

                        consumer.Shutdown += (obj, evtArgs) =>
                        {
                            _onError(new Exception(evtArgs.ReplyText), $"{QueueName}: Consumer shutdown: {evtArgs.ReplyText}");
                            _stopConsumerEvent.Set();
                        };

                        consumer.ConsumerCancelled += (obj, evtArgs) =>
                        {
                            _onError(new Exception("Consumer cancelled"), $"{QueueName}: Consumer cancelled: {evtArgs.ConsumerTag}");
                            _stopConsumerEvent.Set();
                        };

                        _channel.BasicConsume(QueueName, noAck, consumer);

                        _stopConsumerEvent.Wait();
                    }
                    catch (Exception ex)
                    {
                        _onError(ex, $"Error while trying to consume from {QueueName}: {ex.Message}");
                    }

                    Task.Delay(250).Wait();
                }
            });
        }

        protected void AckMessage(ulong deliveryTag, bool multiple)
        {
            _channel.BasicAck(deliveryTag, multiple);
        }

        protected void NackMessage(ulong deliveryTag, bool multiple, bool requeue)
        {
            _channel.BasicNack(deliveryTag, multiple, requeue);
        }

        public void Dispose()
        {
            _isRunning = false;
            _stopConsumerEvent.Set();
            _channel?.Dispose();
        }
    }
}
