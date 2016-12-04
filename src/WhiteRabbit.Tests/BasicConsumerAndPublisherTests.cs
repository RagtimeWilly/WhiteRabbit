using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rhino.Mocks;

namespace WhiteRabbit.Tests.Integration
{
    [TestFixture, Explicit]
    public class BasicConsumerAndPublisherTests
    {
        private TestContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new TestContext();
        }

        [Test]
        public void Basic_consumer_receives_published_messages()
        {
            _context.ArrangeConsumer();

            _context.ActPublishMessage();

            _context.AssertMessageConsumed();

            _context.Teardown();
        }

        private class TestContext
        {
            private readonly IFixture _fixture;

            private ModelFactory _modelFactory;
            private IMessageHandler _handler;
            private Guid _correlationId;
            private string _queueName;
            private BasicConsumer _sut;


            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                ArrangeModelFactory();
            }

            public void ArrangeConsumer()
            {
                _correlationId = Guid.NewGuid();
                _queueName = Guid.NewGuid().ToString();

                _handler = _fixture.Freeze<IMessageHandler>();

                _handler
                    .Expect(f => f.Handle(Arg<BasicDeliverEventArgs>.Matches(a => a.BasicProperties.CorrelationId == _correlationId.ToString())))
                    .Repeat
                    .Once();

                var queue = new QueueConfig(_queueName, string.Empty, ExchangeConfig.Default, false, true, true, null, null);

                using (var channel = _modelFactory.CreateModel())
                {
                    channel.DeclareAndBindQueue(queue);
                }

                _sut = new BasicConsumer(_handler, _modelFactory, _queueName, (_, __) => { });

                Task.Run(() => _sut.Start(true));

                Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            }

            public void ActPublishMessage()
            {
                var serializer = _fixture.Freeze<ISerializer>();

                serializer
                    .Stub(f => f.Serialize(Arg<object>.Is.Anything))
                    .IgnoreArguments()
                    .Return(_fixture.Create<byte[]>());

                var publisher = new Publisher(_modelFactory, serializer, ExchangeConfig.Default.Name);

                publisher.Publish(Guid.NewGuid(), _queueName, _correlationId).Wait();

                Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            }

            public void AssertMessageConsumed()
            {
                _handler.VerifyAllExpectations();
            }

            public void Teardown()
            {
                _sut.Dispose();
            }

            private void ArrangeModelFactory()
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost",
                    VirtualHost = "/",
                    Port = 5672
                };

                _modelFactory = new ModelFactory(connectionFactory);
            }
        }
    }
}
