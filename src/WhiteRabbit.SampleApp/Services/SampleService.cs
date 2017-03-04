using System;
using System.Threading;
using System.Threading.Tasks;
using WhiteRabbit.SampleApp.Contracts;

namespace WhiteRabbit.SampleApp.Services
{
    public class SampleService
    {
        private readonly IConsumer _messageCommandConsumer;
        private readonly IPublisher _publisher;
        private readonly TopologyBootstrapper _topologyBootstrapper;
        private bool _stopped;
        private string _contentType;

        public SampleService(IConsumer messageCommandConsumer, IPublisher publisher, TopologyBootstrapper topologyBootstrapper, ContentType contentType)
        {
            _messageCommandConsumer = messageCommandConsumer;
            _publisher = publisher;
            _topologyBootstrapper = topologyBootstrapper;
            _stopped = false;
            _contentType = contentType.Type;
        }

        public void Start()
        {
            _topologyBootstrapper.Init();

            var publishingTask = PublishTestCommands();
            var consumingTask = _messageCommandConsumer.Start();
        }

        public void Stop()
        {
            _stopped = true;
            _messageCommandConsumer.Dispose();
        }

        private Task PublishTestCommands()
        {
            return Task.Run(() =>
            {
                while (!_stopped)
                {
                    var id = Guid.NewGuid();

                    var addMessage = new AddMessage { MessageId = id, Text = "This is a new message." };
                    var correlationId = Guid.NewGuid();

                    _publisher.Publish(addMessage, "WhiteRabbit.SampleApp.Commands", correlationId, _contentType).Wait();
                    Console.WriteLine($"Published AddMessage with CorrelationId = {correlationId}");

                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    var updatedMessage = new UpdateMessage { MessageId = id, UpdatedText = "This is an update." };
                    correlationId = Guid.NewGuid();

                    _publisher.Publish(updatedMessage, "WhiteRabbit.SampleApp.Commands", correlationId, _contentType).Wait();
                    Console.WriteLine($"Published UpdateMessage with CorrelationId = {correlationId}");

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            });
        }
    }
}
